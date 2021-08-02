using MahApps.Metro.Controls;
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

namespace Yomiage.GUI.Graph.Dialog
{
    /// <summary>
    /// YomiEditorDialog.xaml の相互作用ロジック
    /// </summary>
    public partial class YomiEditorDialog : MetroWindow
    {
        public bool Ok = false;

        public YomiEditorDialog()
        {
            InitializeComponent();
        }

        private void Button_Cancel(object sender, RoutedEventArgs e)
        {
            this.Ok = false;
            this.Close();
        }

        private void Button_Ok(object sender, RoutedEventArgs e)
        {
            this.Ok = true;
            this.Close();
        }

        private void MetroWindow_Loaded(object sender, RoutedEventArgs e)
        {
            this.text.Focus();
            this.text.SelectAll();
        }

        private void text_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                Button_Ok(null, null);
            }
            if (e.Key == Key.Escape)
            {
                Button_Cancel(null, null);
            }
        }
    }
}
