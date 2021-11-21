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
        public List<string> Fonts { get; }
        public List<string> OutputDevices { get; }

        public SettingEnvironmentViewModel(SettingService settingService)
        {
            SettingService = settingService;

            ThemeIndex = new ReactivePropertySlim<int>(
                settingService.Theme.Value switch
                {
                    "System" => 0,
                    "Light.Aoi" => 1,
                    "Dark.Aoi" => 2,
                    "Dark.Akane" => 3,
                    "Light.Zunda" => 4,
                    "Dark.Yukari" => 5,
                    "Dark.Akari" => 6,
                    "Light.Kiritan" => 7,
                    "Light.Maki" => 8,
                    "Light.Monochrome" => 9,
                    "Dark.Monochrome" => 10,
                    _ => 0,
                });
            ThemeIndex.Subscribe(i =>
            {
                SettingService.Theme.Value = i switch
                {
                    0 => "System",
                    1 => "Light.Aoi",
                    2 => "Dark.Aoi",
                    3 => "Dark.Akane",
                    4 => "Light.Zunda",
                    5 => "Dark.Yukari",
                    6 => "Dark.Akari",
                    7 => "Light.Kiritan",
                    8 => "Light.Maki",
                    9 => "Light.Monochrome",
                    10 => "Dark.Monochrome",
                    _ => "System",
                };
            }).AddTo(Disposables);

            this.Fonts = new InstalledFontCollection().Families.Select(ff => ff.Name).ToList();

            {
                // 出力デバイスの検索
                List<string> deviceList = new();
                for (int i = 0; i < WaveOut.DeviceCount; i++)
                {
                    var capabilities = WaveOut.GetCapabilities(i);
                    deviceList.Add(capabilities.ProductName);
                }
                this.OutputDevices = deviceList;
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
