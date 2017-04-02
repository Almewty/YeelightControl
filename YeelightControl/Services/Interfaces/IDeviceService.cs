using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YeelightControl.Models;

namespace YeelightControl.Services.Interfaces
{
    public interface IDeviceService
    {
        IEnumerable<Device> LoadDevices();
        Device LoadDevice(long id);
        void SaveDevice(Device device);
        void SaveDevices(IEnumerable<Device> devices);

        IEnumerable<Device> DiscoverDevices();
    }
}
