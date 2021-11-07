using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
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
    /// MoraPoint.xaml の相互作用ロジック
    /// </summary>
    public partial class MoraPoint : Thumb
    {
        public MoraPoint()
        {
            InitializeComponent();
        }

        private bool? active = null;
        public bool? Active
        {
            get => active;
            set
            {
                if (value == true)
                {
                    var circle = new FrameworkElementFactory(typeof(CircleActive));
                    var template = new ControlTemplate(typeof(Thumb))
                    {
                        VisualTree = circle
                    };
                    this.Template = template;
                    this.Cursor = Cursors.Hand;
                    this.menu.Visibility = Visibility.Visible;
                }
                else
                {
                    var circle = new FrameworkElementFactory(typeof(Circle));
                    var template = new ControlTemplate(typeof(Thumb))
                    {
                        VisualTree = circle
                    };
                    this.Template = template;
                    this.menu.Visibility = Visibility.Collapsed;
                }
                active = value;
            }
        }

        public TalkScript Phrase;
        public Mora Mora { get; init; }

        public Action<Mora, string> Action { get; init; }


        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            if (sender is MenuItem item && Action != null)
            {
                switch (item.Header.ToString())
                {
                    case "無声化する":
                        this.Mora.Voiceless = true;
                        break;
                    case "無声化しない":
                        this.Mora.Voiceless = false;
                        break;
                    case "無声化を指定しない":
                        this.Mora.Voiceless = null;
                        break;
                };
                Action(this.Mora, item.Header.ToString());
            }
        }

        private void OnMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton != MouseButton.Right) { return; }
            this.yomi.IsEnabled = true;
            this.join.IsEnabled = CanJoin();
            this.split.IsEnabled = CanSplit();
            this.d.IsEnabled = (Mora.Voiceless != true);
            this.v.IsEnabled = (Mora.Voiceless != false);
            this.dv.IsEnabled = (Mora.Voiceless != null);
            this.removeMora.IsEnabled = this.Phrase.MoraCount > 1;
            this.removeSection.IsEnabled = this.Phrase.Sections.Count > 1;
        }
        private bool CanJoin()
        {
            foreach (var section in this.Phrase.Sections)
            {
                if (section.Moras.Contains(Mora))
                {
                    var sectionIndex = this.Phrase.Sections.IndexOf(section);
                    var moraIndex = section.Moras.IndexOf(Mora);
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
                if (section.Moras.Contains(Mora))
                {
                    // var sectionIndex = this.Phrase.Sections.IndexOf(section);
                    var moraIndex = section.Moras.IndexOf(Mora);
                    if (moraIndex > 0)
                    {
                        return true;
                    }
                    break;
                }
            }
            return false;
        }
    }
}
