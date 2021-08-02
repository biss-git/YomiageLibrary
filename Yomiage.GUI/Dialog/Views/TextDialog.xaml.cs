using ControlzEx.Theming;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Yomiage.GUI.Dialog.Views
{
    /// <summary>
    /// TextDialog.xaml の相互作用ロジック
    /// </summary>
    public partial class TextDialog : UserControl
    {
        public TextDialog()
        {
            InitializeComponent();
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            if(this.Parent is MetroWindow window)
            {
                ThemeManager.Current.ChangeTheme(window, "Light.Aoi");
            }

        }
    }
}
