// <copyright file="BoolSetting.cs" company="bisu">
// © 2021 bisu
// </copyright>

namespace Yomiage.SDK.Settings
{
    /// <summary>
    /// 真偽値の設定
    /// </summary>
    public class BoolSetting : SettingBase, ISetting<bool>
    {
        /// <inheritdoc/>
        public bool Value { get; set; }

        /// <inheritdoc/>
        public bool DefaultValue { get; set; }

        /// <inheritdoc/>
        public override void ResetValue()
        {
            Value = DefaultValue;
        }

        /// <inheritdoc/>
        public override void Fix()
        {
            // 特にはじくものは無いので、処理なし
        }
    }
}
