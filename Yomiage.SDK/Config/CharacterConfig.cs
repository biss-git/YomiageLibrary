using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
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

        public void Fix()
        {
            Type = Type.Fix();
            BasicFormat.Fix();
            DarkBackGroundColor = DarkBackGroundColor.Fix();
            LightBackGroundColor = LightBackGroundColor.Fix();
        }
    }


    public class BasicFormat : IFixAble
    {
        /// <summary>
        /// 何もしていないときの立ち絵画像
        /// </summary>
        public string Base { get; set; }
        /// <summary>
        /// 発音しているときの口を空けている立ち絵画像（口パク用）
        /// </summary>
        public string MouthOpen { get; set; }
        /// <summary>
        /// 目を閉じている立ち絵画像（目パチ用）
        /// </summary>
        public string EyeClose { get; set; }
        /// <summary>
        /// 寝ているときの立ち絵１
        /// １と２は１秒くらいずつで交互に表示される。寝息用。
        /// </summary>
        public string Sleep1 { get; set; }
        /// <summary>
        /// 寝ているときの立ち絵２
        /// </summary>
        public string Sleep2 { get; set; }

        public void Fix()
        {
            Base = Base.Fix();
            MouthOpen = MouthOpen.Fix();
            EyeClose = EyeClose.Fix();
            Sleep1 = Sleep1.Fix();
            Sleep2 = Sleep2.Fix();
        }
    }
}
