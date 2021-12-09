// <copyright file="TalkScript.cs" company="bisu">
// © 2021 bisu
// </copyright>

using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using Yomiage.SDK.Common;
using Yomiage.SDK.Config;
using Yomiage.SDK.VoiceEffects;

namespace Yomiage.SDK.Talk
{
    /// <summary>
    /// 文章の読み上げ指示情報
    /// フレーズ編集情報
    /// </summary>
    public class TalkScript : VoiceEffectValueBase
    {
        /// <inheritdoc/>
        [JsonIgnore]
        public override string Type => "Sentence";

        /// <summary>
        /// ユーザーが打ち込んだ文字列そのもの
        /// </summary>
        public string OriginalText { get; set; }

        /// <summary>
        /// ボイスプリセットタグで指定がある場合に一時的に使用される。
        /// </summary>
        [JsonIgnore]
        public string PresetName { get; set; }

        /// <summary>
        /// IFileConverter でローカル辞書を登録するとき登録先エンジン名を指定するために一時的に使用される。
        /// </summary>
        [JsonIgnore]
        public string EngineName { get; set; }

        /// <summary>
        /// 複数のmoraとポーズからなるアクセント句のリスト。
        /// </summary>
        public List<Section> Sections { get; set; } = new List<Section>();

        /// <summary>
        /// 文末のポーズと音声効果。モーラは無いかわりに、？や！などの記号情報が入る。
        /// </summary>
        [JsonPropertyName("End")]
        public EndSection EndSection { get; set; } = new EndSection();

        /// <summary>
        /// モーラのトータル数
        /// </summary>
        public int MoraCount
        {
            get => this.Sections.Select(s => s.Moras.Count).Sum();
        }

        /// <summary>
        /// GetPhraseJsonText_toSave() で作った保存用テキストから
        /// 読み上げ指示情報　を復元する。
        /// </summary>
        /// <param name="jsonText">jsonテキスト</param>
        /// <returns>復元した情報</returns>
        public static TalkScript GetTalkScript_fromPhraseJsonText(string jsonText)
        {
            if (jsonText.Contains("\""))
            {
                return JsonUtil.DeserializeFromString<TalkScript>(jsonText);
            }

            return JsonUtil.DeserializeFromString<TalkScript>(jsonText.Replace("!", "\""));
        }

        /// <summary>
        /// nullになっている部分を全て埋める
        /// </summary>
        /// <param name="config">エンジンコンフィグ</param>
        /// <param name="shortPause">短ポーズ</param>
        /// <param name="longPause">長ポーズ</param>
        public void Fill(EngineConfig config, int shortPause, int longPause)
        {
            var section = new VoiceEffectValue(config);
            var mora = new VoiceEffectValue(config);
            Sections.ForEach(s => s.Fill(section, mora, shortPause, longPause));
            EndSection.Fill(section, mora, config, shortPause, longPause);
            var curve = new VoiceEffectValue(config);
            EndSection.FillCurve(curve, config);
            for (int i = Sections.Count - 1; i >= 0; i--)
            {
                Sections[i].FillCurve(curve, config);
            }

            {
                var talk = new VoiceEffectValue(config);
                Volume ??= talk.Volume;
                Speed ??= talk.Speed;
                Pitch ??= talk.Pitch;
                Emphasis ??= talk.Emphasis;
                foreach (var s in talk.AdditionalEffect)
                {
                    var val = GetAdditionalValue(s.Key);
                    if (val == null)
                    {
                        SetAdditionalValue(s.Key, s.Value);
                    }
                }
            }
        }

        /// <summary>
        /// 不要なパラメータを削除する。
        /// </summary>
        /// <param name="engineConfig">エンジンコンフィグ</param>
        public new void RemoveUnnecessaryParameters(EngineConfig engineConfig)
        {
            Sections.ForEach(s =>
            {
                s.RemoveUnnecessaryParameters(engineConfig);
                s.Moras.ForEach(m => m.RemoveUnnecessaryParameters(engineConfig));
            });
            EndSection.RemoveUnnecessaryParameters(engineConfig);

            base.RemoveUnnecessaryParameters(engineConfig);
        }

        /// <summary>
        /// プリセット名付きのテキストを返す
        /// </summary>
        /// <param name="promptString">＞</param>
        /// <returns>プリセット名付きのテキスト</returns>
        public string GetOriginalTextWithPresetName(string promptString = "＞")
        {
            if (!string.IsNullOrWhiteSpace(PresetName))
            {
                return PresetName + promptString + OriginalText;
            }

            return OriginalText;
        }

        /// <summary>
        /// Jsonに保存する用のテキストを取得する。
        /// </summary>
        /// <returns>Jsonに保存する用のテキスト</returns>
        public string GetPhraseJsonText_toSave()
        {
            this.OriginalText = null;
            var copy = JsonUtil.DeepClone(this);
            var jsonText = JsonUtil.SerializeToString(copy);
            if (jsonText.Contains("!"))
            {
                return jsonText;
            }

            return jsonText.Replace("\"", "!");
        }

        /// <summary>
        /// 読み（全部カタカナ）を取得する。
        /// </summary>
        /// <param name="withSpace">空白をセクションの間に入れるか</param>
        /// <returns>読み</returns>
        public string GetYomi(bool withSpace = true)
        {
            string yomi = string.Empty;
            this.Sections.ForEach(s =>
            {
                s.Moras.ForEach(m =>
                {
                    yomi += m.Character;
                });
                if (withSpace)
                {
                    yomi += "　";
                }
            });
            yomi += EndSection.EndSymbol;
            return yomi;
        }

        /// <summary>
        /// 全てのカナはカタカナで記述される
        /// アクセント句は/または、で区切る。、で区切った場合に限り無音区間が挿入される。
        /// カナの手前に_を入れるとそのカナは無声化される
        /// アクセント位置を'で指定する。全てのアクセント句にはアクセント位置を1つ指定する必要がある。
        /// 長音は使えない
        /// </summary>
        /// <param name="splitChar">区切り文字 / or 、</param>
        /// <returns>AquesTalkライクなテキスト</returns>
        public string GetYomiForAquesTalkLike(string splitChar = "/")
        {
            var vowel = "ア";
            string yomi = string.Empty;
            this.Sections.ForEach(s =>
            {
                bool accentFlag = true;
                for (int i = 0; i < s.Moras.Count; i++)
                {
                    var m = s.Moras[i];
                    var character = m.Character;
                    if (character == "ー")
                    {
                        character = vowel;
                    }
                    else if(character == "ン")
                    {
                        vowel = "ウ";
                    }
                    else
                    {
                        var v = CharacterUtil.FindBoin(m);
                        if (v != null)
                        {
                            vowel = v;
                        }
                    }

                    yomi += (m.Voiceless == true ? "_" : string.Empty) + character;
                    if (accentFlag &&
                        (m == s.Moras.Last() || (m.Accent && !s.Moras[i + 1].Accent)))
                    {
                        yomi += "'";
                        accentFlag = false;
                    }
                }

                if (s != Sections.Last())
                {
                    yomi += splitChar;
                }
            });
            return yomi;
        }
    }
}
