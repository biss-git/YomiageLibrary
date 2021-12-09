using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Unicode;
using System.Threading.Tasks;
using Yomiage.Core.Types;
using Yomiage.Core.Models;
using Yomiage.GUI.Classes;
using Yomiage.GUI.Util;
using Yomiage.SDK;
using Yomiage.SDK.Common;
using Yomiage.SDK.Config;
using Yomiage.SDK.Settings;
using Yomiage.SDK.VoiceEffects;
using System.Windows;
using Yomiage.GUI.Data;
using System.Diagnostics;
using System.IO.Compression;

namespace Yomiage.GUI.Models
{
    public class ConfigService
    {
        public readonly string HomeDirectory;
        public readonly string EngineDirectory;
        public readonly string LibraryDirectory;
        public readonly string PhraseDirectory;
        public readonly string PauseDirectory;
        public readonly string PresetDirectory;
        public readonly string WordDirectory;
        private readonly VoiceEngineService voiceEngineService;
        private readonly VoiceLibraryService voiceLibraryService;
        private readonly VoicePresetService voicePresetService;
        private readonly SettingService settingService;
        private readonly AppConfig appConfig;

        public ConfigService(
            SettingService settingService,
            VoiceEngineService voiceEngineService,
            VoiceLibraryService voiceLibraryService,
            VoicePresetService voicePresetService)
        {
            this.voiceEngineService = voiceEngineService;
            this.voiceLibraryService = voiceLibraryService;
            this.voicePresetService = voicePresetService;
            this.settingService = settingService;


            var obj = Application.Current.Properties["AppConfig"];
            appConfig = new AppConfig();
            if (obj is AppConfig config)
            {
                appConfig = config;
            }

            if (appConfig.DocumentDirectoryMode)
            {
                var DocumentDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
                HomeDirectory = Path.Combine(DocumentDirectory, "YomiageUI");
                CreateDirectory(HomeDirectory);
            }
            else
            {
                using var processModule = Process.GetCurrentProcess().MainModule;
                if (processModule != null)
                {
                    HomeDirectory = Path.GetDirectoryName(processModule.FileName);
                }
            }
            EngineDirectory = Path.Combine(HomeDirectory, "00_Engine");
            CreateDirectory(EngineDirectory);
            LibraryDirectory = Path.Combine(HomeDirectory, "01_Library");
            CreateDirectory(LibraryDirectory);
            PhraseDirectory = Path.Combine(HomeDirectory, "02_PhraseDictionaries");
            CreateDirectory(PhraseDirectory);
            PauseDirectory = Path.Combine(HomeDirectory, "03_SymbolDictionaries");
            CreateDirectory(PauseDirectory);
            PresetDirectory = Path.Combine(HomeDirectory, "04_VoicePresets");
            CreateDirectory(PresetDirectory);
            WordDirectory = Path.Combine(HomeDirectory, "05_WordDictionaries");
            CreateDirectory(WordDirectory);
        }

        private static void CreateDirectory(string directory)
        {
            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }
        }

        Action<string, double> submitState;
        int directoryNum = 4;
        int loadCount = 0;

        public void Init(Action<string, double> submitState)
        {
            this.submitState = submitState;
            directoryNum = 4;
            if (!string.IsNullOrWhiteSpace(appConfig?.EngineDirectory))
            {
                directoryNum += 1;
            }
            if (!string.IsNullOrWhiteSpace(appConfig?.LibraryDirectory))
            {
                directoryNum += 1;
            }
            loadCount = 1;
            LoadEngine(Path.Combine(Directory.GetCurrentDirectory(), "Engine"), $"( {loadCount} / {(directoryNum + 1)} )エンジンの読み込み。");
            loadCount += 1;
            LoadEngine(EngineDirectory, $"( {loadCount} / {(directoryNum + 1)} )エンジンの読み込み。");
            loadCount += 1;
            LoadLibrary(Path.Combine(Directory.GetCurrentDirectory(), "Library"), $"( {loadCount} / {(directoryNum + 1)} )ライブラリの読み込み。");
            loadCount += 1;
            LoadLibrary(LibraryDirectory, $"( {loadCount} / {(directoryNum + 1)} )ライブラリの読み込み。");
            loadCount += 1;
            if (!string.IsNullOrWhiteSpace(appConfig?.EngineDirectory))
            {
                LoadEngine(appConfig.EngineDirectory, $"( {loadCount} / {(directoryNum + 1)} )エンジンの読み込み。");
                loadCount += 1;
            }
            if (!string.IsNullOrWhiteSpace(appConfig?.LibraryDirectory))
            {
                LoadLibrary(appConfig.LibraryDirectory, $"( {loadCount} / {(directoryNum + 1)} )ライブラリの読み込み。");
                loadCount += 1;
            }
            submitState($"( {loadCount} / {(directoryNum + 1)} )プリセットの初期化・読み込み。", 0.9);
            InitPreset();
            submitState($"( {loadCount} / {(directoryNum + 1)} )プリセットの初期化・読み込み。", 0.95);
            LoadPresets();
            this.submitState = null;
        }

        public void LoadEngine(string EngineDirectory, string Title = "")
        {
            if (string.IsNullOrWhiteSpace(EngineDirectory)) { return; }
            var files = new List<string>();
            Utility.SearchFile(EngineDirectory, "engine.config.json", 8, files);
            var count = 0;
            foreach (var f in files)
            {
                submitState?.Invoke(Title + $" ( {count + 1} / {files.Count} )\n{f}",
                    ((double)(loadCount - 1) / (directoryNum + 1)) + (double)count / files.Count / (directoryNum + 1));
                count += 1;
                try
                {
                    var config = JsonUtil.Deserialize<EngineConfig>(f);
                    if (string.IsNullOrWhiteSpace(config?.Key) || voiceEngineService.AllEngines.Any(e => e.EngineConfig.Key == config?.Key)) { continue; }
                    var directory = Path.GetDirectoryName(f);

                    var settings = new EngineSettings();
                    {
                        // settings 読み込み
                        var settingsPath = Path.Combine(directory, "engine.settings.json");
                        if (File.Exists(settingsPath))
                        {
                            try
                            {
                                settings = JsonUtil.Deserialize<EngineSettings>(settingsPath);
                            }
                            catch (Exception)
                            {

                            }
                            if (settings == null)
                            {
                                settings = new EngineSettings();
                            }
                        }
                    }

                    var dllPath = Path.Combine(directory, config.AssemblyName);
                    if (!File.Exists(dllPath)) { continue; }
                    var asm = Assembly.LoadFrom(dllPath);       // アセンブリの読み込み
                    Type type = null;

                    if (!string.IsNullOrWhiteSpace(config.ModuleName) && !string.IsNullOrWhiteSpace(config.TypeName))
                    {
                        var module = asm.GetModule(config.ModuleName);        // アセンブリからモジュールを取得
                        type = module?.GetType(config.TypeName);    // 利用するクラス(or 構造体)を取得
                    }

                    if (type == null)
                    {
                        foreach (var module in asm.GetModules())
                        {
                            foreach (var t in module.GetTypes())
                            {
                                if (typeof(IVoiceEngine).IsAssignableFrom(t))
                                {
                                    type = t;
                                    break;
                                }
                            }
                        }
                    }

                    if (type == null) { continue; }

                    var engine = Activator.CreateInstance(type);
                    if (engine is IVoiceEngine voiceEngine)
                    {
                        var dllDirectory = Path.GetDirectoryName(dllPath);
                        voiceEngineService.Add(new Engine(directory, dllDirectory, voiceEngine, config, settings));
                    }
                }
                catch (Exception e)
                {
                    string aaa = e.ToString();
                }
            }
        }

        public void LoadLibrary(string LibraryDirectory, string Title = "")
        {
            if (string.IsNullOrWhiteSpace(LibraryDirectory)) { return; }
            var files = new List<string>();
            Utility.SearchFile(LibraryDirectory, "library.config.json", 8, files);
            var count = 0;
            foreach (var f in files)
            {
                submitState?.Invoke(Title + $" ( {count + 1} / {files.Count} )\n{f}",
                    ((double)(loadCount - 1) / (directoryNum + 1)) + (double)count / files.Count / (directoryNum + 1));
                count += 1;
                try
                {
                    var config = JsonUtil.Deserialize<LibraryConfig>(f);
                    if (string.IsNullOrWhiteSpace(config?.Key) || voiceLibraryService.AllLibrarys.Any(l => l.LibraryConfig.Key == config?.Key)) { continue; }
                    var directory = Path.GetDirectoryName(f);

                    var settings = LoadLibrarySettings(Path.Combine(directory, "library.settings.json"));

                    var dllPath = Path.Combine(directory, config.AssemblyName);

                    var voiceLibrary = CreateLibrary(directory, config.AssemblyName, config.ModuleName, config.TypeName);

                    if (voiceLibrary == null)
                    {
                        continue;
                    }

                    var characterPath = Path.Combine(directory, "character.config.json");
                    var characterConfig = JsonUtil.Deserialize<CharacterConfig>(characterPath);

                    //if (library is IVoiceLibrary voiceLibrary)
                    {
                        var dllDirectory = Path.GetDirectoryName(dllPath);
                        voiceLibraryService.Add(new Library(directory, dllDirectory, voiceLibrary, config, settings, characterConfig));
                    }
                }
                catch (Exception)
                {

                }
            }
        }

        private IVoiceLibrary CreateLibrary(string directory, string assemblyName, string moduleName, string typeName)
        {
            if (string.IsNullOrWhiteSpace(assemblyName))
            {
                return new VoiceLibraryDummy();
            }
            var dllPath = Path.Combine(directory, assemblyName);
            if (!File.Exists(dllPath)) { return null; }
            var asm = Assembly.LoadFrom(dllPath);       // アセンブリの読み込み
            Type type = null;

            if (!string.IsNullOrWhiteSpace(moduleName) && !string.IsNullOrWhiteSpace(typeName))
            {
                var module = asm.GetModule(moduleName);        // アセンブリからモジュールを取得
                type = module?.GetType(typeName);    // 利用するクラス(or 構造体)を取得
            }

            if (type == null)
            {
                foreach (var module in asm.GetModules())
                {
                    foreach (var t in module.GetTypes())
                    {
                        if (typeof(IVoiceLibrary).IsAssignableFrom(t))
                        {
                            type = t;
                            break;
                        }
                    }
                }
            }
            if (type == null) { return null; }

            var library = Activator.CreateInstance(type);

            if (library is IVoiceLibrary voiceLibrary)
            {
                return voiceLibrary;
            }

            return null;
        }

        public void InitPreset()
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                foreach (var engine in voiceEngineService.AllEngines)
                {
                    if (engine.EngineConfig.LibraryFormat == null || engine.EngineConfig.LibraryFormat.Length == 0)
                    {
                        // エンジン単体でプリセットとなる

                        var files = new List<string>();
                        var keys = new List<string>();
                        Utility.SearchFile(engine.ConfigDirectory, "library.config.json", 5, files);
                        Utility.SearchFile(engine.DllDirectory, "library.config.json", 5, files);
                        files = files.Distinct().ToList();
                        foreach (var f in files)
                        {
                            try
                            {
                                var config = JsonUtil.Deserialize<LibraryConfig>(f);
                                if (string.IsNullOrWhiteSpace(config?.Key) || keys.Any(k => k == config?.Key)) { continue; }
                                var directory = Path.GetDirectoryName(f);

                                var settings = LoadLibrarySettings(Path.Combine(directory, "library.settings.json"));

                                var characterPath = Path.Combine(directory, "character.config.json");
                                var characterConfig = JsonUtil.Deserialize<CharacterConfig>(characterPath);

                                var dllDirectory = string.Empty;

                                IVoiceLibrary voiceLibrary = CreateLibrary(directory, config.AssemblyName, config.ModuleName, config.TypeName);

                                if (voiceLibrary is VoiceLibraryDummy)
                                {
                                    dllDirectory = engine.DllDirectory;
                                }
                                else
                                {
                                    dllDirectory = Path.GetDirectoryName(Path.Combine(directory, config.AssemblyName));
                                }

                                var library = new Library(directory, dllDirectory, voiceLibrary, config, settings, characterConfig, engine.ConfigDirectory);
                                voicePresetService.Add(new VoicePreset(engine, library)
                                {
                                    Type = PresetType.Standard,
                                    Name = library.LibraryConfig.Name
                                });
                                keys.Add(config.Key);
                            }
                            catch (Exception)
                            {

                            }
                        }

                    }
                    else
                    {
                        // VoiceLibrary と組み合わせる
                        foreach (var library in voiceLibraryService.AllLibrarys)
                        {
                            bool match = engine.EngineConfig.LibraryFormat.Any(f => library.LibraryConfig.LibraryFormat?.Contains(f) == true);
                            if (!match) { continue; }
                            if (voicePresetService.StandardPreset.Any(p => p.EngineKey == engine.EngineConfig.Key && p.LibraryKey == library.LibraryConfig.Key)) { continue; }
                            voicePresetService.Add(new VoicePreset(engine, library)
                            {
                                Type = PresetType.Standard,
                                Name = library.LibraryConfig.Name
                            });
                        }
                    }
                }
                voicePresetService.SelectedPreset.Value = voicePresetService.AllPresets.FirstOrDefault();
            });
        }

        private LibrarySettings LoadLibrarySettings(string filePath)
        {
            var settings = new LibrarySettings();
            {
                // settings 読み込み
                var settingsPath = Path.Combine(filePath);
                if (File.Exists(settingsPath))
                {
                    try
                    {
                        settings = JsonUtil.Deserialize<LibrarySettings>(settingsPath);
                    }
                    catch (Exception)
                    {

                    }
                    if (settings == null)
                    {
                        settings = new LibrarySettings();
                    }
                }
            }
            return settings;
        }

        public void SavePresets()
        {
            var filePath = CheckFilePath();
            if (string.IsNullOrWhiteSpace(filePath) || !File.Exists(filePath)) { return; }

            var dict = voicePresetService.AllPresets.Select(p =>
            new PresetData(p, p == voicePresetService.SelectedPreset.Value));
            var options = new JsonSerializerOptions
            {
                WriteIndented = true,
                Encoder = JavaScriptEncoder.Create(UnicodeRanges.All)
            };
            var jsonstr = JsonSerializer.Serialize(dict, options);
            File.WriteAllText(filePath, jsonstr);
        }

        public void LoadPresets()
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                var filePath = CheckFilePath();
                if (string.IsNullOrWhiteSpace(filePath) || !File.Exists(filePath)) { return; }
                voicePresetService.RemoveUserPresets();
                var jsonstr = File.ReadAllText(filePath);
                try
                {
                    var dict = JsonSerializer.Deserialize<IEnumerable<PresetData>>(jsonstr);
                    foreach (var data in dict)
                    {
                        var originalPreset = this.voicePresetService.AllPresets.FirstOrDefault(p => p.Engine.EngineConfig.Key == data.EngineKey && p.Library.LibraryConfig.Key == data.LibraryKey);
                        if (originalPreset == null)
                        {
                            continue;
                        }

                        if (data.Type == PresetType.Standard)
                        {
                            var preset = originalPreset;
                            if (preset == null) { continue; }
                            if (!string.IsNullOrWhiteSpace(data.Key))
                            {
                                preset.Key = data.Key;
                            }
                            preset.Name = data.Name;
                            preset.VoiceEffect = data.VoiceEffect;
                            if (data.IsSelected) { voicePresetService.SelectedPreset.Value = preset; }
                        }
                        else if (data.Type == PresetType.User)
                        {
                            var engine = originalPreset.Engine;
                            var library = originalPreset.Library;
                            if (engine == null || library == null) { continue; }
                            var preset = new VoicePreset(engine, library)
                            {
                                Type = data.Type,
                                Name = data.Name,
                                VoiceEffect = data.VoiceEffect,
                                SubPresetKey = data.SubPresetKey,
                            };
                            if (!string.IsNullOrWhiteSpace(data.Key))
                            {
                                preset.Key = data.Key;
                            }
                            this.voicePresetService.Add(preset);
                            if (data.IsSelected) { voicePresetService.SelectedPreset.Value = preset; }
                        }
                    }
                }
                catch
                {
                }
                foreach (var preset in this.voicePresetService.AllPresets)
                {
                    if (string.IsNullOrWhiteSpace(preset.SubPresetKey)) { continue; }
                    preset.SubPreset = this.voicePresetService.AllPresets.FirstOrDefault(p => p.Key == preset.SubPresetKey);
                }
            });
        }

        private string CheckFilePath()
        {
            if (string.IsNullOrWhiteSpace(this.settingService.PresetFilePath) || this.settingService.PresetFilePath == "未登録")
            {
                this.settingService.PresetFilePath = Path.Combine(this.PresetDirectory,
                    Path.GetFileNameWithoutExtension(Process.GetCurrentProcess().MainModule.FileName) + ".yvpc");
            }
            string filePath = this.settingService.PresetFilePath;
            if (!File.Exists(filePath))
            {
                try
                {
                    File.Create(filePath).Close();
                }
                catch
                {
                    return null;
                }
                if (!File.Exists(filePath)) { return null; }
            }
            return filePath;
        }


        public void UnZipVEng(string filePath, List<string> configs)
        {
            var directorys = Directory.GetDirectories(EngineDirectory);
            var directory = string.Empty;
            for (int i = 0; i < 10000; i++)
            {
                directory = Path.Combine(EngineDirectory, "Engine_" + i.ToString("000"));
                if (!directorys.Contains(directory)) { break; }
            }
            try
            {
                ZipFile.ExtractToDirectory(filePath, directory, Encoding.GetEncoding("sjis"));
                Util.Utility.SearchFile(directory, "engine.config.json", 6, configs);
            }
            catch (Exception)
            {
            }
        }

        public void UnZipVLib(string filePath, List<string> configs)
        {
            var directorys = Directory.GetDirectories(LibraryDirectory).ToList();
            var directory = string.Empty;
            for (int i = 0; i < 10000; i++)
            {
                directory = Path.Combine(LibraryDirectory, "Library_" + i.ToString("000"));
                if (!directorys.Contains(directory)) { break; }
            }
            try
            {
                ZipFile.ExtractToDirectory(filePath, directory, Encoding.GetEncoding("sjis"));
                directorys.Add(directory);
                Util.Utility.SearchFile(directory, "library.config.json", 6, configs);
            }
            catch (Exception)
            {
            }
        }
    }
}
