// <copyright file="CharacterConfig.cs" company="bisu">
// © 2021 bisu
// </copyright>

using Yomiage.SDK.Common;
using Yomiage.SDK.Settings;

namespace Yomiage.SDK.Config
{
    /// <summary>
    /// キャラクターの描画に関する設定
    /// character.config.json として音声ライブラリと一緒に置いておく
    /// </summary>
    public class CharacterConfig : IFixAble
    {
        /// <summary>
        /// どうやってキャラクターを描画するか
        /// 今のところ BasicFormat のみ
        /// </summary>
        public string Type { get; set; }

        /// <summary>
        /// １～５枚の画像ファイルを使う標準的な方法
        /// </summary>
        public BasicFormat BasicFormat { get; set; } = new BasicFormat();

        /// <summary>
        /// アプリが黒ベースのときの背景色
        /// 色はHTML形式で指定
        /// つまり、#AARRGGBB とか #RRGGBB の形式
        /// 例　#FF0000 で赤
        /// </summary>
        public string DarkBackGroundColor { get; set; }

        /// <summary>
        /// アプリが白ベースのときの背景色
        /// </summary>
        public string LightBackGroundColor { get; set; }

        /// <inheritdoc/>
        public void Fix()
        {
            Type = Type.Fix();
            BasicFormat.Fix();
            DarkBackGroundColor = DarkBackGroundColor.Fix();
            LightBackGroundColor = LightBackGroundColor.Fix();
        }
    }
}
