using System;
using System.Collections.Generic;
using System.Text;
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
        /// GUIからの入力方法を指定する。主に 文字列 用。
        /// 以下のうちどれかにしておく
        /// textbox | combobox | file 
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
}
