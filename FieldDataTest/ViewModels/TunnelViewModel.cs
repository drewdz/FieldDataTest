using DataFactory.Model;
using FieldDataTest.Commands;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;

namespace FieldDataTest.ViewModels
{
    public class TunnelViewModel : BaseViewModel, IViewModel
    {
        #region Constants

        private const int INTERVAL = 1000;         //  1 minute
        private const int FREQUENCY = 100;
        private const double VARIANCE = 4000;       //  4 seconds (+- 2 seconds)
        private const int RUN_TIME = 10000;         //  10 seconds
        private const string BASE_URL = "https://{0}-service-dot-hwp-legends.wl.r.appspot.com/";
        private const string STATE_POLLING = "Polling";
        private const string STATE_PROCESSING = "Processing";
        public const string STATE_STOP = "Stop";


        #endregion Constants

        #region Fields

        private Random _Random = new Random();
        private Timer _Timer;
        private List<string> _Resources;
        private int _Poll = 0;

        private Font _Font = new Font(FontFamily.GenericMonospace, 12);
        private Pen _Pen = new Pen(Brushes.Black, 3);

        #endregion Fields

        #region Constructors

        #endregion Constructors

        #region Properties

        private string _State = "Polling";
        public string State
        {
            get => _State;
            set => SetProperty(ref _State, value);
        }

        private ActivityGroup _Group = null;
        public ActivityGroup Group
        {
            get => _Group;
            set => SetProperty(ref _Group, value);
        }

        private int _Countdown;
        public int Countdown
        {
            get => _Countdown;
            set => SetProperty(ref _Countdown, value);
        }

        private string _Feedback = string.Empty;
        public string Feedback
        {
            get => _Feedback;
            set => SetProperty(ref _Feedback, value);
        }

        #endregion Properties

        #region Commands

        private ICommand _CancelCommand = null;
        public ICommand CancelCommand
        {
            get
            {
                _CancelCommand = _CancelCommand ?? new BasicCommand(Cancel);
                return _CancelCommand;
            }
        }

        #endregion Commands

        #region Operations

        public override void Initialize()
        {
            base.Initialize();
            _Resources = new List<string>();
            //  get resources
            _Resources = this.GetType().Assembly.GetManifestResourceNames().Where(n => n.Contains("image")).ToList();
            //  start polling
            Task.Run(PollAsync);
        }

        private async Task PollAsync()
        {
            try
            {
                do
                {
                    //  wait
                    await Task.Delay(FREQUENCY);
                    if (State.Equals(STATE_POLLING))
                    {
                        if (_Poll == INTERVAL)
                        {
                            _Poll = 0;
                            //  get data
                            using (var service = new RestService())
                            {
                                var result = await service.GetAsync<ServiceResult<ActivityGroup>>(string.Format($"{BASE_URL}activitygroup?activityId=1ce27578-99f0-47c5-ab9b-a6ba812f8d88", "activity"));
                                if (result.Status != ServiceResultStatus.Success)
                                {
                                    Debug.WriteLine($"{nameof(TunnelViewModel)}.{nameof(PollAsync)} - Polling failed: {result.Message}");
                                    AddFeedback($"Polling failed: {result.Message}");
                                    continue;
                                }
                                await ProcessAsync(result.Payload, service);
                            }
                        }
                        else
                        {
                            _Poll += 100;
                        }
                    }
                }
                while (!State.Equals(STATE_STOP));
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"{nameof(TunnelViewModel)}.{nameof(PollAsync)} - Exception: {ex}");
                AddFeedback(ex.Message);
            }
        }

        private async Task ProcessAsync(ActivityGroup group, RestService service)
        {
            if (Group == null)
            {
                if ((group == null) || (group.State >= 3))
                {
                    AddFeedback($"No active group record found. Group state: {group.State}");
                    return;
                }

                AddFeedback($"Group found: {group.Id}, State: {group.State}");
                Group = group;
                //  update state to Busy
                Group.State = 2;
                var result = await service.PostAsync<ServiceResult<ActivityGroup>, ActivityGroup>(string.Format($"{BASE_URL}activitygroup/update", "activity"), Group);
                if (result.Status != ServiceResultStatus.Success)
                {
                    Debug.WriteLine($"{nameof(TunnelViewModel)}.{nameof(ProcessAsync)} - Update Group Failed: {result.Message}");
                    throw new Exception(result.Message);
                }
                Group = result.Payload;
                //  randomize a countdown
                Countdown = (int)(((_Random.NextDouble() * VARIANCE) - (VARIANCE / 2)) + RUN_TIME);
                var countdown = Math.Round(Countdown / 1000D, 2);
                Debug.WriteLine($"{nameof(TunnelViewModel)}.{nameof(ProcessAsync)} - Countdown set to: {countdown}s, Starting Timer");
                Feedback = $"Waiting for players to run. Countdown = {countdown}s";
                if (_Timer != null) _Timer.Dispose();
                _Timer = new Timer(Trigger, null, _Countdown, RUN_TIME);
            }
            //  check that the group we received is the same as the one we already have
            if (!group.Id.Equals(Group.Id))
            {
                //  reset everything
                Group = null;
                Feedback = string.Empty;
            }
            else if (group.State != Group.State)
            {
                if (group.State < 3)
                {
                    //  this should never happen - state is changed but < 3
                    //  reset
                    Group = null;
                    Feedback = string.Empty;
                }
                else
                {
                    Group = group;
                }
            }
            else if (group.State == group.State)
            {
                //  this allows one cycle before taking action so that the UI can show something - maybe
                if (group.State > 2)
                {
                    //  reset
                    Group = null;
                    Feedback = string.Empty;
                }
            }
        }

        private void Trigger(object state)
        {
            //  don't let the timer tick again
            _Timer.Dispose();
            _Timer = null;
            AddFeedback("STATE = PROCESSING - PHOTOS TAKEN, CANCEL NOT AVAILABLE");
            //  stop polling
            State = STATE_PROCESSING;
            _ = Task.Run(async () =>
            {
                try
                {
                    using (var service = new RestService())
                    {
                        foreach (var player in Group.PlayerActivities)
                        {
                            if (player.PlayerActivityData == null) player.PlayerActivityData = new List<PlayerActivityData>();
                            AddFeedback($"Uploading images for player {player.Id}");
                            //  upload our 5 photos
                            foreach (var resource in _Resources)
                            {
                                AddFeedback($"Uploading image {resource}");
                                var data = GetPhotoWithMeta(resource);
                                var id = await UploadMedia(service, resource, data);
                                if (id == null) continue;
                                //  create and add the data record
                                player.PlayerActivityData.Add(new PlayerActivityData { PlayerActivityId = player.Id, MediaMetaId = id });
                            }
                        }
                        //  upload the final result
                        Group.State = 4;
                        var result = await service.PostAsync<ServiceResult<ActivityGroup>, ActivityGroup>(string.Format($"{BASE_URL}activitygroup/update", "activity"), Group);
                        if (result.Status != ServiceResultStatus.Success)
                        {
                            Debug.WriteLine($"{nameof(TunnelViewModel)}.{nameof(Trigger)} - Update Group Failed: {result.Message}");
                            throw new Exception(result.Message);
                        }
                        Group = result.Payload;
                        State = STATE_POLLING;
                    }
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"{nameof(TunnelViewModel)}.{nameof(Trigger)} - Exception: {ex}");
                    AddFeedback(ex.Message);
                    //  reset
                    Group = null;
                    Feedback = string.Empty;
                    State = STATE_POLLING;
                }
            });
        }

        private async Task<string> UploadMedia(RestService service, string resource, byte[] data)
        {
            Debug.WriteLine($"{nameof(TunnelViewModel)}.{nameof(UploadMedia)} - Uploading resource. ({resource})");
            AddFeedback($"Uploading resource. ({resource})");

            var parts = resource.Split(new char[] { '.' });
            var filename = $"{parts[parts.Length - 2]}.{parts[parts.Length - 1]}";

            var result = await service.PostFileDataAsync<ServiceResult<string>>(string.Format($"{BASE_URL}media/upload", "media"), "file", data, filename);
            return (result.Status == ServiceResultStatus.Success) ? result.Payload : null;
        }

        private byte[] GetPhotoWithMeta(string name)
        {
            Debug.WriteLine($"{nameof(TunnelViewModel)}.{nameof(GetPhotoWithMeta)} - Processing image ({name})");
            AddFeedback($"Processing image ({name})");
            var data = GetType().Assembly.GetManifestResourceStream(name);
            using (var source = (Bitmap)Image.FromStream(data))
            {
                using (var target = new Bitmap(source.Width, source.Height))
                {
                    using (var gc = Graphics.FromImage(target))
                    {
                        gc.DrawImage(source, 0, 0, source.Width, source.Height);
                        //  add date and time
                        var s = DateTime.Now.ToString("dd MMM yyyy HH:mm:ss");
                        var size = gc.MeasureString(s, _Font);
                        gc.DrawString(s, _Font, Brushes.Black, 20, 20);
                        s = $"Image: {name}";
                        gc.DrawString(s, _Font, Brushes.Black, 20, size.Height + 40);
                        gc.Flush();
                        Debug.WriteLine($"{nameof(TunnelViewModel)}.{nameof(GetPhotoWithMeta)} - Timestamp - {s} ({name})");
                        AddFeedback($"Timestamp - {s} ({name})");
                    }
                    using (var ms = new MemoryStream())
                    {
                        target.Save(ms, ImageFormat.Jpeg);
                        return ms.ToArray();
                    }
                }
            }
        }

        private void AddFeedback(string feedback)
        {
            Feedback += (string.IsNullOrWhiteSpace(Feedback)) ? feedback : $"\r\n{feedback}";
        }

        private void Cancel(object obj)
        {
            Task.Run(async () =>
            {
                try
                {
                    AddFeedback("Preparing to cancel group from Kiosk. Pause polling...");
                    State = STATE_PROCESSING;
                    using (var service = new RestService())
                    {
                        //  upload the final result
                        Group.State = 3;
                        var result = await service.PostAsync<ServiceResult<ActivityGroup>, ActivityGroup>(string.Format($"{BASE_URL}activitygroup/set", "activity"), Group);
                        if (result.Status != ServiceResultStatus.Success)
                        {
                            Debug.WriteLine($"{nameof(TunnelViewModel)}.{nameof(Trigger)} - Update Group Failed: {result.Message}");
                            throw new Exception(result.Message);
                        }
                        Group = result.Payload;
                        State = STATE_POLLING;
                        AddFeedback("Cancel complete. Resuming polling.");
                    }
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"{nameof(TunnelViewModel)}.{nameof(Trigger)} - Exception: {ex}");
                    AddFeedback(ex.Message);
                }
            });
        }

        #endregion Operations
    }
}
