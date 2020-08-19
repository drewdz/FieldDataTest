using DataFactory;
using DataFactory.Model;
using MvvmCross.Commands;
using MvvmCross.ViewModels;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace FieldData.Core.ViewModels
{
    public class MainViewModel : MvxViewModel
    {
        #region Fields

        private Timer _FieldTimer;
        private long _Millis;

        #endregion Fields

        #region Constuctors

        public MainViewModel()
        {
        }

        #endregion Constuctors

        #region Properties

        private bool _ScanOn = false;
        public bool ScanOn
        {
            get => _ScanOn;
            set => SetProperty(ref _ScanOn, value);
        }

        private bool _TxOn = false;
        public bool TxOn
        {
            get => _TxOn; 
            set => SetProperty(ref _TxOn, value); 
        }

        private bool _Running = false;
        public bool Running
        {
            get => _Running; 
            set => SetProperty(ref _Running, value); 
        }


        public PlayingField Field;

        public List<EventData> Data { get; set; } = new List<EventData>();

        #endregion Properties

        #region Actions

        public Action OpenFileAction { get; set; }

        #endregion Actions

        #region Commands

        private IMvxCommand _LoadCommand = null;
        public IMvxCommand LoadCommand
        {
            get
            {
                _LoadCommand = _LoadCommand ?? new MvxCommand(() =>
                {
                    OpenFileAction?.Invoke();
                });
                return _LoadCommand;
            }
        }

        private IMvxCommand _StartCommand = null;
        public IMvxCommand StartCommand
        {
            get
            {
                _StartCommand = _StartCommand ?? new MvxCommand(() =>
                {
                    if (Running)
                    {
                        RunField();
                    }
                    else
                    {
                        if (_FieldTimer == null) return;
                        _FieldTimer.Dispose();
                        _FieldTimer = null;
                    }
                });
                return _StartCommand;
            }
        }

        private IMvxCommand _ToggleScanCommand = null;
        public IMvxCommand ToggleScanCommand
        {
            get
            {
                _ToggleScanCommand = _ToggleScanCommand ?? new MvxCommand(() => Debug.WriteLine("Toggle scan pressed"));
                return _ToggleScanCommand;
            }
        }

        private IMvxCommand _ToggleTxCommand = null;
        public IMvxCommand ToggleTxCommand
        {
            get
            {
                _ToggleTxCommand = _ToggleTxCommand ?? new MvxCommand(() => Debug.WriteLine("Toggle transmit pressed"));
                return _ToggleTxCommand;
            }
        }

        private IMvxCommand _OpenFileCommand = null;
        public IMvxCommand OpenFileCommand
        {
            get
            {
                _OpenFileCommand = _OpenFileCommand ?? new MvxCommand<string>((s) =>
                {
                    Field = Serializer.Deserialize<PlayingField>(s);
                    //  convert to meters
                    foreach (var activity in Field.Activities)
                    {
                        activity.Init(0);
                        //activity.Bounds.X0 *= DataFactory.Constants.FEET_TO_METERS;
                        //activity.Bounds.Y0 *= DataFactory.Constants.FEET_TO_METERS;
                        //activity.Bounds.X1 *= DataFactory.Constants.FEET_TO_METERS;
                        //activity.Bounds.Y1 *= DataFactory.Constants.FEET_TO_METERS;
                        if (activity.Targets != null)
                        {
                            //foreach (var target in activity.Targets)
                            //{
                            //    target.Bounds.X0 *= DataFactory.Constants.FEET_TO_METERS;
                            //    target.Bounds.Y0 *= DataFactory.Constants.FEET_TO_METERS;
                            //}
                        }
                        //activity.QueuePoint.X0 *= DataFactory.Constants.FEET_TO_METERS;
                        //activity.QueuePoint.Y0 *= DataFactory.Constants.FEET_TO_METERS;
                        //activity.CollectionPoint.X0 *= DataFactory.Constants.FEET_TO_METERS;
                        //activity.CollectionPoint.Y0 *= DataFactory.Constants.FEET_TO_METERS;
                    }
                });
                return _OpenFileCommand;
            }
        }

        #endregion Commands

        #region Operations

        private void RunField()
        {
            _Millis = DateTime.Now.Subtract(Constants.UnixEpoch).Ticks / 10000;
            Debug.WriteLine($"Start: {_Millis}");
            foreach (var activity in Field.Activities) activity.Init(_Millis);
            _FieldTimer = new Timer(Tick, ScanOn, 0, 100);
        }

        private void Tick(object o)
        {
            if (Data.Count > 0)
            {
                var millis = Data.Max(d => d.Timestamp) - 10000;
                Data.RemoveAll(d => d.Timestamp < millis);
            }
            var waitScan = (o == null) ? false : (bool)o;
            foreach (var activity in Field.Activities)
            {
                var data = activity.CreateSamples(_Millis, waitScan);
                //  stream data
                var d = data.ToList();
                //Task.Run(() => StreamData(d));
                Data.AddRange(d);
            }
            _Millis += 100;
        }

        #endregion Operations
    }
}
