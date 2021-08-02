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
    /// PauseGraph.xaml の相互作用ロジック
    /// </summary>
    public partial class PauseGraph : UserControl
    {
        private int span_ms;
        public int Span_ms {
            get => span_ms;
            set
            {
                this.pauseText.Text = value.ToString();
                span_ms = value;
            }
        }

        private PauseType pauseType = PauseType.None;
        public PauseType PauseType
        {
            get
            {
                return this.pauseType;
            }
            set
            {
                switch (value)
                {
                    case PauseType.Short:
                        bigCircle.Visibility = Visibility.Collapsed;
                        bigText.Visibility = Visibility.Collapsed;
                        littleCircle.Visibility = Visibility.Visible;
                        littleText.Visibility = Visibility.Visible;
                        pauseText.Visibility = Visibility.Collapsed;
                        break;
                    case PauseType.Long:
                        bigCircle.Visibility = Visibility.Visible;
                        bigText.Visibility = Visibility.Visible;
                        littleCircle.Visibility = Visibility.Collapsed;
                        littleText.Visibility = Visibility.Collapsed;
                        pauseText.Visibility = Visibility.Collapsed;
                        break;
                    case PauseType.Manual:
                        bigCircle.Visibility = Visibility.Visible;
                        bigText.Visibility = Visibility.Visible;
                        littleCircle.Visibility = Visibility.Collapsed;
                        littleText.Visibility = Visibility.Collapsed;
                        pauseText.Visibility = Visibility.Visible;
                        break;
                    case PauseType.None:
                        bigCircle.Visibility = Visibility.Collapsed;
                        bigText.Visibility = Visibility.Collapsed;
                        littleCircle.Visibility = Visibility.Collapsed;
                        littleText.Visibility = Visibility.Collapsed;
                        pauseText.Visibility = Visibility.Collapsed;
                        break;
                }
                this.pauseType = value;
            }
        }

        private Pause pause;
        public Pause Pause
        {
            get
            {
                return pause;
            }
            set
            {
                Span_ms = value.Span_ms;
                PauseType = value.Type;
                pause = value;
            }
        }

        public Action<Pause, string> Action;

        public PauseGraph(double h)
        {
            InitializeComponent();
            this.square.Points.Clear();
            this.square.Points.Add(new Point(-18, -h / 2));
            this.square.Points.Add(new Point(18, -h / 2));
            this.square.Points.Add(new Point(18, h / 2));
            this.square.Points.Add(new Point(-18, h / 2));
        }

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            if (sender is MenuItem item && Action != null)
            {
                switch (item.Header.ToString())
                {
                    case "短ポーズを挿入": this.pause.Type = PauseType.Short; break;
                    case "長ポーズを挿入": this.pause.Type = PauseType.Long; break;
                    case "ポーズを削除": this.pause.Type = PauseType.None; break;
                }
                Action(this.pause, item.Header.ToString());
            }
        }

        private void OnMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton != MouseButton.Right) { return; }
            this.shortItem.IsEnabled = (this.pause.Type != PauseType.Short);
            this.longItem.IsEnabled = (this.pause.Type != PauseType.Long);
            // this.manualItem.IsEnabled = (this.pause.Type != PauseType.Manual);
            this.noneItem.IsEnabled = (this.pause.Type != PauseType.None);
        }
    }
}
