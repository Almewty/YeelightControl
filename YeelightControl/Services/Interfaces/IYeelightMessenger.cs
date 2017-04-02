using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YeelightControl.Services.Interfaces
{
    public interface IYeelightMessenger
    {
        IEnumerable<string> SearchNetwork(int timeOut = 1000);
    }
}