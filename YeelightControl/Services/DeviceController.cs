using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Catel;
using Newtonsoft.Json;
using YeelightControl.Models;
using YeelightControl.Models.Enums;
using YeelightControl.Services.Interfaces;

namespace YeelightControl.Services
{
    public class DeviceController : IDeviceController
    {
        private readonly IPEndPoint _endPoint;
        private readonly TcpClient _client;
        private readonly IDictionary<int, TaskCompletionSource<IEnumerable<object>>> _completionSources;

        private CancellationTokenSource _tokenSource;

        private StreamWriter _jsonWriter;
        private StreamReader _jsonReader;

        private int _idCounter;

        public bool IsConnected => _client.Connected;

        public DeviceController(Uri location)
        {
            _completionSources = new ConcurrentDictionary<int, TaskCompletionSource<IEnumerable<object>>>();
            _endPoint = new IPEndPoint(IPAddress.Parse(location.Host), location.Port);
            _client = new TcpClient();
        }

        public void Connect()
        {
            _client.Connect(_endPoint);
            var stream = _client.GetStream();
            _jsonWriter = new StreamWriter(stream) { AutoFlush = true };
            _jsonReader = new StreamReader(stream);
            _tokenSource = new CancellationTokenSource();
            new Thread(ReaderThread).Start();
        }

        public void Disconnect()
        {
            _tokenSource.Cancel();
            _jsonReader.Close();
            _jsonWriter.Close();
            _client.Close();
        }

        public string GetProperty(Property property)
        {
            var method = CamelCaseToUnderscore(property.ToString());
            var res = SendCommandAsync("get_prop", method).Result;
            return res.First().ToString();
        }

        public bool GetPropertyAsBool(Property property)
        {
            var res = GetProperty(property);
            return res == "on";
        }

        public int GetPropertyAsInt(Property property)
        {
            var res = GetProperty(property);
            return int.Parse(res);
        }

        public IDictionary<Property, object> GetProperties(params Property[] properties)
        {
            var methods = properties.Select(p => p.ToString()).Select(CamelCaseToUnderscore);
            var res = SendCommandAsync("get_prop", methods.Cast<object>().ToArray()).Result.ToList();
            return res.ToDictionary(o => properties[res.IndexOf(o)]);
        }

        public void SetColorTemperature(int temperature, Effect effect, int duration)
        {
            SendCommandAsync("set_ct_abx", temperature, effect.ToString().ToLower(), duration).Wait();
        }

        public void SetRgb(Color color, Effect effect, int duration)
        {
            throw new NotImplementedException();
        }

        public void SetHsv(int hue, int sat, Effect effect, int duration)
        {
            throw new NotImplementedException();
        }

        public void SetBrightness(int brightness, Effect effect, int duration)
        {
            throw new NotImplementedException();
        }

        public void SetPower(bool @on, Effect effect, int duration)
        {
            throw new NotImplementedException();
        }

        public void Toogle()
        {
            throw new NotImplementedException();
        }

        public void SetDefault()
        {
            throw new NotImplementedException();
        }

        public void Dispose()
        {
            _client.Dispose();
        }

        private Task<IEnumerable<object>> SendCommandAsync(string method, params object[] parameters)
        {
            var messageId = Interlocked.Increment(ref _idCounter);

            var request = new RequestMessage
            {
                id = messageId,
                method = method,
                @params = parameters.ToList()
            };

            var json = JsonConvert.SerializeObject(request);
            _jsonWriter.WriteLine(json);

            var completionSource = new TaskCompletionSource<IEnumerable<object>>();
            new CancellationTokenSource(1000).Token.Register(() =>
            {
                if (completionSource.Task.IsCompleted)
                    return;
                completionSource.SetCanceled();
                _completionSources.Remove(messageId);
            });
            _completionSources[messageId] = completionSource;
            return completionSource.Task;
        }

        private void ReaderThread()
        {
            try
            {
                string line;
                while ((line = _jsonReader.ReadLine()) != null)
                {
                    var answer = JsonConvert.DeserializeObject<AnswerMessage>(line);
                    if (
                        !_completionSources.TryGetValue(answer.id,
                            out TaskCompletionSource<IEnumerable<object>> completionSource))
                        continue;
                    completionSource.SetResult(answer.result);
                }
            }
            catch (Exception e)
                when (e is NullReferenceException ||
                      e is IOException ||
                      e is ObjectDisposedException)
            {
                // ignore
            }
        }

        private string CamelCaseToUnderscore(string camelCaseString)
        {
            return string.Concat(
                camelCaseString.Select(
                    (s, i) => i > 0 && char.IsUpper(s) ? $"_{char.ToLower(s)}" : char.ToLower(s).ToString()));
        }

        // Json Serialization is used. keys are lowercase in json so...
#pragma warning disable IDE1006 // Naming Styles
        // ReSharper disable InconsistentNaming
        private class RequestMessage
        {
            public int id { get; set; }
            public string method { get; set; }
            public List<object> @params { get; set; }
        }

        private class AnswerMessage
        {
            public int id { get; set; }
            public List<object> result { get; set; }
        }

        // ReSharper restore InconsistentNaming
#pragma warning restore IDE1006 // Naming Styles
    }
}