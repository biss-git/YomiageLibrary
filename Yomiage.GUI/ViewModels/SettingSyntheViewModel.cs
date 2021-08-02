using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yomiage.GUI.Models;

namespace Yomiage.GUI.ViewModels
{
    class SettingSyntheViewModel : ViewModelBase
    {
        public SettingService SettingService { get; }
        public SettingSyntheViewModel(SettingService settingService) : base()
        {
            this.SettingService = settingService;
        }
    }
}
