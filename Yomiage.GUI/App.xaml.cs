using ControlzEx.Theming;
using MahApps.Metro.Theming;
using Microsoft.Win32;
using Prism.Ioc;
using Prism.Modularity;
using Reactive.Bindings.Notifiers;
using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Yomiage.Core.Models;
using Yomiage.GUI.Data;
using Yomiage.GUI.Dialog;
using Yomiage.GUI.Dialog.ViewModels;
using Yomiage.GUI.Dialog.Views;
using Yomiage.GUI.Models;
using Yomiage.GUI.Util;
using Yomiage.SDK.Common;

namespace Yomiage.GUI
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App
    {
        private AppConfig appConfig;
        private Splash splash;
        protected override void OnStartup(StartupEventArgs e)
        {
            appConfig = new AppConfig();
            {
                using var processModule = Process.GetCurrentProcess().MainModule;
                var configFile = processModule?.FileName + ".config.json";
                try
                {
                    if (File.Exists(configFile))
                    {
                        var config = JsonUtil.Deserialize<AppConfig>(configFile);
                        if (config != null)
                        {
                            appConfig = config;
                        }
                    }
                }
                catch (Exception)
                {
                    // ここはログに出せない
                }
            }
            this.Properties["AppConfig"] = appConfig;


            if (!appConfig.AllowMultiProcess)
            {
                string myProcessName = Process.GetCurrentProcess().ProcessName;
                if (Process.GetProcessesByName(myProcessName).Length > 1)
                {
                    var result = MessageBox.Show(myProcessName + " はすでに起動しています。\n多重起動での動作は保障されません。それでも起動しますか？", "確認", MessageBoxButton.YesNo);
                    if (result == MessageBoxResult.No)
                    {
                        this.Shutdown();
                    }
                }
            }


            splash = new Splash();
            splash.Show();
            this.Properties["Splash"] = splash;

            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance); // Shift-JIS


            AppLog.Init();

            // マネージコード内で例外がスローされると最初に必ず発生する（.NET 4.0より）
            AppDomain.CurrentDomain.FirstChanceException
              += AppLog.CurrentDomain_FirstChanceException;

            // UIスレッドで実行されているコードで処理されなかったら発生する（.NET 3.0より）
            this.DispatcherUnhandledException
              += AppLog.App_DispatcherUnhandledException;

            TaskScheduler.UnobservedTaskException
              += AppLog.TaskScheduler_UnobservedTaskException;

            // 例外が処理されなかったら発生する（.NET 1.0より）
            AppDomain.CurrentDomain.UnhandledException
              += AppLog.CurrentDomain_UnhandledException;


            // Add custom theme resource dictionaries
            // You should replace SampleApp with your application name
            // and the correct place where your custom theme lives
            var darkAkane = ThemeManager.Current.AddLibraryTheme(
                new LibraryTheme(
                    new Uri("pack://application:,,,/Yomiage.GUI;component/Themes/Dark.Akane.xaml"),
                    MahAppsLibraryThemeProvider.DefaultInstance
                    )
                );

            var dark = ThemeManager.Current.AddLibraryTheme(
                new LibraryTheme(
                    new Uri("pack://application:,,,/Yomiage.GUI;component/Themes/Dark.Aoi.xaml"),
                    MahAppsLibraryThemeProvider.DefaultInstance
                    )
                );

            var light = ThemeManager.Current.AddLibraryTheme(
                new LibraryTheme(
                    new Uri("pack://application:,,,/Yomiage.GUI;component/Themes/Light.Aoi.xaml"),
                    MahAppsLibraryThemeProvider.DefaultInstance
                    )
                );

            var zunda = ThemeManager.Current.AddLibraryTheme(
                new LibraryTheme(
                    new Uri("pack://application:,,,/Yomiage.GUI;component/Themes/Light.Zunda.xaml"),
                    MahAppsLibraryThemeProvider.DefaultInstance
                    )
                );

            var yukari = ThemeManager.Current.AddLibraryTheme(
                new LibraryTheme(
                    new Uri("pack://application:,,,/Yomiage.GUI;component/Themes/Dark.Yukari.xaml"),
                    MahAppsLibraryThemeProvider.DefaultInstance
                    )
                );

            var akari = ThemeManager.Current.AddLibraryTheme(
                new LibraryTheme(
                    new Uri("pack://application:,,,/Yomiage.GUI;component/Themes/Dark.Akari.xaml"),
                    MahAppsLibraryThemeProvider.DefaultInstance
                    )
                );

            var kiritan = ThemeManager.Current.AddLibraryTheme(
                new LibraryTheme(
                    new Uri("pack://application:,,,/Yomiage.GUI;component/Themes/Light.Kiritan.xaml"),
                    MahAppsLibraryThemeProvider.DefaultInstance
                    )
                );

            var maki = ThemeManager.Current.AddLibraryTheme(
                new LibraryTheme(
                    new Uri("pack://application:,,,/Yomiage.GUI;component/Themes/Light.Maki.xaml"),
                    MahAppsLibraryThemeProvider.DefaultInstance
                    )
                );

            var darkMono = ThemeManager.Current.AddLibraryTheme(
                new LibraryTheme(
                    new Uri("pack://application:,,,/Yomiage.GUI;component/Themes/Dark.Monochrome.xaml"),
                    MahAppsLibraryThemeProvider.DefaultInstance
                    )
                );

            var lightMono = ThemeManager.Current.AddLibraryTheme(
                new LibraryTheme(
                    new Uri("pack://application:,,,/Yomiage.GUI;component/Themes/Light.Monochrome.xaml"),
                    MahAppsLibraryThemeProvider.DefaultInstance
                    )
                );

            base.OnStartup(e);
        }

        /// <summary>
        /// アプリ終了時の処理
        /// 一部設定はここで保存するかも
        /// </summary>
        /// <param name="e"></param>
        protected override void OnExit(ExitEventArgs e)
        {
            base.OnExit(e);
        }

        protected override Window CreateShell()
        {
            return Container.Resolve<MainWindow>();
        }

        protected override void RegisterTypes(IContainerRegistry containerRegistry)
        {
            // containerRegistry.RegisterSingleton<IMessageBroker, MessageBroker>();
            //containerRegistry.register
            containerRegistry.RegisterDialogWindow<MetroDialogWindow>();
            containerRegistry.RegisterDialog<PresetFilterDialog, PresetFilterViewModel>();
            containerRegistry.RegisterDialog<PresetSelectDialog, PresetFilterViewModel>();
            containerRegistry.RegisterDialog<PresetFusionDialog, PresetFusionViewModel>();
            containerRegistry.RegisterDialog<PresetNewDialog, PresetNewViewModel>();
            containerRegistry.RegisterDialog<LibraryListDialog, LibraryListViewModel>();
            containerRegistry.RegisterDialog<EngineListDialog, EngineListViewModel>();
            containerRegistry.RegisterDialog<SaveVoiceDialog, SaveVoiceViewModel>();
            containerRegistry.RegisterDialog<HinshiSelectDialog, HinshiSelectViewModel>();
            containerRegistry.RegisterDialog<PauseCharacterDialog, PauseCharacterViewModel>();
            containerRegistry.RegisterDialog<PauseEditDialog, PauseEditViewModel>();
            containerRegistry.RegisterDialog<PhraseListDialog, PhraseListViewModel>();
            containerRegistry.RegisterDialog<WordListDialog, WordListViewModel>();
            containerRegistry.RegisterDialog<VersionInfoDialog, VersionInfoViewModel>();
            containerRegistry.RegisterDialog<SettingProjectDialog, SettingProjectViewModel>();
            containerRegistry.RegisterDialog<SettingEnvironmentDialog, SettingEnvironmentViewModel>();
            containerRegistry.RegisterDialog<SettingShortcutDialog, PresetFilterViewModel>();
            containerRegistry.RegisterDialog<SettingsEngineDialog, SettingsEngineViewModel>();
            containerRegistry.RegisterDialog<SettingsLibraryDialog, SettingsLibraryViewModel>();
            containerRegistry.RegisterDialog<TextDialog, TextViewModel>();
            containerRegistry.RegisterDialog<ApiDialog, ApiViewModel>();

            var settingService = new SettingService();

            containerRegistry.RegisterSingleton<SettingService>(_ => settingService);
            containerRegistry.RegisterSingleton<VoiceEngineService>();
            containerRegistry.RegisterSingleton<VoiceLibraryService>();
            containerRegistry.RegisterSingleton<VoicePresetService>();
            containerRegistry.RegisterSingleton<ConfigService>();
            containerRegistry.RegisterSingleton<IMessageBroker, MessageBroker>();
            containerRegistry.RegisterSingleton<LayoutService>();
            containerRegistry.RegisterSingleton<PauseDictionaryService>();
            containerRegistry.RegisterSingleton<PhraseDictionaryService>();
            containerRegistry.RegisterSingleton<PhraseService>();
            containerRegistry.RegisterSingleton<ScriptService>();
            containerRegistry.RegisterSingleton<VoicePlayerService>();
            containerRegistry.RegisterSingleton<WordDictionaryService>();
            containerRegistry.RegisterSingleton<ApiService>();

            containerRegistry.RegisterSingleton<TextService>();

        }

        protected override void ConfigureModuleCatalog(IModuleCatalog moduleCatalog)
        {
            // moduleCatalog.AddModule<CoreModule>(InitializationMode.WhenAvailable);
        }

    }
}
