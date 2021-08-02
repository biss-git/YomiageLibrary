using Reactive.Bindings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yomiage.GUI.Models
{
    public class LayoutService
    {
        /// <summary>
        /// ボイスプリセットの表示
        /// </summary>
        public ReactivePropertySlim<bool> PresetVisible { get; } = new ReactivePropertySlim<bool>(Settings.Default.PresetVisible);
        /// <summary>
        /// チューニングの表示
        /// </summary>
        public ReactivePropertySlim<bool> TuningVisible { get; } = new ReactivePropertySlim<bool>(Settings.Default.TuningVisible);
        /// <summary>
        /// キャラクターの表示
        /// </summary>
        public ReactivePropertySlim<bool> CharacterVisible { get; } = new ReactivePropertySlim<bool>(Settings.Default.CharacterVisible);
        /// <summary>
        /// キャラクターを大きく表示
        /// </summary>
        public ReactivePropertySlim<bool> IsCharacterMaximized { get; } = new ReactivePropertySlim<bool>(Settings.Default.IsCharacterMaximized);
        /// <summary>
        /// 台本に行数を表示
        /// </summary>
        public ReactivePropertySlim<bool> IsLineNumberVisible { get; } = new ReactivePropertySlim<bool>(Settings.Default.IsLineNumberVisible);
        
        /// <summary>
        /// レイアウトの初期化コマンド
        /// </summary>
        public ReactiveCommand InitializeCommand { get; } = new ReactiveCommand();

        public ReactiveCommand<TabType> ShowTabCommand { get; } = new ReactiveCommand<TabType>();

        public LayoutService()
        {
            PresetVisible.Subscribe(value => { Settings.Default.PresetVisible = value; Save(); });
            TuningVisible.Subscribe(value => { Settings.Default.TuningVisible = value; Save(); });
            CharacterVisible.Subscribe(value => { Settings.Default.CharacterVisible = value; Save(); });
            IsCharacterMaximized.Subscribe(value => { Settings.Default.IsCharacterMaximized = value; Save(); });
            IsLineNumberVisible.Subscribe(value => { Settings.Default.IsLineNumberVisible = value; Save(); });
        }

        private void Save()
        {
            Settings.Default.Save();
        }
    }

    public enum TabType
    {
        UserTab,
    }
}
