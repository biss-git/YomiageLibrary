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
                    var template = new ControlTemplate(typeof(Thumb));
                    template.VisualTree = circle;
                    this.Template = template;
                    this.Cursor = Cursors.Hand;
                    this.menu.Visibility = Visibility.Visible;
                }
                else
                {
                    var circle = new FrameworkElementFactory(typeof(Circle));
                    var template = new ControlTemplate(typeof(Thumb));
                    template.VisualTree = circle;
                    this.Template = template;
                    this.menu.Visibility = Visibility.Collapsed;
                }
                active = value;
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
                mora = value;
            }
        }

        public Action<Mora, string> Action;


        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            if (sender is MenuItem item && Action != null)
            {
                switch (item.Header.ToString())
                {
                    case "無声化する":
                        this.mora.Voiceless = true;
                        break;
                    case "無声化しない":
                        this.mora.Voiceless = false;
                        break;
                    case "無声化を指定しない":
                        this.mora.Voiceless = null;
                        break;
                };
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
    }
}
