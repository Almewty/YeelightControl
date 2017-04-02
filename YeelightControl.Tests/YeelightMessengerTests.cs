using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using NUnit.Framework;
using YeelightControl.Models.Enums;
using YeelightControl.Services;

namespace YeelightControl.Tests
{
    [TestFixture]
    public class YeelightMessengerTests
    {
        [Test]
        public void TestSearch()
        {
            using (var messenger = new YeelightMessenger())
            {
                var deviceService = new DeviceService(new SettingsService(), messenger);
                var devices = deviceService.DiscoverDevices().ToList();
                Console.WriteLine($"{devices.Count} Devices found");
                Assert.AreEqual(3, devices.Count);
                Parallel.ForEach(devices, device =>
                {
                    using (var controller = new DeviceController(device.Location))
                    {
                        controller.Connect();
                        controller.SetColorTemperature(2700, Effect.Sudden, 1000);
                        Thread.Sleep(1000);
                        controller.SetColorTemperature(6000, Effect.Sudden, 1000);
                        Thread.Sleep(1000);
                        controller.SetColorTemperature(2700, Effect.Sudden, 1000);
                        Thread.Sleep(1000);
                        controller.SetColorTemperature(6000, Effect.Sudden, 1000);
                        controller.Disconnect();
                    }
                });
            }
        }
    }
}