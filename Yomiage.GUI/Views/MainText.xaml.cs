using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Yomiage.GUI.Data;
using Yomiage.GUI.ViewModels;
using Reactive.Bindings.Notifiers;
using Yomiage.GUI.EventMessages;
using Yomiage.GUI.Models;
using Reactive.Bindings;
using Yomiage.SDK.Talk;

namespace Yomiage.GUI.Views
{
    /// <summary>
    /// MainText.xaml の相互作用ロジック
    /// </summary>
    public partial class MainText : UserControl
    {
        MainTextViewModel MainTextViewModel;

        public ReactiveCommand AddPresetCommand { get; }
        public ReactiveCommand RemovePresetCommand { get; }
        public ReactiveCommand WordCommand { get; }

        public MainText()
        {
            InitializeComponent();
            var paragraph = new Paragraph();
            paragraph.Inlines.Add(new Run(ConstData.Num1000));
            this.numberDocument.Blocks.Add(paragraph);

            this.AddPresetCommand = new ReactiveCommand().WithSubscribe(AddPresetAction);
            this.RemovePresetCommand = new ReactiveCommand().WithSubscribe(RemovePresetAction);
            this.WordCommand = new ReactiveCommand().WithSubscribe(WordAction);
            add.Command = AddPresetCommand;
            remove.Command = RemovePresetCommand;
            word.Command = WordCommand;
        }

        private void UserControl_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (this.DataContext is MainTextViewModel vm)
            {
                MainTextViewModel = vm;
            }
        }

        private void rich_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (MainTextViewModel != null)
            {
                MainTextViewModel.IsDirty.Value = true;
            }
        }

        private void rich_ScrollChanged(object sender, ScrollChangedEventArgs e)
        {
            if (e.VerticalChange != 0)
            {
                this.scrollViewer.ScrollToVerticalOffset(e.VerticalOffset);
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            var pos = this.rich.CaretPosition;
            this.rich.CaretPosition = pos.DocumentStart;
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            var pos = this.rich.CaretPosition;
            this.rich.CaretPosition = pos.DocumentEnd;
        }

        public string GetContent()
        {
            string text = "";
            var document = this.rich.Document;
            foreach (var block in document.Blocks)
            {
                if (block is Paragraph p)
                {
                    foreach (var l in p.Inlines)
                    {
                        if (l is Run r)
                        {
                            text += r.Text;
                        }
                        if (l != p.Inlines.Last())
                        {
                            text += "\r\r";
                        }
                    }
                }
                if (block != document.Blocks.Last())
                {
                    text += Environment.NewLine;
                }
            }
            return text;
        }

        int changedId = 0;
        private void rich_SelectionChanged(object sender, RoutedEventArgs e)
        {
            var id = (changedId + 1) % 1000000;
            changedId = id;
            Task.Run(async () =>
            {
                await Task.Delay(300);
                if(changedId == id)
                {
                    this.Dispatcher.Invoke(() =>
                    {
                        (int row, int col, int num) = GetPosition();
                        MessageBroker.Default.Publish(new TextCursorPosition() { Row = row, Col = col, Num = num });
                    });
                }
            });
        }

        private (int, int, int) GetPosition()
        {
            var pos = this.rich.CaretPosition;
            var text = GetContent();
            string subText = new TextRange(pos.DocumentStart, pos).Text;
            var lines = subText.Split('\n');
            int row = lines.Length;
            int col = lines.Last().Replace("\r", "").Replace("\n", "").Length;
            int num = text.Replace("\r", "").Replace("\n", "").Length;
            return (row, col, num);
        }

        private void MenuItem_Cut(object sender, RoutedEventArgs e)
        {
            this.rich.Cut();
        }

        private void MenuItem_Copy(object sender, RoutedEventArgs e)
        {
            this.rich.Copy();
        }

        private void MenuItem_Paste(object sender, RoutedEventArgs e)
        {
            this.rich.Paste();
        }

        private void AddPresetAction()
        {
            (int row, int col, int num) = GetPosition();
            if(this.DataContext is MainTextViewModel vm)
            {
                vm.AddPresetAction(row);
            }
        }
        private void RemovePresetAction()
        {
            (int row, int col, int num) = GetPosition();
            if (this.DataContext is MainTextViewModel vm)
            {
                vm.RemovePresetAction(row);
            }
        }
        private void WordAction()
        {
            var word = this.rich.Selection.Text;
            word = word.Replace("\r", "").Replace("\n", "");
            if(word.Length > 30) { word = word.Substring(0, 30); }
            if (string.IsNullOrWhiteSpace(word)) { return; }
            if (this.DataContext is MainTextViewModel vm)
            {
                vm.WordAction(word);
            }
        }

        private void rich_Drop(object sender, DragEventArgs e)
        {
            AddPresetAction();
        }

        private void rich_PreviewDragOver(object sender, DragEventArgs e)
        {
            var data = e.Data;
            var d = data.GetDataPresent("Data");
            //ファイルがドラッグされたとき、カーソルをドラッグ中のアイコンに変更し、そうでない場合は何もしない。
            //e.Effects = (e.Data.GetDataPresent(DataFormats.FileDrop)) ? DragDropEffects.Copy : e.Effects = DragDropEffects.None;
            e.Effects = d ? DragDropEffects.Move : e.Effects = DragDropEffects.None;
            e.Handled = true;
        }

        private void rich_PreviewDrop(object sender, DragEventArgs e)
        {

        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            if(this.DataContext is MainTextViewModel vm)
            {
                vm.GetSelectedText = () => this.rich.Selection.Text;
            }
        }

    }
}
