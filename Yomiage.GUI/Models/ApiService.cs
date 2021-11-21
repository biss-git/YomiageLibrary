using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Yomiage.API;
using Yomiage.Core.Models;
using Yomiage.GUI.Data;

namespace Yomiage.GUI.Models
{
    public class ApiService
    {
        readonly VoicePlayerService voicePlayer;
        readonly TextService textService;
        readonly SettingService settingService;
        readonly PhraseDictionaryService phraseDictionaryService;
        readonly WordDictionaryService wordDictionaryService;
        readonly PauseDictionaryService pauseDictionaryService;
        readonly PhraseService phraseService;
        public ApiService(
            VoicePlayerService voicePlayerService,
            TextService textService,
            SettingService settingService,
            PhraseService phraseService,
            PhraseDictionaryService phraseDictionaryService,
            WordDictionaryService wordDictionaryService,
            PauseDictionaryService pauseDictionaryService
            )
        {
            {
                var obj = Application.Current.Properties["AppConfig"];
                var appConfig = new AppConfig();
                if (obj is AppConfig config)
                {
                    appConfig = config;
                }
                ApiServer.Start(appConfig.PortNumber);
            }
            CommandService.PlayVoice = PlayVoice;
            CommandService.StopAction = StopAction;
            this.voicePlayer = voicePlayerService;
            this.textService = textService;
            this.settingService = settingService;
            this.phraseDictionaryService = phraseDictionaryService;
            this.wordDictionaryService = wordDictionaryService;
            this.pauseDictionaryService = pauseDictionaryService;
            this.phraseService = phraseService;
        }

        private bool PlayVoice(string text)
        {
            if (voicePlayer.IsPlaying.Value) { return false; }
            var scripts = this.textService.Parse(
                text,
                settingService.SplitByEnter,
                settingService.PromptStringEnable,
                settingService.PromptString,
                phraseDictionaryService.SearchDictionary,
                this.wordDictionaryService.WordDictionarys,
                this.pauseDictionaryService.PauseDictionary.ToList());
            Application.Current.Dispatcher.Invoke(() =>
            {
                _ = voicePlayer.Play(scripts, index =>
                  {
                      Application.Current.Dispatcher.Invoke(() =>
                      {
                          if (index < 0 || index >= scripts.Length) { return; }
                          var script = scripts[index];
                          if (!this.voicePlayer.IsPlaying.Value) { return; }
                          this.phraseService.Send(script);
                      });
                  });
            });
            return true;
        }

        private void StopAction()
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                _ = voicePlayer.Stop();
            });
        }
    }
}
