using Reactive.Bindings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yomiage.GUI.Util
{
    public static class MvvmHelpersExtensions
    {
        public static IObservable<bool> ChangedAsObservable<T>(this ReactiveProperty<T> self, bool skipFirst = true)
        {
            var result = self.AsObservable();
            if (skipFirst)
            {
                result = result.Skip(1);
            }
            return result.Select(_ => true);
        }
    }
}
