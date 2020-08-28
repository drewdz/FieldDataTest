using DataFactory;
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

        private List<EventData> _Data = new List<EventData>();
        private List<EventData> _LogData = new List<EventData>();
        private PlayingField _Field = new PlayingField();
        private bool _Running = false;
        private float _ScaleX, _ScaleY;
        private System.Threading.Timer _FieldTimer;
        private long _Millis = 0;
        private float _Width, _Height;
        private DateTime _LastClick;
        private Bitmap _Background;
        private System.Windows.Forms.Timer _RenderTimer;
        private bool _RunLogger = false;
        //private string _BaseUrl = "https://{0}-service-dot-hwp-legends-dev.nw.r.appspot.com/";
        private string _BaseUrl = "https://{0}-service-dot-hwp-legends.wl.r.appspot.com/";

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

        #region Init

        private void Init()
        {
            if ((_Field == null) || (_Field.Activities == null)) return;
            foreach (var activity in _Field.Activities)
            {
                //  add action menu item
                var menuItem = new ToolStripMenuItem(activity.Name);
                menuItem.Click += OnActionItemClicked;
                menuItem.Tag = activity.Type;
                ActionMenu.DropDownItems.Add(menuItem);
                //  add execute menu item
                menuItem = new ToolStripMenuItem(activity.Name);
                menuItem.Click += OnExecuteItemClicked;
                menuItem.Tag = activity.Type;
                ExecuteMenu.DropDownItems.Add(menuItem);
            }
        }

        #endregion Init

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
                Init();
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Exception: {ex}");
            }
        }

        private void GenerateMenu_Click(object sender, EventArgs e)
        {
            if ((_Field == null) || (_Field.Activities == null) || (_Field.Activities.Count == 0)) return;
            _Data.Clear();

            foreach (var activity in _Field.Activities)
            {
                for (int i = 0; i < 10; i++)
                {
                    //  get the appropriate data
                    _Data.AddRange(activity.Generator.Generate(DateTime.Now, 10));
                }
            }
            Stats();
        }

        private void MainForm_Load(object sender, System.EventArgs e)
        {
            _Background = new Bitmap(GetType(), "field.png");
            //_Background = new Bitmap(GetType(), "pitch.png");
            _Width = (float)MainImage.ClientRectangle.Width;
            _Height = (float)MainImage.ClientRectangle.Height;

            Stats();
            _RenderTimer = new System.Windows.Forms.Timer();
            _RenderTimer.Tick += RenderLoop;
            _RenderTimer.Interval = 20;
            _RenderTimer.Start();
        }

        private void SideViewMenu_Click(object sender, EventArgs e)
        {
            if ((_Data == null) || (_Data.Count == 0)) return;
            var frm = new SideView(_Data);
            frm.ShowDialog(this);
        }

        private void RunMenu_Click(object sender, EventArgs e)
        {
            var menu = (ToolStripMenuItem)sender;
            bool waitScan = (menu.Text.Equals("Start No Scanner")) ? false : true;
            _Running = !_Running;
            //  start running
            if (_Running)
            {
                StartWaitMenu.Visible = false;
                _Millis = DateTime.Now.Subtract(DataFactory.Constants.UnixEpoch).Ticks / 10000;
                Debug.WriteLine($"Start: {_Millis}");
                foreach (var activity in _Field.Activities) activity.Init(_Millis);
                _FieldTimer = new System.Threading.Timer(Tick, waitScan, 0, 100);
            }
            else
            {
                StartWaitMenu.Visible = true;
                _FieldTimer.Dispose();
                _FieldTimer = null;
            }
            RunMenu.Text = (_Running) ? "Stop" : "Start No Scanner";
        }

        private void ResetMenu_Click(object sender, EventArgs e)
        {
            if (_Data != null) _Data.Clear();
        }

        private void StartMenu_Click(object sender, EventArgs e)
        {
            BaseAddressMenu.Enabled = false;
            new Thread(LoggerLoop).Start();
        }

        private void StopMenu_Click(object sender, EventArgs e)
        {
            _RunLogger = false;
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

        private void GetPlayerMenu_Click(object sender, EventArgs e)
        {

        }

        private void GetPlayerActivityMenu_Click(object sender, EventArgs e)
        {

        }

        private void ViewActivitiesMenu_Click(object sender, EventArgs e)
        {

        }

        #endregion Event Handlers

        #region Actions

        private void Tick(object o)
        {
            if (_Data.Count > 0)
            {
                var millis = _Data.Max(d => d.Timestamp) - 10000;
                _Data.RemoveAll(d => d.Timestamp < millis);
            }
            var waitScan = (o == null) ? false : (bool)o;
            foreach (var activity in _Field.Activities)
            {
                var data = activity.CreateSamples(_Millis, waitScan);
                //  stream data
                if ((data == null) || (data.Count() == 0)) continue;
                //  add data to the log list
                _LogData.AddRange(data.Select(d => d.Copy()).ToList());
                //  add data to plot list
                _Data.AddRange(data.Select(d => d.Copy()).ToList());
            }
            _Millis += 100;
        }

        private void RenderLoop(object sender, EventArgs e)
        {
            try
            {
                if ((_Field == null) || (_Field.Activities == null) || (_Field.Activities.Count == 0)) return;

                var data = _Data.ToList();

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

                    if ((data != null) && (data.Count() > 0))
                    {
                        foreach (var d in data)
                        {
                            if (d == null) continue;
                            //Debug.WriteLine(d);
                            float x = (_Field.Padding.X0 + d.X) * _ScaleX, y = (height - _Field.Padding.Y0 - d.Y) * _ScaleY;
                            gc.DrawRectangle(Pens.Black, x, y - 1, 1, 1);
                        }
                    }
                    data = null;
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

        private void LoggerLoop(object o)
        {
            if (_RunLogger) return;
            _RunLogger = true;


            try
            {
                Debug.WriteLine("Starting logging stream");
                //  connect
                var client = new TcpClient("34.94.247.16", 9000);
                var stream = client.GetStream();
                using (var sw = new StreamWriter(stream))
                {
                    while (_RunLogger)
                    {
                        //  get records to send
                        var dd = _LogData.ToList();
                        if ((dd != null) && (dd.Count > 0))
                        {
                            foreach (var d in dd)
                            {
                                if (d == null) continue;
                                sw.WriteLine(d.ToString());
                                _LogData.Remove(d);
                            }
                        }
                        Thread.Sleep(5);
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
                BaseAddressMenu.Enabled = true;
            }
            Debug.WriteLine("Stop logging stream");
        }

        #endregion Logger

        #region Helpers

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
                var frm = new InputForm("Barcode", "Scan/Enter Barcode", string.Empty);
                if (frm.ShowDialog(this) == DialogResult.Cancel) return;
                //  create player activity object
                var player = new PlayerActivity
                {
                    ActivityId = activity.Id,
                    LanyardId = frm.Result,
                    Scanned = DateTime.Now.ToUniversalTime(),
                    StartTime = DateTime.Now.ToUniversalTime(),
                    State = 1
                };
                var s = Serializer.Serialize(player);
                //  create new player activity record
                var result = await RestService.PostAsync<PlayerActivity, PlayerActivity>(string.Format($"{_BaseUrl}playeractivity/set", "activity"), player);
                if (result.Status != ServiceResultStatus.Success)
                {
                    throw new Exception(result.Message);
                }
                activity.CurrentPlayer = result.Payload;
                activity.ReadyUp(_Millis);
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
                //  update the player
                player.EndTime = DateTime.Now.ToUniversalTime();
                player.State = 4;
                //  update it
                await RestService.PostAsync<PlayerActivity, PlayerActivity>(string.Format($"{_BaseUrl}playeractivity/set", "activity"), player);
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
