using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Yomiage.SDK
{
    /// <summary>
    /// IVoiceEngine と IVoiceLibrary の共通項目
    /// </summary>
    public interface IVoiceBase : IDisposable
    {
        /// <summary>
        /// ライセンスが通っているかどうか
        /// ライセンスとかつけないよって場合は
        /// 常に true を返せばいい
        /// </summary>
        bool IsActivated { get; }
        /// <summary>
        /// ライセンスのアクティベーション
        /// ライセンスキーがGUIから送られる
        /// </summary>
        /// <param name="key">ライセンスキー</param>
        /// <returns>true: 成功、 false: 失敗 (失敗した場合はStateTextに理由を入れておいて欲しい)</returns>
        Task<bool> Activate(string key);
        /// <summary>
        /// ライセンスのディスアクティベーション
        /// </summary>
        /// <returns>true: 成功、 false: 失敗 (失敗した場合はStateTextに理由を入れておいて欲しい)</returns>
        Task<bool> DeActivate();
        /// <summary>
        /// 使用可能かどうか
        /// </summary>
        bool IsEnable { get; }
        /// <summary>
        /// 処理に成功したとか失敗したとか
        /// ユーザーに伝えたい情報があればここで返す。
        /// GUIのどこかに表示される。
        /// </summary>
        string StateText { get; }
        /// <summary>
        /// メジャーバージョン
        /// 過去のバージョンと互換性がなくなるような変更時にインクリメントしてください。
        /// </summary>
        int MajorVersion { get; }
        /// <summary>
        /// マイナーバージョン
        /// 過去のバージョンと互換性が保たれるような修正に対してインクリメントしてください。
        /// </summary>
        int MinorVersion { get; }
    }
}
