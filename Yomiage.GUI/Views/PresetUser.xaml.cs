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

namespace Yomiage.GUI.Views
{
    /// <summary>
    /// PresetUser.xaml の相互作用ロジック
    /// </summary>
    public partial class PresetUser : UserControl
    {
        public PresetUser()
        {
            InitializeComponent();
        }

        bool mouseDown = false;

        private void ListView_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            mouseDown = true;
        }
        private void ListView_MouseUp(object sender, MouseButtonEventArgs e)
        {
            mouseDown = false;
        }
        private void ListView_PreviewMouseMove(object sender, MouseEventArgs e)
        {
            //==== 送信者判定 ====//
            if (mouseDown && sender is ItemsControl itemsControl)
            {
                var dragData = new DataObject("Data", "AddPreset");
                DragDrop.DoDragDrop(itemsControl, dragData, DragDropEffects.Move);
                mouseDown = false;
            }
        }

    }
}
