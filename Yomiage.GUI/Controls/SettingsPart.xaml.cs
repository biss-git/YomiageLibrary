using Microsoft.Win32;
using Microsoft.WindowsAPICodePack.Dialogs;
using Reactive.Bindings;
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
using Yomiage.SDK.Settings;

namespace Yomiage.GUI.Controls
{
    /// <summary>
    /// SettingsPart.xaml の相互作用ロジック
    /// </summary>
    public partial class SettingsPart : UserControl
    {
        public ReactivePropertySlim<string> Title { get; } = new();
        public ReactivePropertySlim<string> Description { get; } = new();

        public ReactivePropertySlim<bool> bVal { get; } = new();
        public ReactivePropertySlim<int> iVal { get; } = new();
        public ReactivePropertySlim<double> dVal { get; } = new();
        public ReactivePropertySlim<string> sVal { get; } = new();
        public ReactivePropertySlim<double> Minimum { get; } = new();
        public ReactivePropertySlim<double> Maximum { get; } = new();
        public ReactivePropertySlim<double> SmallStep { get; } = new();
        public ReactivePropertySlim<string> Format { get; } = new();
        public ReactivePropertySlim<int> MaxLength { get; } = new();
        public ReactivePropertySlim<string[]> ComboboxItems { get; } = new();

        public static readonly DependencyProperty AccentVisibleProperty =
            DependencyProperty.Register(
            "Setting",
            typeof(ISetting),
            typeof(SettingsPart),
            new PropertyMetadata(null, AccentPropertyChanged));

        public ISetting Setting
        {
            get { return (ISetting)GetValue(AccentVisibleProperty); }
            set { SetValue(AccentVisibleProperty, value); }
        }

        private static void AccentPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is SettingsPart p && e.NewValue is SettingBase s) {
                p.boolSection.Visibility = Visibility.Collapsed;
                p.intSection.Visibility = Visibility.Collapsed;
                p.doubleSection.Visibility = Visibility.Collapsed;
                p.textSection.Visibility = Visibility.Collapsed;
                p.comboSection.Visibility = Visibility.Collapsed;
                p.fileSection.Visibility = Visibility.Collapsed;
                p.folderSection.Visibility = Visibility.Collapsed;

                p.Title.Value = s.Name;
                p.Description.Value = s.Description;

                if (s is BoolSetting sb)
                {
                    p.boolSection.Visibility = Visibility.Visible;
                    p.bVal.Value = sb.Value;
                }

                if (s is IntSetting si)
                {
                    p.intSection.Visibility = Visibility.Visible;
                    p.iVal.Value = si.Value;
                    p.Minimum.Value = si.Min;
                    p.Maximum.Value = si.Max;
                    p.SmallStep.Value = Math.Max(si.SmallStep, 1);
                }

                if (s is DoubleSetting sd)
                {
                    p.doubleSection.Visibility = Visibility.Visible;
                    p.dVal.Value = sd.Value;
                    p.Minimum.Value = sd.Min;
                    p.Maximum.Value = sd.Max;
                    p.SmallStep.Value = (sd.SmallStep > 0) ? sd.SmallStep : 1;
                    p.Format.Value = sd.StringFormat;
                }

                if (s is StringSetting ss)
                {
                    switch (ss.Type)
                    {
                        case "file":
                            p.fileSection.Visibility = Visibility.Visible;
                            break;
                        case "folder":
                            p.folderSection.Visibility = Visibility.Visible;
                            break;
                        case "combobox":
                            p.comboSection.Visibility = Visibility.Visible;
                            p.ComboboxItems.Value = ss.ComboItems;
                            break;
                        default:
                            p.textSection.Visibility = Visibility.Visible;
                            p.MaxLength.Value = Math.Max(ss.MaxLength, 1);
                            break;
                    }
                    p.sVal.Value = ss.Value;
                }
            }
        }

        public SettingsPart()
        {
            InitializeComponent();
            bVal.Subscribe(v => { if (Setting is BoolSetting s) { s.Value = v; } });
            iVal.Subscribe(v => { if (Setting is IntSetting s) { s.Value = v; } });
            dVal.Subscribe(v => { if (Setting is DoubleSetting s) { s.Value = v; } });
            sVal.Subscribe(v => { if (Setting is StringSetting s) { s.Value = v; } });
        }

        private void SelectFile(object sender, RoutedEventArgs e)
        {
            if(Setting is StringSetting s)
            {
                var ofd = new OpenFileDialog()
                {
                    Title = Title.Value,
                    Filter = s.FileDialogFilter,
                };
                if(ofd.ShowDialog() != true) { return; }
                sVal.Value = ofd.FileName;
            }
        }

        private void SelectFolder(object sender, RoutedEventArgs e)
        {
            using (var cofd = new CommonOpenFileDialog()
                {
                    Title = Title.Value,
                    // フォルダ選択モードにする
                    IsFolderPicker = true,
                })
            {
                if (cofd.ShowDialog() != CommonFileDialogResult.Ok) { return; }
                sVal.Value = cofd.FileName;
            }
        }
    }
}
