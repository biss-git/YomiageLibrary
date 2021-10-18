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

        VoicePlayerService voicePlayer;
        TextService textService;
        SettingService settingService;
        PhraseDictionaryService phraseDictionaryService;
        WordDictionaryService wordDictionaryService;
        PauseDictionaryService pauseDictionaryService;
        PhraseService phraseService;
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
            ApiServer.Start();
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
            var scripts = this.textService.Parse(text, settingService.SplitByEnter, settingService.PromptStringEnable, settingService.PromptString, phraseDictionaryService.SearchDictionary, this.wordDictionaryService.WordDictionarys, this.pauseDictionaryService.PauseDictionary.ToList());
            Application.Current.Dispatcher.Invoke(() =>
            {
                voicePlayer.Play(scripts, index =>
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
                voicePlayer.Stop();
            });
        }
    }
}
