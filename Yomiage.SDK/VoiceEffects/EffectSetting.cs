// <copyright file="EffectSetting.cs" company="bisu">
// © 2021 bisu
// </copyright>

using System;
using System.Collections.Generic;
using Yomiage.SDK.Settings;

namespace Yomiage.SDK.VoiceEffects
{
    /// <summary>
    /// 音声効果用のクラス。
    /// DoubleSetting とほぼ同じだが、
    /// 単位を設定できる、入力範囲の拡張ができる、色を設定できる、といった違いがある。
    /// </summary>
    public class EffectSetting : SettingBase, Common.IFixAble
    {
        /// <summary>
        /// 表示単位 特になければ空文字でいい
        /// </summary>
        public string Unit { get; set; } = string.Empty;

        /// <summary>
        /// 設定値
        /// </summary>
        public double Value { get; set; }

        /// <summary>
        /// 初期値
        /// </summary>
        public double DefaultValue { get; set; }

        /// <summary>
        /// 設定可能な最小値
        /// </summary>
        public double Min { get; set; }

        /// <summary>
        /// 設定可能な最小値（入力範囲が拡張されたとき）
        /// </summary>
        public double MinExtend { get; set; }

        /// <summary>
        /// 設定可能な最大値
        /// </summary>
        public double Max { get; set; }

        /// <summary>
        /// 設定可能な最大値（入力範囲が拡張されたとき）
        /// </summary>
        public double MaxExtend { get; set; }

        /// <summary>
        /// GUIでの最小ステップ。マウスホイールとかで一度に動く値。
        /// ただし、入力の最小値ではない。
        /// 例えば SmallStep = 2 としても、偶数のみ、奇数のみの入力になるわけではない。
        /// </summary>
        public double SmallStep { get; set; }

        /// <summary>
        /// 0.00 とすると小数点２桁まで表示
        /// </summary>
        public string StringFormat { get; set; }

        /// <summary>
        /// 範囲毎に表示を変えたい場合に使う。
        /// </summary>
        public Dictionary<double, string> StringRule { get; set; }

        /// <summary>
        /// 色はHTML形式で指定
        /// つまり、#AARRGGBB とか #RRGGBB の形式
        /// 例　#FF0000 で赤
        /// </summary>
        public string Color { get; set; }

        /// <summary>
        /// フレーズ編集に利用されるアイコン名
        /// MahApps.Metro.IconPacks の BoxIcons の Kind に適用される。
        /// どんなアイコンがあるのかは公式のデモアプリをご確認ください。
        /// https://github.com/MahApps/MahApps.Metro.IconPacks
        /// </summary>
        public string IconKind { get; set; }

        /// <summary>
        /// 初期値に戻す
        /// </summary>
        public override void ResetValue()
        {
            Value = DefaultValue;
        }

        /// <summary>
        /// 不正な値をはじく
        /// </summary>
        public override void Fix()
        {
            Value = Value.Fix();
            DefaultValue = DefaultValue.Fix();
            Min = Min.Fix();
            MinExtend = MinExtend.Fix();
            Max = Max.Fix();
            MaxExtend = MaxExtend.Fix();
            SmallStep = SmallStep.Fix();
            StringFormat = StringFormat.Fix();
            if (Min > Max)
            {
                var temp = Min;
                Min = Max;
                Max = temp;
            }

            MinExtend = Math.Min(Min, MinExtend);
            MaxExtend = Math.Max(Max, MaxExtend);
            Value = DefaultValue.Clamp(MinExtend, MaxExtend);
            DefaultValue = DefaultValue.Clamp(Min, Max);
            if (SmallStep <= 0)
            {
                SmallStep = (Max - Min) / 100;
            }
        }

        /// <summary>
        /// Mora か Section かが Type と一致しているかを確認します。
        /// 一致していなければ辞書に保存するときには消して大丈夫
        /// </summary>
        /// <param name="isMora">isMora</param>
        /// <param name="isEndSection">isEndSection</param>
        /// <returns>
        /// true: isMora が Type と一致している
        /// false: isMora が Type と一致していない
        /// </returns>
        public bool CheckIsMora(bool isMora, bool isEndSection)
        {
            return isEndSection || (Type == "Mora" && isMora) || (Type != "Mora" && !isMora);
        }
    }
}
