using Prism.Services.Dialogs;
using Reactive.Bindings;
using Reactive.Bindings.Extensions;
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
using Yomiage.Core.Utility;
using Yomiage.GUI.Graph.Dialog;
using Yomiage.GUI.ViewModels;
using Yomiage.SDK.Config;
using Yomiage.SDK.Talk;
using Yomiage.SDK.VoiceEffects;

namespace Yomiage.GUI.Graph
{
    /// <summary>
    /// グラフ
    /// 長い。もっとちゃんとまとめて小分けに書くこともできるが、
    /// 下手に小分けにするより、まとめてしまった方が良いのではないかと思う。
    /// regionは使いたくないが、この長さなら使った方がいいか。
    /// </summary>
    public partial class PhraseGraph : UserControl
    {
        // 無理やりダイアログサービスを使う。
        public static IDialogService DialogService;

        #region DependencyProperty
        public static readonly DependencyProperty AccentSelectedProperty =
            DependencyProperty.Register(
            "AccentSelected",
            typeof(bool),
            typeof(PhraseGraph),
            new PropertyMetadata(true, AccentPropertyChanged));

        public bool AccentSelected
        {
            get { return (bool)GetValue(AccentSelectedProperty); }
            set { SetValue(AccentSelectedProperty, value); }
        }

        public static readonly DependencyProperty AccentVisibleProperty =
            DependencyProperty.Register(
            "AccentVisible",
            typeof(bool),
            typeof(PhraseGraph),
            new PropertyMetadata(true, AccentPropertyChanged));

        public bool AccentVisible
        {
            get { return (bool)GetValue(AccentVisibleProperty); }
            set { SetValue(AccentVisibleProperty, value); }
        }

        private static void AccentPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is PhraseGraph p)
            {
                p.Dispatcher.Invoke(() =>
                {
                    p.Draw_Lines();
                    p.Draw_Accent();
                    p.mouse_grid.Visibility = (p.AccentSelected) ? Visibility.Collapsed : Visibility.Visible;
                });
            }
        }

        public static readonly DependencyProperty VolumeSelectedProperty =
            DependencyProperty.Register(
            "VolumeSelected",
            typeof(bool),
            typeof(PhraseGraph),
            new PropertyMetadata(false, VolumePropertyChanged));

        public bool VolumeSelected
        {
            get { return (bool)GetValue(VolumeSelectedProperty); }
            set { SetValue(VolumeSelectedProperty, value); }
        }

        public static readonly DependencyProperty VolumeVisibleProperty =
            DependencyProperty.Register(
            "VolumeVisible",
            typeof(bool),
            typeof(PhraseGraph),
            new PropertyMetadata(false, VolumePropertyChanged));

        public bool VolumeVisible
        {
            get { return (bool)GetValue(VolumeVisibleProperty); }
            set { SetValue(VolumeVisibleProperty, value); }
        }

        private static void VolumePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is PhraseGraph p)
            {
                p.Dispatcher.Invoke(() =>
                {
                    p.Draw_Lines(); p.Draw_Volume();
                });
            }
        }


        public static readonly DependencyProperty SpeedSelectedProperty =
            DependencyProperty.Register(
            "SpeedSelected",
            typeof(bool),
            typeof(PhraseGraph),
            new PropertyMetadata(false, SpeedPropertyChanged));

        public bool SpeedSelected
        {
            get { return (bool)GetValue(SpeedSelectedProperty); }
            set { SetValue(SpeedSelectedProperty, value); }
        }

        public static readonly DependencyProperty SpeedVisibleProperty =
            DependencyProperty.Register(
            "SpeedVisible",
            typeof(bool),
            typeof(PhraseGraph),
            new PropertyMetadata(false, SpeedPropertyChanged));

        public bool SpeedVisible
        {
            get { return (bool)GetValue(SpeedVisibleProperty); }
            set { SetValue(SpeedVisibleProperty, value); }
        }

        private static void SpeedPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is PhraseGraph p)
            {
                p.Dispatcher.Invoke(() =>
                {
                    p.Draw_Lines(); p.Draw_Speed();
                });
            }
        }


        public static readonly DependencyProperty PitchSelectedProperty =
            DependencyProperty.Register(
            "PitchSelected",
            typeof(bool),
            typeof(PhraseGraph),
            new PropertyMetadata(false, PitchPropertyChanged));

        public bool PitchSelected
        {
            get { return (bool)GetValue(PitchSelectedProperty); }
            set { SetValue(PitchSelectedProperty, value); }
        }

        public static readonly DependencyProperty PitchVisibleProperty =
            DependencyProperty.Register(
            "PitchVisible",
            typeof(bool),
            typeof(PhraseGraph),
            new PropertyMetadata(false, PitchPropertyChanged));

        public bool PitchVisible
        {
            get { return (bool)GetValue(PitchVisibleProperty); }
            set { SetValue(PitchVisibleProperty, value); }
        }

        private static void PitchPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is PhraseGraph p)
            {
                p.Dispatcher.Invoke(() =>
                {
                    p.Draw_Lines(); p.Draw_Pitch();
                });
            }
        }


        public static readonly DependencyProperty EmphasisSelectedProperty =
            DependencyProperty.Register(
            "EmphasisSelected",
            typeof(bool),
            typeof(PhraseGraph),
            new PropertyMetadata(false, EmphasisPropertyChanged));

        public bool EmphasisSelected
        {
            get { return (bool)GetValue(EmphasisSelectedProperty); }
            set { SetValue(EmphasisSelectedProperty, value); }
        }

        public static readonly DependencyProperty EmphasisVisibleProperty =
            DependencyProperty.Register(
            "EmphasisVisible",
            typeof(bool),
            typeof(PhraseGraph),
            new PropertyMetadata(false, EmphasisPropertyChanged));

        public bool EmphasisVisible
        {
            get { return (bool)GetValue(EmphasisVisibleProperty); }
            set { SetValue(EmphasisVisibleProperty, value); }
        }

        private static void EmphasisPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is PhraseGraph p)
            {
                p.Dispatcher.Invoke(() =>
                {
                    p.Draw_Lines(); p.Draw_Emphasis();
                });
            }
        }


        public static readonly DependencyProperty AdditionalsProperty =
            DependencyProperty.Register(
            "Additionals",
            typeof(ReactiveCollection<PhraseSettingConfig>),
            typeof(PhraseGraph),
            new PropertyMetadata(null, AdditionalsChanged));

        public ReactiveCollection<PhraseSettingConfig> Additionals
        {
            get { return (ReactiveCollection<PhraseSettingConfig>)GetValue(AdditionalsProperty); }
            set { SetValue(AdditionalsProperty, value); }
        }

        private static void AdditionalsChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is PhraseGraph pg &&
                e.NewValue is ReactiveCollection<PhraseSettingConfig> adds)
            {
                adds.CollectionChanged += (s, e) => pg.AdditionalsChanged(adds);
                pg.AdditionalsChanged(adds);
            }
        }
        private int additionalChangedId = 0;
        public void AdditionalsChanged(ReactiveCollection<PhraseSettingConfig> adds)
        {
            var id = (additionalChangedId + 1) % 1000000;
            additionalChangedId = id;
            Task.Run(async () =>
            {
                await Task.Delay(50);
                if (id != additionalChangedId) { return; }
                Dispatcher.Invoke(() =>
                {
                    foreach (var l in SettingsLines)
                    {
                        Remove_Elements(l.Value);
                    }
                    SettingsLines.Clear();
                    foreach (var p in SettingsPoints)
                    {
                        Remove_Elements(p.Value);
                    }
                    SettingsPoints.Clear();

                    foreach (var s in adds)
                    {
                        SettingsLines.Add(s.Key, new());
                        SettingsPoints.Add(s.Key, new());
                    }
                    Draw();
                });
            });
        }


        public static readonly DependencyProperty PhraseProperty =
            DependencyProperty.Register(
            "Phrase",
            typeof(TalkScript),
            typeof(PhraseGraph),
            new PropertyMetadata(null, PhrasePropertyChanged));
        public TalkScript Phrase
        {
            get { return (TalkScript)GetValue(PhraseProperty); }
            set { SetValue(PhraseProperty, value); }
        }
        private static void PhrasePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is PhraseGraph p) { p.Draw(); }
        }

        public static readonly DependencyProperty IsExtendProperty =
            DependencyProperty.Register(
            "IsExtend",
            typeof(bool),
            typeof(PhraseGraph),
            new PropertyMetadata(true, PhrasePropertyChanged));
        public bool IsExtend
        {
            get { return (bool)GetValue(IsExtendProperty); }
            set { SetValue(IsExtendProperty, value); }
        }

        public static readonly DependencyProperty UpdateCommandProperty =
            DependencyProperty.Register(
            "UpdateCommand",
            typeof(ReactiveCommand<string>),
            typeof(PhraseGraph),
            new PropertyMetadata(null, UpdateCommandPropertyChanged));
        public ReactiveCommand<string> UpdateCommand
        {
            get { return (ReactiveCommand<string>)GetValue(UpdateCommandProperty); }
            set { SetValue(UpdateCommandProperty, value); }
        }
        private static void UpdateCommandPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is PhraseGraph p && e.NewValue is ReactiveCommand<string> command && command != null)
            {
                command.Subscribe(p.UpdateAction);
            }
        }

        private void UpdateAction(string param)
        {
            if (param == "ToGraph")
            {
                Draw();
            }
        }

        public static readonly DependencyProperty EngineConfigProperty =
            DependencyProperty.Register(
            "EngineConfig",
            typeof(EngineConfig),
            typeof(PhraseGraph),
            new PropertyMetadata(null, EngineConfigPropertyChanged));
        public EngineConfig EngineConfig
        {
            get { return (EngineConfig)GetValue(EngineConfigProperty); }
            set { SetValue(EngineConfigProperty, value); }
        }
        private static void EngineConfigPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is PhraseGraph p) { p.Draw(); }
        }


        public static readonly DependencyProperty PlayPositionProperty =
            DependencyProperty.Register(
            "PlayPosition",
            typeof(int),
            typeof(PhraseGraph),
            new FrameworkPropertyMetadata(0, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, PlayPositionPropertyChanged));
        public int PlayPosition
        {
            get { return (int)GetValue(PlayPositionProperty); }
            set { SetValue(PlayPositionProperty, value); }
        }
        private static void PlayPositionPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is PhraseGraph p) { p.Draw_PlayPosition(); }
        }

        public static readonly DependencyProperty PlayPositionEnableProperty =
            DependencyProperty.Register(
            "PlayPositionEnable",
            typeof(bool),
            typeof(PhraseGraph),
            new FrameworkPropertyMetadata(true, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, PlayPositionEnablePropertyChanged));
        public bool PlayPositionEnable
        {
            get { return (bool)GetValue(PlayPositionEnableProperty); }
            set { SetValue(PlayPositionEnableProperty, value); }
        }
        private static void PlayPositionEnablePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is PhraseGraph p && e.NewValue is bool visible)
            {
                p.positionBar.Visibility = visible ? Visibility.Visible : Visibility.Collapsed;
            }
        }

        #endregion


        private List<UIElement> MoraPoints = new();
        private List<Button> MoraButtons = new();

        private List<Polyline> AccentLines = new();
        private List<MoraPoint> AccentPoints = new();
        Dictionary<MoraPoint, (Mora, Yomiage.SDK.Talk.Section)> moraDict = new();
        private Polyline PreAccentLine = new Polyline();
        private List<MoraPoint> PreAccentPoints = new();
        private List<PauseGraph> PausePoints = new();

        private List<Polyline> VolumeLines = new();
        private List<PointGraph> VolumePoints = new();

        private List<Polyline> SpeedLines = new();
        private List<PointGraph> SpeedPoints = new();

        private List<Polyline> PitchLines = new();
        private List<PointGraph> PitchPoints = new();

        private List<Polyline> EmphasisLines = new();
        private List<PointGraph> EmphasisPoints = new();

        private Dictionary<string, List<Polyline>> SettingsLines = new();
        private Dictionary<string, List<PointGraph>> SettingsPoints = new();

        private List<Polyline> PreValueLines = new();
        private PreValue PreValueText = new();

        private Brush SelectedBrush
        {
            get
            {
                if (VolumeSelected) { return Brushes.Firebrick; }
                if (SpeedSelected) { return Brushes.ForestGreen; }
                if (PitchSelected) { return Brushes.Orange; }
                if (EmphasisSelected) { return Brushes.MediumOrchid; }
                return Brushes.Gray;
            }
        }

        private double GraphHeight { get => this.ActualHeight - h1 - h2; }
        private EffectSetting SelectedEffectSetting
        {
            get
            {
                if (VolumeSelected) { return EngineConfig?.VolumeSetting; }
                if (SpeedSelected) { return EngineConfig?.SpeedSetting; }
                if (PitchSelected) { return EngineConfig?.PitchSetting; }
                if (EmphasisSelected) { return EngineConfig?.EmphasisSetting; }
                if (Additionals != null)
                {
                    foreach (var s in Additionals)
                    {
                        if (s.IsSelected.Value) { return s.Setting; }
                    }
                }
                return null;
            }
        }
        private Action<VoiceEffectValueBase, double?> SetEffectSelectedValue
        {
            get
            {
                if (VolumeSelected) { return SetEffectVolume; }
                if (SpeedSelected) { return SetEffectSpeed; }
                if (PitchSelected) { return SetEffectPitch; }
                if (EmphasisSelected) { return SetEffectEmphasis; }
                if (Additionals != null)
                {
                    foreach (var s in Additionals)
                    {
                        if (s.IsSelected.Value)
                        {
                            if (s.Setting.Type != "Curve")
                            {
                                return (VoiceEffectValueBase item, double? val) => item.SetAdditionalValue(s.Key, val);
                            }
                            else
                            {
                                return (VoiceEffectValueBase item, double? val) =>
                                {
                                    item.SetAdditionalValues(s.Key, val);
                                    if (item is Yomiage.SDK.Talk.Section section)
                                    {
                                        section.Moras.ForEach(m =>
                                        {
                                            m.SetAdditionalValues(s.Key, val);
                                        });
                                    }
                                };
                            }
                        }
                    }
                }
                return null;
            }
        }
        private void SetEffectVolume(VoiceEffectValueBase item, double? value) { item.Volume = value; }
        private void SetEffectSpeed(VoiceEffectValueBase item, double? value) { item.Speed = value; }
        private void SetEffectPitch(VoiceEffectValueBase item, double? value) { item.Pitch = value; }
        private void SetEffectEmphasis(VoiceEffectValueBase item, double? value) { item.Emphasis = value; }
        private Func<VoiceEffectValueBase, double?> GetEffectSelectedValue
        {
            get
            {
                if (VolumeSelected) { return GetEffectVolume; }
                if (SpeedSelected) { return GetEffectSpeed; }
                if (PitchSelected) { return GetEffectPitch; }
                if (EmphasisSelected) { return GetEffectEmphasis; }
                if (Additionals != null)
                {
                    foreach (var s in Additionals)
                    {
                        if (s.IsSelected.Value)
                        {
                            return (VoiceEffectValueBase item) => item.GetAdditionalValue(s.Key);
                        }
                    }
                }
                return null;
            }
        }
        private double? GetEffectVolume(VoiceEffectValueBase item) => item.Volume;
        private double? GetEffectSpeed(VoiceEffectValueBase item) => item.Speed;
        private double? GetEffectPitch(VoiceEffectValueBase item) => item.Pitch;
        private double? GetEffectEmphasis(VoiceEffectValueBase item) => item.Emphasis;
        private (double, double, double) GetMinDefMax(EffectSetting setting)
        {
            return (
                IsExtend ? setting.MinExtend : setting.Min,
                setting.DefaultValue,
                IsExtend ? setting.MaxExtend : setting.Max);
        }

        private double w_offset = 40;
        private double w1 = 25;
        private double h1 = 30;
        private double h2 = 35;
        private double hr1 = 0.25;
        private double hr2 = 0.75;
        private double hr3 = 0.5;
        private double ar = 7;

        private double AccentVerticalChange;//縦幅用

        public PhraseGraph()
        {
            InitializeComponent();
        }


        int drawId = 0;
        public void Draw()
        {
            var id = (drawId + 1) % 1000000;
            drawId = id;
            Task.Run(async () =>
            {
                await Task.Delay(150);
                if (id != drawId) { return; }
                Dispatcher.Invoke(() =>
                {
                    if (this.Phrase == null) { return; } // Invoke 中じゃないとアクセスエラーになる。
                    Draw_Lines();
                    Draw_PlayPosition();
                    Draw_Mora();
                    Draw_Accent();
                    Draw_Volume();
                    Draw_Speed();
                    Draw_Pitch();
                    Draw_Emphasis();
                    Draw_Settings();
                });
            });
        }
        private void Draw_Lines()
        {
            if (this.Phrase == null) { return; }
            int moraCount = 0;
            foreach (var section in this.Phrase.Sections)
            {
                moraCount += 1 + section.Moras.Count;
            }
            double Width = w_offset + moraCount * w1 + 2 * w1;
            this.topLine.X2 = Width;
            this.bottomLine.X2 = Width;
            this.defaultLine.X2 = Width;
            this.MinWidth = Width;
            this.mouse_grid.Margin = new Thickness() { Left = w_offset + w1, Top = h1 };
            this.mouse_grid.Width = Width - w_offset - w1;
            this.mouse_grid.Height = Math.Max(this.GraphHeight, 0);

            double h = GraphHeight;
            double max = 2;
            double min = 0;
            double def = 1;
            var selectedSettings = SelectedEffectSetting;
            if (selectedSettings != null)
            {
                (double min_, double def_, double max_) = GetMinDefMax(selectedSettings);
                min = min_; def = def_; max = max_;
                var format = selectedSettings.StringFormat;
                this.minText.Content = min.ToString(format);
                this.defaultText.Content = def.ToString(format);
                this.maxText.Content = max.ToString(format);
                this.unitText.Content = selectedSettings.Unit;
            }
            else
            {
                this.minText.Content = "";
                this.defaultText.Content = "";
                this.maxText.Content = "";
                this.unitText.Content = "";
            }
            double rate = (def - min) / (max - min);
            Canvas.SetTop(this.defaultLine, Math.Round(h1 + (1 - rate) * h));
            Canvas.SetTop(this.defaultText, Math.Round(h1 + (1 - rate) * h) - 23);
        }

        public void Draw_PlayPosition()
        {
            if (Phrase == null) { return; }

            if (this.PlayPosition >= this.Phrase.Sections.Count ||
                EngineConfig?.AccentHide == true) { this.PlayPosition = 0; return; }

            int position = this.PlayPosition;

            int moraCount = 0;
            foreach (var section in this.Phrase.Sections)
            {

                if (position == 0)
                {
                    Canvas.SetLeft(this.positionBar, w_offset + moraCount * w1 + 10);
                    break;
                }

                moraCount += 1 + section.Moras.Count;
                position -= 1;
            }

        }



        #region Mora

        public void Draw_Mora()
        {
            Draw_Mora_Points();
            Draw_Mora_Buttons();
        }
        private void Draw_Mora_Points()
        {
            if (Phrase == null) { return; }
            MoraPoints.ForEach(p =>
            {
                if (this.graph_base.Children.Contains(p))
                {
                    this.graph_base.Children.Remove(p);
                }
            });
            MoraPoints.Clear();

            if (EngineConfig?.AccentHide == true) { return; }

            int moraCount = 0;
            foreach (var section in this.Phrase.Sections)
            {
                moraCount += 1;
                foreach (var mora in section.Moras)
                {
                    MoraGraph point = new MoraGraph()
                    {
                        Height = this.grid.ActualHeight,
                        Mora = mora,
                        Action = MoraContextAction,
                        Phrase = this.Phrase,
                    };
                    Canvas.SetLeft(point, w_offset + moraCount * w1);
                    this.graph_base.Children.Add(point);
                    MoraPoints.Add(point);

                    moraCount += 1;
                }
            }
            moraCount += 1;
            EndMoraGraph end = new EndMoraGraph()
            {
                Height = this.grid.ActualHeight,
                End = this.Phrase.EndSection,
                Action = EndContextAction,
            };
            Canvas.SetLeft(end, w_offset + moraCount * w1);
            this.graph_base.Children.Add(end);
            MoraPoints.Add(end);
        }
        private void Draw_Mora_Buttons()
        {
            if (Phrase == null) { return; }
            MoraButtons.ForEach(b =>
            {
                if (this.graph_base.Children.Contains(b))
                {
                    this.graph_base.Children.Remove(b);
                }
            });
            MoraButtons.Clear();

            if (EngineConfig?.AccentHide == true) { return; }

            int position = 0;
            int moraCount = 0;
            foreach (var section in this.Phrase.Sections)
            {
                Button button = new Button() { MinWidth = 2 * w1 };
                var p = position;
                button.Click += (s, e) => { this.PlayPosition = p; };
                Canvas.SetLeft(button, w_offset + moraCount * w1 - w1);
                this.graph_base.Children.Add(button);
                MoraButtons.Add(button);

                position += 1;
                moraCount += 1 + section.Moras.Count;
            }
        }
        private void MoraContextAction(Mora mora, string command)
        {
            if (this.Phrase == null) { return; }
            switch (command)
            {
                case "無声化する": mora.Voiceless = true; break;
                case "無声化しない": mora.Voiceless = false; break;
                case "無声化を指定しない": mora.Voiceless = null; break;
                case "_無声化する": if (this.AccentSelected) mora.Voiceless = true; break;
                case "_無声化しない": if (this.AccentSelected) mora.Voiceless = false; break;
                case "_無声化を指定しない": if (this.AccentSelected) mora.Voiceless = null; break;
                case "読み編集": YomiEdit(mora); break;
                case "アクセント句を結合": AccentJoin(mora); break;
                case "アクセント句を分割": AccentSplit(mora); break;
                case "長音を追加": AddMora(mora); break;
                case "アクセントを削除": RemoveMora(mora); break;
                case "アクセント句を削除": RemoveSection(mora); break;
            }
            if (command.Contains("無声化"))
            {
                UpdateCommand.Execute("VoicelessChanged");
            }
            else
            {
                UpdateCommand.Execute("AccentChanged");
            }
            Draw();
        }
        private void YomiEdit(Mora mora)
        {
            var section = this.Phrase.Sections.FirstOrDefault(s => s.Moras.Any(m => m == mora));
            if (section == null) { return; }
            var yomi = new YomiEditorDialog();
            yomi.text.Text = section.GetYomi();
            yomi.ShowDialog();
            if (yomi.Ok)
            {
                var result = YomiDictionary.SurfaceToYomi(yomi.text.Text);
                if (result.Length == 0) { return; }
                section.Moras = YomiDictionary.TextToMoras(result)
                    .Select(c => new Mora() { Character = c, Accent = true }).ToList();
                section.Moras[0].Accent = false;
            }
        }
        private void AccentSplit(Mora mora)
        {
            foreach (var section in this.Phrase.Sections)
            {
                if (section.Moras.Contains(mora))
                {
                    var sectionIndex = this.Phrase.Sections.IndexOf(section);
                    var moraIndex = section.Moras.IndexOf(mora);
                    if (moraIndex > 0)
                    {
                        var addSection = new Yomiage.SDK.Talk.Section();
                        for (int i = moraIndex; i < section.Moras.Count; i++)
                        {
                            addSection.Moras.Add(section.Moras[i]);
                        }
                        addSection.Moras.ForEach(m => section.Moras.Remove(m));
                        this.Phrase.Sections.Insert(sectionIndex + 1, addSection);
                    }
                    break;
                }
            }
        }
        private void AccentJoin(Mora mora)
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
                        var margedSection = this.Phrase.Sections[sectionIndex - 1];
                        if (margedSection.Moras.Any(m => m.Accent) && section.Moras.Any(m => m.Accent))
                        {
                            bool flag = false;
                            foreach (var m in margedSection.Moras)
                            {
                                flag |= m.Accent;
                                m.Accent = flag;
                            }
                            foreach (var m in section.Moras)
                            {
                                if (m.Accent) { break; }
                                m.Accent = true;
                            }
                        }
                        margedSection.Moras.AddRange(section.Moras);
                        this.Phrase.Sections.Remove(section);
                    }
                    break;
                }
            }
        }
        private void AddMora(Mora mora, int num = 1)
        {
            foreach (var section in this.Phrase.Sections)
            {
                if (section.Moras.Contains(mora))
                {
                    int index = section.Moras.IndexOf(mora);
                    for (int j = 0; j < num; j++)
                    {
                        section.Moras.Insert(index + 1, new Mora() { Character = "ー", Accent = mora.Accent });
                    }
                    break;
                }
            }
        }
        private void RemoveMora(Mora mora)
        {
            if (this.Phrase.MoraCount <= 1) { return; }
            foreach (var section in this.Phrase.Sections)
            {
                if (section.Moras.Contains(mora))
                {
                    section.Moras.Remove(mora);
                    if (section.Moras.Count == 0)
                    {
                        this.Phrase.Sections.Remove(section);
                    }
                    break;
                }
            }
        }
        private void RemoveSection(Mora mora)
        {
            if (this.Phrase.Sections.Count <= 1) { return; }
            foreach (var section in this.Phrase.Sections)
            {
                if (section.Moras.Contains(mora))
                {
                    this.Phrase.Sections.Remove(section);
                    break;
                }
            }
        }

        private void EndContextAction(EndSection end, string command)
        {
            if (this.Phrase == null) { return; }
            this.UpdateCommand.Execute("EndChanged");
            Draw();
        }
        #endregion


        #region Accent
        public void Draw_Accent()
        {
            Remove_Accent();
            Draw_Accent_Main();
            Draw_Accent_Pause();
        }
        private void Remove_Accent()
        {
            Remove_Elements(AccentLines);
            Remove_Elements(AccentPoints);
        }
        private void Draw_Accent_Main()
        {
            if (Phrase == null) { return; }
            if (EngineConfig?.AccentHide == true) { return; }

            var graph = AccentSelected ? this.graph :
                        AccentVisible ? this.graph_bg : null;
            if (graph == null) { return; }

            double high = h1 + hr1 * GraphHeight;
            double low = h1 + hr2 * GraphHeight;

            int moraCount = 0;
            moraDict.Clear();

            if (EngineConfig != null && !EngineConfig.AccentEnable) { return; }

            foreach (var section in this.Phrase.Sections)
            {
                moraCount += 1;
                Polyline line = new Polyline() { Stroke = Brushes.DarkGray };
                foreach (var mora in section.Moras)
                {
                    line.Points.Add(new Point(w_offset + moraCount * w1, mora.Accent ? high : low));

                    MoraPoint point = new MoraPoint()
                    {
                        Width = 2 * ar,
                        Height = 2 * ar,
                        Active = graph == this.graph,
                        Mora = mora,
                        Phrase = this.Phrase,
                    };
                    if (point.Active == true)
                    {
                        moraDict.Add(point, (mora, section));
                        point.DragDelta += Accent_DragDelta;
                        point.DragCompleted += Accent_DragCompleted;
                        point.Action = MoraContextAction;
                    }
                    Canvas.SetLeft(point, w_offset + moraCount * w1 - ar);
                    Canvas.SetTop(point, (mora.Accent ? high : low) - ar);
                    Panel.SetZIndex(point, -1);
                    graph.Children.Add(point);
                    AccentPoints.Add(point);

                    moraCount += 1;
                }
                Panel.SetZIndex(line, -2);
                graph.Children.Add(line);
                AccentLines.Add(line);
            }
        }
        private void Accent_DragDelta(object sender, DragDeltaEventArgs e)
        {
            AccentVerticalChange = e.VerticalChange;
            if (sender is MoraPoint point)
            {
                Remove_PreAccent();
                Draw_PreAccent(point);
            }
        }
        private void Accent_DragCompleted(object sender, DragCompletedEventArgs e)
        {
            if (sender is MoraPoint point)
            {
                var section = moraDict[point].Item2;
                var mora = moraDict[point].Item1;
                double h = (hr2 - hr1) * this.GraphHeight;
                bool[] accent = CalcMoras(section, mora, AccentVerticalChange, h);
                for (int i = 0; i < accent.Length; i++)
                {
                    section.Moras[i].Accent = accent[i];
                }
                Draw_Accent();
                Remove_PreAccent();
                UpdateCommand.Execute("AccentChanged");
            }
        }
        private void Remove_PreAccent()
        {
            if (this.graph.Children.Contains(PreAccentLine))
            {
                this.graph.Children.Remove(PreAccentLine);
            }
            Remove_Elements(PreAccentPoints);
        }
        private void Draw_PreAccent(MoraPoint point)
        {
            if (EngineConfig?.AccentHide == true) { return; }

            double high = h1 + hr1 * GraphHeight;
            double low = h1 + hr2 * GraphHeight;

            int moraCount = 0;
            foreach (var section in this.Phrase.Sections)
            {
                moraCount += 1;
                if (section == moraDict[point].Item2)
                {
                    PreAccentLine = new Polyline() { Stroke = Brushes.DimGray };
                    var mora = moraDict[point].Item1;
                    double h = (hr2 - hr1) * this.GraphHeight;
                    bool[] accent = CalcMoras(section, mora, AccentVerticalChange, h);
                    foreach (var a in accent)
                    {
                        PreAccentLine.Points.Add(new Point(w_offset + moraCount * w1, a ? high : low));
                        MoraPoint p = new MoraPoint() { Width = 2 * ar, Height = 2 * ar };
                        Canvas.SetLeft(p, w_offset + moraCount * w1 - ar);
                        Canvas.SetTop(p, (a ? high : low) - ar);
                        Panel.SetZIndex(p, -2);
                        this.graph.Children.Add(p);
                        PreAccentPoints.Add(p);
                        moraCount += 1;
                    }
                    Panel.SetZIndex(PreAccentLine, -3);
                    this.graph.Children.Add(PreAccentLine);
                    break;
                }
                moraCount += section.Moras.Count;
            }
        }

        private bool[] CalcMoras(Yomiage.SDK.Talk.Section section, Mora mora, double change, double h)
        {
            bool[] result = section.Moras.Select(m => m.Accent).ToArray();
            int index = section.Moras.IndexOf(mora);
            if (mora.Accent)
            {
                // hight のとき
                if (AccentVerticalChange < 0.1 * h) { return result; }
                if (index > 0 && section.Moras[index - 1].Accent ||
                    AccentVerticalChange > 0.5 * h)
                {
                    for (int i = index; i < section.Moras.Count; i++)
                    {
                        result[i] = false;
                    }
                }
                result[index] = false;
            }
            else
            {
                // low のとき
                if (AccentVerticalChange > -0.1 * h) { return result; }
                bool all = (AccentVerticalChange < -0.5 * h);
                for (int i = 0; i < index; i++)
                {
                    if (!section.Moras[i].Accent) { continue; }
                    for (int j = i; j < index; j++)
                    {
                        result[j] = true;
                    }
                    break;
                }
                bool right = false;
                for (int i = section.Moras.Count - 1; i > index; i--)
                {
                    if (!section.Moras[i].Accent) { continue; }
                    right = true;
                    for (int j = i; j > index; j--)
                    {
                        result[j] = true;
                    }
                    break;
                }
                if (!right && all)
                {
                    for (int i = section.Moras.Count - 1; i > index; i--)
                    {
                        result[i] = true;
                    }
                }
                result[index] = true;
            }
            return result;
        }

        private void Draw_Accent_Pause()
        {
            if (Phrase == null) { return; }
            Remove_Elements(PausePoints);

            var graph = AccentSelected ? this.graph :
                        AccentVisible ? this.graph_bg : null;
            if (graph == null) { return; }
            if (EngineConfig?.AccentHide == true) { return; }

            double h = GraphHeight;
            double center = h1 + hr3 * h;

            int moraCount = 0;
            foreach (var section in this.Phrase.Sections)
            {
                // if (section.Pause.Type != PauseType.None)
                {
                    PauseGraph point = new PauseGraph(h)
                    {
                        Pause = section.Pause,
                        Action = PauseContextAction,
                    };
                    Canvas.SetLeft(point, w_offset + moraCount * w1);
                    Canvas.SetTop(point, center);
                    graph.Children.Add(point);
                    PausePoints.Add(point);
                }
                moraCount += 1 + section.Moras.Count;
            }
            //if (this.Phrase.EndSection.Pause.Type != PauseType.None)
            {
                PauseGraph point = new PauseGraph(h)
                {
                    Pause = this.Phrase.EndSection.Pause,
                    Action = PauseContextAction,
                };
                Canvas.SetLeft(point, w_offset + moraCount * w1);
                Canvas.SetTop(point, center);
                graph.Children.Add(point);
                PausePoints.Add(point);
            }
        }
        private void PauseContextAction(Pause pause, string command)
        {
            if (this.Phrase == null) { return; }

            if (command == "任意ポーズを挿入")
            {
                try
                {
                    IDialogParameters param = new DialogParameters();
                    param.Add("pause", pause);
                    param.Add("config", EngineConfig);
                    DialogService.ShowDialog(
                        "PauseEditDialog",
                        param,
                        result =>
                        {
                        });
                }
                catch (Exception e)
                {

                }
            }
            UpdateCommand.Execute("AccentChanged");
            Draw();
        }
        #endregion


        #region Mouse

        double LastX = 0.0;
        double LastY = 0.0;

        private void OnMouseMove(object sender, MouseEventArgs e)
        {
            if (this.Phrase == null) { return; }
            Point p = e.GetPosition(this);
            var x = p.X;
            var y = p.Y;
            double h = GraphHeight;

            if (e.LeftButton == MouseButtonState.Pressed)
            {
                var selectedSetting = SelectedEffectSetting;
                if (selectedSetting == null) { return; }
                if (selectedSetting.Type != "Curve") { return; }

                if (LastX > 0 && LastY > 0)
                {
                    var x1 = (LastX - w_offset - w1) / w1;
                    var x2 = (x - w_offset - w1) / w1;
                    var y1 = (LastY - h1) / h;
                    var y2 = (y - h1) / h;
                    if (x1 < x2)
                    {
                        SetCurve(
                            x1, y1,
                            x2, y2,
                            selectedSetting);
                    }
                    else
                    {
                        SetCurve(
                            x2, y2,
                            x1, y1,
                            selectedSetting);
                    }
                    this.UpdateCommand.Execute("ValueChanged_MouseWheel");
                    Draw();
                }

                LastX = x;
                LastY = y;
                return;
            }
            LastX = 0.0;
            LastY = 0.0;


            Remove_PreValues();

            if (y < 10 || y > this.grid.ActualHeight - 10) { return; }


            if (SelectedEffectSetting != null)
            {
                double rate = (y - h1) / h;
                rate = Math.Max(0, Math.Min(rate, 1));

                Draw_PreValues(x, rate);
            }
        }

        private void SetCurve(double x1_index, double y1_rate, double x2_index, double y2_rate, EffectSetting setting)
        {
            if (x1_index == x2_index) { return; }
            (double min, double def, double max) = GetMinDefMax(setting);

            Func<VoiceEffectValueBase, double[]> getValues = (VoiceEffectValueBase item) =>
            {
                return item.GetAdditionalValuesOrAdd(setting.Key, setting.DefaultValue);
            };
            int moraCount = 0;
            Action<double[]> setValues = (double[] values) =>
            {
                for (int i = 0; i < values.Length; i++)
                {
                    var x = moraCount + (double)i / values.Length;
                    if (x1_index <= x && x < x2_index)
                    {
                        double rate = ((x2_index - x) * y1_rate + (x - x1_index) * y2_rate) / (x2_index - x1_index);
                        values[i] = (1 - rate) * max + rate * min;
                    }
                }
            };
            foreach (var section in this.Phrase.Sections)
            {
                if (moraCount > 0)
                {
                    if (Math.Floor(x1_index) <= moraCount &&
                        moraCount < Math.Ceiling(x2_index))
                    {
                        var values = getValues(section);
                        setValues(values);
                    }
                    moraCount += 1;
                }
                foreach (var mora in section.Moras)
                {
                    if (Math.Floor(x1_index) <= moraCount &&
                        moraCount < Math.Ceiling(x2_index))
                    {
                        var values = getValues(mora);
                        setValues(values);
                    }
                    moraCount += 1;
                }
            }
            {
                if (Math.Floor(x1_index) <= moraCount &&
                    moraCount < Math.Ceiling(x2_index))
                {
                    var values = getValues(this.Phrase.EndSection);
                    setValues(values);
                }
            }
        }

        private double lastX = 0;
        private void OnMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (this.Phrase == null) { return; }
            Point p = e.GetPosition(this);
            var x = p.X;
            var y = p.Y;
            if (e.ChangedButton == MouseButton.Right)
            {
                lastX = x;
            }
            if (e.ChangedButton != MouseButton.Left ||
                !sender.Equals(this.mouse_grid)) { return; }
            var selectedSetting = SelectedEffectSetting;
            if (selectedSetting == null) { return; }
            if (selectedSetting.Type == "Curve") { return; }
            var index = GetSmIndex(x, selectedSetting.Type != "Mora");
            if (index.Item1 < -1) { return; }

            double h = GraphHeight;

            if (y < 10 || y > this.grid.ActualHeight - 10) { return; }

            (double min, double def, double max) = GetMinDefMax(selectedSetting);
            string format = selectedSetting.StringFormat;

            double rate = (y - h1) / h;
            rate = Math.Max(0, Math.Min(rate, 1));

            double value = rate * min + (1 - rate) * max;

            if (double.TryParse(value.ToString(format), out double textValue))
            {
                value = textValue;
            }

            var setValue = SetEffectSelectedValue;
            if (setValue != null)
            {
                setValue(index.Item3, value);
            }
            Draw_SelectedValues();
            this.UpdateCommand.Execute("ValueChanged_MouseDown");
        }

        private void OnMouseWheel(object sender, MouseWheelEventArgs e)
        {
            e.Handled = true;
            if (this.Phrase == null) { return; }
            Point p = e.GetPosition(this);
            var x = p.X;
            double delta = e.Delta;

            var selectedSetting = SelectedEffectSetting;

            if (selectedSetting == null) { return; }
            if (selectedSetting.Type == "Curve") { return; }
            var index = GetSmIndex(x, selectedSetting.Type != "Mora");
            if (index.Item1 < -1) { return; }

            var setValue = SetEffectSelectedValue;
            var getValue = GetEffectSelectedValue;

            if (selectedSetting != null && setValue != null && getValue != null)
            {
                (double min, double def, double max) = GetMinDefMax(selectedSetting);
                double smallStep = selectedSetting.SmallStep;
                string format = selectedSetting.StringFormat;

                if (getValue(index.Item3) == null)
                {
                    setValue(index.Item3, def);
                }

                double value = (double)getValue(index.Item3) + (delta > 0 ? smallStep : -smallStep);
                if (double.TryParse(value.ToString(format), out double textValue))
                {
                    value = textValue;
                }
                setValue(index.Item3, Math.Max(min, Math.Min(value, max)));

                Draw_SelectedValues();

                this.UpdateCommand.Execute("ValueChanged_MouseWheel");
            }
        }

        #endregion

        public void Draw_SelectedValues()
        {
            if (this.VolumeSelected) { Draw_Volume(); }
            if (this.SpeedSelected) { Draw_Speed(); }
            if (this.PitchSelected) { Draw_Pitch(); }
            if (this.EmphasisSelected) { Draw_Emphasis(); }
            if (Additionals != null)
            {
                foreach (var s in Additionals)
                {
                    if (s.IsSelected.Value)
                    {
                        Draw_Setting(s);
                    }
                }
            }
        }

        #region Volume
        public void Draw_Volume()
        {
            Remove_Elements(VolumeLines);
            Remove_Elements(VolumePoints);

            if (Phrase == null) { return; }
            if (EngineConfig?.AccentHide == true) { return; }

            var graph = VolumeSelected ? this.graph :
                        VolumeVisible ? this.graph_bg : null;
            if (graph == null) { return; }
            if (this.EngineConfig == null) { return; }

            string format = this.EngineConfig.VolumeSetting.StringFormat;
            var rule = this.EngineConfig.VolumeSetting.StringRule;
            double?[] values = Get_VolumeValues();

            (double min, double def, double max) = GetMinDefMax(this.EngineConfig.VolumeSetting);
            Draw_Values(graph, values, min, def, max, format, rule, VolumeLines, VolumePoints, Brushes.Firebrick, 3);
        }
        #endregion

        #region Speed
        public void Draw_Speed()
        {
            Remove_Elements(SpeedLines);
            Remove_Elements(SpeedPoints);

            if (Phrase == null) { return; }
            if (EngineConfig?.AccentHide == true) { return; }

            var graph = SpeedSelected ? this.graph :
                        SpeedVisible ? this.graph_bg : null;
            if (graph == null) { return; }
            if (this.EngineConfig == null) { return; }

            string format = this.EngineConfig.SpeedSetting.StringFormat;
            var rule = this.EngineConfig.SpeedSetting.StringRule;
            double?[] values = Get_SpeedValues();

            (double min, double def, double max) = GetMinDefMax(this.EngineConfig.SpeedSetting);
            Draw_Values(graph, values, min, def, max, format, rule, SpeedLines, SpeedPoints, Brushes.ForestGreen, 3);
        }
        #endregion

        #region Pitch
        public void Draw_Pitch()
        {
            Remove_Elements(PitchLines);
            Remove_Elements(PitchPoints);

            if (Phrase == null) { return; }
            if (EngineConfig?.AccentHide == true) { return; }

            var graph = PitchSelected ? this.graph :
                        PitchVisible ? this.graph_bg : null;
            if (graph == null) { return; }
            if (this.EngineConfig == null) { return; }

            string format = this.EngineConfig.PitchSetting.StringFormat;
            var rule = this.EngineConfig.PitchSetting.StringRule;
            double?[] values = Get_PitchValues();

            (double min, double def, double max) = GetMinDefMax(this.EngineConfig.PitchSetting);
            Draw_Values(graph, values, min, def, max, format, rule, PitchLines, PitchPoints, Brushes.Orange, 3);
        }
        #endregion

        #region Emphasis
        public void Draw_Emphasis()
        {
            Remove_Elements(EmphasisLines);
            Remove_Elements(EmphasisPoints);

            if (Phrase == null) { return; }
            if (EngineConfig?.AccentHide == true) { return; }

            var graph = EmphasisSelected ? this.graph :
                        EmphasisVisible ? this.graph_bg : null;
            if (graph == null) { return; }
            if (this.EngineConfig == null) { return; }

            string format = this.EngineConfig.EmphasisSetting.StringFormat;
            var rule = this.EngineConfig.EmphasisSetting.StringRule;
            double?[] values = Get_EmphasisValues();

            (double min, double def, double max) = GetMinDefMax(this.EngineConfig.EmphasisSetting);
            Draw_Values(graph, values, min, def, max, format, rule, EmphasisLines, EmphasisPoints, Brushes.MediumOrchid, 3);
        }
        #endregion

        public void Draw_Settings()
        {
            if (Additionals == null) { return; }
            foreach (var s in Additionals)
            {
                Draw_Setting(s);
            }
        }
        public void Draw_Setting(PhraseSettingConfig setting)
        {
            if (!SettingsLines.TryGetValue(setting.Key, out var Lines)) { return; }
            if (!SettingsPoints.TryGetValue(setting.Key, out var Points)) { return; }

            Remove_Elements(Lines);
            Remove_Elements(Points);

            if (Phrase == null) { return; }
            if (EngineConfig?.AccentHide == true) { return; }

            var graph = setting.IsSelected.Value ? this.graph :
                        setting.IsVisible.Value ? this.graph_bg : null;
            if (graph == null) { return; }

            string format = setting.Setting.StringFormat;
            var rule = setting.Setting.StringRule;
            (double min, double def, double max) = GetMinDefMax(setting.Setting);

            if (setting.Setting.Type == "Curve")
            {
                double[] values = Get_Curves(setting);
                Draw_Curve(graph, values, min, def, max, Lines, setting.Color, 3);
            }
            else
            {
                double?[] values = Get_SettingValues(setting);
                Draw_Values(graph, values, min, def, max, format, rule, Lines, Points, setting.Color, 3);
            }
        }


        private double?[] Get_SelectedValues()
        {
            if (VolumeSelected) { return Get_VolumeValues(); }
            if (SpeedSelected) { return Get_SpeedValues(); }
            if (PitchSelected) { return Get_PitchValues(); }
            if (EmphasisSelected) { return Get_EmphasisValues(); }
            if (Additionals != null)
            {
                foreach (var s in Additionals)
                {
                    if (s.IsSelected.Value) { return Get_SettingValues(s); }
                }
            }
            return null;
        }
        private double?[] Get_VolumeValues()
        {
            return Get_Values(GetEffectVolume);
        }
        private double?[] Get_SpeedValues()
        {
            return Get_Values(GetEffectSpeed);
        }
        private double?[] Get_PitchValues()
        {
            return Get_Values(GetEffectPitch);
        }
        private double?[] Get_EmphasisValues()
        {
            return Get_Values(GetEffectEmphasis);
        }
        private double?[] Get_SettingValues(PhraseSettingConfig setting)
        {
            return Get_Values((VoiceEffectValueBase item) => item.GetAdditionalValue(setting.Key));
        }



        private void Remove_PreValues()
        {
            Remove_PreValue();
            Remove_Elements(PreValueLines);
        }
        private void Draw_PreValues(double x, double rate)
        {
            var selectedEffect = this.SelectedEffectSetting;
            if (selectedEffect == null) { return; }
            if (selectedEffect.Type == "Curve") { return; }
            (double min, double def, double max) = GetMinDefMax(selectedEffect);
            string format = selectedEffect.StringFormat;
            var rule = selectedEffect.StringRule;

            double value = rate * min + (1 - rate) * max;
            if (double.TryParse(value.ToString(format), out double textValue))
            {
                value = textValue;
            }

            double?[] values = Get_SelectedValues();
            if (values == null) { return; }

            var index = GetSmIndex(x, selectedEffect.Type != "Mora");
            if (index.Item1 < -1) { return; }
            if (EngineConfig?.AccentHide == true) { return; }

            values[index.Item2] = value;
            var brush = SelectedBrush;

            Draw_Values(this.graph, values, min, def, max, format, rule, PreValueLines, null, brush, 1);

            PreValueText.Text = valueTipText((double)values[index.Item2], format, rule);
            Canvas.SetLeft(PreValueText, w_offset + index.Item2 * w1);
            Canvas.SetTop(PreValueText, h1 + rate * GraphHeight);
            this.graph.Children.Add(PreValueText);
        }
        private double?[] Get_Values(Func<VoiceEffectValueBase, double?> getValue)
        {
            double?[] values = new double?[this.Phrase.Sections.Count + this.Phrase.MoraCount + 2];
            int moraCount = 0;
            foreach (var section in this.Phrase.Sections)
            {
                moraCount += 1;
                if (getValue(section) != null)
                {
                    values[moraCount] = getValue(section);
                }
                foreach (var mora in section.Moras)
                {
                    if (getValue(mora) != null)
                    {
                        values[moraCount] = getValue(mora);
                    }
                    moraCount += 1;
                }
            }
            values[values.Length - 1] = getValue(this.Phrase.EndSection);
            return values;
        }
        private double[] Get_Curves(PhraseSettingConfig setting)
        {
            var values = new List<double>();
            Func<VoiceEffectValueBase, double[]> getValues = (VoiceEffectValueBase item) =>
            {
                return item.GetAdditionalValuesOrDefault(setting.Key, setting.Setting.DefaultValue);
            };
            bool firstFlag = true;
            foreach (var section in this.Phrase.Sections)
            {
                if (!firstFlag)
                {
                    values.AddRange(getValues(section));
                }
                foreach (var mora in section.Moras)
                {
                    values.AddRange(getValues(mora));
                }
                firstFlag = false;
            }
            values.AddRange(getValues(this.Phrase.EndSection));
            return values.ToArray();
        }
        private void Remove_PreValue()
        {
            if (this.graph.Children.Contains(PreValueText))
            {
                this.graph.Children.Remove(PreValueText);
            }
        }
        private void Remove_Elements<T>(List<T> elements) where T : UIElement
        {
            elements.ForEach(e =>
            {
                if (this.graph.Children.Contains(e))
                {
                    this.graph.Children.Remove(e);
                }
                if (this.graph_bg.Children.Contains(e))
                {
                    this.graph_bg.Children.Remove(e);
                }
            });
            elements.Clear();
        }
        private void Draw_Values(Canvas graph, double?[] values,
            double min, double def, double max, string format, Dictionary<double, string> rule,
            List<Polyline> lines, List<PointGraph> points, Brush brush, int thickness)
        {
            double h = GraphHeight;
            double rate = (def - min) / (max - min);

            Polyline line = new Polyline() { Stroke = brush, StrokeThickness = thickness };
            line.Points.Add(new Point(w_offset + w1, Math.Round(h1 + (1 - rate) * h)));
            for (int i = 0; i < values.Length; i++)
            {
                if (values[i] != null)
                {
                    line.Points.Add(new Point(w_offset + i * w1, Math.Round(h1 + (1 - rate) * h)));
                    Panel.SetZIndex(line, -1);
                    graph.Children.Add(line);
                    lines.Add(line);
                    line = new Polyline() { Stroke = brush, StrokeThickness = thickness };
                    line.Points.Add(new Point(w_offset + i * w1, Math.Round(h1 + (1 - rate) * h)));

                    rate = ((double)values[i] - min) / (max - min);
                    rate = Math.Max(0, Math.Min(rate, 1));
                    line.Points.Add(new Point(w_offset + i * w1, Math.Round(h1 + (1 - rate) * h)));

                    Panel.SetZIndex(line, -1);
                    graph.Children.Add(line);
                    lines.Add(line);
                    line = new Polyline() { Stroke = brush, StrokeThickness = thickness };
                    line.Points.Add(new Point(w_offset + i * w1, Math.Round(h1 + (1 - rate) * h)));

                    if (points != null)
                    {
                        PointGraph point = new PointGraph()
                        {
                            Text = valueTipText((double)values[i], format, rule),
                        };
                        Canvas.SetLeft(point, w_offset + i * w1);
                        Canvas.SetTop(point, Math.Round(h1 + (1 - rate) * h));
                        graph.Children.Add(point);
                        points.Add(point);
                    }
                }
            }
            line.Points.Add(new Point(w_offset + values.Length * w1, Math.Round(h1 + (1 - rate) * h)));
            Panel.SetZIndex(line, -1);
            graph.Children.Add(line);
            lines.Add(line);
        }
        private void Draw_Curve(Canvas graph, double[] values,
            double min, double def, double max,
            List<Polyline> lines, Brush brush, int thickness)
        {
            double h = GraphHeight;
            double x = w_offset + w1;
            double rate = (def - min) / (max - min);

            Polyline line = new Polyline() { Stroke = brush, StrokeThickness = thickness };
            for (int i = 0; i < values.Length; i++)
            {
                if (values[i] == null)
                {
                    values[i] = def;
                }
                x = w_offset + w1 + i / 10.0 * w1;
                rate = ((double)values[i] - min) / (max - min);
                rate = Math.Max(0, Math.Min(rate, 1));
                line.Points.Add(new Point(x, Math.Round(h1 + (1 - rate) * h)));
            }
            // line.Points.Add(new Point(w_offset + values.Length * w1, Math.Round(h1 + (1 - rate) * h)));
            Panel.SetZIndex(line, -1);
            graph.Children.Add(line);
            lines.Add(line);
        }

        private string valueTipText(double value, string format, Dictionary<double, string> rule)
        {
            if (Math.Abs(value) < 0.0000000000001) { value = 0; }
            var valueText = value.ToString(format);
            if (rule != null && rule.Count > 0)
            {
                foreach (var r in rule)
                {
                    if (value < r.Key)
                    {
                        var ruleText = r.Value;
                        valueText = ruleText.Replace("{}", valueText);
                        break;
                    }
                }
            }
            return valueText;
        }

        private (int, int, VoiceEffectValueBase) GetSmIndex(double x, bool SectionGroup)
        {
            double index = (x - w_offset) / w1;
            int moraCount = 0;
            if (index < 1) { return (-10, -10, null); }
            for (int i = 0; i < this.Phrase.Sections.Count; i++)
            {
                var section = this.Phrase.Sections[i];
                moraCount += 1;
                var moraCountSectionTop = moraCount;
                foreach (var mora in section.Moras)
                {
                    if (index < moraCount + 1)
                    {
                        return SectionGroup ? (i, moraCountSectionTop, section) : (i, moraCount, mora);
                    }
                    moraCount += 1;
                }
                if (index < moraCount + 1)
                {
                    return SectionGroup ? (i, moraCountSectionTop, section) : (i, moraCount - 1, section.Moras.Last());
                }
            }
            moraCount += 1;
            if (index < moraCount + 1) { return (-1, moraCount, this.Phrase.EndSection); }
            return (-10, -10, null);
        }


        int sizeChangedId = 0;
        private void Grid_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            mouse_grid.Height = 0;
            mouse_grid.Width = 0;
            var id = (sizeChangedId + 1) % 1000000;
            sizeChangedId = id;
            Task.Run(async () =>
            {
                await Task.Delay(150);
                if (sizeChangedId != id) { return; }
                this.Dispatcher.Invoke(() =>
                {
                    Draw();
                });
            });
        }

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            if (this.Phrase == null) { return; }
            if (sender is MenuItem item)
            {
                var SetValue = SetEffectSelectedValue;
                var selectedEffect = this.SelectedEffectSetting;
                if (SetValue == null || selectedEffect == null) { return; }
                var index = GetSmIndex(lastX, selectedEffect.Type != "Mora");
                var def = selectedEffect.DefaultValue;
                switch (item.Header)
                {
                    case "デフォルト値を設定":
                        SetValue(index.Item3, def);
                        break;
                    case "設定値を削除":
                        SetValue(index.Item3, null);
                        break;
                    case "全てのアクセント句の設定値を削除":
                        var result = MessageBox.Show("全てのアクセント句の設定値を削除してよろしいですか？", "確認", MessageBoxButton.OKCancel);
                        if (result != MessageBoxResult.OK) { return; }
                        foreach (var section in this.Phrase.Sections)
                        {
                            SetValue(section, null);
                            foreach (var mora in section.Moras)
                            {
                                SetValue(mora, null);
                            }
                        }
                        SetValue(this.Phrase.EndSection, null);
                        break;
                }
                Draw();
            }
        }

        private void OnMouseUp(object sender, MouseButtonEventArgs e)
        {
            this.UpdateCommand?.Execute("MouseUp");
        }

    }
}
