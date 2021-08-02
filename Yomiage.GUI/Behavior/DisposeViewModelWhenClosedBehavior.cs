using Microsoft.Xaml.Behaviors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Yomiage.GUI.Behavior
{
    class DisposeViewModelWhenClosedBehavior : Behavior<Window>
    {
        protected override void OnAttached()
        {
            base.OnAttached();
            AssociatedObject.Closed += AssociatedObject_Closed;
        }

        private void AssociatedObject_Closed(object sender, EventArgs e) => (AssociatedObject.DataContext as IDisposable)?.Dispose();

        protected override void OnDetaching()
        {
            base.OnDetaching();
            AssociatedObject.Closed -= AssociatedObject_Closed;
        }
    }
}
