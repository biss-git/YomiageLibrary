using Reactive.Bindings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yomiage.GUI.Data
{
    static class Status
    {
        public static ReactivePropertySlim<string> StatusText { get; } = new ReactivePropertySlim<string>("起動中");
    }
}
