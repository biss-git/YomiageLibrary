// <copyright file="SettingBase.cs" company="bisu">
// © 2021 bisu
// </copyright>

using Yomiage.SDK.Common;

namespace Yomiage.SDK.Settings
{
    /// <summary>
    /// 設定の共通項目
    /// </summary>
    public abstract class SettingBase : ISetting
    {
        /// <summary>
        /// 設定値のキー。重複しないようにしておいたほうがいい。
        /// </summary>
        public string Key { get; set; } = string.Empty;

        /// <summary>
        /// GUI上に表示される名前。
        /// </summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// GUI上に表示される説明文。
        /// </summary>
        public string Description { get; set; } = string.Empty;

        /// <summary>
        /// GUI上に表示される順番。
        /// </summary>
        public int Order { get; set; }

        /// <summary>
        /// GUIからの入力方法を指定する。主に 文字列用　または　フレーズ編集用。
        /// 以下のうちどれかにしておく
        /// textbox | combobox | file
        /// Sentence | Section | Mora | Curve
        /// 上記に該当しない、または有効な値でない場合はデフォルトになる
        /// </summary>
        public string Type { get; set; } = string.Empty;

        /// <summary>
        /// GUI上に表示させたくない場合は true にする。
        /// ユーザーに操作させたくない場合や、
        /// テスト機能のために内部的に設定値を用意したい場合などに利用してください。
        /// </summary>
        public bool Hide { get; set; }

        /// <summary>
        /// 初期値を設定値に代入
        /// </summary>
        public abstract void ResetValue();

        /// <summary>
        /// 不正な値をはじく
        /// </summary>
        public abstract void Fix();
    }
}
