using ControlzEx.Theming;
using NAudio.Wave;
using Prism.Services.Dialogs;
using Reactive.Bindings;
using Reactive.Bindings.Extensions;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Text;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Yomiage.GUI.Models;
using Yomiage.GUI.ViewModels;

namespace Yomiage.GUI.Dialog.ViewModels
{
    class SettingEnvironmentViewModel : DialogViewModelBase
    {
        public override string Title => "環境設定";

        public ReactivePropertySlim<int> ThemeIndex { get; set; }
        public SettingService SettingService { get; }
        public List<string> fonts { get; }
        public List<string> outputDevices { get; }

        public SettingEnvironmentViewModel(SettingService settingService)
        {
            SettingService = settingService;

            ThemeIndex = new ReactivePropertySlim<int>(
                settingService.Theme.Value switch
                {
                    "System" => 0,
                    "Light.Aoi" => 1,
                    "Dark.Akane" => 2,
                    "Light.Zunda" => 3,
                    "Dark.Yukari" => 4,
                    "Dark.Akari" => 5,
                    "Light.Kiritan" => 6,
                    "Light.Maki" => 7,
                    _ => 0,
                });
            ThemeIndex.Subscribe(i =>
            {
                SettingService.Theme.Value = i switch
                {
                    0 => "System",
                    1 => "Light.Aoi",
                    2 => "Dark.Akane",
                    3 => "Light.Zunda",
                    4 => "Dark.Yukari",
                    5 => "Dark.Akari",
                    6 => "Light.Kiritan",
                    7 => "Light.Maki",
                    _ => "System",
                };
            }).AddTo(Disposables);

            this.fonts = new InstalledFontCollection().Families.Select(ff => ff.Name).ToList();

            {
                // 出力デバイスの検索
                List<string> deviceList = new List<string>();
                for (int i = 0; i < WaveOut.DeviceCount; i++)
                {
                    var capabilities = WaveOut.GetCapabilities(i);
                    deviceList.Add(capabilities.ProductName);
                }
                this.outputDevices = deviceList;
            }
        }

        protected override void OkAction()
        {
            SettingService.SaveEnvironment();
            base.RaiseRequestClose(null);
        }


        /// <summary>
        /// 閉じるときにの処理
        /// プレビューのためにアプリ全体に適用してしまっている設定をもとに戻す。
        /// </summary>
        public override void OnDialogClosed()
        {
            base.OnDialogClosed();
            SettingService.Reload();
        }
    }
}
