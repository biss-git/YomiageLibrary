// <copyright file="SettingList{T}.cs" company="bisu">
// © 2021 bisu
// </copyright>

using System;
using System.Collections.Generic;
using System.Linq;

namespace Yomiage.SDK.Settings
{
    /// <summary>
    /// 一応Dictionaryっぽくkeyで見つけられるようにしとく
    /// でもkeyの重複とかは別に気にしてない。
    /// </summary>
    /// <typeparam name="T">型</typeparam>
    /// <typeparam name="T2">型2</typeparam>
    public class SettingList<T, T2> : List<T>
        where T : ISetting<T2>
    {
        /// <summary>
        /// キーに対応する設定を返す。
        /// </summary>
        /// <param name="key">キー</param>
        public T this[string key] => this.FirstOrDefault(i => i.Key == key);

        /// <summary>
        /// 全てのキーを返す。
        /// </summary>
        /// <returns>keys</returns>
        public IEnumerable<string> Keys()
        {
            return this.Select(i => i.Key).Distinct();
        }

        /// <summary>
        /// キーが登録されているか
        /// </summary>
        /// <param name="key">キー</param>
        /// <returns> true:登録されている、false:登録されていない</returns>
        public bool ContainsKey(string key)
        {
            return Keys().Contains(key);
        }

        /// <summary>
        /// 設定を取得する
        /// </summary>
        /// <param name="key">キー</param>
        /// <param name="setting">設定</param>
        /// <returns>成功したかどうか</returns>
        public bool TryGetSetting(string key, out T setting)
        {
            setting = default;
            if (!ContainsKey(key))
            {
                return false;
            }

            setting = this[key];

            return setting != null;
        }

        /// <summary>
        /// 設定値を取得する
        /// </summary>
        /// <param name="key">キー</param>
        /// <param name="defaultValue">見つからなかった場合の値</param>
        /// <returns>設定値</returns>
        public T2 GetSettingValue(string key, T2 defaultValue)
        {
            if (TryGetSetting(key, out var setting))
            {
                return setting.Value;
            }

            return defaultValue;
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
