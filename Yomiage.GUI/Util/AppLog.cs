using NLog;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yomiage.GUI.Util
{
    static class AppLog
    {
        public static string LogDirectory { get; }
        private static Logger logger = LogManager.GetCurrentClassLogger();

        static AppLog()
        {
            var config = new NLog.Config.LoggingConfiguration();


            var directory = GetProcessPath();
            LogDirectory = Path.Combine(directory, "log");
            var filePath = Path.Combine(LogDirectory, "log" + DateTime.Now.ToString("yyMMdd_HHmmss") + ".txt");

            var logfile = new NLog.Targets.FileTarget("logfile") { FileName = filePath };
            config.AddRule(LogLevel.Debug, LogLevel.Fatal, logfile);

            LogManager.Configuration = config;
            logger.Info(" Start");
        }

        public static void Info(string text)
        {
            logger.Info(text);
        }

        static string GetProcessPath()
        {
            using var processModule = Process.GetCurrentProcess().MainModule;
            if (string.IsNullOrWhiteSpace(processModule?.FileName)) { return string.Empty; }
            return Path.GetDirectoryName(processModule.FileName);
        }

        public static void Init()
        {

        }

        public static void CurrentDomain_FirstChanceException(
        object sender,
        System.Runtime.ExceptionServices.FirstChanceExceptionEventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(e.Exception?.Message))
            {
                if (e.Exception.Message == "ウィンドウ ハンドルが無効です。") { return; }
                if (e.Exception.Message.Contains("JpnKanaConversion.XmlSerializers.dll")) { return; }
                if (e.Exception.Message.Contains("Could not load file or assembly")) { return; }
                if (e.Exception.Message.Contains("The path is empty. (Parameter 'path')")) { return; }
                if (e.Exception.Message.Contains("The input does not contain any JSON tokens. Expected the input to start with a valid JSON token,")) { return; }
            }
            if (!string.IsNullOrWhiteSpace(e.Exception?.StackTrace) &&
                !e.Exception.StackTrace.Contains("\n"))
            {
                // スタックトレースが１行のみのものは、ほぼシステム的なものなのでログにはださなくていいや。
                return;
            }
            Data.Status.StatusText.Value = "マネージコード内で例外が発生しました。エラー内容についてはログファイルをご確認ください。";
            var logText = "マネージコード内で例外が発生しました。" + Environment.NewLine;
            logText += e.ToString() + Environment.NewLine;
            logText += "e.Exception.TargetSite.Name : " + e.Exception?.TargetSite?.Name + Environment.NewLine;
            logText += "e.Exception.Message : " + e.Exception?.Message + Environment.NewLine;
            logText += "e.Exception.StackTrace : " + e.Exception?.StackTrace + Environment.NewLine;
            logText += "e.Exception.Source : " + e.Exception?.Source + Environment.NewLine;
            logger.Error(logText);
        }

        public static void App_DispatcherUnhandledException(
                object sender,
                System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
        {
            Data.Status.StatusText.Value = "UIスレッドで例外が発生しました。エラー内容についてはログファイルをご確認ください。";
            var logText = "UIスレッドで例外が発生しました。" + Environment.NewLine;
            logText += e.ToString() + Environment.NewLine;
            logText += "e.Exception.TargetSite.Name : " + e.Exception?.TargetSite?.Name + Environment.NewLine;
            logText += "e.Exception.Message : " + e.Exception?.Message + Environment.NewLine;
            logText += "e.Exception.StackTrace : " + e.Exception?.StackTrace + Environment.NewLine;
            logText += "e.Exception.Source : " + e.Exception?.Source + Environment.NewLine;
            logger.Error(logText);

            e.Handled = true;
        }

        public static void TaskScheduler_UnobservedTaskException(
                object sender,
                UnobservedTaskExceptionEventArgs e)
        {
            Data.Status.StatusText.Value = "バックグラウンドで例外が発生しました。エラー内容についてはログファイルをご確認ください。";
            var logText = "バックグラウンドで例外が発生しました。" + Environment.NewLine;
            logText += e.ToString() + Environment.NewLine;
            logText += "e.Exception.TargetSite.Name : " + e.Exception?.TargetSite?.Name + Environment.NewLine;
            logText += "e.Exception.Message : " + e.Exception?.Message + Environment.NewLine;
            logText += "e.Exception.StackTrace : " + e.Exception?.StackTrace + Environment.NewLine;
            logText += "e.Exception.Source : " + e.Exception?.Source + Environment.NewLine;
            logger.Error(logText);

            e.SetObserved();
        }

        public static void CurrentDomain_UnhandledException(
                  object sender,
                  UnhandledExceptionEventArgs e)
        {
            var logText = "トラップできない例外が発生しました。" + Environment.NewLine;
            logText += e.ToString() + Environment.NewLine;
            var exception = e.ExceptionObject as Exception;
            if (exception == null)
            {
                logText += "System.Exceptionとして扱えない例外" + Environment.NewLine;
                logger.Error(logText);
                return;
            }

            logText += "exception.TargetSite.Name : " + exception.TargetSite?.Name + Environment.NewLine;
            logText += "exception.Message : " + exception.Message + Environment.NewLine;
            logText += "exception.StackTrace : " + exception.StackTrace + Environment.NewLine;
            logText += "exception.Source : " + exception.Source + Environment.NewLine;
            logger.Error(logText);


            Environment.Exit(0);
        }

    }
}
