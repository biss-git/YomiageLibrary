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
        /// <summary>
        /// library.config.json ファイルのパス
        /// </summary>
        protected virtual string ConfigDirectory { get; private set; }
        /// <summary>
        /// 自身(dll) のパス
        /// </summary>
        protected virtual string DllDirectory { get; private set; }

        private Version version
        {
            get
            {
                //自分自身のAssemblyを取得
                Assembly asm = Assembly.GetExecutingAssembly();
                return asm.GetName().Version;
            }
        }
        /// <summary>
        /// メジャーバージョン
        /// 過去のバージョンと互換性がなくなるような変更時にインクリメントしてください。
        /// </summary>
        public virtual int MajorVersion => version.Major;
        /// <summary>
        /// マイナーバージョン
        /// 過去のバージョンと互換性が保たれるような修正に対してインクリメントしてください。
        /// </summary>
        public virtual int MinorVersion => version.Minor;


        /// <summary>
        /// 音声ライブラリのコンフィグ
        /// GUIからは変えられないシステム的な設定値
        /// </summary>
        public LibraryConfig Config { get; private set; }
        /// <summary>
        /// 音声ライブラリのセッティング
        /// GUIから変更可能な設定値
        /// </summary>
        public LibrarySettings Settings { get; set; }
        /// <summary>
        /// ライセンスが通っているかどうか
        /// ライセンスとかつけないよって場合は
        /// 常に true を返せばいい
        /// </summary>
        public virtual bool IsActivated { get; set; } = true;
        /// <summary>
        /// 使用可能かどうか
        /// </summary>
        public virtual bool IsEnable => IsActivated;
        /// <summary>
        /// 処理に成功したとか失敗したとか
        /// ユーザーに伝えたい情報があればここで返す。
        /// GUIのどこかに表示される。
        /// </summary>
        public virtual string StateText { get; protected set; } = string.Empty;

        /// <summary>
        /// ライセンスのアクティベーション
        /// ライセンスキーがGUIから送られる
        /// </summary>
        /// <param name="key">ライセンスキー</param>
        /// <returns>true: 成功、 false: 失敗 (失敗した場合はStateTextに理由を入れておいて欲しい)</returns>
        public virtual async Task<bool> Activate(string key)
        {
            StateText = "アクティベートされました。";
            IsActivated = true;
            await Task.Delay(100);
            return true;
        }
        /// <summary>
        /// ライセンスのディスアクティベーション
        /// </summary>
        /// <returns>true: 成功、 false: 失敗 (失敗した場合はStateTextに理由を入れておいて欲しい)</returns>
        public virtual async Task<bool> DeActivate()
        {
            StateText = "ディアクティベートされました。";
            IsActivated = false;
            await Task.Delay(100);
            return true;
        }
        /// <summary>
        /// 破棄
        /// </summary>
        public virtual void Dispose()
        {
        }
        /// <summary>
        /// ライブラリの値を取得する。
        /// どんな値を返すかはエンジンの設計に任せる。
        /// </summary>
        /// <param name="command"> キー１ </param>
        /// <param name="key"> キー２ </param>
        /// <returns> キーに対応する値、型指定とか面倒くさい人向け </returns>
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
        /// <summary>
        /// ライブラリの値を取得する。
        /// どんな値を返すかはエンジンの設計に任せる。
        /// </summary>
        /// <typeparam name="T"> 型指定したい場合はこれを渡す。 </typeparam>
        /// <param name="command"> キー１ </param>
        /// <param name="key"> キー２ </param>
        /// <param name="value"> キーに対応する値 </param>
        /// <returns> 成功したかどうか </returns>
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
        /// <summary>
        /// ライブラリの値を複数同時に取得する。
        /// </summary>
        /// <param name="command"> キー１ </param>
        /// <param name="keys"> キー２ </param>
        /// <returns> キーに対応する値、型指定とか面倒くさい人向け </returns>
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
        /// <summary>
        /// ライブラリの値を複数同時に取得する。
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="command"> キー１ </param>
        /// <param name="keys"> キー２ </param>
        /// <param name="values"> キーに対応する値 </param>
        /// <returns> 成功したかどうか </returns>
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
        /// <summary>
        /// 初期化処理
        /// </summary>
        /// <param name="configDirectory"> ライブラリconfigのディレクトリ </param>
        /// <param name="dllDirectory"> ライブラリdllのディレクトリ </param>
        /// <param name="config"> エンジンのコンフィグ、GUIからは変えられないシステム的な設定値 </param>
        public virtual void Initialize(string configDirectory, string dllDirectory, LibraryConfig config)
        {
            StateText = "初期化されました。";
            this.Config = config;
            this.ConfigDirectory = configDirectory;
            this.DllDirectory = dllDirectory;
        }

    }
}
