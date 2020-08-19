using DataFactory;
using DataFactory.Model;
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
    public partial class MainForm : Form
    {
        #region Constants

        private const float FIELD_WIDTH = 360f;
        private const float FIELD_HEIGHT = 160f;

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
                    activity.Init(0);
                    activity.Bounds.X0 *= DataFactory.Constants.FEET_TO_METERS;
                    activity.Bounds.Y0 *= DataFactory.Constants.FEET_TO_METERS;
                    activity.Bounds.X1 *= DataFactory.Constants.FEET_TO_METERS;
                    activity.Bounds.Y1 *= DataFactory.Constants.FEET_TO_METERS;
                    if (activity.Targets != null)
                    {
                        foreach (var target in activity.Targets)
                        {
                            target.Bounds.X0 *= DataFactory.Constants.FEET_TO_METERS;
                            target.Bounds.Y0 *= DataFactory.Constants.FEET_TO_METERS;
                        }
                    }
                    activity.QueuePoint.X0 *= DataFactory.Constants.FEET_TO_METERS;
                    activity.QueuePoint.Y0 *= DataFactory.Constants.FEET_TO_METERS;
                    activity.CollectionPoint.X0 *= DataFactory.Constants.FEET_TO_METERS;
                    activity.CollectionPoint.Y0 *= DataFactory.Constants.FEET_TO_METERS;
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

        private void LoadMenu_Click(object sender, EventArgs e)
        {

        }

        private void MainForm_Load(object sender, System.EventArgs e)
        {
            _Background = new Bitmap(GetType(), "field.png");
            //_Background = new Bitmap(GetType(), "pitch.png");
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
            new Thread(LoggerLoop).Start();
        }

        private void StopMenu_Click(object sender, EventArgs e)
        {
            _RunLogger = false;
        }

        private void MainImage_MouseUp(object sender, MouseEventArgs e)
        {
            Debug.WriteLine($"Mouse click: {e.X}, {e.Y}");
            if (_Field == null) return;
            //  debounce the click
            if (DateTime.Now.Subtract(_LastClick).TotalMilliseconds < 700) return;
            _LastClick = DateTime.Now;

            foreach (var activity in _Field.Activities)
            {
                float x0 = activity.Bounds.X0 * _ScaleX, y0 = activity.Bounds.Y0 * _ScaleY;
                float x1 = activity.Bounds.X1 * _ScaleX, y1 = activity.Bounds.Y1 * _ScaleY;
                float w = x1 - x0, h = y1 - y0;
                x0 += 5;
                y0 += 5;
                var rect = new Rectangle((int)x0, (int)(_Height - y0 - h), (int)w, (int)h);
                if (rect.IntersectsWith(new Rectangle(e.X, e.Y, 1, 1)))
                {
                    Debug.WriteLine($"You have clicked {activity.Name}");
                    activity.ReadyUp(_Millis);
                    break;
                }
            }
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
                Task.Run(() => _LogData.AddRange(data.Select(d => d.Copy()).ToList()));
                _Data.AddRange(data.Select(d => d.Copy()).ToList());
            }
            _Millis += 100;
        }

        private void RenderLoop(object sender, EventArgs e)
        {
            try
            {
                var data = _Data.ToList();
                //  get graphics context
                _Width = (float)MainImage.ClientRectangle.Width;
                _Height = (float)MainImage.ClientRectangle.Height;

                var margin = 5;

                _ScaleX = (_Width - (2 * margin)) / (FIELD_WIDTH + _Field.Padding.X0 + _Field.Padding.X1);
                _ScaleY = (_Height - (2 * margin)) / (FIELD_HEIGHT + _Field.Padding.Y0 + _Field.Padding.Y1);

                Bitmap bmp = new Bitmap((int)_Width, (int)_Height);
                using (var gc = Graphics.FromImage(bmp))
                {

                    gc.Clear(Color.DarkGray);

                    gc.DrawImage(_Background, margin + (_Field.Padding.X0 * _ScaleX), margin + (_Field.Padding.Y0 * _ScaleY), _Width - (2 * margin), _Height - (2 * margin));

                    //  draw activities
                    if ((_Field != null) && (_Field.Activities != null) && (_Field.Activities.Count > 0))
                    {
                        foreach (var activity in _Field.Activities)
                        {
                            float x0 = (activity.Bounds.X0 + _Field.Padding.X0) * _ScaleX, y0 = (activity.Bounds.Y0 + _Field.Padding.Y0) * _ScaleY;
                            float x1 = (activity.Bounds.X1 + _Field.Padding.X0) * _ScaleX, y1 = (activity.Bounds.Y1 + _Field.Padding.Y0) * _ScaleY;
                            float w = x1 - x0, h = y1 - y0;
                            x0 += margin;
                            y0 += margin;
                            var p = (activity.State == ActivityState.Ready) ? Pens.Orange : (activity.State == ActivityState.Waiting) ? Pens.LightGreen : Pens.Red;
                            gc.DrawRectangle(p, x0, _Height - y0 - h, w, h);
                            //  queue point
                            x0 = activity.QueuePoint.X0 * _ScaleX; y0 = activity.QueuePoint.Y0 * _ScaleY;
                            x0 += margin;
                            y0 += margin;
                            gc.FillEllipse(Brushes.Black, x0, _Height - y0 - 5, 5, 5);
                            //  collect point
                            x0 = activity.CollectionPoint.X0 * _ScaleX; y0 = activity.CollectionPoint.Y0 * _ScaleY;
                            x0 += margin;
                            y0 += margin;
                            gc.FillEllipse(Brushes.Red, x0, _Height - y0 - 5, 5, 5);
                        }
                    }

                    if ((data != null) && (data.Count() > 0))
                    {
                        foreach (var d in data)
                        {
                            if (d == null) continue;
                            gc.DrawRectangle(Pens.Black, ((d.X + _Field.Padding.X0) * _ScaleX) + margin, _Height - (((d.Y + _Field.Padding.Y0) * _ScaleY) + margin), 1, 1);
                        }
                    }
                    data = null;
                    gc.Flush();
                }
                MainImage.Image = bmp;
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
                Debug.WriteLine($"Exception: {ex}");
            }
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

        #endregion Helpers
    }
}
