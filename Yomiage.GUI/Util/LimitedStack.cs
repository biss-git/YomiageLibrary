using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yomiage.GUI.Util
{
    public class LimitedStack<T> where T: class
    {
        private List<T> list = new List<T>();

        private int capacity;
        private bool distinct;

        public LimitedStack(int capacity = 1000, bool distinct = true)
        {
            this.capacity = Math.Max(1, capacity);
            this.distinct = distinct;
        }

        public int Count => this.list.Count;

        public T Pop()
        {
            if(this.list.Count == 0) { return default(T); }
            var state = this.list.First();
            this.list.Remove(state);
            return state;
        }

        public bool Push(T state)
        {
            if (distinct && this.list.Count > 0 && state.Equals(this.list.FirstOrDefault())) { return false; }
            this.list.Insert(0, state);
            if (this.list.Count > this.capacity)
            {
                this.list.Remove(this.list.Last());
            }
            return true;
        }

        public T Peek() => this.list.FirstOrDefault();
        public void Clear() => this.list.Clear();
    }
}
