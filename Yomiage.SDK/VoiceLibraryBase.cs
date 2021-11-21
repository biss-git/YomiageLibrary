// <copyright file="VoiceLibraryBase.cs" company="bisu">
// © 2021 bisu
// </copyright>

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Yomiage.SDK.Config;
using Yomiage.SDK.Settings;

namespace Yomiage.SDK
{
    /// <summary>
    /// 音声ライブラリのベースクラス。
    /// これを使わずに IVoiceLibrary だけ継承して１から作っても問題ない。
    /// </summary>
    public abstract class VoiceLibraryBase : IVoiceLibrary
    {
        /// <inheritdoc/>
        public virtual int MajorVersion => Version.Major;

        /// <inheritdoc/>
        public virtual int MinorVersion => Version.Minor;

        /// <inheritdoc/>
        public LibraryConfig Config { get; private set; }

        /// <inheritdoc/>
        public LibrarySettings Settings { get; set; }

        /// <inheritdoc/>
        public virtual bool IsActivated { get; set; } = true;

        /// <inheritdoc/>
        public virtual bool IsEnable => IsActivated;

        /// <inheritdoc/>
        public virtual string StateText { get; protected set; } = string.Empty;

        /// <summary>
        /// library.config.json ファイルのパス
        /// </summary>
        protected virtual string ConfigDirectory { get; private set; }

        /// <summary>
        /// 自身(dll) のパス
        /// </summary>
        protected virtual string DllDirectory { get; private set; }

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
        }

        /// <inheritdoc/>
        public virtual object GetValue(string command, string key)
        {
            StateText = "呼び出されました。";

            // 引数は 名前空間 + ファイル名
            var wave = new double[5 * 44100];
            for (int i = 0; i < wave.Length; i++)
            {
                wave[i] = 0.5 * Math.Sin(i * 2 * Math.PI / 441);
            }

            return wave;
        }

        /// <inheritdoc/>
        public virtual bool TryGetValue<T>(string command, string key, out T value)
        {
            if (typeof(T) == typeof(double[]))
            {
                value = (T)GetValue(command, key);
                return true;
            }

            value = default(T);
            return false;
        }

        /// <inheritdoc/>
        public virtual Dictionary<string, object> GetValues(string command, string[] keys)
        {
            var dict = new Dictionary<string, object>();
            keys = keys.Distinct().ToArray();
            foreach (var key in keys)
            {
                dict.Add(key, GetValue(command, key));
            }

            return dict;
        }

        /// <inheritdoc/>
        public virtual bool TryGetValues<T>(string command, string[] keys, out Dictionary<string, T> values)
        {
            values = new Dictionary<string, T>();
            keys = keys.Distinct().ToArray();
            foreach (var key in keys)
            {
                if (TryGetValue(command, key, out T value))
                {
                    values.Add(key, value);
                }
            }

            return values.Count > 0;
        }

        /// <inheritdoc/>
        public virtual void Initialize(string configDirectory, string dllDirectory, LibraryConfig config)
        {
            StateText = "初期化されました。";
            this.Config = config;
            this.ConfigDirectory = configDirectory;
            this.DllDirectory = dllDirectory;
        }
    }
}
