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

        public MySettings Settings { get; }

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
        public ReactivePropertySlim<string> FontFamily { get; } = new();
        public int TextFontSize { get => Settings.Default.TextFontSize; set { Settings.Default.TextFontSize = value; FontSize.Value = value; } }
        public ReactivePropertySlim<int> FontSize { get; } = new();
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
                using var processModule = Process.GetCurrentProcess().MainModule;
                var settingsFile = processModule?.FileName + ".settings.json";
                Settings = new MySettings(settingsFile);
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
                if (theme.ToLower() == "system")
                {
                    ThemeManager.Current.ChangeTheme(Application.Current, SystemThemeIsLight ? "Light.Aoi" : "Dark.Aoi");
                    return;
                }
                ThemeManager.Current.ChangeTheme(Application.Current, theme);
            });
            SetIconSize();
        }

        private void SetIconSize()
        {
            IconSizeNum.Value = IconSize switch
            {
                "小" => 25,
                _ => 50,
            };
        }


        public void SaveEnvironment()
        {
            Settings.Default.Theme = Theme.Value;
            Settings.Default.ExpandEffectRange = ExpandEffectRange.Value;
            Save();
        }
        public void Save(string fileName = null)
        {
            Settings.Save(fileName);
        }

        public void Reset()
        {
            // 辞書のパスなどはリセットしない
            var pausePath = Settings.Default.PauseDictionaryPath;
            var phrasePath = Settings.Default.PhraseDictionaryPath;
            var presetPath = Settings.Default.PresetFilePath;
            var wordPath = Settings.Default.WordDictionaryPath;
            Settings.Reset();
            Settings.Default.PauseDictionaryPath = pausePath;
            Settings.Default.PhraseDictionaryPath = phrasePath;
            Settings.Default.PresetFilePath = presetPath;
            Settings.Default.WordDictionaryPath = wordPath;

            Settings.Save();
            SetReactiveProperty();
            ResetNotification.Execute();
        }

        public void Reload()
        {
            Settings.Reload();
            SetReactiveProperty();
            ResetNotification.Execute();
        }

        public void Load(string filePath)
        {
            var path1 = Settings.Default.PauseDictionaryPath;
            var path2 = Settings.Default.PhraseDictionaryPath;
            var path3 = Settings.Default.PresetFilePath;
            var path4 = Settings.Default.WordDictionaryPath;
            if (Settings.Load(filePath))
            {
                Settings.Default.PauseDictionaryPath = path1;
                Settings.Default.PhraseDictionaryPath = path2;
                Settings.Default.PresetFilePath = path3;
                Settings.Default.WordDictionaryPath = path4;
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
            var effect = new MasterEffectValue
            {
                ShortPause = MasterShortPause.Value,
                LongPause = MasterLongPause.Value,
                EndPause = MasterEndPause.Value,
                Volume = MasterVolume.Value,
                Speed = MasterSpeed.Value,
                Pitch = MasterPitch.Value,
                Emphasis = MasterEmphasis.Value
            };
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
            JsonUtil.Serialize(Default, filePath ?? SettingsFilePath);
        }
        /// <summary>
        /// 最後に Save した値に戻す。
        /// </summary>
        public bool Reload()
        {
            var settings = JsonUtil.Deserialize<SettingValues>(SettingsFilePath);
            if (settings != null)
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
