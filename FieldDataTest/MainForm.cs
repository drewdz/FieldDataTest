using DataFactory;
using DataFactory.Model;
using FieldDataTest.ViewModels;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FieldDataTest
{
    public partial class MainForm : BaseForm<MainViewModel>
    {
        #region Constants

        private const float FIELD_WIDTH = 360f;
        private const float FIELD_HEIGHT = 160f;
        private const string CONTENT_TYPE = "application/json";

        #endregion Constants

        #region Fields

        private List<EventData> _Data = new List<EventData>();
        private List<EventData> _LogData = new List<EventData>();
        private PlayingField _Field = new PlayingField();
        private bool _Running = false;
        private float _ScaleX, _ScaleY;
        private System.Threading.Timer _FieldTimer;
        private float _Width, _Height;
        private DateTime _LastClick;
        private Bitmap _Background;
        private System.Windows.Forms.Timer _RenderTimer;
        private bool _RunLogger = false;
        //private string _BaseUrl = "https://{0}-service-dot-hwp-legends-dev.nw.r.appspot.com/";
        private string _BaseUrl = "https://{0}-service-dot-hwp-legends.wl.r.appspot.com/";
        private DateTime _LastTick = DateTime.Now;
        private bool _ToFile = false;
        private string _StreamFile = string.Empty;
        private string _StreamText = string.Empty;
        private bool _PauseOnly = false;
        private Font _Font;
        private FieldActivity _SetupActivity = null;

        #endregion Fields

        #region Constructors

        public MainForm()
        {
            InitializeComponent();
            _Font = new Font("Arial Narrow", 8);
        }

        public MainForm(List<EventData> data)
            : this()
        {
            _Data = data;
        }

        #endregion Constructors

        #region Event Handlers

        private void MainForm_Load(object sender, System.EventArgs e)
        {
            _Background = new Bitmap(GetType(), "field.png");
            _Width = (float)MainImage.ClientRectangle.Width;
            _Height = (float)MainImage.ClientRectangle.Height;

            Status.Text = string.Empty;

            //  find the field setup in resources
            foreach (var name in GetType().Assembly.GetManifestResourceNames().Where(n => n.EndsWith("field_setup.json")))
            {
                using (var stream = GetType().Assembly.GetManifestResourceStream(name))
                {
                    _Field = Serializer.Deserialize<PlayingField>(stream);
                    //  convert to meters
                    foreach (var activity in _Field.Activities)
                    {
                        activity.OnDoneAction = async (p) => await DoneAction(p);
                        activity.Init(0);
                        var item = new ToolStripMenuItem(activity.Name);
                        item.Tag = activity;
                        item.Click += ActivitySetupMenu_Click;
                        SetupMenu.DropDownItems.Add(item);
                    }
                }
            }

            //  get activities
            Task.Run(async () =>
            {
                await GetActivitiesAsync();
                RunMenu_Click(this, e);
            });

            _RenderTimer = new System.Windows.Forms.Timer();
            _RenderTimer.Tick += RenderLoop;
            _RenderTimer.Interval = 20;
            _RenderTimer.Start();
        }

        private void RunMenu_Click(object sender, EventArgs e)
        {
            _Running = !_Running;
            //  start running
            if (_Running)
            {
                RunMenu.Visible = false;
                foreach (var activity in _Field.Activities) activity.Init(DateTimeOffset.Now.ToUnixTimeMilliseconds());
                _FieldTimer = new System.Threading.Timer(Tick, true, 0, 100);
            }
            else
            {
                RunMenu.Visible = true;
                _FieldTimer.Dispose();
                _FieldTimer = null;
            }
            RunMenu.Text = (_Running) ? "Stop" : "Start No Scanner";
        }

        private void StartMenu_Click(object sender, EventArgs e)
        {
            if (StartMenu.Text.Equals("Stop Streaming"))
            {
                _RunLogger = false;
                return;
            }

            if (_ToFile)
            {
                if (string.IsNullOrWhiteSpace(_StreamFile))
                {
                    if (SaveFile.ShowDialog(this) == DialogResult.Cancel) return;
                    _StreamFile = SaveFile.FileName;
                }
                if (File.Exists(_StreamFile))
                {
                    if (MessageBox.Show(this, $"The file \"{_StreamFile}\" already exists. Would you like to overwrite it?", string.Empty, MessageBoxButtons.YesNo) == DialogResult.No)
                    {
                        _StreamFile = string.Empty;
                        return;
                    }
                    File.Delete(_StreamFile);
                }
            }

            _LastTick = DateTime.Now;
            new Thread(LoggerLoop).Start();
            new Thread(CheckLogger).Start();
        }

        private void MainImage_MouseUp(object sender, MouseEventArgs e)
        {
            if (_Field == null) return;
            //  debounce the click
            if (DateTime.Now.Subtract(_LastClick).TotalMilliseconds < 700) return;
            _LastClick = DateTime.Now;

            if ((_Field == null) || (_Field.Activities == null)) return;
            float x = e.X / _ScaleX, y = (_Height - e.Y) / _ScaleY;
            x -= _Field.Padding.X0; y -= _Field.Padding.Y0;
            Debug.WriteLine($"Mouse click: {e.X} ({x}), {e.Y} ({y})");

            foreach (var activity in _Field.Activities)
            {
                if (activity.ActivityMode == ActivityMode.Cone1)
                {
                    Debug.WriteLine($"{activity.Name} - placing cone 1");
                    _SetupActivity.Cones[0].X = x; _SetupActivity.Cones[0].Y = y;
                    Status.Text = $"{activity.Name} - Place cone 2";
                    activity.ActivityMode = ActivityMode.Cone2;
                }
                else if (activity.ActivityMode == ActivityMode.Cone2)
                {
                    Debug.WriteLine($"{activity.Name} - placing cone 2");
                    _SetupActivity.Cones[1].X = x; _SetupActivity.Cones[1].Y = y;
                    Status.Text = $"{activity.Name} - Place cone 3";
                    activity.ActivityMode = ActivityMode.Cone3;
                }
                else if (activity.ActivityMode == ActivityMode.Cone3)
                {
                    Debug.WriteLine($"{activity.Name} - placing cone 3");
                    _SetupActivity.Cones[2].X = x; _SetupActivity.Cones[2].Y = y;
                    Status.Text = $"{activity.Name} - Place cone 4";
                    activity.ActivityMode = ActivityMode.Cone4;
                }
                else if (activity.ActivityMode == ActivityMode.Cone4)
                {
                    Debug.WriteLine($"{activity.Name} - placing cone 4");
                    _SetupActivity.Cones[3].X = x; _SetupActivity.Cones[3].Y = y;
                    //  find min and max
                    float dx0 = float.MaxValue, dy0 = float.MaxValue, dx1 = float.MinValue, dy1 = float.MinValue;
                    foreach (var cone in _SetupActivity.Cones)
                    {
                        if (cone.X < dx0) dx0 = cone.X;
                        if (cone.X > dx1) dx1 = cone.X;
                        if (cone.Y < dy0) dy0 = cone.Y;
                        if (cone.Y > dy1) dy1 = cone.Y;
                    }
                    Status.Text = $"{activity.Name} - Waiting for calibratrion";
                    activity.ActivityMode = ActivityMode.Calibration;
                    new Thread(StartCallibration).Start(null);
                }
                else if (activity.ActivityMode == ActivityMode.Calibration)
                {
                    Debug.WriteLine($"{activity.Name} - Cancel calibration");
                    Status.Text = $"{activity.Name} - Cancel calibration";
                    activity.ActivityMode = ActivityMode.Fail;
                }
                else if (activity.ActivityMode == ActivityMode.Fail)
                {
                    Debug.WriteLine($"{activity.Name} - Calibration failed");
                    Status.Text = $"{activity.Name} - Cancel calibration";
                    _SetupActivity = null;
                    activity.ActivityMode = ActivityMode.Ready;
                }
                else
                {
                    Status.Text = string.Empty;
                    var h = activity.Y1 - activity.Y0;
                    var w = activity.X1 - activity.X0;
                    Debug.WriteLine($"Checking activity: {activity.Name} - {activity.X0}, {activity.Y0}, {w}, {h}");
                    var rect = new Rectangle((int)activity.X0, (int)activity.Y0, (int)w, (int)h);
                    if (rect.IntersectsWith(new Rectangle((int)x, (int)y, 1, 1)))
                    {
                        Debug.WriteLine($"You have clicked {activity.Name} - ({activity.X0},{activity.Y0},{activity.X1},{activity.Y1})");
                        if (e.Button == MouseButtons.Left)
                        {
                            SendScan(activity);
                        }
                    }
                }
            }
        }

        private void MainImage_Resize(object sender, EventArgs e)
        {
            //  get graphics context
            _Width = (float)MainImage.ClientRectangle.Width;
            _Height = (float)MainImage.ClientRectangle.Height;
        }

        private void PauseMenu_Click(object sender, EventArgs e)
        {
            _PauseOnly = !_PauseOnly;
            PauseMenu.Checked = _PauseOnly;
            SimStatus.Text = (_PauseOnly) ? "Use Scanner App" : "Simulate Scanner";
            SimStatus.BackColor = (_PauseOnly) ? Color.IndianRed : SystemColors.Control;
        }

        private void ExitMenu_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void ActivitySetupMenu_Click(object sender, EventArgs e)
        {
            FieldActivity activity = null;
            ToolStripMenuItem item = null;
            if (sender != null)
            {
                item = (ToolStripMenuItem)sender;
                activity = (FieldActivity)item.Tag;
            }
            else
            {
                activity = _SetupActivity;
                foreach (var child in SetupMenu.DropDownItems)
                {
                    if (((ToolStripMenuItem)child).Tag == activity)
                    {
                        item = (ToolStripMenuItem)child;
                        break;
                    }
                }
            }

            if (activity.ActivityMode != ActivityMode.Ready)
            {
                Debug.WriteLine($"{_SetupActivity.Name} already in setup mode - cancel");
                _SetupActivity.ActivityMode = ActivityMode.Ready;
                foreach (var child in item.GetCurrentParent().Items)
                {
                    SetEnabled((ToolStripMenuItem)child, true);
                }
                return;
            }

            foreach (var child in item.GetCurrentParent().Items)
            {
                SetEnabled((ToolStripMenuItem)child, false);
            }

            _SetupActivity = activity;
            StartSetup();
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (_RunLogger) _RunLogger = false;
        }

        private void FileOpenMenu_Click(object sender, EventArgs e)
        {
            try
            {
                if (OpenFile.ShowDialog(this) != DialogResult.OK) return;
                //  deserialize
                if (new FileInfo(OpenFile.FileName).Extension.EndsWith("csv", StringComparison.InvariantCultureIgnoreCase))
                {
                    var data = Program.ImportData(OpenFile.FileName)?.ToList();
                    var min = data.Min(d => d.Timestamp);
                    data.ForEach(d => d.Timestamp = DateTimeOffset.Now.ToUnixTimeMilliseconds() + (d.Timestamp - min) + 10000);
                    _Data = data;
                }
                else
                {
                    using (var fs = new FileStream(OpenFile.FileName, FileMode.Open, FileAccess.Read))
                    {
                        using (var sr = new StreamReader(fs))
                        {
                            var data = Serializer.Deserialize<List<ZebraData>>(sr.ReadToEnd())?.Select(z => (EventData)z).ToList();
                            var min = data.Min(d => d.Timestamp);
                            data.ForEach(d => d.Timestamp = DateTimeOffset.Now.ToUnixTimeMilliseconds() + (d.Timestamp - min) + 10000);
                            _Data = data;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, ex.Message);
            }
        }

        private void GetDataMenu_Click(object sender, EventArgs e)
        {
            Task.Run(async () =>
            {
                try
                {
                    using (var restService = new RestService())
                    {
                        //  get activity
                        var baseUrl = string.Format(_BaseUrl, "domain/zebradata");
                        var result = await restService.GetAsync<ServiceResult<List<ZebraData>>>(baseUrl);
                        if (result.Status != ServiceResultStatus.Success) throw new Exception("Could not get Zebra Data");
                        var min = result.Payload.Min(d => d.Timestamp);
                        result.Payload.ForEach(d => d.Timestamp = DateTimeOffset.Now.ToUnixTimeMilliseconds() + (d.Timestamp - min) + 10000);
                        _Data = result.Payload.Select(d => (EventData)d)?.ToList();
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(this, ex.Message);
                }
            });
        }

        private void ClearDataMenu_Click(object sender, EventArgs e)
        {
            Task.Run(async () =>
            {
                try
                {
                    using (var restService = new RestService())
                    {
                        //  get activity
                        var baseUrl = string.Format(_BaseUrl, "domain/zebradata/delete/all");
                        var result = await restService.GetAsync<ServiceResult>(baseUrl);
                        if (result.Status != ServiceResultStatus.Success) throw new Exception("Could not delete Zebra Data");
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(this, ex.Message);
                }
            });
        }

        #endregion Event Handlers

        #region Actions

        private void Tick(object o)
        {
            if (_Data.Count > 0)
            {
                _Data.RemoveAll(d => d.Timestamp < (DateTimeOffset.Now.ToUnixTimeMilliseconds() - 10000));
            }
            var waitScan = (o == null) ? false : (bool)o;
            var holding = new List<EventData>();
            foreach (var activity in _Field.Activities)
            {
                holding.AddRange(activity.CreateSamples(DateTimeOffset.Now.ToUnixTimeMilliseconds(), waitScan));
            }
            _Data.AddRange(holding);
            if (_RunLogger) holding.ForEach(d => _LogData.Add(d.Copy()));
        }

        private void RenderLoop(object sender, EventArgs e)
        {
            try
            {
                if ((_Field == null) || (_Field.Activities == null) || (_Field.Activities.Count == 0)) return;

                float width = FIELD_WIDTH + _Field.Padding.X0 + _Field.Padding.X1, height = FIELD_HEIGHT + _Field.Padding.Y0 + _Field.Padding.Y1;
                _ScaleX = _Width / width; _ScaleY = _Height / height;
                Bitmap back = new Bitmap((int)_Width, (int)_Height);
                using (var gc = Graphics.FromImage(back))
                {
                    //  draw field
                    gc.DrawImage(_Background, new Rectangle((int)(_Field.Padding.X0 * _ScaleX), (int)(_Field.Padding.Y0 * _ScaleY), (int)(FIELD_WIDTH * _ScaleX), (int)(FIELD_HEIGHT * _ScaleY)));
                    gc.DrawRectangle(Pens.Red, (int)(_Field.Padding.X0 * _ScaleX), (int)(_Field.Padding.Y0 * _ScaleY), (int)(FIELD_WIDTH * _ScaleX), (int)(FIELD_HEIGHT * _ScaleY));

                    //  draw activities
                    foreach (var activity in _Field.Activities)
                    {
                        var p = (activity.State == ActivityState.Ready) ? Pens.Orange : (activity.State == ActivityState.Waiting) ? Pens.LightGreen : Pens.Red;
                        float x = (_Field.Padding.X0 + activity.X0) * _ScaleX, y = (height - _Field.Padding.Y0 - activity.Y0) * _ScaleY;
                        float w = (activity.X1 - activity.X0) * _ScaleX, h = (activity.Y1 - activity.Y0) * _ScaleY;
                        gc.DrawRectangle(p, x, y - h, w, h);
                        //  name
                        var size = gc.MeasureString(activity.Name, _Font);
                        x = x + (w / 2) - (size.Width / 2); y = y - (h / 2) - (size.Height / 2);
                        gc.DrawString(activity.Name, _Font, Brushes.Black, x, y);
                        //  queue point
                        x = (_Field.Padding.X0 + activity.QueuePoint.X0) * _ScaleX; y = (height - _Field.Padding.Y0 - activity.QueuePoint.Y0) * _ScaleY;
                        gc.FillEllipse(Brushes.Black, x, y - 5, 5, 5);
                        //  collect point
                        x = (_Field.Padding.X0 + activity.CollectionPoint.X0) * _ScaleX; y = (height - _Field.Padding.Y0 - activity.CollectionPoint.Y0) * _ScaleY;
                        gc.FillEllipse(Brushes.Red, x, y - 5, 5, 5);
                    }

                    if ((_Data != null) && (_Data.Count > 0))
                    {
                        foreach (var d in _Data)
                        {
                            if (d == null) continue;
                            //Debug.WriteLine(d);
                            float x = (_Field.Padding.X0 + d.X) * _ScaleX, y = (height - _Field.Padding.Y0 - d.Y) * _ScaleY;
                            gc.DrawRectangle(Pens.Black, x, y - 1, 1, 1);
                        }
                    }
                    gc.Flush();
                }
                MainImage.Image = back;
            }
            catch
            {
                Debug.WriteLine("Render exception");
            }
        }

        #endregion Actions

        #region Logger

        private void CheckLogger(object o)
        {
            Debug.WriteLine("Checker running...");
            try
            {
                while (_RunLogger)
                {
                    if (DateTime.Now.Subtract(_LastTick).TotalMilliseconds > 1000) break;
                    Thread.Sleep(1000);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Exception in logger checker: {ex}");
            }
            finally
            {
                Debug.WriteLine("Logger seems to have stopped, restarting...");
                //  restart
                if (_RunLogger)
                {
                    _RunLogger = false;
                    StartMenu_Click(null, null);
                }
            }
        }

        private void LoggerLoop(object o)
        {
            if (_RunLogger) return;
            _RunLogger = true;
            _StreamText = string.Empty;
            SetStreamStatus("Streaming");

            try
            {
                Debug.WriteLine("Starting logging stream");
                //  connect
                var client = new TcpClient("34.94.121.139", 9000);
                var stream = client.GetStream();
                int count = 0;
                using (var sw = new StreamWriter(stream))
                {
                    while (_RunLogger)
                    {
                        _LastTick = DateTime.Now;
                        //  get records to send
                        if ((_LogData != null) && (_LogData.Count > 0))
                        {
                            var dd = _LogData.Select(d => d.Copy()).ToList();
                            Debug.WriteLine($"Log: Sending {dd.Count} records");
                            if ((dd != null) && (dd.Count > 0))
                            {
                                foreach (var d in dd)
                                {
                                    if (d == null) continue;
                                    var s = d.ToString();
                                    sw.WriteLine(s);
                                    Debug.WriteLine($"Log: {s}");
                                    if (_ToFile) _StreamText += (string.IsNullOrWhiteSpace(_StreamText)) ? s : $"\r\n{s}";
                                    _LogData.Remove(_LogData.FirstOrDefault(i => i.Id.Equals(d.Id)));
                                }
                            }
                        }
                        Thread.Sleep(10);
                        count++;
                        if (count >= 500)
                        {
                            if (!string.IsNullOrWhiteSpace(_StreamText))
                            {
                                using (var fileStream = new FileStream(_StreamFile, FileMode.OpenOrCreate, FileAccess.Write))
                                {
                                    using (var fileSw = new StreamWriter(fileStream))
                                    {
                                        fileSw.WriteLine(_StreamText);
                                        fileSw.Flush();
                                    }
                                }
                            }
                            count = 0;
                        }
                    }
                    sw.Flush();
                }
                client.Close();
                client.Dispose();
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Logging stream exception: {ex}");
            }
            finally
            {
                SetStreamStatus("Not Streaming");
            }
            Debug.WriteLine("Stop logging stream");
        }

        private void SetStreamStatus(string text)
        {
            try
            {
                if (MainStatus.InvokeRequired)
                {
                    MainStatus.Invoke(new CrossDelegateMethod(SetStreamStatus), new object[] { text });
                }
                else
                {
                    var streaming = text.Equals("Streaming");
                    var color = (streaming) ? Color.DarkSeaGreen : SystemColors.Control;
                    StartMenu.Text = (streaming) ? "Stop Streaming" : "Start Streaming";
                    StreamStatus.Text = text;
                    StreamStatus.BackColor = color;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Unable to set status: {ex}");
            }
        }

        #endregion Logger

        #region Helpers

        private void StartSetup()
        {
            var frm = new ConesForm();
            if (frm.ShowDialog(this) != DialogResult.OK) return;
            _SetupActivity.Cones = new List<TrackingTag>
            {
                new TrackingTag { TagId = frm.ViewModel.Cone1 },
                new TrackingTag { TagId = frm.ViewModel.Cone2 },
                new TrackingTag { TagId = frm.ViewModel.Cone3 },
                new TrackingTag { TagId = frm.ViewModel.Cone4 }
            };
            Status.Text = $"{_SetupActivity?.Name} - Place cone 1";
            _SetupActivity.ActivityMode = ActivityMode.Cone1;
        }

        private async void StartCallibration(object o)
        {
            try
            {
                using (var restService = new RestService())
                {
                    //  get activity
                    var baseUrl = string.Format(_BaseUrl, "activity");
                    var result = await restService.GetAsync<ServiceResult<FieldActivity>>($"{baseUrl}activity/id?activityId={_SetupActivity.Id}");
                    if (result.Status != ServiceResultStatus.Success) throw new Exception($"Could not get activity {_SetupActivity.Name}, {_SetupActivity.Id}");
                    //  update
                    result.Payload.Payload = _SetupActivity.Cones;
                    result.Payload.ActivityMode = ActivityMode.Calibration;
                    result = await restService.PostAsync<ServiceResult<FieldActivity>, FieldActivity>($"{baseUrl}activity/update", result.Payload);
                    if (result.Status != ServiceResultStatus.Success) throw new Exception($"Could not update activity {_SetupActivity.Name}, {_SetupActivity.Id}");
                    //  poll for status change
                    var start = DateTime.Now;
                    while (true)
                    {
                        if (DateTime.Now.Subtract(start).TotalSeconds >= 60)
                        {
                            Debug.WriteLine($"Activity setup failed - Timeout expired, {_SetupActivity.Name}");
                            Status.Text = $"{_SetupActivity.Name} - Setup failed, timeout expired.";
                            await Task.Delay(1500);
                            Status.Text = string.Empty;
                            throw new Exception("Timeout expired");
                        }
                        Thread.Sleep(100);
                        result = await restService.GetAsync<ServiceResult<FieldActivity>>($"{baseUrl}activity/id?activityId={_SetupActivity.Id}");
                        if (result.Status != ServiceResultStatus.Success) throw new Exception($"Could not get activity {_SetupActivity.Name}, {_SetupActivity.Id}");
                        if (result.Payload.ActivityMode == ActivityMode.Ready)
                        {
                            Debug.WriteLine($"Activity setup complete, {_SetupActivity.Name}");
                            _SetupActivity.X0 = result.Payload.X0;
                            _SetupActivity.Y0 = result.Payload.Y0;
                            _SetupActivity.X1 = result.Payload.X1;
                            _SetupActivity.Y1 = result.Payload.Y1;
                            _SetupActivity.Direction = result.Payload.Direction;
                            _SetupActivity.Generator.SetQueues();
                            ActivitySetupMenu_Click(null, null);
                            Status.Text = $"{_SetupActivity.Name} - Setup complete.";
                            await Task.Delay(1500);
                            Status.Text = string.Empty;
                            break;
                        }
                        else if (result.Payload.ActivityMode == ActivityMode.Fail)
                        {
                            Debug.WriteLine($"Activity setup failed, {_SetupActivity.Name}");
                            _SetupActivity.ActivityMode = ActivityMode.Fail;
                            Status.Text = $"{_SetupActivity.Name} - Setup failed.";
                            await Task.Delay(1500);
                            Status.Text = string.Empty;
                            ActivitySetupMenu_Click(null, null);
                            break;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Failed to start callibration for {_SetupActivity.Name}: {ex}");
                Status.Text = $"{_SetupActivity.Name} - Setup exception.";
                await Task.Delay(1500);
                Status.Text = string.Empty;
                ActivitySetupMenu_Click(null, null);
            }
        }

        private void SetStatus(string status)
        {
            if (!MainStatus.InvokeRequired)
            {
                Status.Text = status;
            }
            else
            {
                MainStatus.Invoke(new CrossDelegateMethod(SetStatus), new object[] { status });
            }
        }

        private void SetEnabled(ToolStripMenuItem item, bool enabled)
        {
            if (!MainMenu.InvokeRequired)
            {
                item.Enabled = enabled;
            }
            else
            {
                MainMenu.Invoke(new CrossDelegateMethod<ToolStripMenuItem, bool>(SetEnabled), new object[] { item, enabled });
            }
        }

        public static Color ToColor(string hex)
        {
            if (string.IsNullOrEmpty(hex)) return Color.White;
            var converter = new ColorConverter();
            return (Color)converter.ConvertFromString(hex);
        }

        private async Task GetActivitiesAsync()
        {
            try
            {
                //  create new player activity record
                using (var restService = new RestService())
                {
                    restService.EchoOn = true;
                    var result = await restService.GetAsync<ServiceResult<List<FieldActivity>>>(string.Format($"{_BaseUrl}activities", "activity"));
                    if (result.Status != ServiceResultStatus.Success)
                    {
                        throw new Exception(result.Message);
                    }
                    //  apply bounds to setup
                    foreach (var item in result.Payload)
                    {
                        //  find the activity
                        var activity = _Field.Activities.FirstOrDefault(a => a.Id.Equals(item.Id));
                        if (activity == null)
                        {
                            Debug.WriteLine($"Could not find activity - {item.Name}, {item.Id}");
                            continue;
                        }
                        activity.X0 = item.X0;
                        activity.Y0 = item.Y0;
                        activity.X1 = item.X1;
                        activity.Y1 = item.Y1;
                        activity.Direction = item.Direction;
                        activity.Generator.SetQueues();
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Could not get activities. Exception: {ex}");
            }
        }

        private void SimTunnelRun_Click(object sender, EventArgs e)
        {
            var frm = new TunnelView();
            frm.ShowDialog(this);
        }

        private async Task SendScan(FieldActivity activity)
        {
            if ((activity == null) || (activity.State != ActivityState.Waiting)) return;
            try
            {
                if (_PauseOnly)
                {
                    MessageBox.Show(this, "Click ok after scanning a lanyard.", "Scan Pause", MessageBoxButtons.OK);
                    activity.ReadyUp(DateTimeOffset.Now.ToUnixTimeMilliseconds());
                    return;
                }

                var frm = new InputForm("Barcode", "Scan/Enter Barcode", string.Empty);
                if (frm.ShowDialog(this) == DialogResult.Cancel) return;
                //  create player activity object
                var player = new PlayerActivity
                {
                    ActivityId = activity.Id,
                    LanyardId = frm.Result,
                    Scanned = DateTimeOffset.UtcNow,
                    StartTime = DateTime.UtcNow,
                    State = 1
                };
                var s = Serializer.Serialize(player);
                //  create new player activity record
                using (var restService = new RestService())
                {
                    restService.EchoOn = true;
                    var result = await restService.PostAsync<ServiceResult<PlayerActivity>, PlayerActivity>(string.Format($"{_BaseUrl}playeractivity/set", "activity"), player);
                    if (result.Status != ServiceResultStatus.Success)
                    {
                        throw new Exception(result.Message);
                    }
                    activity.CurrentPlayer = result.Payload;
                    activity.ReadyUp(DateTimeOffset.Now.ToUnixTimeMilliseconds());
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Exception: {ex}");
            }
        }

        private async Task DoneAction(PlayerActivity player)
        {
            try
            {
                if (_PauseOnly) return;
                //  update the player
                player.EndTime = DateTimeOffset.UtcNow;
                player.State = 4;
                //  update it
                using (var restService = new RestService())
                {
                    restService.EchoOn = true;
                    await restService.PostAsync<PlayerActivity, PlayerActivity>(string.Format($"{_BaseUrl}playeractivity/set", "activity"), player);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Exception: {ex}");
            }
        }

        #endregion Helpers
    }
}
