// <copyright file="VoiceEngineBase.cs" company="bisu">
// © 2021 bisu
// </copyright>

using System;
using System.Reflection;
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
    /// 音声合成エンジンのベースクラス。
    /// これを使わずに IVoiceEngine だけ継承して１から作っても問題ない。
    /// </summary>
    public abstract class VoiceEngineBase : IVoiceEngine
    {
        /// <inheritdoc/>
        public virtual IFileConverter FileConverter { get; }

        /// <inheritdoc/>
        public virtual int MajorVersion => Version.Major;

        /// <inheritdoc/>
        public virtual int MinorVersion => Version.Minor;

        /// <inheritdoc/>
        public EngineConfig Config { get; private set; }

        /// <inheritdoc/>
        public EngineSettings Settings { get; set; }

        /// <inheritdoc/>
        public virtual bool IsActivated { get; protected set; } = true;

        /// <inheritdoc/>
        public virtual bool IsEnable => !IsPlaying && IsActivated;

        /// <inheritdoc/>
        public virtual string StateText { get; protected set; } = string.Empty;

        /// <summary>
        /// Playing という名前だが合成処理中を表すフラグ
        /// </summary>
        protected virtual bool IsPlaying { get; set; } = false;

        /// <summary>
        /// 停止指令がきたときに使うフラグ
        /// </summary>
        protected virtual bool StopFlag { get; set; } = false;

        /// <summary>
        /// engin.config.json ファイルのパス
        /// </summary>
        protected string ConfigDirectory { get; private set; }

        /// <summary>
        /// 自身(dll) のパス
        /// </summary>
        protected string DllDirectory { get; private set; }

        private Version Version
        {
            get
            {
                // 自分自身のAssemblyを取得
                Assembly asm = Assembly.GetExecutingAssembly();
                return asm.GetName().Version;
            }
        }

        /// <inheritdoc/>
        public virtual async Task<bool> Activate(string key)
        {
            StateText = "アクティベートされました。";
            IsActivated = true;
            await Task.Delay(100);
            return true;
        }

        /// <inheritdoc/>
        public virtual async Task<bool> DeActivate()
        {
            StateText = "ディアクティベートされました。";
            IsActivated = false;
            await Task.Delay(100);
            return true;
        }

        /// <inheritdoc/>
        public virtual void Dispose()
        {
            FileConverter?.Dispose();
        }

        /// <inheritdoc/>
        public virtual void Initialize(string configDirectory, string dllDirectory, EngineConfig config)
        {
            StateText = "初期化されました。";
            this.Config = config;
            this.ConfigDirectory = configDirectory;
            this.DllDirectory = dllDirectory;
        }

        /// <inheritdoc/>
        public virtual async Task<double[]> Play(VoiceConfig mainVoice, VoiceConfig subVoice, TalkScript talkScript, MasterEffectValue masterEffect, Action<int> setSamplingRate_Hz)
        {
            StateText = "再生されました。";
            if (IsPlaying)
            {
                return null;
            }

            IsPlaying = true;
            StopFlag = false;

            setSamplingRate_Hz(44100);
            if (!mainVoice.Library.TryGetValue(string.Empty, string.Empty, out double[] wave))
            {
                wave = new double[0];
            }

            await Task.Delay(100);

            IsPlaying = false;
            StopFlag = false;
            return wave;
        }

        /// <inheritdoc/>
        public virtual async Task Save(VoiceConfig mainVoice, VoiceConfig subVoice, TalkScript[] talkScripts, MasterEffectValue masterEffect, string filePath, int startPause, int endPause, bool saveWithText, Encoding encoding)
        {
            StateText = "保存機能は実装されていません。";
            await Task.Delay(100);
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public virtual async Task<bool> Stop()
        {
            if (!IsPlaying)
            {
                return true;
            }

            StopFlag = true;
            for (int i = 0; i < 10; i++)
            {
                await Task.Delay(100);
                if (!StopFlag)
                {
                    break;
                }
            }

            return !StopFlag;
        }
    }
}
