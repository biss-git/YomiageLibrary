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
    /// MoraGraph.xaml の相互作用ロジック
    /// </summary>
    public partial class MoraGraph : UserControl
    {
        private bool? voiceless;
        public bool? Voiceless
        {
            get => voiceless;
            set
            {
                markD.Visibility = (value == true) ? Visibility.Visible : Visibility.Collapsed;
                markV.Visibility = (value == false) ? Visibility.Visible : Visibility.Collapsed;
                voiceless = value;
            }
        }

        public string Char
        {
            get => this.moraText.Text;
            set
            {
                this.moraText.Text = value;
            }
        }


        public TalkScript Phrase;
        private Mora mora;
        public Mora Mora
        {
            get
            {
                return mora;
            }
            set
            {
                Voiceless = value.Voiceless;
                Char = value.Character;
                mora = value;
            }
        }

        public Action<Mora, string> Action { get; set; }

        public MoraGraph()
        {
            InitializeComponent();
        }

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            if(sender is MenuItem item && Action != null)
            {
                Action(this.mora, item.Header.ToString());
            }
        }

        private void OnMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton != MouseButton.Right) { return; }
            this.yomi.IsEnabled = true;
            this.join.IsEnabled = CanJoin();
            this.split.IsEnabled = CanSplit();
            this.d.IsEnabled = (mora.Voiceless != true);
            this.v.IsEnabled = (mora.Voiceless != false);
            this.dv.IsEnabled = (mora.Voiceless != null);
            this.removeMora.IsEnabled = this.Phrase.MoraCount > 1;
            this.removeSection.IsEnabled = this.Phrase.Sections.Count > 1;
        }
        private bool CanJoin()
        {
            foreach (var section in this.Phrase.Sections)
            {
                if (section.Moras.Contains(mora))
                {
                    var sectionIndex = this.Phrase.Sections.IndexOf(section);
                    var moraIndex = section.Moras.IndexOf(mora);
                    if (sectionIndex > 0 &&
                        moraIndex == 0)
                    {
                        return true;
                    }
                    break;
                }
            }
            return false;
        }
        private bool CanSplit()
        {
            foreach (var section in this.Phrase.Sections)
            {
                if (section.Moras.Contains(mora))
                {
                    var sectionIndex = this.Phrase.Sections.IndexOf(section);
                    var moraIndex = section.Moras.IndexOf(mora);
                    if (moraIndex > 0)
                    {
                        return true;
                    }
                    break;
                }
            }
            return false;
        }

        private void moraText_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton != MouseButton.Left) { return; }
            string command = this.mora.Voiceless switch
            {
                true => "_無声化しない",
                false => "_無声化を指定しない",
                null => "_無声化する",
            };
            Action(this.mora, command);
        }
    }
}
