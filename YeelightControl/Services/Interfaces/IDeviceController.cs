using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YeelightControl.Models;
using YeelightControl.Models.Enums;

namespace YeelightControl.Services.Interfaces
{
    public interface IDeviceController : IDisposable
    {
        bool IsConnected { get; }

        void Connect();
        void Disconnect();

        string GetProperty(Property property);
        IDictionary<Property, object> GetProperties(params Property[] properties);

        void SetColorTemperature(int temperature, Effect effect, int duration);
        void SetRgb(Color color, Effect effect, int duration);
        void SetHsv(int hue, int sat, Effect effect, int duration);
        void SetBrightness(int brightness, Effect effect, int duration);
        void SetPower(bool on, Effect effect, int duration);
        void Toogle();
        void SetDefault();

        // TODO: implement rest (page 13)
    }
}
