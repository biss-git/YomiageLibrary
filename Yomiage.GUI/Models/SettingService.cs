using ControlzEx.Theming;
using Microsoft.Win32;
using Prism.Mvvm;
using Reactive.Bindings;
using Reactive.Bindings.Notifiers;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using System.Windows;
using Yomiage.GUI.EventMessages;
using Yomiage.SDK.Common;
using Yomiage.SDK.VoiceEffects;

namespace Yomiage.GUI.Models
{
    public class SettingService
    {

        public MySettings settings { get; }

        public ReactiveCommand ResetNotification { get; } = new();

        public ReactivePropertySlim<string> Theme { get; } = new();

        public ReactivePropertySlim<double> MasterVolume { get; } = new();
        public ReactivePropertySlim<double> MasterSpeed { get; } = new();
        public ReactivePropertySlim<double> MasterPitch { get; } = new();
        public ReactivePropertySlim<double> MasterEmphasis { get; } = new();
        public ReactivePropertySlim<int> MasterShortPause { get; } = new();
        public ReactivePropertySlim<int> MasterLongPause { get; } = new();
        public ReactivePropertySlim<int> MasterEndPause { get; } = new();

        public ReactivePropertySlim<bool> ExpandEffectRange { get; } = new();

        public string PresetFilePath { get => settings.Default.PresetFilePath; set { settings.Default.PresetFilePath = value; } }
        public bool PromptStringEnable { get => settings.Default.PromptStringEnable; set { settings.Default.PromptStringEnable = value; } }
        public string PromptString { get => settings.Default.PromptString; set { settings.Default.PromptString = value; } }
        public bool PromptStringOutput { get => settings.Default.PromptStringOutput; set { settings.Default.PromptStringOutput = value; } }
        public bool PhraseDictionaryEnable { get => settings.Default.PhraseDictionaryEnable; set { settings.Default.PhraseDictionaryEnable = value; } }
        public string PhraseDictionaryPath { get => settings.Default.PhraseDictionaryPath; set { settings.Default.PhraseDictionaryPath = value; } }
        public bool WordDictionaryEnable { get => settings.Default.WordDictionaryEnable; set { settings.Default.WordDictionaryEnable = value; } }
        public string WordDictionaryPath { get => settings.Default.WordDictionaryPath; set { settings.Default.WordDictionaryPath = value; } }
        public bool PauseDictionaryEnable { get => settings.Default.PauseDictionaryEnable; set { settings.Default.PauseDictionaryEnable = value; } }
        public string PauseDictionaryPath { get => settings.Default.PauseDictionaryPath; set { settings.Default.PauseDictionaryPath = value; } }
        public int StartPause { get => settings.Default.StartPause; set { settings.Default.StartPause = value; } }
        public int EndPause { get => settings.Default.EndPause; set { settings.Default.EndPause = value; } }
        public bool SplitByEnter { get => settings.Default.SplitByEnter; set { settings.Default.SplitByEnter = value; } }
        public bool OutputSingleFile { get => settings.Default.OutputSingleFile; set { settings.Default.OutputSingleFile = value; } }
        public bool OutputMultiFile { get => settings.Default.OutputMultiFile; set { settings.Default.OutputMultiFile = value; } }
        public bool OutputMultiByChar { get => settings.Default.OutputMultiByChar; set { settings.Default.OutputMultiByChar = value; } }
        public string OutputSplitChar { get => settings.Default.OutputSplitChar; set { settings.Default.OutputSplitChar = value; } }
        public bool OutputModeWav { get => settings.Default.OutputModeWav; set { settings.Default.OutputModeWav = value; } }
        public bool OutputModeMp3 { get => settings.Default.OutputModeMp3; set { settings.Default.OutputModeMp3 = value; } }
        public bool OutputModeWma { get => settings.Default.OutputModeWma; set { settings.Default.OutputModeWma = value; } }
        public string OutputFormatWav { get => settings.Default.OutputFormatWav; set { settings.Default.OutputFormatWav = value; } }
        public string OutputFormatMp3 { get => settings.Default.OutputFormatMp3; set { settings.Default.OutputFormatMp3 = value; } }
        public string OutputFormatWma { get => settings.Default.OutputFormatWma; set { settings.Default.OutputFormatWma = value; } }
        public bool FileHeaderEnable { get => settings.Default.FileHeaderEnable; set { settings.Default.FileHeaderEnable = value; } }
        public bool SaveWithText { get => settings.Default.SaveWithText; set { settings.Default.SaveWithText = value; } }
        public string Encoding { get => settings.Default.Encoding; set { settings.Default.Encoding = value; } }
        public bool SaveByDialog { get => settings.Default.SaveByDialog; set { settings.Default.SaveByDialog = value; } }
        public bool SaveByRule { get => settings.Default.SaveByRule; set { settings.Default.SaveByRule = value; } }
        public string RuleFolderPath { get => settings.Default.RuleFolderPath; set { settings.Default.RuleFolderPath = value; } }
        public string Rule { get => settings.Default.Rule; set { settings.Default.Rule = value; } }
        public int RuleTextLength { get => settings.Default.RuleTextLength; set { settings.Default.RuleTextLength = value; } }
        public int RuleNumDigits { get => settings.Default.RuleNumDigits; set { settings.Default.RuleNumDigits = value; } }
        public int RuleStartNum { get => settings.Default.RuleStartNum; set { settings.Default.RuleStartNum = value; } }
        public string TextFontName { get => settings.Default.TextFontName; set { settings.Default.TextFontName = value; FontFamily.Value = value; } }
        public ReactivePropertySlim<string> FontFamily { get; } = new();
        public int TextFontSize { get => settings.Default.TextFontSize; set { settings.Default.TextFontSize = value; FontSize.Value = value; } }
        public ReactivePropertySlim<int> FontSize { get; } = new();
        public bool TextWordWrap { get => settings.Default.TextWordWrap; set { settings.Default.TextWordWrap = value; } }
        public bool ShowDialogWithSave { get => settings.Default.ShowDialogWithSave; set { settings.Default.ShowDialogWithSave = value; } }
        public bool AudioDefault { get => settings.Default.AudioDefault; set { settings.Default.AudioDefault = value; } }
        public string AudioName { get => settings.Default.AudioName; set { settings.Default.AudioName = value; } }
        public string MessageLevel { get => settings.Default.MessageLevel; set { settings.Default.MessageLevel = value; } }
        public bool ShowToolTip { get => settings.Default.ShowToolTip; set { settings.Default.ShowToolTip = value; } }
        public string IconSize { get => settings.Default.IconSize; set { settings.Default.IconSize = value; SetIconSize(); } }
        public ReactivePropertySlim<int> IconSizeNum { get; } = new();
        public int TuneTabIndex { get => settings.Default.TuneTabIndex; set { settings.Default.TuneTabIndex = value; } }

        public int CharacterSize { get => settings.Default.CharacterSize; set { settings.Default.CharacterSize = value; } }
        public int CharacterSleep { get => settings.Default.CharacterSleep; set { settings.Default.CharacterSleep = value; } }
        public bool CharacterEye { get => settings.Default.CharacterEye; set { settings.Default.CharacterEye = value; } }
        public bool CharacterMouth { get => settings.Default.CharacterMouth; set { settings.Default.CharacterMouth = value; } }


        public bool SystemThemeIsLight { get; }

        public SettingService()
        {
            {
                using var processModule = Process.GetCurrentProcess().MainModule;
                var settingsFile = processModule?.FileName + ".settings.json";
                settings = new MySettings(settingsFile);
                SetReactiveProperty();
            }
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
            settings.Default.Theme = Theme.Value;
            settings.Default.ExpandEffectRange = ExpandEffectRange.Value;
            Save();
        }
        public void Save(string fileName = null)
        {
            settings.Save(fileName);
        }

        public void Reset()
        {
            // 辞書のパスなどはリセットしない
            var pausePath = settings.Default.PauseDictionaryPath;
            var phrasePath = settings.Default.PhraseDictionaryPath;
            var presetPath = settings.Default.PresetFilePath;
            var wordPath = settings.Default.WordDictionaryPath;
            settings.Reset();
            settings.Default.PauseDictionaryPath = pausePath;
            settings.Default.PhraseDictionaryPath = phrasePath;
            settings.Default.PresetFilePath = presetPath;
            settings.Default.WordDictionaryPath = wordPath;

            settings.Save();
            SetReactiveProperty();
            ResetNotification.Execute();
        }

        public void Reload()
        {
            settings.Reload();
            SetReactiveProperty();
            ResetNotification.Execute();
        }

        public void Load(string filePath)
        {
            var path1 = settings.Default.PauseDictionaryPath;
            var path2 = settings.Default.PhraseDictionaryPath;
            var path3 = settings.Default.PresetFilePath;
            var path4 = settings.Default.WordDictionaryPath;
            if (settings.Load(filePath))
            {
                settings.Default.PauseDictionaryPath = path1;
                settings.Default.PhraseDictionaryPath = path2;
                settings.Default.PresetFilePath = path3;
                settings.Default.WordDictionaryPath = path4;
                SetReactiveProperty();
                Save();
                ResetNotification.Execute();
            }
        }

        /// <summary>
        /// ReactiveProperty で扱っているやつはリセット時などにこれで適用しないとダメ。
        /// </summary>
        private void SetReactiveProperty()
        {
            this.Theme.Value = settings.Default.Theme;
            this.ExpandEffectRange.Value = settings.Default.ExpandEffectRange;
            this.MasterVolume.Value = settings.Default.MasterVolume;
            this.MasterSpeed.Value = settings.Default.MasterSpeed;
            this.MasterPitch.Value = settings.Default.MasterPitch;
            this.MasterEmphasis.Value = settings.Default.MasterEmphasis;
            this.MasterShortPause.Value = settings.Default.MasterShortPause;
            this.MasterLongPause.Value = settings.Default.MasterLongPause;
            this.MasterEndPause.Value = settings.Default.MasterEndPause;
            this.FontSize.Value = this.TextFontSize;
            this.FontFamily.Value = this.TextFontName;
            SetIconSize();
        }

        public void SaveMaster()
        {
            settings.Default.MasterVolume = this.MasterVolume.Value;
            settings.Default.MasterSpeed = this.MasterSpeed.Value;
            settings.Default.MasterPitch = this.MasterPitch.Value;
            settings.Default.MasterEmphasis = this.MasterEmphasis.Value;
            settings.Default.MasterShortPause = this.MasterShortPause.Value;
            settings.Default.MasterLongPause = this.MasterLongPause.Value;
            settings.Default.MasterEndPause = this.MasterEndPause.Value;
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


    public class MySettings
    {
        public string SettingsFilePath;
        public SettingValues Default { get; private set; }
        public MySettings(string settingsFilePath, bool reload = true)
        {
            this.SettingsFilePath = settingsFilePath;
            if (reload)
            {
                if (!Reload())
                {
                    Reset();
                }
            }
            else
            {
                Reset();
            }
        }
        /// <summary>
        /// 設定を保存する。
        /// </summary>
        public void Save(string filePath = null)
        {
            JsonUtil.Serialize(Default, filePath == null ? SettingsFilePath : filePath);
        }
        /// <summary>
        /// 最後に Save した値に戻す。
        /// </summary>
        public bool Reload()
        {
            var settings = JsonUtil.Deserialize<SettingValues>(SettingsFilePath);
            if(settings != null)
            {
                Default = settings;
                return true;
            }
            return false;
        }
        /// <summary>
        /// システムに保存された初期値に戻す。
        /// </summary>
        public void Reset()
        {
            Default = new();
        }
        public bool Load(string filePath)
        {
            var settings = JsonUtil.Deserialize<SettingValues>(filePath);
            if (settings != null)
            {
                Default = settings;
                return true;
            }
            return false;
        }
    }

    public class SettingValues
    {

        public string Theme { get; set; } = "System";

        public double MasterVolume { get; set; } = 1;
        public double MasterSpeed { get; set; } = 1;
        public double MasterPitch { get; set; } = 1;
        public double MasterEmphasis { get; set; } = 1;
        public int MasterShortPause { get; set; } = 150;
        public int MasterLongPause { get; set; } = 370;
        public int MasterEndPause { get; set; } = 800;

        public bool ExpandEffectRange { get; set; } = false;

        public string PresetFilePath { get; set; } = "未登録";
        public bool PromptStringEnable { get; set; } = true;
        public string PromptString { get; set; } = "＞";
        public bool PromptStringOutput { get; set; } = true;
        public bool PhraseDictionaryEnable { get; set; } = true;
        public string PhraseDictionaryPath { get; set; } = "未登録";
        public bool WordDictionaryEnable { get; set; } = true;
        public string WordDictionaryPath { get; set; } = "未登録";
        public bool PauseDictionaryEnable { get; set; } = true;
        public string PauseDictionaryPath { get; set; } = "未登録";
        public int StartPause { get; set; } = 0;
        public int EndPause { get; set; } = 800;
        public bool SplitByEnter { get; set; } = true;
        public bool OutputSingleFile { get; set; } = true;
        public bool OutputMultiFile { get; set; } = false;
        public bool OutputMultiByChar { get; set; } = false;
        public string OutputSplitChar { get; set; } = "/";
        public bool OutputModeWav { get; set; } = true;
        public bool OutputModeMp3 { get; set; } = false;
        public bool OutputModeWma { get; set; } = false;
        public string OutputFormatWav { get; set; } = "44100Hz 16bit";
        public string OutputFormatMp3 { get; set; } = "高品質 (128 kbps)";
        public string OutputFormatWma { get; set; } = "高品質 (48 kbps)";
        public bool FileHeaderEnable { get; set; } = true;
        public bool SaveWithText { get; set; } = true;
        public string Encoding { get; set; } = "UTF-8";
        public bool SaveByDialog { get; set; } = true;
        public bool SaveByRule { get; set; } = false;
        public string RuleFolderPath { get; set; } = "";
        public string Rule { get; set; } = "";
        public int RuleTextLength { get; set; } = 10;
        public int RuleNumDigits { get; set; } = 3;
        public int RuleStartNum { get; set; } = 1;
        public string TextFontName { get; set; } = "メイリオ";
        public int TextFontSize { get; set; } = 14;
        public bool TextWordWrap { get; set; } = true;
        public bool ShowDialogWithSave { get; set; } = false;
        public bool AudioDefault { get; set; } = false;
        public string AudioName { get; set; } = "";
        public string MessageLevel { get; set; } = "冗長";
        public bool ShowToolTip { get; set; } = true;
        public string IconSize { get; set; } = "大";
        public int TuneTabIndex { get; set; } = 1;

        public int CharacterSize { get; set; } = 3;
        public int CharacterSleep { get; set; } = 3;
        public bool CharacterEye { get; set; } = true;
        public bool CharacterMouth { get; set; } = true;

        public bool PresetVisible { get; set; } = true;
        public bool TuningVisible { get; set; } = true;
        public bool CharacterVisible { get; set; } = true;
        public bool IsCharacterMaximized { get; set; } = false;
        public bool IsLineNumberVisible { get; set; } = false;

        public double PresetWidth { get; set; } = 290;
        public double CharacterWidth { get; set; } = 190;
        public double TuningHeight { get; set; } = 330;

        public bool WindowMaximized { get; set; } = false;
        public double WindowLeft { get; set; } = 100;
        public double WindowTop { get; set; } = 100;
        public double WindowWidth { get; set; } = 1010;
        public double WindowHeight { get; set; } = 700;
    }
}
