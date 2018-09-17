using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using ZibaobaoLib.Annotations;
using ZibaobaoLib.Data;
using ZibaobaoLib.Helpers;

namespace ZibaobaoLib.Model
{
    public class BaobaoModel: ViewModelBase
    {
        public const string ResourceBase = "data";
        public const string HttpResourceBase = "http://www.relaxingtech.com/" + ResourceBase + "/";
        public const string LocalPathPrefix = "file://";
        Dictionary<BaobaoBook, ExamPaper> _books = new Dictionary<BaobaoBook, ExamPaper>();
        public ObservableCollection<BaobaoBook> BookList { get; } = new ObservableCollection<BaobaoBook>();
        BaobaoBook _activeBook;

        public ObservableCollection<QuestionItem> QuestionList { get; } = new ObservableCollection<QuestionItem>();

        public ObservableCollection<AnswerItem> AnswerList { get; } = new ObservableCollection<AnswerItem>();

        BaobaoBookList _serverBookList;
        readonly BaobaoBookList _localBookList;

        int _currentQuestionIndex;
        int _currentAnswerIndex;
        public BaobaoModel(string indexFileName)
        {
            X1LogHelper.LogLevel = (int)X1EventLogEntryType.Verbose;
            _localBookList = FileSettingsHelper<BaobaoBookList>.LoadSetting() ?? new BaobaoBookList();
            IndexFileName = indexFileName;
            UpdateBookList(false);

            AWSHelper.Instance.OnFileAvailable += Download_OnFileAvailable;
            AWSHelper.Instance.DownloadFile(Path.Combine(ResourceBase, IndexFileName),
                Path.Combine(ZibaobaoLibContext.Instance.PersistentStorage.DownloadPath, IndexFileName)).Forget();
        }

        bool IsBookExist(string name)
        {
            return BookList.FirstOrDefault(b => b.Name == name) != null;
        }
        public ICommand AddPaperCommand => new Command.Command(() =>
        {
            string name = "new paper";
            int index = 0;
            while (IsBookExist(name))
            {
                index++;
                name = $"new paper_{index}";
            }
            var book = new BaobaoBook {Name = name,Version = BaobaoVersion.FromString("0.0.0.0"), IsDirty = true};
            BookList.Add(book);
            ActiveBook = book;
        });

        public ICommand AddQuestionCommand=>new Command.Command(() =>
        {
            if (_activeBook != null && _books.ContainsKey(_activeBook))
            {
                _activeBook.IsDirty = true;
                var examPaper = _books[_activeBook];
                if (examPaper == null)
                {
                    examPaper = new ExamPaper {Title = "new paper"};
                    _books[_activeBook] = examPaper;
                }

                var question = new QuestionItem {Question = "new question"};
                QuestionList.Add(question);
                examPaper.Questions.Add(question);
                CurrentQuestionIndex = examPaper.Questions.Count - 1;
            }
        });

        public ICommand DeleteQuestionCommand => new Command.Command(() =>
        {
            if (CurrentQuestionIndex >= 0 && CurrentQuestionIndex < QuestionList.Count)
            {
                var question = QuestionList[CurrentQuestionIndex];
                QuestionList.Remove(question);
                if (_activeBook != null && _books.ContainsKey(_activeBook))
                {
                    _activeBook.IsDirty = true;
                    var examPaper = _books[_activeBook];
                    examPaper.Questions.Remove(question);
                }
            }
        });

        public ICommand AddAnswerCommand => new Command.Command(() =>
        {
            var currentQuestion = CurrentQuestion;
            if (currentQuestion != null)
            {
                _activeBook.IsDirty = true;
                var newAnswer = new AnswerItem {Value = "new answer"};
                currentQuestion.Answers.Add(newAnswer);
                AnswerList.Add(newAnswer);
            }
        });

        public ICommand DeleteAnswerCommand => new Command.Command(() =>
        {
            var currentQuestion = CurrentQuestion;
            if (currentQuestion != null && CurrentAnswerIndex >= 0 && CurrentAnswerIndex < AnswerList.Count)
            {
                _activeBook.IsDirty = true;
                var answer = AnswerList[CurrentAnswerIndex];
                AnswerList.Remove(answer);
                currentQuestion.Answers.Remove(answer);
            }
        });

        public ICommand SaveCommand => new Command.Command(() =>
        {
            lock (_updateBookLock)
            {
                _activeBook.IsDirty = true;
                _localBookList.Books.Clear();
                foreach (var book in BookList)
                {
                    if (book.IsDirty)
                    {
                        book.Version.VersionBuild++;
                        book.IsDirty = false;
                        var examPaper = _books[book];
                        var paperString = NewtonJsonSerializer.ToJSON(examPaper);
                        AWSHelper.Instance.UploadString(Path.Combine(ResourceBase, book.Name), paperString).Forget();
                    }
                    _localBookList.Books.Add(book);
                }
                var content = NewtonJsonSerializer.ToJSON(_localBookList);
                AWSHelper.Instance.UploadString(Path.Combine(ResourceBase, IndexFileName), content).Forget();
            }
        });


        public void Download_OnFileAvailable(object sender, StringEventArgs e)
        {
            var filePath = e.Value;
            if (Path.GetFileName(filePath) == IndexFileName)
            {
                UpdateBookVersion(filePath);
            }
            else
            {
                AddBook(filePath);
            }
        }

        public string IndexFileName { get; set; }
        public BaobaoBook ActiveBook
        {
            get { return _activeBook; }
            set
            {
                if (_activeBook != value)
                {
                    _activeBook = value;
                    LoadExamPaper(_activeBook);
                    OnPropertyChanged();
                }
            }
        }
        public void LoadExamPaper(BaobaoBook book)
        {
            ExamPaper paper = null;
            AnswerList.Clear();
            QuestionList.Clear();
            if (book != null)
            {
                if (!_books.ContainsKey(book) || _books[book] == null)
                {
                    string examPaper = book.Name;
                    paper = FileSettingsHelper<ExamPaper>.LoadSetting(Path.Combine(ZibaobaoLibContext.Instance.PersistentStorage.DataPath, examPaper));
                    _books[book] = paper;
                }
                else
                {
                    paper = _books[book];
                }
            }
            if (paper != null)
            {
                foreach (var question in paper.Questions)
                {
                    QuestionList.Add(question);
                }

                if (QuestionList.Count > 0)
                {
                    _currentQuestionIndex = -1;
                    CurrentQuestionIndex = 0;
                }
            }
        }

        public QuestionItem CurrentQuestion
        {
            get
            {
                return CurrentQuestionIndex >= 0 & CurrentQuestionIndex < QuestionList.Count
                    ? QuestionList[CurrentQuestionIndex]
                    : new QuestionItem();
            }
            set
            {
                if (value != null)
                {
                    CurrentQuestionIndex = QuestionList.IndexOf(value);
                }
            }
        } 
        public void LoadQuestions()
        {
            var currentQuestion = CurrentQuestion;
            if(currentQuestion != null)
            {
                AnswerList.Clear();
                foreach (var answer in currentQuestion.Answers)
                {
                    AnswerList.Add(answer);
                }
            }
        }
        public int CurrentQuestionIndex
        {
            get { return _currentQuestionIndex; }
            set
            {
                if (_currentQuestionIndex != value)
                {
                    _currentQuestionIndex = value;
                    OnPropertyChanged();
                    LoadQuestions();
                    //BaobaoGameContextFactory.Instance.Game?.SetQuestionTitle($"第 {_currentQuestionIndex+1} 题: {QuestionList[CurrentQuestionIndex].Question}");
                }
            }
        }

        public int CurrentAnswerIndex
        {
            get { return _currentAnswerIndex; }
            set
            {
                if (_currentAnswerIndex != value)
                {
                    _currentAnswerIndex = value;
                    OnPropertyChanged();
                }
            }
        }
        public void UpdateBookVersion(string filePath)
        {
            _serverBookList = FileSettingsHelper<BaobaoBookList>.LoadSetting(filePath);
            if (_serverBookList == null)
            {
                return;
            }

            bool gotNewBook = false;
            foreach (var book in _serverBookList.Books)
            {
                var existingBook = GetExistingBook(book.Name);
                if (existingBook == null || existingBook.Version < book.Version)
                {
                    gotNewBook = true;
                    AWSHelper.Instance.DownloadFile(Path.Combine(ResourceBase, book.Name), 
                        Path.Combine(ZibaobaoLibContext.Instance.PersistentStorage.DownloadPath, book.Name)).Forget();
                }
            }

            if (!gotNewBook)
            {
               UpdateBookList();
            }
        }

        object _updateBookLock = new object();
        void UpdateBookList(bool setActive=true)
        {
            lock (_updateBookLock)
            {
                BookList.Clear();
                foreach (var book in _localBookList.Books)
                {
                    _books[book] = null;
                    BookList.Add(book);
                }

                if (setActive && BookList.Count > 0)
                {
                    ActiveBook = BookList[0];
                }
            }
        }
        BaobaoBook GetExistingBook(string bookName)
        {
            lock (_updateBookLock)
            {
                return _localBookList?.Books.FirstOrDefault(o => o.Name == bookName);
            }
        }
        public void AddBook(string bookPath)
        {
            lock (_updateBookLock)
            {
                var bookName = Path.GetFileName(bookPath);
                if (String.IsNullOrEmpty(bookName))
                {
                    return;
                }
                var storage = ZibaobaoLibContext.Instance.PersistentStorage;
                var dbPath = Path.Combine(storage.DataPath, bookName);
                storage.CopyFile(bookPath, dbPath);
                var newBook = _serverBookList.Books.FirstOrDefault(o => o.Name == bookName);
                if (newBook == null)
                {
                    return;
                }

                var book = GetExistingBook(bookName);
                if (book != null)
                {
                    _localBookList.Books.Remove(book);
                }

                _localBookList.Books.Add(newBook);
                FileSettingsHelper<BaobaoBookList>.SaveSetting(_localBookList);
            }

            UpdateBookList();
        }
        public static string GetImageUrl(string id, string baseUrl = HttpResourceBase + "images")
        {
            try
            {
                if (String.IsNullOrEmpty(id))
                {
                    return null;
                }
                return baseUrl + "/" + id;
            }
            catch (Exception e)
            {
                X1LogHelper.Exception(e);
            }
            return null;
        }
    }
}