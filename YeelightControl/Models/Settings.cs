using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Catel.Data;

namespace YeelightControl.Models
{
    public class Settings : SavableModelBase<Settings>
    {
        public ObservableCollection<Device> Devices { get; set; }
    }
}
