using ControlzEx.Theming;
using Microsoft.Win32;
using Prism.Mvvm;
using Reactive.Bindings;
using Reactive.Bindings.Notifiers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Yomiage.GUI.EventMessages;
using Yomiage.SDK.VoiceEffects;

namespace Yomiage.GUI.Models
{
    public class SettingService
    {
        public ReactiveCommand ResetNotification { get; } = new ReactiveCommand();

        public ReactivePropertySlim<string> Theme { get; } = new ReactivePropertySlim<string>(Settings.Default.Theme);

        public ReactivePropertySlim<double> MasterVolume { get; } = new ReactivePropertySlim<double>(Settings.Default.MasterVolume);
        public ReactivePropertySlim<double> MasterSpeed { get; } = new ReactivePropertySlim<double>(Settings.Default.MasterSpeed);
        public ReactivePropertySlim<double> MasterPitch { get; } = new ReactivePropertySlim<double>(Settings.Default.MasterPitch);
        public ReactivePropertySlim<double> MasterEmphasis { get; } = new ReactivePropertySlim<double>(Settings.Default.MasterEmphasis);
        public ReactivePropertySlim<int> MasterShortPause { get; } = new ReactivePropertySlim<int>(Settings.Default.MasterShortPause);
        public ReactivePropertySlim<int> MasterLongPause { get; } = new ReactivePropertySlim<int>(Settings.Default.MasterLongPause);
        public ReactivePropertySlim<int> MasterEndPause { get; } = new ReactivePropertySlim<int>(Settings.Default.MasterEndPause);

        public ReactivePropertySlim<bool> ExpandEffectRange { get; } = new ReactivePropertySlim<bool>(Settings.Default.ExpandEffectRange);

        public string PresetFilePath { get => Settings.Default.PresetFilePath; set { Settings.Default.PresetFilePath = value; } }
        public bool PromptStringEnable { get => Settings.Default.PromptStringEnable; set { Settings.Default.PromptStringEnable = value; } }
        public string PromptString { get => Settings.Default.PromptString; set { Settings.Default.PromptString = value; } }
        public bool PromptStringOutput { get => Settings.Default.PromptStringOutput; set { Settings.Default.PromptStringOutput = value; } }
        public bool PhraseDictionaryEnable { get => Settings.Default.PhraseDictionaryEnable; set { Settings.Default.PhraseDictionaryEnable = value; } }
        public string PhraseDictionaryPath { get => Settings.Default.PhraseDictionaryPath; set { Settings.Default.PhraseDictionaryPath = value; } }
        public bool WordDictionaryEnable { get => Settings.Default.WordDictionaryEnable; set { Settings.Default.WordDictionaryEnable = value; } }
        public string WordDictionaryPath { get => Settings.Default.WordDictionaryPath; set { Settings.Default.WordDictionaryPath = value; } }
        public bool PauseDictionaryEnable { get => Settings.Default.PauseDictionaryEnable; set { Settings.Default.PauseDictionaryEnable = value; } }
        public string PauseDictionaryPath { get => Settings.Default.PauseDictionaryPath; set { Settings.Default.PauseDictionaryPath = value; } }
        public int StartPause { get => Settings.Default.StartPause; set { Settings.Default.StartPause = value; } }
        public int EndPause { get => Settings.Default.EndPause; set { Settings.Default.EndPause = value; } }
        public bool SplitByEnter { get => Settings.Default.SplitByEnter; set { Settings.Default.SplitByEnter = value; } }
        public bool OutputSingleFile { get => Settings.Default.OutputSingleFile; set { Settings.Default.OutputSingleFile = value; } }
        public bool OutputMultiFile { get => Settings.Default.OutputMultiFile; set { Settings.Default.OutputMultiFile = value; } }
        public bool OutputMultiByChar { get => Settings.Default.OutputMultiByChar; set { Settings.Default.OutputMultiByChar = value; } }
        public string OutputSplitChar { get => Settings.Default.OutputSplitChar; set { Settings.Default.OutputSplitChar = value; } }
        public bool OutputModeWav { get => Settings.Default.OutputModeWav; set { Settings.Default.OutputModeWav = value; } }
        public bool OutputModeMp3 { get => Settings.Default.OutputModeMp3; set { Settings.Default.OutputModeMp3 = value; } }
        public bool OutputModeWma { get => Settings.Default.OutputModeWma; set { Settings.Default.OutputModeWma = value; } }
        public string OutputFormatWav { get => Settings.Default.OutputFormatWav; set { Settings.Default.OutputFormatWav = value; } }
        public string OutputFormatMp3 { get => Settings.Default.OutputFormatMp3; set { Settings.Default.OutputFormatMp3 = value; } }
        public string OutputFormatWma { get => Settings.Default.OutputFormatWma; set { Settings.Default.OutputFormatWma = value; } }
        public bool FileHeaderEnable { get => Settings.Default.FileHeaderEnable; set { Settings.Default.FileHeaderEnable = value; } }
        public bool SaveWithText { get => Settings.Default.SaveWithText; set { Settings.Default.SaveWithText = value; } }
        public string Encoding { get => Settings.Default.Encoding; set { Settings.Default.Encoding = value; } }
        public bool SaveByDialog { get => Settings.Default.SaveByDialog; set { Settings.Default.SaveByDialog = value; } }
        public bool SaveByRule { get => Settings.Default.SaveByRule; set { Settings.Default.SaveByRule = value; } }
        public string RuleFolderPath { get => Settings.Default.RuleFolderPath; set { Settings.Default.RuleFolderPath = value; } }
        public string Rule { get => Settings.Default.Rule; set { Settings.Default.Rule = value; } }
        public int RuleTextLength { get => Settings.Default.RuleTextLength; set { Settings.Default.RuleTextLength = value; } }
        public int RuleNumDigits { get => Settings.Default.RuleNumDigits; set { Settings.Default.RuleNumDigits = value; } }
        public int RuleStartNum { get => Settings.Default.RuleStartNum; set { Settings.Default.RuleStartNum = value; } }
        public string TextFontName { get => Settings.Default.TextFontName; set { Settings.Default.TextFontName = value; FontFamily.Value = value; } }
        public ReactivePropertySlim<string> FontFamily { get; } = new(Settings.Default.TextFontName);
        public int TextFontSize { get => Settings.Default.TextFontSize; set { Settings.Default.TextFontSize = value; FontSize.Value = value; } }
        public ReactivePropertySlim<int> FontSize { get; } = new(Settings.Default.TextFontSize);
        public bool TextWordWrap { get => Settings.Default.TextWordWrap; set { Settings.Default.TextWordWrap = value; } }
        public bool ShowDialogWithSave { get => Settings.Default.ShowDialogWithSave; set { Settings.Default.ShowDialogWithSave = value; } }
        public bool AudioDefault { get => Settings.Default.AudioDefault; set { Settings.Default.AudioDefault = value; } }
        public string AudioName { get => Settings.Default.AudioName; set { Settings.Default.AudioName = value; } }
        public string MessageLevel { get => Settings.Default.MessageLevel; set { Settings.Default.MessageLevel = value; } }
        public bool ShowToolTip { get => Settings.Default.ShowToolTip; set { Settings.Default.ShowToolTip = value; } }
        public string IconSize { get => Settings.Default.IconSize; set { Settings.Default.IconSize = value; SetIconSize(); } }
        public ReactivePropertySlim<int> IconSizeNum { get; } = new();
        public int TuneTabIndex { get => Settings.Default.TuneTabIndex; set { Settings.Default.TuneTabIndex = value; } }

        public int CharacterSize { get => Settings.Default.CharacterSize; set { Settings.Default.CharacterSize = value; } }
        public int CharacterSleep { get => Settings.Default.CharacterSleep; set { Settings.Default.CharacterSleep = value; } }
        public bool CharacterEye { get => Settings.Default.CharacterEye; set { Settings.Default.CharacterEye = value; } }
        public bool CharacterMouth { get => Settings.Default.CharacterMouth; set { Settings.Default.CharacterMouth = value; } }


        public bool SystemThemeIsLight { get; }

        public SettingService()
        {
            {
                // Windows のアプリのテーマ設定（ダーク or ライト）を取得する。　MetroRadiance のコードから拝借した。
                const string keyName = @"HKEY_CURRENT_USER\SOFTWARE\Microsoft\Windows\CurrentVersion\Themes\Personalize";
                const string valueName = "AppsUseLightTheme";
                SystemThemeIsLight = Registry.GetValue(keyName, valueName, null) as int? == 1;
            }
            Theme.Subscribe(theme =>
            {
                if(theme.ToLower() == "system")
                {
                    ThemeManager.Current.ChangeTheme(Application.Current, SystemThemeIsLight ? "Light.Aoi" : "Dark.Akane");
                    return;
                }
                ThemeManager.Current.ChangeTheme(Application.Current, theme);
            });
            SetIconSize();
        }

        private void SetIconSize()
        {
            switch (IconSize)
            {
                case "小": IconSizeNum.Value = 25; break;
                default: IconSizeNum.Value = 50; break;
            }
        }


        public void SaveEnvironment()
        {
            Settings.Default.Theme = Theme.Value;
            Settings.Default.ExpandEffectRange = ExpandEffectRange.Value;
            Save();
        }
        public void Save()
        {
            Settings.Default.Save();
        }

        public void Reset()
        {
            // 辞書のパスなどはリセットしない
            var temp = 1;
            Settings.Default.Reset();
            var setting = temp;

            Settings.Default.Save();
            SetReactiveProperty();
            ResetNotification.Execute();
        }

        public void Reload()
        {
            Settings.Default.Reload();
            SetReactiveProperty();
        }

        /// <summary>
        /// ReactiveProperty で扱っているやつはリセット時などにこれで適用しないとダメ。
        /// </summary>
        private void SetReactiveProperty()
        {
            this.Theme.Value = Settings.Default.Theme;
            this.ExpandEffectRange.Value = Settings.Default.ExpandEffectRange;
            this.MasterVolume.Value = Settings.Default.MasterVolume;
            this.MasterSpeed.Value = Settings.Default.MasterSpeed;
            this.MasterPitch.Value = Settings.Default.MasterPitch;
            this.MasterEmphasis.Value = Settings.Default.MasterEmphasis;
            this.MasterShortPause.Value = Settings.Default.MasterShortPause;
            this.MasterLongPause.Value = Settings.Default.MasterLongPause;
            this.MasterEndPause.Value = Settings.Default.MasterEndPause;
            this.FontSize.Value = this.TextFontSize;
            this.FontFamily.Value = this.TextFontName;
            SetIconSize();
        }

        public void SaveMaster()
        {
            Settings.Default.MasterVolume = this.MasterVolume.Value;
            Settings.Default.MasterSpeed = this.MasterSpeed.Value;
            Settings.Default.MasterPitch = this.MasterPitch.Value;
            Settings.Default.MasterEmphasis = this.MasterEmphasis.Value;
            Settings.Default.MasterShortPause = this.MasterShortPause.Value;
            Settings.Default.MasterLongPause = this.MasterLongPause.Value;
            Settings.Default.MasterEndPause = this.MasterEndPause.Value;
            Save();
        }
        public void ResetMaster()
        {
            this.MasterVolume.Value = 1;
            this.MasterSpeed.Value = 1;
            this.MasterPitch.Value = 1;
            this.MasterEmphasis.Value = 1;
            this.MasterShortPause.Value = 150;
            this.MasterLongPause.Value = 370;
            this.MasterEndPause.Value = 800;
            SaveMaster();
        }

        public MasterEffectValue GetMasterEffectValue()
        {
            var effect = new MasterEffectValue();
            effect.ShortPause = MasterShortPause.Value;
            effect.LongPause = MasterLongPause.Value;
            effect.EndPause = MasterEndPause.Value;
            effect.Volume = MasterVolume.Value;
            effect.Speed = MasterSpeed.Value;
            effect.Pitch = MasterPitch.Value;
            effect.Emphasis = MasterEmphasis.Value;
            return effect;
        }
    }
}
