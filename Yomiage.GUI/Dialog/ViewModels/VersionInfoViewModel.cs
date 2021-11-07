using Prism.Services.Dialogs;
using Reactive.Bindings;
using Reactive.Bindings.Extensions;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Yomiage.Core;
using Yomiage.GUI.ViewModels;

namespace Yomiage.GUI.Dialog.ViewModels
{
    class VersionInfoViewModel : DialogViewModelBase
    {
        public ReactivePropertySlim<Version> Version { get; } = new ReactivePropertySlim<Version>();

        public ReactiveCollection<ComponentItem> ComponentList { get; } = new ReactiveCollection<ComponentItem>();

        public ReactiveCommand AuthorCommand { get; }

        public override string Title => "バージョン情報";

        public VersionInfoViewModel()
        {
            AuthorCommand = new ReactiveCommand().WithSubscribe(AuthorAction).AddTo(Disposables);

            var assembly = Assembly.GetExecutingAssembly();
            var assemblyName = assembly.GetName();
            Version.Value = assemblyName.Version;

            ComponentList.Add(new ComponentItem(Assembly.GetAssembly(typeof(AvalonDock.DockingManager)), "https://github.com/Dirkster99/AvalonDock/blob/master/LICENSE"));
            ComponentList.Add(new ComponentItem(Assembly.GetAssembly(typeof(AvalonDock.Themes.VS2013.Themes.ResourceKeys)), "https://www.nuget.org/packages/Dirkster.AvalonDock.Themes.VS2013/4.60.0/License"));
            ComponentList.Add(new ComponentItem(Assembly.GetAssembly(typeof(MahApps.Metro.Controls.MetroWindow)), "https://github.com/MahApps/MahApps.Metro/blob/develop/LICENSE"));
            ComponentList.Add(new ComponentItem(Assembly.GetAssembly(typeof(MahApps.Metro.IconPacks.BasePackIconExtension)), "https://github.com/MahApps/MahApps.Metro.IconPacks/blob/develop/LICENSE"));
            ComponentList.Add(new ComponentItem(Assembly.GetAssembly(typeof(NAudio.Dsp.FastFourierTransform)), "https://github.com/naudio/NAudio/blob/master/license.txt"));
            ComponentList.Add(new ComponentItem(Assembly.GetAssembly(typeof(NLog.Config.AdvancedAttribute)), "https://github.com/NLog/NLog/blob/dev/LICENSE.txt"));
            ComponentList.Add(new ComponentItem(Assembly.GetAssembly(typeof(Prism.DryIoc.PrismApplication)), "https://github.com/PrismLibrary/Prism/blob/master/LICENSE"));
            ComponentList.Add(new ComponentItem(Assembly.GetAssembly(typeof(Reactive.Bindings.ReactiveProperty)), "https://github.com/runceel/ReactiveProperty/blob/main/LICENSE.txt"));

            ComponentList.Add(new ComponentItem(AssemblyList.GetJPNKanaConv(), ""));
            ComponentList.Add(new ComponentItem(AssemblyList.GetLibNMeCab(), "https://licenses.nuget.org/LGPL-2.1-or-later"));

            ComponentList.Add(new ComponentItem("OpenJtalkのアクセント辞書", null, "http://open-jtalk.sourceforge.net/"));

        }

        private void AuthorAction()
        {
            ProcessStartInfo pi = new()
            {
                FileName = "https://biss-git.github.io/Portfolio/",
                UseShellExecute = true,
            };
            Process.Start(pi);
        }
    }

    class ComponentItem
    {
        public string Name { get; }
        public Version Version { get; }
        public ReactivePropertySlim<bool> CanShow { get; } = new ReactivePropertySlim<bool>();
        public ReactiveCommand ShowCommand { get; }
        public string Link { get; }

        public ComponentItem(Assembly assembly, string link = null)
        {
            this.Name = assembly.GetName().Name;
            this.Version = assembly.GetName().Version;
            this.Link = link;
            CanShow.Value = !string.IsNullOrWhiteSpace(link);
            ShowCommand = CanShow.ToReactiveCommand().WithSubscribe(ShowAction);
        }
        public ComponentItem(string name, Version version = null, string link = null)
        {
            this.Name = name;
            this.Version = version;
            this.Link = link;
            CanShow.Value = !string.IsNullOrWhiteSpace(link);
            ShowCommand = CanShow.ToReactiveCommand().WithSubscribe(ShowAction);
        }

        private void ShowAction()
        {
            ProcessStartInfo pi = new()
            {
                FileName = Link,
                UseShellExecute = true,
            };
            Process.Start(pi);
        }
    }
}
