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
using Yomiage.SDK.Talk;

namespace Yomiage.GUI.Graph
{
    /// <summary>
    /// UpDown.xaml の相互作用ロジック
    /// </summary>
    public partial class UpDown : UserControl
    {
        public UpDown(Mora mora)
        {
            InitializeComponent();
            this.up.Visibility = mora.Accent ? Visibility.Collapsed : Visibility.Visible;
            this.down.Visibility = mora.Accent ? Visibility.Visible : Visibility.Collapsed;
            this.Mora = mora;
        }

        public Action<Mora, string> Action { get; init; }

        public Mora Mora { get; }

        private void Up_MouseEnter(object sender, MouseEventArgs e)
        {
            Action(Mora, "MouseEnter");
        }

        private void Up_MouseLeave(object sender, MouseEventArgs e)
        {
            Action(Mora, "MouseLeave");
        }

        private void Up_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Action(Mora, "ToggleAccent");
        }
    }
}
