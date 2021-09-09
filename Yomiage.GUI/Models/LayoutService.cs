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
        public ReactivePropertySlim<bool> PresetVisible { get; } = new();
        /// <summary>
        /// チューニングの表示
        /// </summary>
        public ReactivePropertySlim<bool> TuningVisible { get; } = new();
        /// <summary>
        /// キャラクターの表示
        /// </summary>
        public ReactivePropertySlim<bool> CharacterVisible { get; } = new();
        /// <summary>
        /// キャラクターを大きく表示
        /// </summary>
        public ReactivePropertySlim<bool> IsCharacterMaximized { get; } = new();
        /// <summary>
        /// テキストに行数を表示
        /// </summary>
        public ReactivePropertySlim<bool> IsLineNumberVisible { get; } = new();
        
        /// <summary>
        /// レイアウトの初期化コマンド
        /// </summary>
        public ReactiveCommand InitializeCommand { get; } = new();

        public ReactiveCommand<TabType> ShowTabCommand { get; } = new();

        SettingService settingService;
        public LayoutService(SettingService settingService)
        {
            this.settingService = settingService;
            PresetVisible.Value = settingService.settings.Default.PresetVisible;
            TuningVisible.Value = settingService.settings.Default.TuningVisible;
            CharacterVisible.Value = settingService.settings.Default.CharacterVisible;
            IsCharacterMaximized.Value = settingService.settings.Default.IsCharacterMaximized;
            IsLineNumberVisible.Value = settingService.settings.Default.IsLineNumberVisible;

            PresetVisible.Subscribe(value => { settingService.settings.Default.PresetVisible = value; Save(); });
            TuningVisible.Subscribe(value => { settingService.settings.Default.TuningVisible = value; Save(); });
            CharacterVisible.Subscribe(value => { settingService.settings.Default.CharacterVisible = value; Save(); });
            IsCharacterMaximized.Subscribe(value => { settingService.settings.Default.IsCharacterMaximized = value; Save(); });
            IsLineNumberVisible.Subscribe(value => { settingService.settings.Default.IsLineNumberVisible = value; Save(); });
        }

        private void Save()
        {
            settingService.Save();
        }
    }

    public enum TabType
    {
        UserTab,
    }
}
