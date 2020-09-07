﻿using DataFactory;
using DataFactory.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.Drawing.Printing;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Xsl;

namespace FieldDataTest
{
    public partial class MainForm : Form
    {
        #region Constants

        private const float FIELD_WIDTH = 360f;
        private const float FIELD_HEIGHT = 160f;
        private const string CONTENT_TYPE = "application/json";

        #endregion Constants

        #region Fields

        private delegate void CrossDelegateMethod(string s);
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

        #endregion Fields

        #region Constructors

        public MainForm()
        {
            InitializeComponent();
        }

        public MainForm(List<EventData> data)
            : this()
        {
            _Data = data;
        }

        #endregion Constructors

        #region Event Handlers

        private void OnActionItemClicked(object sender, EventArgs e)
        {
            if (sender == null) return;
            var menu = (ToolStripMenuItem)sender;
            //  get the activity
            var activity = _Field.Activities.FirstOrDefault(a => a.Type.Equals(menu.Tag));
            if (activity == null) return;
            //  get iterations
            using (var frm = new IterationForm())
            {
                frm.Iterations = 1;
                if (frm.ShowDialog(this) == DialogResult.OK)
                {
                    _Data.AddRange(activity.Generator.Generate(DateTime.Now, frm.Iterations));
                }
            }
        }

        private void OnExecuteItemClicked(object sender, EventArgs e)
        {
            if (sender == null) return;
            var menu = (ToolStripMenuItem)sender;
            //  get the activity
            var activity = _Field.Activities.FirstOrDefault(a => a.Type.Equals(menu.Tag));
            if (activity == null) return;
        }

        private void FieldMenu_Click(object sender, EventArgs e)
        {
            try
            {
                if (OpenFile.ShowDialog(this) != DialogResult.OK) return;
                _Field = Serializer.DeserializeFile<PlayingField>(OpenFile.FileName);
                //  convert to meters
                foreach (var activity in _Field.Activities)
                {
                    activity.OnDoneAction = async (p) => await DoneAction(p);
                    activity.Init(0);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Exception: {ex}");
            }
        }

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
                    }
                }
            }

            _RenderTimer = new System.Windows.Forms.Timer();
            _RenderTimer.Tick += RenderLoop;
            _RenderTimer.Interval = 20;
            _RenderTimer.Start();

            RunMenu_Click(this, e);
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
                var h = activity.Bounds.Y1 - activity.Bounds.Y0;
                var w = activity.Bounds.X1 - activity.Bounds.X0;
                Debug.WriteLine($"Checking activity: {activity.Name} - {activity.Bounds.X0}, {activity.Bounds.Y0}, {w}, {h}");
                var rect = new Rectangle((int)activity.Bounds.X0, (int)activity.Bounds.Y0, (int)w, (int)h);
                if (rect.IntersectsWith(new Rectangle((int)x, (int)y, 1, 1)))
                {
                    Debug.WriteLine($"You have clicked {activity.Name}");
                    SendScan(activity);
                    break;
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
                        float x = (_Field.Padding.X0 + activity.Bounds.X0) * _ScaleX, y = (height - _Field.Padding.Y0 - activity.Bounds.Y0) * _ScaleY;
                        float w = (activity.Bounds.X1 - activity.Bounds.X0) * _ScaleX, h = (activity.Bounds.Y1 - activity.Bounds.Y0) * _ScaleY;
                        gc.DrawRectangle(p, x, y - h, w, h);
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
                var client = new TcpClient("34.94.247.16", 9000);
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

        #endregion Logger

        #region Helpers

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

        public static Color ToColor(string hex)
        {
            if (string.IsNullOrEmpty(hex)) return Color.White;
            var converter = new ColorConverter();
            return (Color)converter.ConvertFromString(hex);
        }

        private void Stats()
        {
            if ((_Data == null) || (_Data.Count == 0)) return;
            var xMin = _Data.Min(d => d.X);
            var yMin = _Data.Min(d => d.Y);
            var xMax = _Data.Max(d => d.X);
            var yMax = _Data.Max(d => d.Y);
            var vMin = _Data.Min(d => d.V);
            var vMax = _Data.Max(d => d.V);
            var rMin = _Data.Min(d => d.R);
            var rMax = _Data.Max(d => d.R);
            Status.Text = $"{_Data?.Count}; Bounds ({xMin}, {yMin}), ({xMax}, {yMax}); Velocity ({vMin}, {vMax}); RVelocity ({rMin}, {rMax})";
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

        private void ExitMenu_Click(object sender, EventArgs e)
        {
            Close();
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

        private void BaseAddressMenu_Click(object sender, EventArgs e)
        {
            try
            {
                var frm = new InputForm("Base Address", "Enter Base Address", _BaseUrl);
                if (frm.ShowDialog(this) == DialogResult.Cancel) return;
                if (_BaseUrl.Equals(frm.Result)) return;
                _BaseUrl = frm.Result;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Exception: {ex}");
            }
        }

        #endregion Helpers
    }
}
