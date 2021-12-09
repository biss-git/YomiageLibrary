// <copyright file="IVoiceEngine.cs" company="bisu">
// © 2021 bisu
// </copyright>

using System;
using System.Text;
using System.Threading.Tasks;
using Yomiage.SDK.Config;
using Yomiage.SDK.FileConverter;
using Yomiage.SDK.Settings;
using Yomiage.SDK.Talk;
using Yomiage.SDK.VoiceEffects;

namespace Yomiage.SDK
{
    /// <summary>
    /// 音声合成エンジンのインターフェース
    /// </summary>
    public interface IVoiceEngine : IVoiceBase
    {
        /// <summary>
        /// ファイル開く設定
        /// </summary>
        IFileConverter FileConverter { get; }

        /// <summary>
        /// 音声ライブラリのコンフィグ
        /// GUIからは変えられないシステム的な設定値
        /// </summary>
        EngineConfig Config { get; }

        /// <summary>
        /// エンジンのセッティング
        /// GUIから変更可能な設定値
        /// </summary>
        EngineSettings Settings { get; set; }

        /// <summary>
        /// 初期化処理
        /// </summary>
        /// <param name="configDirectory"> ライブラリconfigのディレクトリ </param>
        /// <param name="dllDirectory"> ライブラリdllのディレクトリ </param>
        /// <param name="config"> エンジンのコンフィグ、GUIからは変えられないシステム的な設定値 </param>
        void Initialize(string configDirectory, string dllDirectory, EngineConfig config);

        /// <summary>
        /// 音声の合成処理
        /// 再生ボタンや保存ボタンを押されたときに呼ばれる。
        /// </summary>
        /// <param name="mainVoice"> 読み上げてほしい子のボイス設定 </param>
        /// <param name="subVoice"> サブの音声情報。EnginConfig で VoiceFusionEnable を true にしたときにのみ呼ばれる。基本的には使わなくていい。 </param>
        /// <param name="talkScript"> 読み上げる内容 </param>
        /// <param name="masterEffect"> マスターコントロールの音声効果 </param>
        /// <param name="setSamplingRate_Hz"> サンプリング周波数を教えてほしい </param>
        /// <param name="submitWavePart">音声波形を分割して渡す</param>
        /// <returns> 音声波形 </returns>
        Task<double[]> Play(VoiceConfig mainVoice, VoiceConfig subVoice, TalkScript talkScript, MasterEffectValue masterEffect, Action<int> setSamplingRate_Hz, Action<double[]> submitWavePart);

        /// <summary>
        /// 音声の保存処理
        /// 基本的には実装しなくてよい。
        /// EngineSaveEnable が true の場合、保存ボタンを押したときに呼ばれる。
        /// UI 側で保存処理は行われないので、この関数内で保存処理を行う。
        /// </summary>
        /// <param name="mainVoice"> 読み上げてほしい子のボイス設定 </param>
        /// <param name="subVoice"> サブの音声情報。EnginConfig で VoiceFusionEnable を true にしたときにのみ呼ばれる。基本的には使わなくていい。 </param>
        /// <param name="talkScripts"> 読み上げる内容 </param>
        /// <param name="masterEffect"> マスターコントロールの音声効果 </param>
        /// <param name="filePath"> 保存先の指定アドレス (拡張子は .wav, .mp3 の可能性がある) </param>
        /// <param name="startPause"> 保存時の前ポーズ [ms] </param>
        /// <param name="endPause"> 保存時の後ポーズ [ms] </param>
        /// <param name="saveWithText"> テキストを一緒に保存するかどうか </param>
        /// <param name="encoding"> テキストのエンコード情報 </param>
        /// <returns> 音声波形 </returns>
        Task Save(VoiceConfig mainVoice, VoiceConfig subVoice, TalkScript[] talkScripts, MasterEffectValue masterEffect, string filePath, int startPause, int endPause, bool saveWithText, Encoding encoding);

        /// <summary>
        /// エンジン固有の読みを取得する。
        /// </summary>
        /// <param name="text">読み上げる文章</param>
        /// <returns>読み方</returns>
        Task<TalkScript> GetDictionary(string text);

        /// <summary>
        /// 音声生成の中断処理
        /// </summary>
        /// <returns> true: 成功、false: 失敗 </returns>
        Task<bool> Stop();
    }
}
