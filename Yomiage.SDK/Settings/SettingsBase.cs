using System;
using System.Collections.Generic;
using System.Linq;

namespace Yomiage.SDK.Settings
{
    /// <summary>
    /// プラグイン（音声合成エンジン、音声ライブラリのこと）の設定内容。
    /// つまり、プラグインの開発者が決めた設定項目であり、
    /// Hide を False にしておけばGUIからも変更可能で、ユーザーに設定内容を任せることもできる。
    /// </summary>
    public abstract class SettingsBase : Common.IFixAble
    {
        /// <summary>
        /// 真偽値の設定値
        /// </summary>
        public SettingList<BoolSetting> Bools { get; set; } = new SettingList<BoolSetting>();
        /// <summary>
        /// 整数値の設定値
        /// </summary>
        public SettingList<IntSetting> Ints { get; set; } = new SettingList<IntSetting>();
        /// <summary>
        /// 実数値の設定値
        /// </summary>
        public SettingList<DoubleSetting> Doubles { get; set; } = new SettingList<DoubleSetting>();
        /// <summary>
        /// 文字列の設定値
        /// </summary>
        public SettingList<StringSetting> Strings { get; set; } = new SettingList<StringSetting>();

        /// <summary>
        /// 初期値を設定値に代入
        /// </summary>
        public void ResetAllValues()
        {
            Bools.ResetValues();
            Ints.ResetValues();
            Doubles.ResetValues();
            Strings.ResetValues();
        }
        /// <summary>
        /// 不正な値をはじく
        /// </summary>
        public void Fix()
        {
            Bools.Fix();
            Ints.Fix();
            Doubles.Fix();
            Strings.Fix();
        }
    }

    /// <summary>
    /// 一応Dictionaryっぽくkeyで見つけられるようにしとく
    /// でもkeyの重複とかは別に気にしてない。
    /// </summary>
    public class SettingList<T> : List<T> where T : ISetting
    {
        /// <summary>
        /// キーに対応する設定を返す。
        /// </summary>
        public T this[string key]
        {
            get
            {
                return this.FirstOrDefault(i => i.Key == key);
            }
        }
        /// <summary>
        /// 全てのキーを返す。
        /// </summary>
        public IEnumerable<string> Keys()
        {
            return this.Select(i => i.Key).Distinct();
        }
        /// <summary>
        /// キーが登録されているか
        /// </summary>
        /// <returns> true:登録されている、false:登録されていない</returns>
        public bool ContainsKey(string key)
        {
            return Keys().Contains(key);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <param name="setting"></param>
        /// <returns></returns>
        public bool TryGetSetting(string key, out T setting)
        {
            setting = default;
            if (!ContainsKey(key)) { return false; }
            setting = this[key];
            if (setting == null) { return false; }
            return true;
        }
        /// <summary>
        /// 初期値を設定値に代入
        /// </summary>
        public void ResetValues()
        {
            this.ForEach(i => i.ResetValue());
        }
        /// <summary>
        /// null や 空白 をはじく
        /// </summary>
        public void Fix()
        {
            this.ForEach(i => i.Fix());
        }
    }

}
