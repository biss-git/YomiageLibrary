using Prism.Ioc;
using Prism.Modularity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yomiage.Core.Models;

namespace Yomiage.Core
{
    public class CoreModule : IModule
    {
        public void OnInitialized(IContainerProvider containerProvider)
        {
        }

        public void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.RegisterSingleton<VoiceEngineService>();
            containerRegistry.RegisterSingleton<VoiceLibraryService>();
            containerRegistry.RegisterSingleton<VoicePresetService>();
        }
    }
}
