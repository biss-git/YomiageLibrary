using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Yomiage.SDK.Settings
{
    /// <summary>
    /// 真偽値の設定
    /// </summary>
    public class BoolSetting : SettingBase
    {
        /// <summary>
        /// 設定値
        /// </summary>
        public bool Value { get; set; }
        /// <summary>
        /// 初期値
        /// </summary>
        public bool DefaultValue { get; set; }

        /// <summary>
        /// 初期値を設定値に代入
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
            // 特にはじくものは無いので、処理なし
        }
    }
}
