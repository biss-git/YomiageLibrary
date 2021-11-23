using Reactive.Bindings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yomiage.GUI.Dialog.ViewModels
{
    class SettingShortcutViewModel : DialogViewModelBase
    {
        public override string Title => "ショートカットキー";

        public ReactiveCollection<Item> Items { get; } = new ReactiveCollection<Item>();

        public SettingShortcutViewModel()
        {
            Items.Add(new Item() { Operation = "テキスト欄の再生", Key = "F5", Target = "テキスト" });
            Items.Add(new Item() { Operation = "テキスト欄の停止", Key = "F6", Target = "テキスト" });

            Items.Add(new Item() { Operation = "前の行の再生", Key = "Ctrl + Left", Target = "テキスト" });
            Items.Add(new Item() { Operation = "後の行の再生", Key = "Ctrl + Right", Target = "テキスト" });

            Items.Add(new Item() { Operation = "音声の保存", Key = "Ctrl + Shift + Alt + S", Target = "テキスト または フレーズ編集" });

            Items.Add(new Item() { Operation = "フレーズの再生と停止", Key = "Space", Target = "フレーズ編集" });

            Items.Add(new Item() { Operation = "フレーズの登録", Key = "Shift + S", Target = "フレーズ編集" });
            Items.Add(new Item() { Operation = "フレーズ編集タブのコピー", Key = "Shift + C", Target = "フレーズ編集" });

            Items.Add(new Item() { Operation = "イントネーション画面の表示", Key = "1", Target = "フレーズ編集" });
            Items.Add(new Item() { Operation = "パラメータ画面２の表示", Key = "2", Target = "フレーズ編集" });
            Items.Add(new Item() { Operation = "パラメータ画面３の表示", Key = "3", Target = "フレーズ編集" });
            Items.Add(new Item() { Operation = "パラメータ画面４の表示", Key = "4", Target = "フレーズ編集" });
            Items.Add(new Item() { Operation = "パラメータ画面５の表示", Key = "5", Target = "フレーズ編集" });
            Items.Add(new Item() { Operation = "パラメータ画面６の表示", Key = "6", Target = "フレーズ編集" });
            Items.Add(new Item() { Operation = "パラメータ画面７の表示", Key = "7", Target = "フレーズ編集" });
            Items.Add(new Item() { Operation = "パラメータ画面８の表示", Key = "8", Target = "フレーズ編集" });
            Items.Add(new Item() { Operation = "パラメータ画面９の表示", Key = "9", Target = "フレーズ編集" });

            Items.Add(new Item() { Operation = "語尾を --- にする", Key = "NumPad0", Target = "フレーズ編集" });
            Items.Add(new Item() { Operation = "語尾を 通常。 にする", Key = "NumPad1", Target = "フレーズ編集" });
            Items.Add(new Item() { Operation = "語尾を 呼び掛け♪ にする", Key = "NumPad2", Target = "フレーズ編集" });
            Items.Add(new Item() { Operation = "語尾を 疑問？ にする", Key = "NumPad3", Target = "フレーズ編集" });
            Items.Add(new Item() { Operation = "語尾を 断定！ にする", Key = "NumPad4", Target = "フレーズ編集" });

            Items.Add(new Item() { Operation = "アクセントマークの上下切り替え", Key = "A", Target = "フレーズ編集（音素）" });
            Items.Add(new Item() { Operation = "無声化の切り替え", Key = "V", Target = "フレーズ編集（音素）" });
            Items.Add(new Item() { Operation = "アクセント句の結合と分割", Key = "S", Target = "フレーズ編集（音素）" });
            Items.Add(new Item() { Operation = "アクセント句の削除", Key = "Shift + D", Target = "フレーズ編集（音素）" });
            Items.Add(new Item() { Operation = "文中ポーズの切り替え", Key = "P", Target = "フレーズ編集（音素）" });
            Items.Add(new Item() { Operation = "任意ポーズの設定", Key = "Shift + P", Target = "フレーズ編集（音素）" });
            Items.Add(new Item() { Operation = "読み編集", Key = "Y", Target = "フレーズ編集（音素）" });
            Items.Add(new Item() { Operation = "長音（ー）を増やす", Key = "M", Target = "フレーズ編集（音素）" });
            Items.Add(new Item() { Operation = "長音（ー）を減らす", Key = "Shift + M", Target = "フレーズ編集（音素）" });
            Items.Add(new Item() { Operation = "促音（ー）を増やす", Key = "T", Target = "フレーズ編集（音素）" });
            Items.Add(new Item() { Operation = "促音（ー）を減らす", Key = "Shift + T", Target = "フレーズ編集（音素）" });
            Items.Add(new Item() { Operation = "母音と長音の切り替え", Key = "B", Target = "フレーズ編集（音素）" });
            Items.Add(new Item() { Operation = "通常の発音と拗音の切り替え", Key = "N", Target = "フレーズ編集（音素）" });

        }

    }

    class Item
    {
        public string Operation { get; init; }
        public string Key { get; init; }
        public string Target { get; init; }
    }
}
