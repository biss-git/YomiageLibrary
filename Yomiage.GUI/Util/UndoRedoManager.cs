using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yomiage.GUI.Util
{
    public sealed class UndoRedoManager<T> where T : class
    {
        private readonly LimitedStack<T> undoStack;
        private readonly LimitedStack<T> redoStack;

        private bool isContinuousStack = false;
        private T continuousStack;

        public UndoRedoManager(int capacity = 1000, bool distinct = true)
        {
            undoStack = new LimitedStack<T>(capacity, distinct);
            redoStack = new LimitedStack<T>(capacity, distinct);
        }

        public bool CanUndo => isContinuousStack || this.undoStack.Count > 1;
        public bool CanRedo => this.redoStack.Count > 0;

        public T Undo()
        {
            if (!CanUndo) { return default(T); }
            if (isContinuousStack)
            {
                isContinuousStack = false;
                this.redoStack.Push(continuousStack);
                return this.undoStack.Peek();
            }
            T state = this.undoStack.Pop();
            this.redoStack.Push(state);
            return this.undoStack.Peek();
        }

        public T Peek()
        {
            if(this.undoStack.Count == 0) { return default(T); }
            return this.undoStack.Peek();
        }

        public T Redo()
        {
            if (!CanRedo) { return default(T); }
            T state = this.redoStack.Pop();
            this.undoStack.Push(state);
            return state;
        }

        public void AddState(T state)
        {
            bool success = false;
            if (isContinuousStack)
            {
                isContinuousStack = false;
                success = this.undoStack.Push(continuousStack);
            }
            if(this.undoStack.Push(state) || success)
            {
                this.redoStack.Clear();
            }
        }

        public void AddState_Continuous(T state)
        {
            isContinuousStack = true;
            continuousStack = state;
            this.redoStack.Clear();
        }

        public void Clear()
        {
            redoStack.Clear();
            undoStack.Clear();
        }
    }
}
