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
using System.Windows.Threading;

namespace Yomiage.GUI
{
    /// <summary>
    /// Splash.xaml の相互作用ロジック
    /// </summary>
    public partial class Splash : Window
    {
        public Splash()
        {
            InitializeComponent();

            System.Reflection.Assembly sra = System.Reflection.Assembly.GetExecutingAssembly();
            {
                using var stream = sra.GetManifestResourceStream("Yomiage.GUI.スプラッシュ.png");
                var bf = BitmapFrame.Create(stream);
                image2.Source = bf;
            }
        }

        public void SetProgress(string state, double progress)
        {
            this.Dispatcher.Invoke(() =>
            {
                text.Text = state;
                progBar.Value = Math.Clamp(progress, 0, 1);
            });
        }
    }
}
