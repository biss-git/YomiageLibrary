using MahApps.Metro.Controls;
using Prism.Services.Dialogs;
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
    /// MetroDialogWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class MetroDialogWindow : MetroWindow, IDialogWindow
    {
        public MetroDialogWindow()
        {
            InitializeComponent();
        }

        public IDialogResult Result { get; set; }

        private void MetroWindow_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                this.Close();
            }
        }
    }
}
