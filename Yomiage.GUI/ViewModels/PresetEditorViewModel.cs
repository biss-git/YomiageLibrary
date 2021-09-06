using Prism.Services.Dialogs;
using Reactive.Bindings;
using Reactive.Bindings.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;
using Yomiage.Core.Types;
using Yomiage.Core.Models;
using Yomiage.GUI.Models;
using Yomiage.GUI.Util;
using Yomiage.SDK.Talk;
using Yomiage.SDK.VoiceEffects;
using System.Windows.Media;
using Yomiage.SDK.Settings;

namespace Yomiage.GUI.ViewModels
{
    class PresetEditorViewModel : ViewModelBase
    {
        public ReactiveProperty<string> PresetName { get; } = new();
        public ReactivePropertySlim<VoicePreset> SelectedPreset { get; } = new(mode: ReactivePropertyMode.None);
        public ReactivePropertySlim<SliderConfig> VolumeConfig { get; } = new();
        public ReactivePropertySlim<SliderConfig> SpeedConfig { get; } = new();
        public ReactivePropertySlim<SliderConfig> PitchConfig { get; } = new();
        public ReactivePropertySlim<SliderConfig> EmphasisConfig { get; } = new();
        public ReactiveCollection<SliderConfig> AdditionalSettings { get; } = new();
        public ReactiveProperty<bool> PauseOverride { get; } = new();
        public ReactiveProperty<double> ShortPause { get; } = new();
        public ReactiveProperty<double> LongPause { get; } = new();

        public ReactivePropertySlim<bool> NameReadOnly { get; } = new(true);
        public ReactivePropertySlim<bool> SinglePresetMode { get; } = new();
        public ReadOnlyReactivePropertySlim<bool> SubPresetMode { get; }

        public ReactivePropertySlim<IntSetting> ShortPauseSetting { get; } = new();
        public ReactivePropertySlim<IntSetting> LongPauseSetting { get; } = new();

        public ReactiveProperty<bool> IsDirty { get; }

        public ReactiveCommand SaveCommand { get; }
        public ReactiveCommand ResetCommand { get; }
        public ReactiveCommand InitializeCommand { get; }
        public ReactiveCommand<string> OpenSettingsCommand { get; }
        public ReactiveCommand SelectSubPresetCommand { get; }
        ConfigService configService;

        public PresetEditorViewModel(
            ConfigService configService,
            VoicePresetService voicePresetService,
            IDialogService dialogService) : base(dialogService)
        {
            this.configService = configService;

            this.SubPresetMode = this.SinglePresetMode.Select(v => !v).ToReadOnlyReactivePropertySlim();

            PauseOverride.Subscribe(v => { if (this.SelectedPreset.Value != null) this.SelectedPreset.Value.VoiceEffect.PauseOverride = v; });
            ShortPause.Subscribe(v => { if (this.SelectedPreset.Value != null) this.SelectedPreset.Value.VoiceEffect.ShortPause = v; });
            LongPause.Subscribe(v => { if (this.SelectedPreset.Value != null) this.SelectedPreset.Value.VoiceEffect.LongPause = v; });

            this.IsDirty = Observable.Merge(
                this.PresetName.ChangedAsObservable(),
                this.PauseOverride.ChangedAsObservable(),
                this.ShortPause.ChangedAsObservable(),
                this.LongPause.ChangedAsObservable())
                .ToReactiveProperty(false).AddTo(Disposables);

            this.IsDirty.Subscribe(v =>
            {
                if (this.SelectedPreset.Value != null) this.SelectedPreset.Value.IsDirty = v;
            });

            SaveCommand = this.IsDirty.ToReactiveCommand().WithSubscribe(SaveAction).AddTo(Disposables);
            ResetCommand = this.IsDirty.ToReactiveCommand().WithSubscribe(ResetAction).AddTo(Disposables);
            InitializeCommand = new ReactiveCommand().WithSubscribe(InitializeAction).AddTo(Disposables);
            OpenSettingsCommand = new ReactiveCommand<string>().WithSubscribe(OpenSettingsAction).AddTo(Disposables);
            SelectSubPresetCommand = new ReactiveCommand().WithSubscribe(SelectSubPresetAction).AddTo(Disposables);

            voicePresetService.SelectedPreset.Subscribe(SetPreset).AddTo(Disposables);
        }

        private void SetPreset(VoicePreset preset)
        {
            if(preset == null) { return; }
            if(!preset.IsDirty || preset.VoiceEffectSaved == null)
            {
                preset.MakeSavedEffect();
            }
            var isDirty = preset.IsDirty;
            this.SelectedPreset.Value = preset;
            this.PresetName.Value = preset.Name;
            this.NameReadOnly.Value = preset.Type == PresetType.Standard;

            if(preset.Type == PresetType.Standard || !preset.Engine.EngineConfig.SubPresetEnable)
            {
                SinglePresetMode.Value = true;
            }
            else
            {
                SinglePresetMode.Value = false;
            }

            var config = preset.Library.LibraryConfig;

            this.VolumeConfig.Value = new SliderConfig(config.VolumeSetting, preset.VoiceEffect.Volume);
            this.SpeedConfig.Value = new SliderConfig(config.SpeedSetting, preset.VoiceEffect.Speed);
            this.PitchConfig.Value = new SliderConfig(config.PitchSetting, preset.VoiceEffect.Pitch);
            this.EmphasisConfig.Value = new SliderConfig(config.EmphasisSetting, preset.VoiceEffect.Emphasis);
            this.AdditionalSettings.Clear();
            config.AdditionalSettings?.ForEach(s =>
            {
                if (s.Hide) { return; }
                var val = preset.VoiceEffect.GetAdditionalValueOrDefault(s.Key, s.DefaultValue);
                this.AdditionalSettings.Add(new SliderConfig(s, val));
            });
            this.PauseOverride.Value = preset.VoiceEffect.PauseOverride;
            this.ShortPause.Value = preset.VoiceEffect.ShortPause;
            this.LongPause.Value = preset.VoiceEffect.LongPause;

            this.ShortPauseSetting.Value = preset.Engine.EngineConfig.ShortPauseSetting;
            this.LongPauseSetting.Value = preset.Engine.EngineConfig.LongPauseSetting;

            this.VolumeConfig.Value.Value.Subscribe(v => { SetDirty(); this.SelectedPreset.Value.VoiceEffect.Volume = v; }).AddTo(Disposables);
            this.SpeedConfig.Value.Value.Subscribe(v => { SetDirty(); this.SelectedPreset.Value.VoiceEffect.Speed = v; }).AddTo(Disposables);
            this.PitchConfig.Value.Value.Subscribe(v => { SetDirty(); this.SelectedPreset.Value.VoiceEffect.Pitch = v; }).AddTo(Disposables);
            this.EmphasisConfig.Value.Value.Subscribe(v => { SetDirty(); this.SelectedPreset.Value.VoiceEffect.Emphasis = v; }).AddTo(Disposables);
            foreach(var s in this.AdditionalSettings)
            {
                s.Value.Subscribe(v => {
                    SetDirty();
                    if (!this.SelectedPreset.Value.VoiceEffect.AdditionalEffect.ContainsKey(s.Key))
                    {
                        this.SelectedPreset.Value.VoiceEffect.AdditionalEffect.Add(s.Key, 0);
                    }
                    this.SelectedPreset.Value.VoiceEffect.AdditionalEffect[s.Key] = v;
                }).AddTo(Disposables);
            }
            this.IsDirty.Value = isDirty;
        }

        private void SaveAction()
        {
            this.SelectedPreset.Value.Name = this.PresetName.Value;
            var effect = this.SelectedPreset.Value.VoiceEffect;
            effect.Volume = this.VolumeConfig.Value.Value.Value;
            effect.Speed = this.SpeedConfig.Value.Value.Value;
            effect.Pitch = this.PitchConfig.Value.Value.Value;
            effect.Emphasis = this.EmphasisConfig.Value.Value.Value;
            foreach (var s in this.AdditionalSettings)
            {
                if (!this.SelectedPreset.Value.VoiceEffect.AdditionalEffect.ContainsKey(s.Key))
                {
                    this.SelectedPreset.Value.VoiceEffect.AdditionalEffect.Add(s.Key, 0);
                }
                this.SelectedPreset.Value.VoiceEffect.AdditionalEffect[s.Key] = s.Value.Value;
            }
            effect.PauseOverride = this.PauseOverride.Value;
            effect.ShortPause = this.ShortPause.Value;
            effect.LongPause = this.LongPause.Value;
            configService.SavePresets();
            this.SelectedPreset.Value.MakeSavedEffect();
            ResetDirty();
        }

        private void ResetAction()
        {
            this.SelectedPreset.Value.ResetEffect();
            SetPreset(this.SelectedPreset.Value);
            ResetDirty();
        }

        private void InitializeAction()
        {
            this.VolumeConfig.Value.SetDefault();
            this.SpeedConfig.Value.SetDefault();
            this.PitchConfig.Value.SetDefault();
            this.EmphasisConfig.Value.SetDefault();
            foreach (var s in this.AdditionalSettings)
            {
                s.SetDefault();
            }

            PauseOverride.Value = false;
            var shortPause = this.SelectedPreset.Value.Engine.EngineConfig.ShortPauseSetting?.DefaultValue;
            ShortPause.Value = (shortPause != null) ? (double)shortPause : 150;
            var longPause = this.SelectedPreset.Value.Engine.EngineConfig.LongPauseSetting?.DefaultValue;
            LongPause.Value = (longPause != null) ? (double)longPause : 370;
        }

        private void OpenSettingsAction(string param)
        {
            IDialogParameters parameters = new DialogParameters();
            switch (param)
            {
                case "Main":
                    parameters.Add("Library", this.SelectedPreset.Value.Library);
                    this.DialogService.ShowDialog("SettingsLibraryDialog", parameters, result => { });
                    break;
                case "Sub":
                    if(this.SelectedPreset.Value.SubPreset == null) { return; }
                    parameters.Add("Library", this.SelectedPreset.Value.SubPreset.Library);
                    this.DialogService.ShowDialog("SettingsLibraryDialog", parameters, result => { });
                    break;
                case "Engine":
                    parameters.Add("Engine", this.SelectedPreset.Value.Engine);
                    this.DialogService.ShowDialog("SettingsEngineDialog", parameters, result => { });
                    break;
            }
        }

        private void SelectSubPresetAction()
        {
            IDialogParameters parameters = new DialogParameters();
            parameters.Add("Preset", this.SelectedPreset.Value);
            this.DialogService.ShowDialog("PresetFusionDialog",
                parameters,
                result => {
                    if(result.Result == ButtonResult.OK)
                    {
                        this.SelectedPreset.Value = this.SelectedPreset.Value;
                        SetDirty();
                    }
                });
        }

        private void SetDirty()
        {
            this.IsDirty.Value = true;
        }

        public void ResetDirty()
        {
            this.IsDirty.Value = false;
        }

    }

    public class SliderConfig
    {
        public ReactiveProperty<double> Value { get; }
        public string Key { get; set; }
        public double Default { get; set; }
        public double Min { get; set; }
        public double Max { get; set; }
        public double Tick { get; set; }
        public double Small { get; set; }
        public string Format { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Unit { get; set; }
        public bool Hide { get; set; }
        public Brush Color { get; set; }

        public SliderConfig(EffectSetting setting, double? value)
        {
            Value = new ReactiveProperty<double>(value == null ? setting.DefaultValue : (double)value);
            Key = setting.Key;
            Default = setting.DefaultValue;
            Min = setting.Min;
            Max = setting.Max;
            Tick = setting.SmallStep * 10;
            Small = setting.SmallStep;
            Format = setting.StringFormat;
            Title = setting.Name;
            if (!string.IsNullOrWhiteSpace(setting.Description))
            {
                Description = setting.Description;
            }
            else
            {
                Description = Title;
            }
            Unit = setting.Unit;
            Hide = setting.Hide;
            try
            {
                if (!string.IsNullOrWhiteSpace(setting.Color))
                {
                    this.Color = new SolidColorBrush((Color)ColorConverter.ConvertFromString(setting.Color));
                }
            }
            catch
            {
            }

        }

        public void SetDefault()
        {
            Value.Value = Default;
        }
    }
}
