using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yomiage.Core.Types;

namespace Yomiage.Core.Models
{
    public class VoiceEngineService : IDisposable
    {

        private readonly ObservableCollection<Engine> engines = new ObservableCollection<Engine>();
        public ReadOnlyObservableCollection<Engine> AllEngines { get; }

        public VoiceEngineService()
        {
            AllEngines = new ReadOnlyObservableCollection<Engine>(engines);
        }

        public bool Add(Engine engine)
        {
            if( engine.EngineConfig == null ||
                string.IsNullOrWhiteSpace(engine.EngineConfig.Key) ||
                engine.EngineConfig.Key == "Key" ||
                engines.Any(e => e.EngineConfig?.Key == engine.EngineConfig.Key))
            {
                return false;
            }
            engines.Add(engine);
            return true;
        }

        public void Dispose()
        {
            foreach(var engine in engines)
            {
                engine.VoiceEngine.Dispose();
            }
            engines.Clear();
        }

    }
}
