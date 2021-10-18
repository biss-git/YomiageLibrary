using System;
using System.Collections.Generic;
using System.ComponentModel;
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

namespace Yomiage.YukarinettePlugin
{
    /// <summary>
    /// SettingsWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class SettingsWindow : Window, INotifyPropertyChanged
    {
        public int[] PortNoList { get; }
        public int SelectedPortNo
        {
            get { return selectedPortNo; }
            set
            {
                if (Equals(selectedPortNo, value)) { return; }
                selectedPortNo = value;
                OnPropertyChanged(nameof(SelectedPortNo));
            }
        }
        private int selectedPortNo = 42503;

        public bool ResultIsOk = false;

        public SettingsWindow()
        {
            InitializeComponent();
            this.DataContext = this;
            var list = new List<int>();
            for (int i = 42503; i < 42600; i++)
            {
                list.Add(i);
            }
            PortNoList = list.ToArray();
        }

        private void OkButton_Click(object sender, RoutedEventArgs e)
        {
            ResultIsOk = true;
            this.Close();
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            ResultIsOk = false;
            this.Close();
        }




        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string name)
        {
            if (PropertyChanged == null) return;

            PropertyChanged(this, new PropertyChangedEventArgs(name));
        }

        private void ResetButton_Click(object sender, RoutedEventArgs e)
        {
            SelectedPortNo = 42503;
        }
    }
}
