/*      This monitor acts as a TCP Client and will try to connect to a configured endpoint
 *      It also collates incoming tag data and serializes it in order to send it
 */
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Threading;

namespace DataMonitor
{
    public class Monitor
    {
        class QueueData
        {
            public string Id { get; set; } = Guid.NewGuid().ToString();

            public string Data { get; set; }
        }

        #region Fields

        private Timer _Timer;
        private TcpClient _Client;
        private List<QueueData> _Queue = new List<QueueData>();

        #endregion Fields

        #region Properties

        [JsonProperty]
        public string Address { get; set; }

        [JsonProperty]
        public int Port { get; set; }

        [JsonProperty]
        public bool Simulate { get; set; } = false;

        #endregion Properties

        #region Operations

        public void Start()
        {
            try
            {
                //  TODO: connect to server
                if (!Simulate) _Client = new TcpClient(Address, Port);
                //  start timer
                if (Simulate)
                {
                    _Timer = new Timer(SendDataSim, null, 0, 100);
                }
                else
                {
                    _Timer = new Timer(SendData, null, 0, 100);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Exception: {ex}");
            }
        }

        public void Stop()
        {
            try
            {
                if (_Timer != null)
                {
                    _Timer.Dispose();
                    _Timer = null;
                }
                if (_Client != null)
                {
                    _Client.Close();
                    _Client.Dispose();
                    _Client = null;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Exception: {ex}");
            }
        }

        public void AddData(string data)
        {
            _Queue.Add(new QueueData { Data = data });
        }

        private List<QueueData> GetData()
        {
            if ((_Queue == null) || (_Queue.Count == 0)) return new List<QueueData>();
            var data = new QueueData[_Queue.Count];
            _Queue.CopyTo(data);
            return data.ToList();
        }

        private void RemoveData(List<QueueData> data)
        {
            if ((_Queue == null) || (_Queue.Count == 0)) return;
            foreach (var item in data)
            {
                var q = _Queue.FirstOrDefault(i => i.Id.Equals(item.Id));
                if (q == null) continue;
                _Queue.Remove(q);
            }
        }

        #endregion Operations

        #region Callbacks

        private void SendData(object o)
        {
            try
            {
                if (!_Client.Connected) throw new Exception("Client not connected");
                //  get data to send
                var queue = GetData();
                if ((queue == null) || (queue.Count == 0)) return;
                var stream = _Client.GetStream();
                using (var writer = new StreamWriter(stream))
                {
                    foreach (var item in queue)
                    {
                        writer.WriteLine(item.Data);
                    }
                    writer.Flush();
                }
                //  clear sent items from the list
                RemoveData(queue);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Could not send data - Exception: {ex}");
            }
        }

        private void SendDataSim(object o)
        {
            try
            {
                if (!Simulate) throw new Exception("Not in simulate mode");
                //  get data to send
                var queue = GetData();
                if ((queue == null) || (queue.Count == 0))
                {
                    Debug.WriteLine("Sending - Nothing to send");
                    return;
                }
                foreach (var item in queue)
                {
                    Debug.WriteLine($"Sending - {item.Data}");
                }
                //  clear sent items from the list
                RemoveData(queue);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Could not send data - Exception: {ex}");
            }
        }

        #endregion Callbacks
    }
}
