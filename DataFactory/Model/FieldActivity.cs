﻿using DataFactory.Generators;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Runtime.CompilerServices;

namespace DataFactory.Model
{
    public enum ActivityState
    {
        Ready,
        Waiting,
        ToQueue
    }

    public class FieldActivity : INotifyPropertyChanged
    {
        #region Events

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion Events

        #region Fields

        private List<TrackingTag> _Tags;
        protected PointF _Entrance = new PointF(30, 0);
        private List<EventData> _ActivityData;
        private TrackingTag _SelectedTag;
        private long _MaxMillis = 0;

        #endregion Fields

        #region Properties

        [JsonProperty]
        public string Id { get; set; }

        [JsonProperty]
        public string Name { get; set; }

        [JsonProperty]
        public string Type { get; set; }

        [JsonProperty]
        public string Color { get; set; }

        [JsonProperty]
        public BoundingBox Bounds { get; set; }

        [JsonProperty]
        public int Direction { get; set; }

        [JsonProperty]
        public BoundingBox QueuePoint { get; set; }

        [JsonProperty]
        public BoundingBox CollectionPoint { get; set; }

        [JsonProperty]
        public int TagCount { get; set; }

        [JsonProperty]
        public List<FieldActivity> Targets { get; set; }

        [JsonProperty]
        public bool UseBalls { get; set; }

        public IGenerator Generator { get; private set; }

        public ActivityState State { get; set; } = ActivityState.Waiting;

        #endregion Properties

        #region Init

        public void Init(long millis)
        {
            Debug.WriteLine($"Initializing {Type}: {millis}, {TagCount}");
            Generator = GeneratorFactory.Create(this);
            if (TagCount == 0) TagCount = 1;
            _Tags = new List<TrackingTag>();
            for (int i = 0; i < TagCount; i++)
            {
                var tag = new TrackingTag
                {
                    IsBall = UseBalls,
                    Timestamp = 0,
                    Used = true,
                    InUse = false
                };
                _Tags.Add(tag);
            }
            //  get to the queue point
            _MaxMillis = 0;
            GeneratePath(millis, false);
        }

        #endregion Init

        #region Operations

        public void ReadyUp(long millis)
        {
            if (State != ActivityState.Waiting) return;
            Debug.WriteLine($"Readyup {Name}");
            GeneratePath(millis, false);
        }

        public IEnumerable<EventData> CreateSamples(long millis, bool waitScan)
        {
            //  get a data point for this time
            var data = _ActivityData?.FirstOrDefault(d => d.Timestamp == millis);
            //  need to generate a new path
            if ((State != ActivityState.Waiting) && (data == null) && (millis > _MaxMillis))
            {
                GeneratePath(millis, waitScan);
                data = _ActivityData?.FirstOrDefault(d => d.Timestamp == millis);
            }
            //  return data
            foreach (var tag in _Tags)
            {
                if ((data != null) && (_SelectedTag != null) && (tag == _SelectedTag))
                {
                    //  the in-use tag moves
                    tag.X = data.X; tag.Y = data.Y; tag.Z = data.Z; tag.V = data.V; tag.R = data.R;
                    yield return new EventData { Timestamp = data.Timestamp, TagId = tag.TagId, X = data.X, Y = data.Y, V = data.V, R = data.R };
                }
                else
                {
                    yield return tag.ToEventData(millis);
                }
                //  make sure all the tags are at the queue point when this state changes
                if ((data != null) && (State == ActivityState.ToQueue))
                {
                    tag.X = data.X; tag.Y = data.Y; tag.Z = data.Z; tag.V = data.V; tag.R = data.R;
                }
            }
            //  remove data point from list
            if ((_ActivityData != null) && (data != null)) _ActivityData.Remove(data);
        }

        private void GeneratePath(long millis, bool waitScan)
        {
            var count = _Tags.Count(t => (t != _SelectedTag) && (!t.Used));
            if (count == 0)
            {
                //  all tags used - need to take back to the queue point
                var to = new PointF(QueuePoint.X0, QueuePoint.Y0);
                var from = (_MaxMillis == 0) ? _Entrance : new PointF(CollectionPoint.X0, CollectionPoint.Y0);
                _ActivityData = ((GeneratorBase)Generator).Walk(millis, string.Empty, new List<PointF> { from, to });
                //  reset all tags
                foreach (var tag in _Tags)
                {
                    tag.InUse = false;
                    tag.Used = false;
                }
                _MaxMillis = _ActivityData.Max(d => d.Timestamp);
                //  walk back to the queue point
                State = ActivityState.ToQueue;
            }
            else if (!waitScan)
            {
                //  generate a new action
                _ActivityData = Generator.Generate(millis);
                if (_SelectedTag != null)
                {
                    _SelectedTag.Used = true;
                    _SelectedTag.InUse = false;
                }
                //  select a new tag
                _SelectedTag = _Tags.FirstOrDefault(t => !t.Used && !t.InUse);
                _SelectedTag.InUse = true;
                _MaxMillis = _ActivityData.Max(d => d.Timestamp);
                State = ActivityState.Ready;
            }
            else
            {
                State = ActivityState.Waiting;
            }
        }

        private void RaisePropertyChanged([CallerMemberName] string name = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        #endregion Operations
    }
}
