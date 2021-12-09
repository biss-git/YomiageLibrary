// <copyright file="ConfigBase.cs" company="bisu">
// © 2021 bisu
// </copyright>

using System.Linq;
using Yomiage.SDK.Settings;
using Yomiage.SDK.VoiceEffects;

namespace Yomiage.SDK.Config
{
    /// <summary>
    /// 設定内容
    /// json に保存・読み込みされるので、
    /// エンジン開発者や音声ライブラリ開発者が用途に合わせて編集する
    /// </summary>
    public abstract class ConfigBase : Common.IFixAble
    {
        /// <summary>
        /// 固有のキー。
        /// これは他の開発者とも重複しないようになるべくユニークな名前にすること。
        /// </summary>
        public string Key { get; set; } = string.Empty;

        /// <summary>
        /// 表示上の名前。
        /// 被っても大丈夫だけど、わかりずらいので、なるべくユニークな名前にすること。
        /// </summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// 簡単な説明文。
        /// </summary>
        public string Description { get; set; } = string.Empty;

        /// <summary>
        /// 音声ライブラリのフォーマット。
        /// このフォーマット名が一致する音声ライブラリとエンジンがセットで利用される。
        /// 他の開発者と被らない名前にしておく。
        /// または他の開発者のフォーマットに倣う場合は同じにしておく。
        /// </summary>
        public string[] LibraryFormat { get; set; } = { };

        /// <summary>
        /// 対応OS
        /// まあ今のところ Windows10 しか対応できないですが。
        /// </summary>
        public string OS { get; set; } = string.Empty;

        /// <summary>
        /// 音声ライブラリまたは音声合成エンジンのdllの.config.jsonからの相対パス
        /// </summary>
        public string AssemblyName { get; set; } = string.Empty;

        /// <summary>
        /// 音声ライブラリまたは音声合成エンジンのモジュール名
        /// </summary>
        public string ModuleName { get; set; } = string.Empty;

        /// <summary>
        /// 音声ライブラリまたは音声合成エンジンのクラス名
        /// </summary>
        public string TypeName { get; set; } = string.Empty;

        /// <summary>
        /// 対応言語
        /// まあ今のところ Japanese しか対応できないですが。
        /// 変数名は Culture のほうが自然かな？とも思ったけど
        /// 別に時刻とかまで変えるわけじゃないから Language でいいや。
        /// </summary>
        public string Language { get; set; } = string.Empty;

        /// <summary>
        /// 音量の設定
        /// </summary>
        public EffectSetting VolumeSetting { get; set; } = new EffectSetting();

        /// <summary>
        /// 話速の設定
        /// </summary>
        public EffectSetting SpeedSetting { get; set; } = new EffectSetting();

        /// <summary>
        /// 高さの設定
        /// </summary>
        public EffectSetting PitchSetting { get; set; } = new EffectSetting();

        /// <summary>
        /// 抑揚の設定
        /// </summary>
        public EffectSetting EmphasisSetting { get; set; } = new EffectSetting();

        /// <summary>
        /// 追加の設定
        /// </summary>
        public SettingList<EffectSetting, double> AdditionalSettings { get; set; }

        /// <summary>
        /// 不正な値を取り除く
        /// </summary>
        public virtual void Fix()
        {
            Key = Key?.Fix();
            Name = Name?.Fix();
            Description = Description?.Fix();
            LibraryFormat = (LibraryFormat == null) ? new string[0] :
                LibraryFormat
                .Select(f => f.Fix())
                .Where(f => !string.IsNullOrWhiteSpace(f))
                .ToArray();
            OS = OS?.Fix();
            Language = Language?.Fix();
            VolumeSetting?.Fix();
            SpeedSetting?.Fix();
            PitchSetting?.Fix();
            EmphasisSetting?.Fix();
            AdditionalSettings?.Fix();
        }
    }
}
