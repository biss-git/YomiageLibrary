using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace Yomiage.SDK.Talk
{
    /// <summary>
    /// ポーズ情報
    /// </summary>
    public class Pause
    {
        /// <summary>
        /// ポーズ時間
        /// 単位 [ms]
        /// 基本的には非負整数だけど、とりあえず int で（負の数を入れたい変態がいるかもしれないので）
        /// </summary>
        [JsonIgnore]
        public int Span_ms { get; set; }
        /// <summary>
        /// ポーズの種類
        /// </summary>
        [JsonIgnore]
        public PauseType Type { get; set; } = PauseType.None;

        public string p {
            get
            {
                switch (Type)
                {
                    case PauseType.None: return null;
                    case PauseType.Short: return "S";
                    case PauseType.Long: return "L";
                    case PauseType.Manual: return Span_ms.ToString();
                }
                return null;
            }
            set
            {
                switch (value)
                {
                    case null: Type = PauseType.None; break;
                    case "": Type = PauseType.None; break;
                    case "S": Type = PauseType.Short; break;
                    case "L": Type = PauseType.Long; break;
                    default :
                        Type = PauseType.Manual;
                        if (int.TryParse(value, out int span))
                        {
                            Span_ms = span;
                        }
                        break;
                }
            }
        }
    }

    /// <summary>
    /// ポーズの種類
    /// </summary>
    public enum PauseType
    {
        /// <summary>
        /// ポーズ無し
        /// </summary>
        None,
        /// <summary>
        /// 指定された時間
        /// </summary>
        Manual,
        /// <summary>
        /// 短ポーズ
        /// </summary>
        Short,
        /// <summary>
        /// 長ポーズ
        /// </summary>
        Long,
    }
}
