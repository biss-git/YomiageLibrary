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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Yomiage.GUI.Dialog.Views
{
    /// <summary>
    /// PauseEditDialog.xaml の相互作用ロジック
    /// </summary>
    public partial class PauseEditDialog : UserControl
    {
        public PauseEditDialog()
        {
            InitializeComponent();
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            this.InputTextBox.Focus();
            Task.Run(async () =>
            {
                await Task.Delay(300);
                this.Dispatcher.Invoke(() =>
                {
                    this.InputTextBox.SelectAll();
                });
            });
        }
    }
}
