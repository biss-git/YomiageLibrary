using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Yomiage.GUI.Dialog
{
    /// <summary>
    /// WaitDialog.xaml の相互作用ロジック
    /// </summary>
    public partial class WaitDialog : Window , IDisposable
    {
        public Action CancelAction;

        public WaitDialog()
        {
            InitializeComponent();
        }

        public void SetProgress(string state, double progress)
        {
            this.Dispatcher.Invoke(() =>
            {
                this.text.Text = state;
                this.progress.Value = Math.Clamp(progress, 0, 1);
            });
        }

        public void Dispose()
        {
            this.Close();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if(CancelAction != null)
            {
                CancelAction();
            }
            this.cancelButton.IsEnabled = false;
        }
    }
}
