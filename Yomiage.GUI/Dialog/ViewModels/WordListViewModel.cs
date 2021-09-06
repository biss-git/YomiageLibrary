using Prism.Mvvm;
using Reactive.Bindings;
using Reactive.Bindings.Extensions;
using Reactive.Bindings.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Yomiage.GUI.Data;
using Yomiage.GUI.Models;

namespace Yomiage.GUI.Dialog.ViewModels
{
    class WordListViewModel : DialogViewModelBase
    {
        public override string Title => "単語一覧";

        private readonly ReactiveCollection<WordDisplaySet> WordListOrigin = new();
        public IFilteredReadOnlyObservableCollection<WordDisplaySet> WordList { get; }
        public ReactivePropertySlim<WordDisplaySet> Selected { get; } = new();
        public ReadOnlyReactivePropertySlim<bool> CanAction { get; }

        public ReactivePropertySlim<string> SearchText { get; } = new();
        public ReactivePropertySlim<bool> TargetAll { get; } = new(true);
        public ReactivePropertySlim<bool> TargetText { get; } = new();
        public ReactivePropertySlim<bool> TargetYomi { get; } = new();
        public ReactivePropertySlim<bool> MatchTypeStart { get; } = new();
        public ReactivePropertySlim<bool> MatchTypeAny { get; } = new(true);
        public ReactivePropertySlim<bool> MatchTypeEnd { get; } = new();
        public ReactivePropertySlim<bool> MatchTypeAnd { get; } = new(true);
        public ReactivePropertySlim<bool> MatchTypeOr { get; } = new();


        public ReactivePropertySlim<int> TotalSize { get; } = new(10);
        public ReactivePropertySlim<int> FilterdSize { get; } = new(10);
        public ReactivePropertySlim<int> ListSize { get; } = new(100);
        public ReactivePropertySlim<int> PageSize { get; } = new(0);
        public ReactivePropertySlim<int> PageIndex { get; } = new(1);
        public ReactivePropertySlim<int> StartIndex { get; } = new(1);
        public ReactivePropertySlim<int> EndIndex { get; } = new(1);

        public ReactivePropertySlim<SearchGroup> Group { get; } = new(SearchGroup.All);

        public ReactiveCommand EditCommand { get; }
        public ReactiveCommand DeleteCommand { get; }

        WordDictionaryService wordDictionaryService;

        public WordListViewModel(
            WordDictionaryService wordDictionaryService)
        {
            this.wordDictionaryService = wordDictionaryService;
            foreach(var pair in wordDictionaryService.WordDictionarys)
            {
                WordDisplaySet set = new()
                {
                    OriginalText = pair.Key,
                    Yomi = pair.Value.Yomi,
                    Priority = pair.Value.Priority,
                };
                WordListOrigin.Add(set);
            }
            WordList = WordListOrigin.ToFilteredReadOnlyObservableCollection(x => x.Visible.Value).AddTo(Disposables);
            Observable.Merge(TargetAll, TargetText, TargetYomi, MatchTypeStart, MatchTypeAny, MatchTypeEnd, MatchTypeAnd, MatchTypeOr)
                .Subscribe(_ => Filter()).AddTo(Disposables);
            SearchText.Subscribe(_ => Filter()).AddTo(Disposables);
            Group.Subscribe(_ => Filter()).AddTo(Disposables);
            Observable.Merge(FilterdSize, ListSize).Subscribe(_ => PageSize.Value = 1 + (FilterdSize.Value - 1) / ListSize.Value).AddTo(Disposables);
            PageSize.Subscribe(_ => {
                PageIndex.Value = Math.Clamp(PageIndex.Value, 1, Math.Max(1, PageSize.Value));
                Paging();
                }).AddTo(Disposables);
            PageIndex.Subscribe(_ => Paging()).AddTo(Disposables);

            CanAction = Selected.Select(x => x != null).ToReadOnlyReactivePropertySlim();
            EditCommand = CanAction.ToReactiveCommand().WithSubscribe(EditAction).AddTo(Disposables);
            DeleteCommand = CanAction.ToReactiveCommand().WithSubscribe(DeleteAction).AddTo(Disposables);
        }

        private void EditAction()
        {
            this.wordDictionaryService.Edit(Selected.Value.OriginalText);
            RaiseRequestClose(null);
        }
        private void DeleteAction()
        {
            var result = MessageBox.Show("選択されている単語を削除してよろしいですか？", "確認", MessageBoxButton.OKCancel);
            if(result != MessageBoxResult.OK) { return; }
            wordDictionaryService.UnRegiserDictionary(Selected.Value.OriginalText);
            WordListOrigin.Remove(Selected.Value);
            Filter();
        }

        private void Filter()
        {
            foreach(var item in WordListOrigin)
            {
                if(!SearchGroupUtil.GroupMatch_katakana(item.Yomi, Group.Value)) {
                    item.Filterd.Value = false;
                    continue;
                }

                // 検索文字列がなしなら
                if (string.IsNullOrWhiteSpace(SearchText.Value))
                {
                    item.Filterd.Value = true;
                    continue;
                }

                item.Filterd.Value = Search(item);
            }
            TotalSize.Value = WordListOrigin.Count;
            FilterdSize.Value = WordListOrigin.Where(w => w.Filterd.Value).Count();
            Paging();
        }


        private bool Search(WordDisplaySet item)
        {
            var texts = SearchText.Value.Split(' ').Where(t => !string.IsNullOrWhiteSpace(t));
            if(texts.Count() == 0) { return true; }
            if (TargetAll.Value)
            {
                return SearchSub(item.OriginalText, texts) || SearchSub(item.Yomi, texts) || SearchSub(item.Priority, texts);
            }
            else if (TargetText.Value)
            {
                return SearchSub(item.OriginalText, texts);
            }
            else if (TargetYomi.Value)
            {
                return SearchSub(item.Yomi, texts);
            }
            return false;
        }

        private bool SearchSub(string targetText, IEnumerable<string> texts)
        {
            if (MatchTypeAny.Value)
            {
                if (MatchTypeOr.Value)
                {
                    return texts.Any(t => targetText.Contains(t));
                }
                else
                {
                    return texts.All(t => targetText.Contains(t));
                }
            }
            else if (MatchTypeStart.Value)
            {
                return texts.Any(t => targetText.StartsWith(t));
            }
            else if (MatchTypeEnd.Value)
            {
                return texts.Any(t => targetText.EndsWith(t));
            }
            return false;
        }

        private void Paging()
        {
            var start = Math.Max(ListSize.Value * (PageIndex.Value - 1), 0);
            var end = start + ListSize.Value;

            int count = 0;
            foreach(var item in WordListOrigin)
            {
                if (item.Filterd.Value)
                {
                    item.Visible.Value = (start <= count && count < end);
                    count += 1;
                }
                else
                {
                    item.Visible.Value = false;
                }
            }
            var visibleCount = WordListOrigin.Where(w => w.Visible.Value).Count();
            if(visibleCount == 0)
            {
                StartIndex.Value = 0;
                EndIndex.Value = 0;
            }
            else
            {
                StartIndex.Value = start + 1;
                EndIndex.Value = StartIndex.Value + visibleCount - 1;
            }
            if(!WordListOrigin.Any(w => w.Visible.Value && Selected.Value == w))
            {
                Selected.Value = null;
            }
        }
    }

    class WordDisplaySet : BindableBase
    {
        public string OriginalText { get; set; }
        public string Yomi { get; set; }
        public string Priority { get; set; }
        public ReactivePropertySlim<bool> Filterd { get; } = new(true);
        public ReactivePropertySlim<bool> Visible { get; } = new(true);

        public WordDisplaySet()
        {
            this.Filterd.Subscribe(_ => RaisePropertyChanged(nameof(Filterd)));
            this.Visible.Subscribe(_ => RaisePropertyChanged(nameof(Visible)));
        }
    }
}
