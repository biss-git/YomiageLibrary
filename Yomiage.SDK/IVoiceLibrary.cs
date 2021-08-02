using System;
using System.Collections.Generic;

using Yomiage.SDK.Config;
using Yomiage.SDK.Settings;

namespace Yomiage.SDK
{
    /// <summary>
    /// 音声ライブラリのインターフェース
    /// </summary>
    public interface IVoiceLibrary : IVoiceBase
    {
        /// <summary>
        /// 音声ライブラリのコンフィグ
        /// GUIからは変えられないシステム的な設定値
        /// </summary>
        LibraryConfig Config { get;}
        /// <summary>
        /// 音声ライブラリのセッティング
        /// GUIから変更可能な設定値
        /// </summary>
        LibrarySettings Settings { get; set; }
        /// <summary>
        /// 初期化処理
        /// </summary>
        /// <param name="configDirectory"> ライブラリconfigのディレクトリ </param>
        /// <param name="dllDirectory"> ライブラリdllのディレクトリ </param>
        /// <param name="config"> エンジンのコンフィグ、GUIからは変えられないシステム的な設定値 </param>
        void Initialize(string configDirectory, string dllDirectory, LibraryConfig config);
        /// <summary>
        /// ライブラリの値を取得する。
        /// どんな値を返すかはエンジンの設計に任せる。
        /// </summary>
        /// <param name="command"> キー１ </param>
        /// <param name="key"> キー２ </param>
        /// <returns> キーに対応する値、型指定とか面倒くさい人向け </returns>
        object GetValue(string command, string key);
        /// <summary>
        /// ライブラリの値を取得する。
        /// どんな値を返すかはエンジンの設計に任せる。
        /// </summary>
        /// <typeparam name="T"> 型指定したい場合はこれを渡す。 </typeparam>
        /// <param name="command"> キー１ </param>
        /// <param name="key"> キー２ </param>
        /// <param name="value"> キーに対応する値 </param>
        /// <returns> 成功したかどうか </returns>
        bool TryGetValue<T>(string command, string key, out T value);
        /// <summary>
        /// ライブラリの値を複数同時に取得する。
        /// </summary>
        /// <param name="command"> キー１ </param>
        /// <param name="keys"> キー２ </param>
        /// <returns> キーに対応する値、型指定とか面倒くさい人向け </returns>
        Dictionary<string, object> GetValues(string command, string[] keys);
        /// <summary>
        /// ライブラリの値を複数同時に取得する。
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="command"> キー１ </param>
        /// <param name="keys"> キー２ </param>
        /// <param name="values"> キーに対応する値 </param>
        /// <returns> 成功したかどうか </returns>
        bool TryGetValues<T>(string command, string[] keys, out Dictionary<string, T> values);
    }
}
