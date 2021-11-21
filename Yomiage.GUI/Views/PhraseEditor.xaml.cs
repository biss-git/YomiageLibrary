using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
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

namespace Yomiage.GUI.Views
{
    /// <summary>
    /// PhraseEditor.xaml の相互作用ロジック
    /// </summary>
    public partial class PhraseEditor : UserControl
    {
        public PhraseEditor()
        {
            InitializeComponent();
        }

        private void ScrollViewer_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            // 横方向の移動
            ScrollViewer scv = this.scrollViewer;
            scv.ScrollToHorizontalOffset(scv.HorizontalOffset - e.Delta);
            e.Handled = true;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            // レイアウトを再計算させる
            var size = new Size(this.ActualWidth, this.ActualHeight);

            var scale = 2;

            // VisualObjectをBitmapに変換する
            var renderBitmap = new RenderTargetBitmap((int)size.Width * scale,       // 画像の幅
                                                      (int)size.Height * scale,      // 画像の高さ
                                                      96.0d * scale,                 // 横96.0DPI
                                                      96.0d * scale,                 // 縦96.0DPI
                                                      PixelFormats.Pbgra32); // 32bit(RGBA各8bit)
            renderBitmap.Render(this);

            SaveFileDialog sfd = new()
            {
                Filter = "画像|*.png",
            };

            if (sfd.ShowDialog() == true)
            {
                // 出力用の FileStream を作成する
                using var os = new FileStream(sfd.FileName, FileMode.Create);
                // 変換したBitmapをエンコードしてFileStreamに保存する。
                // BitmapEncoder が指定されなかった場合は、PNG形式とする。
                var encoder = new PngBitmapEncoder();
                encoder.Frames.Add(BitmapFrame.Create(renderBitmap));
                encoder.Save(os);
            }
        }

        private void UserControl_MouseDown(object sender, MouseButtonEventArgs e)
        {
            this.Focus();
        }
    }
}
