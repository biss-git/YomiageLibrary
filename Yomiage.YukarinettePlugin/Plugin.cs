using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace Yomiage.YukarinettePlugin
{
    public class Plugin : Yukarinette.IYukarinetteInterface
    {
        private readonly SettingsValues settings = new SettingsValues();
        HttpClient client;

        string settingsFilePath;

        public override string Name
        {
            get
            {
                return "ユニコエ";
            }
        }

        public override string GUID => "00a97f86-55e6-4977-9805-98754387648aa";

        /// <summary>
        /// ゆかりネット起動直後にプラグインが読み込まれたタイミングで呼ばれる
        /// </summary>
        public override void Loaded()
        {
            var assembly = Assembly.GetAssembly(typeof(Plugin));
            var documentDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
            var settingsDirectory = Path.Combine(documentDirectory, "ゆかりねっと");
            if (!Directory.Exists(settingsDirectory))
            {
                Directory.CreateDirectory(settingsDirectory);
            }
            settingsFilePath = Path.Combine(settingsDirectory, Path.GetFileName(assembly.Location) + ".settings");
            if (File.Exists(settingsFilePath))
            {
                var texts = File.ReadAllLines(settingsFilePath);
                if (texts.Length >= 1 && int.TryParse(texts[0], out int portNo))
                {
                    settings.PortNo = portNo;
                }
            }

            try
            {
                this.IconIMage = new BitmapImage(new Uri(@"pack://application:,,,/Yomiage.YukarinettePlugin;component/icon.png"));
            }
            catch (Exception)
            {

            }
        }

        /// <summary>
        /// ゆかりネットの終了時に呼ばれる
        /// </summary>
        public override void Closed()
        {
            var text = "";
            text += settings.PortNo.ToString() + Environment.NewLine;
            try
            {
                File.WriteAllText(settingsFilePath, text);
            }
            catch (Exception)
            {

            }
        }

        /// <summary>
        /// 音声認識タブでプラグインを選択した状態で、開始ボタンを押したときに呼ばれる。
        /// </summary>
        public override void SpeechRecognitionStart()
        {
            client = new HttpClient();
        }

        /// <summary>
        /// 音声認識の停止時に呼ばれる
        /// </summary>
        public override void SpeechRecognitionStop()
        {
            client?.Dispose();
        }

        /// <summary>
        /// 音声認識されたテキストが送られてくる。
        /// </summary>
        /// <param name="text"></param>
        public override void Speech(string text)
        {
            var jsonText = "{\"command\": \"play\", \"TalkText\": \"" + text + "\"}";
            var content = new StringContent(jsonText, Encoding.UTF8);
            client?.PostAsync($"http://localhost:{settings.PortNo}/api/command", content);
        }

        /// <summary>
        /// プラグインタブの設定ボタンを押されたときに呼ばれる
        /// </summary>
        public override void Setting()
        {
            var settingsWindow = new SettingsWindow
            {
                SelectedPortNo = settings.PortNo
            };
            settingsWindow.ShowDialog();
            if (settingsWindow.ResultIsOk)
            {
                settings.PortNo = settingsWindow.SelectedPortNo;
            }
        }

    }

}
