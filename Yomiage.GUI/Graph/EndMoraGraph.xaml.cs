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
    /// EndMoraGraph.xaml の相互作用ロジック
    /// </summary>
    public partial class EndMoraGraph : UserControl
    {

        private EndSection end;
        public EndSection End
        {
            get
            {
                return end;
            }
            set
            {
                end = value;
                this.moraText.Text = value.EndSymbol;
            }
        }

        public Action<EndSection, string> Action { get; set; }

        public EndMoraGraph()
        {
            InitializeComponent();
        }

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            if (sender is MenuItem item && Action != null)
            {
                switch (item.Header.ToString())
                {
                    case "通常。": end.EndSymbol = "。"; break;
                    case "呼びかけ♪": end.EndSymbol = "♪"; break;
                    case "疑問？": end.EndSymbol = "？"; break;
                    case "断定！": end.EndSymbol = "！"; break;
                    case "なし": end.EndSymbol = ""; break;
                }
                Action(this.end, item.Header.ToString());
            }
        }

        private void moraText_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if(e.ChangedButton != MouseButton.Right) { return; }
            this.end1.IsEnabled = (end?.EndSymbol != "。");
            this.end2.IsEnabled = (end?.EndSymbol != "♪");
            this.end3.IsEnabled = (end?.EndSymbol != "？");
            this.end4.IsEnabled = (end?.EndSymbol != "！");
            this.end5.IsEnabled = (end?.EndSymbol != "");
        }
    }
}
