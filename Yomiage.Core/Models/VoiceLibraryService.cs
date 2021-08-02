using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yomiage.Core.Types;

namespace Yomiage.Core.Models
{
    public class VoiceLibraryService : IDisposable
    {
        private readonly ObservableCollection<Library> librarys = new ObservableCollection<Library>();
        public ReadOnlyObservableCollection<Library> AllLibrarys { get; }

        public VoiceLibraryService()
        {
            AllLibrarys = new ReadOnlyObservableCollection<Library>(librarys);
        }

        public bool Add(Library library)
        {
            if (library.LibraryConfig == null ||
                string.IsNullOrWhiteSpace(library.LibraryConfig.Key) ||
                library.LibraryConfig.Key == "Key" ||
                librarys.Any(l => l.LibraryConfig?.Key == library.LibraryConfig.Key))
            {
                return false;
            }
            librarys.Add(library);
            return true;
        }

        public void Dispose()
        {
            foreach (var library in librarys)
            {
                library.VoiceLibrary.Dispose();
            }
            librarys.Clear();
        }
    }
}
