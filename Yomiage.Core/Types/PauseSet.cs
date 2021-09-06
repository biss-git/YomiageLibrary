using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Text;

namespace Yomiage.Core.Types
{
    public class PauseSet : BindableBase
    {
        private string _key;
        public string key
        {
            get => _key;
            set => SetProperty(ref _key, value);
        }
        private int _span_ms;
        public int span_ms
        {
            get => _span_ms;
            set => SetProperty(ref _span_ms, value);
        }

        public PauseSet(string key, int span_ms)
        {
            this.key = key;
            this.span_ms = span_ms;
        }
    }
}
