using Reactive.Bindings;
using Reactive.Bindings.Notifiers;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using Yomiage.GUI.Data;
using Yomiage.GUI.EventMessages;
using Yomiage.GUI.ViewModels;

namespace Yomiage.GUI.Views
{
    /// <summary>
    /// MainText.xaml の相互作用ロジック
    /// </summary>
    public partial class MainText : UserControl
    {
        private double richScrollOffset;
        private bool isMaking = false;

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
            if (MainTextViewModel != null && e.Changes.Count > 0)
            {
                MainTextViewModel.IsDirty.Value = true;
            }
        }

        private void rich_ScrollChanged(object sender, ScrollChangedEventArgs e)
        {
            if (e.VerticalChange != 0)
            {
                this.scrollViewer.ScrollToVerticalOffset(e.VerticalOffset);
                this.richScrollOffset = e.VerticalOffset;
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
                if (changedId == id)
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
        private (int, int) GetLineRange()
        {
            var row1 = 0;
            var row2 = 0;
            {
                var pos = this.rich.Selection.Start;
                var text = GetContent();
                string subText = new TextRange(pos.DocumentStart, pos).Text;
                var lines = subText.Split('\n');
                row1 = lines.Length;
            }
            {
                var pos = this.rich.Selection.End;
                var text = GetContent();
                string subText = new TextRange(pos.DocumentStart, pos).Text;
                var lines = subText.Split('\n');
                row2 = lines.Length;
            }
            return (row1, row2);
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
            var pos = Math.Abs(this.rich.Selection.Start.GetOffsetToPosition(this.rich.CaretPosition.DocumentStart)) - (col - 1);
            (int row1, int row2) = GetLineRange();
            if (this.DataContext is MainTextViewModel vm)
            {
                vm.AddPresetAction(row1, row2);
                Task.Run(async () =>
                {
                    await Task.Delay(100);
                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        this.rich.CaretPosition = this.rich.CaretPosition.DocumentStart.GetPositionAtOffset(pos);
                    });
                });
            }
        }
        private void RemovePresetAction()
        {
            (int row, int col, int num) = GetPosition();
            var pos = Math.Abs(this.rich.Selection.Start.GetOffsetToPosition(this.rich.CaretPosition.DocumentStart)) - (col - 1);
            (int row1, int row2) = GetLineRange();
            if (this.DataContext is MainTextViewModel vm)
            {
                vm.RemovePresetAction(row1, row2);
                Task.Run(async () =>
                {
                    await Task.Delay(100);
                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        this.rich.CaretPosition = this.rich.CaretPosition.DocumentStart.GetPositionAtOffset(pos);
                    });
                });
            }
        }
        private void WordAction()
        {
            var word = this.rich.Selection.Text;
            word = word.Replace("\r", "").Replace("\n", "");
            if (word.Length > 30) { word = word.Substring(0, 30); }
            if (string.IsNullOrWhiteSpace(word)) { return; }
            if (this.DataContext is MainTextViewModel vm)
            {
                vm.WordAction(word);
            }
        }

        private void rich_Drop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent("Data"))
            {
                AddPresetAction();
                e.Handled = true;
            }
        }

        private void rich_DragOver(object sender, DragEventArgs e)
        {
            //ファイルがドラッグされたとき、カーソルをドラッグ中のアイコンに変更し、そうでない場合は何もしない。
            //e.Effects = (e.Data.GetDataPresent(DataFormats.FileDrop)) ? DragDropEffects.Copy : e.Effects = DragDropEffects.None;
            e.Effects = (e.Data.GetDataPresent("Data") || e.Data.GetDataPresent(DataFormats.FileDrop)) ? DragDropEffects.Move : DragDropEffects.None;
            e.Handled = true;
        }


        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            if (this.DataContext is MainTextViewModel vm)
            {
                vm.MainText = this;
            }
        }

        public (string, string, string) GetSelectedText()
        {
            var beforeRange = new TextRange(this.rich.Document.ContentStart, this.rich.Selection.Start);
            var afterRange = new TextRange(this.rich.Selection.End, this.rich.Document.ContentEnd);
            if (string.IsNullOrWhiteSpace(this.rich.Selection.Text))
            {
                return ("", "", "");
            }
            else
            {
                return (beforeRange.Text, this.rich.Selection.Text, afterRange.Text);
            }
        }

        public (string, string, string) GetCursorText()
        {
            var pos = this.rich.CaretPosition;
            var text = GetContent();
            string subText = new TextRange(pos.DocumentStart, pos).Text;
            if (!subText.Contains("\n"))
            {
                return ("", text, "");
            }
            var index = subText.LastIndexOf('\n');
            var cursorText = text.Substring(index + 1);
            var beforeText = text.Substring(0, index + 1);
            return (beforeText, cursorText, "");
        }

        public (string, string, string) CtrlLeft()
        {
            var pos = this.rich.CaretPosition;
            var text = GetContent();
            string subText = new TextRange(pos.DocumentStart, pos).Text;
            var index1 = subText.Contains('\n') ? subText.LastIndexOf('\n') + 1 : 0;
            var beforeText = text.Substring(0, index1);
            var cursorText = text.Substring(index1);
            var index2 = cursorText.Contains('\n') ? cursorText.IndexOf('\n') + 1 : cursorText.Length;
            var afterText = cursorText.Substring(index2);
            cursorText = cursorText.Substring(0, index2);

            this.rich.CaretPosition = this.rich.CaretPosition.GetPositionAtOffset(index1 - subText.Length);
            return (beforeText, cursorText, afterText);
        }

        public (string, string, string) CtrlRight()
        {
            var pos = this.rich.CaretPosition;
            var text = GetContent();
            string subText = new TextRange(pos.DocumentStart, pos).Text;
            var index1 = subText.Contains('\n') ? subText.LastIndexOf('\n') + 1 : 0;
            var beforeText = text.Substring(0, index1);
            var cursorText = text.Substring(index1);
            var index2 = cursorText.Contains('\n') ? cursorText.IndexOf('\n') + 1 : cursorText.Length;
            var afterText = cursorText.Substring(index2);
            cursorText = cursorText.Substring(0, index2);

            this.rich.CaretPosition = this.rich.CaretPosition.GetPositionAtOffset(index1 - subText.Length + index2);
            return (beforeText, cursorText, afterText);
        }

        public void SetPlayingDocument(FlowDocument document)
        {
            isMaking = true;
            Application.Current.Dispatcher.Invoke(() =>
            {
                this.rich_playing.Document = document;
                this.rich_playing.ScrollToVerticalOffset(this.richScrollOffset);
                isMaking = false;
            });
        }

        private void rich_playing_ScrollChanged(object sender, ScrollChangedEventArgs e)
        {
            if (isMaking) { return; }
            if (e.VerticalChange != 0)
            {
                this.rich.ScrollToVerticalOffset(e.VerticalOffset);
            }
        }
    }
}
