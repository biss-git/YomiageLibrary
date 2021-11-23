using Reactive.Bindings;
using Reactive.Bindings.Extensions;
using Reactive.Bindings.Notifiers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Yomiage.Core.Models;
using Yomiage.Core.Types;
using Yomiage.GUI.Data;
using Yomiage.GUI.EventMessages;
using Yomiage.GUI.Models;

namespace Yomiage.GUI.ViewModels
{
    class CharacterPanelViewModel : ViewModelBase
    {
        public ReactivePropertySlim<BitmapImage> Image { get; } = new();
        public ReactivePropertySlim<CharacterSize> Size { get; }
        public ReadOnlyReactivePropertySlim<double> Scale { get; }
        public ReactivePropertySlim<SleepMode> Sleep { get; }
        public ReactivePropertySlim<bool> EyeEnable { get; }
        public ReactivePropertySlim<bool> MouthEnable { get; }
        public ReactivePropertySlim<bool> IsCharacterMaximized { get; }
        public ReactivePropertySlim<string> PresetName { get; } = new();

        public ReactivePropertySlim<VoicePreset> Preset { get; } = new();

        public ReactivePropertySlim<Brush> Background { get; } = new ReactivePropertySlim<Brush>();

        private Brush darkBackground;
        private Brush lightBackground;

        private BitmapImage Base;
        private BitmapImage MouthOpen;
        private BitmapImage EyeClose;
        private BitmapImage Sleep1;
        private BitmapImage Sleep2;
        private CharacterState state = CharacterState.Normal;

        private ReactiveTimer timer_Eye = new(new TimeSpan(0, 0, 6));
        private ReactiveTimer timer_Talk = new(new TimeSpan(0, 0, 0, 0, 300));
        private ReactiveTimer timer_Sleep = new(new TimeSpan(0, 0, 10));
        private ReactiveTimer timer_Sleeping = new(new TimeSpan(0, 0, 0, 1, 500));

        private Random random = new();

        VoicePresetService voicePresetService;
        SettingService settingService;

        public CharacterPanelViewModel(
            VoicePresetService voicePresetService,
            SettingService settingService,
            IMessageBroker messageBroker,
            LayoutService layoutService
            )
        {
            this.voicePresetService = voicePresetService;
            this.settingService = settingService;

            this.Size = new((CharacterSize)settingService.CharacterSize);
            this.Size.Subscribe(s => settingService.CharacterSize = (int)s).AddTo(Disposables);
            this.Sleep = new((SleepMode)settingService.CharacterSleep);
            this.Sleep.Subscribe(s => settingService.CharacterSleep = (int)s).AddTo(Disposables);
            this.EyeEnable = new(settingService.CharacterEye);
            this.EyeEnable.Subscribe(e => settingService.CharacterEye = e).AddTo(Disposables);
            this.MouthEnable = new(settingService.CharacterMouth);
            this.MouthEnable.Subscribe(e => settingService.CharacterMouth = e).AddTo(Disposables);
            settingService.ResetNotification.Subscribe(() =>
            {
                this.Size.Value = (CharacterSize)settingService.CharacterSize;
                this.Sleep.Value = (SleepMode)settingService.CharacterSleep;
                this.EyeEnable.Value = settingService.CharacterEye;
                this.MouthEnable.Value = settingService.CharacterMouth;
            }).AddTo(Disposables);
            Preset.Subscribe(OnChangePreset);
            voicePresetService.SelectedPreset.Subscribe(preset =>
            {
                Preset.Value = preset;
            }).AddTo(Disposables);
            settingService.Theme.Subscribe(theme => SetBackground(theme));
            timer_Eye.Subscribe(async _ => await TimerAction());
            timer_Eye.Start();
            timer_Talk.Subscribe(Talked);
            timer_Sleep.Subscribe(OnSleep);
            timer_Sleep.Start();
            timer_Sleeping.Subscribe(OnSleeping);
            messageBroker.Subscribe<Wakeup>(_ => OnWakeup());
            messageBroker.Subscribe<PlayingEvent>(OnPlaying);
            Scale = Size.Select(x => x.ToDouble()).ToReadOnlyReactivePropertySlim();
            Sleep.Subscribe(x =>
            {
                timer_Sleep.Interval = x.ToTimeSpan();
                OnWakeup();
            }).AddTo(Disposables);
            EyeEnable.Subscribe(x =>
            {
                if (x)
                {
                    timer_Eye.Start();
                }
                else
                {
                    timer_Eye.Stop();
                }
            });
            IsCharacterMaximized = layoutService.IsCharacterMaximized;
        }

        private void OnChangePreset(VoicePreset preset)
        {
            if (preset == null) { return; }
            this.PresetName.Value = preset.Name;
            var directory = preset.Library.ConfigDirectory;
            var config = preset.Library.CharacterConfig;
            if (config == null) { return; }
            this.Base = loadBitmapImage(Path.Combine(directory, config.BasicFormat.Base));
            this.MouthOpen = loadBitmapImage(Path.Combine(directory, config.BasicFormat.MouthOpen));
            this.MouthOpen ??= this.Base;
            this.EyeClose = loadBitmapImage(Path.Combine(directory, config.BasicFormat.EyeClose));
            this.EyeClose ??= this.Base;
            this.Sleep1 = loadBitmapImage(Path.Combine(directory, config.BasicFormat.Sleep1));
            this.Sleep2 = loadBitmapImage(Path.Combine(directory, config.BasicFormat.Sleep2));
            this.Sleep2 ??= this.Sleep1;
            Image.Value = this.Base;
            this.state = CharacterState.Normal;
            try
            {
                this.darkBackground = new SolidColorBrush((Color)ColorConverter.ConvertFromString(config.DarkBackGroundColor));
            }
            catch
            {
            }
            try
            {
                this.lightBackground = new SolidColorBrush((Color)ColorConverter.ConvertFromString(config.LightBackGroundColor));
            }
            catch
            {
            }
            SetBackground(settingService.Theme.Value);
        }

        private BitmapImage loadBitmapImage(string path)
        {
            if (!File.Exists(path)) { return null; }
            try
            {
                var uri = new Uri(path);
                return new BitmapImage(new Uri(path));
            }
            catch
            {
            }
            return null;
        }

        private async Task TimerAction()
        {
            await Task.Delay(random.Next(0, 2000));
            if (state != CharacterState.Normal) { return; }
            if (random.Next(0, 3) != 0)
            {
                this.Image.Value = this.EyeClose;
                await Task.Delay(60);
            }
            else
            {
                this.Image.Value = this.EyeClose;
                await Task.Delay(33);
                this.Image.Value = this.Base;
                await Task.Delay(80);
                this.Image.Value = this.EyeClose;
                await Task.Delay(33);

            }
            this.Image.Value = this.Base;
        }

        private void OnSleep(long count)
        {
            if (count == 0 || Sleep.Value == SleepMode.NONE) { return; }
            if (this.Sleep1 == null) { return; }
            this.state = CharacterState.Sleeping;
            this.Image.Value = this.Sleep1;
            timer_Sleep.Reset();
            timer_Sleeping.Reset();
            timer_Sleeping.Start();
        }
        private void OnSleeping(long count)
        {
            if (count == 0) { return; }
            this.state = CharacterState.Sleeping;
            this.Image.Value = (this.Image.Value != this.Sleep1) ? this.Sleep1 : this.Sleep2;
        }
        private void OnWakeup()
        {
            timer_Sleeping.Reset();
            timer_Sleep.Reset();
            timer_Sleep.Start();
            if (this.state == CharacterState.Sleeping)
            {
                this.state = CharacterState.Normal;
                this.Image.Value = this.Base;
            }
        }

        private async void OnPlaying(PlayingEvent playingEvent)
        {
            OnWakeup();
            this.state = CharacterState.Talking;
            var max = playingEvent.part != null ? playingEvent.part.Max() : 0;

            Preset.Value = playingEvent.preset != null ? playingEvent.preset : voicePresetService.SelectedPreset.Value;

            this.Image.Value = (max > 0.01 && MouthEnable.Value) ? this.MouthOpen : this.Base;
            timer_Talk.Reset();
            timer_Talk.Start();
        }
        private void Talked(long count)
        {
            if (count == 0) { return; }
            this.state = CharacterState.Normal;
            this.Image.Value = this.Base;
            timer_Talk.Reset();
        }

        private void SetBackground(string theme)
        {
            if (theme.Contains("dark", StringComparison.OrdinalIgnoreCase) ||
                (theme.ToLower() == "system" && !this.settingService.SystemThemeIsLight))
            {
                this.Background.Value = darkBackground;
            }
            else
            {
                this.Background.Value = lightBackground;
            }
        }
    }

    enum CharacterState
    {
        Normal,
        Talking,
        Sleeping,
    }
}
