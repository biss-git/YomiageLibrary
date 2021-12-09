// <copyright file="ISetting.cs" company="bisu">
// © 2021 bisu
// </copyright>

using Yomiage.SDK.Common;

namespace Yomiage.SDK.Settings
{
    /// <summary>
    /// 設定値のインターフェス
    /// </summary>
    public interface ISetting : IFixAble
    {
        /// <summary>
        /// 設定値のキー。重複しないようにしておいたほうがいい。
        /// </summary>
        string Key { get; set; }

        /// <summary>
        /// GUI上に表示される名前。
        /// </summary>
        string Name { get; set; }

        /// <summary>
        /// GUI上に表示される説明文。
        /// </summary>
        string Description { get; set; }

        /// <summary>
        /// GUI上に表示される順番。
        /// </summary>
        int Order { get; set; }

        /// <summary>
        /// GUIからの入力方法を指定する。
        /// 以下のうちどれかにしておく
        /// combobox | textbox | file
        /// 上記に該当しない、または有効な値でない場合はデフォルトになる
        /// </summary>
        string Type { get; set; }

        /// <summary>
        /// GUI上に表示させたくない場合は true にする。
        /// ユーザーに操作させたくない場合や、
        /// テスト機能のために内部的に設定値を用意したい場合などに利用してください。
        /// </summary>
        bool Hide { get; set; }

        /// <summary>
        /// 初期値を設定値に代入
        /// </summary>
        void ResetValue();
    }

    /// <summary>
    /// 型付きの設定値
    /// </summary>
    /// <typeparam name="T">型</typeparam>
    public interface ISetting<T> : ISetting
    {
        /// <summary>
        /// 設定値
        /// </summary>
        public T Value { get; set; }

        /// <summary>
        /// 初期値
        /// </summary>
        public T DefaultValue { get; set; }
    }
}
