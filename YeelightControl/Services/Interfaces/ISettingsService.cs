using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YeelightControl.Models;

namespace YeelightControl.Services.Interfaces
{
    public interface ISettingsService
    {
        Settings LoadSettings();
        void SaveSettings(Settings settings);
    }
}
