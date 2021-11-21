// <copyright file="EngineConfig.cs" company="bisu">
// © 2021 bisu
// </copyright>

using Yomiage.SDK.Settings;

namespace Yomiage.SDK.Config
{
    /// <summary>
    /// エンジンの設定内容
    /// engine.config.json としてエンジンと一緒に置いておく
    /// </summary>
    public class EngineConfig : ConfigBase
    {
        /// <summary>
        /// アクセント句のアクセントを編集可能にする。
        /// AccentEnable が false のとき、フレーズの分割や結合、発音の変更はできても、アクセントの丸い点を上下にはできない。
        /// AccentEnable が true  のとき、アクセントの丸い点を上下にできる。
        /// </summary>
        public bool AccentEnable { get; set; }

        /// <summary>
        /// アクセント句を非表示にする。
        /// 調声ができないタイプのエンジンの場合に true にする。
        /// </summary>
        public bool AccentHide { get; set; }

        /// <summary>
        /// アクセントの入力規則。
        /// 今は特に使わない。デフォルトではA.I.VOICEと同じアクセントを入力できるようにする。（アクセントは同じアクセント句の中では一度しか下がらず、下がったあとは上がらない。）
        /// </summary>
        public string AccentRule { get; set; }

        /// <summary>
        /// サブプリセット（ボイスフュージョン）機能を有効にする。
        /// </summary>
        public bool SubPresetEnable { get; set; }

        /// <summary>
        /// エンジンが保存処理まで行いたい場合は true にする。
        /// その場合、保存時には Save が呼ばれる。
        /// </summary>
        public bool EngineSaveEnable { get; set; }

        /// <summary>
        /// 短ポーズの設定
        /// </summary>
        public IntSetting ShortPauseSetting { get; set; } = new IntSetting();

        /// <summary>
        /// 長ポーズの設定
        /// </summary>
        public IntSetting LongPauseSetting { get; set; } = new IntSetting();

        /// <inheritdoc/>
        public override void Fix()
        {
            base.Fix();
            AccentRule = AccentRule.Fix();
            ShortPauseSetting.Fix();
            LongPauseSetting.Fix();
        }
    }
}
