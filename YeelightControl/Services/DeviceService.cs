using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Catel.Collections;
using YeelightControl.Models;
using YeelightControl.Models.Enums;
using YeelightControl.Services.Interfaces;

namespace YeelightControl.Services
{
    public class DeviceService : IDeviceService
    {
        private const string DiscoveryHeader = "HTTP/1.1 200 OK";

        private readonly ISettingsService _settingsService;
        private readonly IYeelightMessenger _messenger;

        public DeviceService(ISettingsService settingsService, IYeelightMessenger messenger)
        {
            _settingsService = settingsService;
            _messenger = messenger;
        }

        public IEnumerable<Device> LoadDevices() =>
            _settingsService.LoadSettings().Devices;

        public Device LoadDevice(long id) =>
            _settingsService.LoadSettings().Devices.FirstOrDefault(d => d.Id == id);

        public void SaveDevice(Device device)
        {
            var settings = _settingsService.LoadSettings();
            settings.Devices.Remove(device);
            settings.Devices.Add(device);
            _settingsService.SaveSettings(settings);
        }

        public void SaveDevices(IEnumerable<Device> devices)
        {
            var settings = _settingsService.LoadSettings();
            settings.Devices = devices as ObservableCollection<Device> ?? new ObservableCollection<Device>(devices);
            _settingsService.SaveSettings(settings);
        }

        public IEnumerable<Device> DiscoverDevices()
        {
            return _messenger.SearchNetwork().SelectMany(ParseDiscovery).Distinct();
        }

        private IEnumerable<Device> ParseDiscovery(string str)
        {
            var lines = str.Split(new[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries);
            if (lines.Length == 0 || lines[0] != DiscoveryHeader)
                yield break;
            var device = new Device
            {
                LastUpdate = DateTime.Now
            };
            foreach (var line in lines.Skip(1))
            {
                var colonPos = line.IndexOf(':');
                var key = line.Substring(0, colonPos).Trim().ToUpper();
                var value = line.Substring(colonPos + 1).Trim();
                switch (key)
                {
                    case "LOCATION":
                        device.Location = new Uri(value);
                        break;
                    case "ID":
                        device.Id = Convert.ToInt64(value, 16);
                        break;
                    case "MODEL":
                        Model model;
                        if (!Enum.TryParse(value, true, out model))
                            yield break;
                        device.Model = model;
                        break;
                    case "FW_VER":
                        device.FirmwareVersion = Convert.ToUInt32(value);
                        break;
                    case "SUPPORT":
                        device.SupportedMethods = new ObservableCollection<string>(value.Split(' '));
                        break;
                    case "POWER":
                        device.IsOn = value.Equals("on", StringComparison.CurrentCultureIgnoreCase);
                        break;
                    case "BRIGHT":
                        device.Brightness = int.Parse(value);
                        break;
                    case "COLOR_MODE":
                        ColorMode colorMode;
                        if (!Enum.TryParse(value, true, out colorMode))
                            yield break;
                        device.ColorMode = colorMode;
                        break;
                    case "CT":
                        device.ColorTemperature = int.Parse(value);
                        break;
                    case "RGB":
                        device.Color = Color.FromInt(int.Parse(value));
                        break;
                    case "HUE":
                        device.Hue = int.Parse(value);
                        break;
                    case "SAT":
                        device.Saturation = int.Parse(value);
                        break;
                    case "NAME":
                        device.Name = value;
                        break;
                }
                
            }

            yield return device;
        }
    }
}