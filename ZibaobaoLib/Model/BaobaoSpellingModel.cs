using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Plugin.TextToSpeech;
using ZibaobaoLib.Data;
using ZibaobaoLib.Helpers;

namespace ZibaobaoLib.Model
{
    public class BaobaoSpellingModel: ViewModelBase, FileHandler
    {
        int _currentSpellingWordIndex;

        string _activeSpellingList = string.Empty;
        public List<SpellingList> _spellingList { get; } = new List<SpellingList>();
        public ObservableCollection<string> SpellingWordList { get; } = new ObservableCollection<string>();
        public ObservableCollection<string> SpellingList { get; } = new ObservableCollection<string>();

        public string UserEmail { get; set; }

        public string UserFolderBase => Path.Combine("zibaobao", "accounts", UserEmail);

        public async Task LoadUserMaterial(string userEmail)
        {
            UserEmail = userEmail;
            SpellingList.Clear();
            foreach (var name in await AWSHelper.Instance.ListFiles(UserFolderBase))
            {
                SpellingList.Add(Path.GetFileNameWithoutExtension(name));
            }
        }

        public void Save(string userEmail, SpellingList spelling)
        {
            var content = NewtonJsonSerializer.ToJSON(spelling);
            AWSHelper.Instance.UploadString(GetSpellingListRemotePath(spelling.Title), content).Forget();
        }

        public string GetSpellingListRemotePath(string name)
        {
            return Path.Combine(UserFolderBase, name + ".txt");
        }
        public string ActiveSpellingList
        {
            get => _activeSpellingList;
            set
            {
                if (_activeSpellingList != value)
                {
                    _activeSpellingList = value;
                    OnPropertyChanged();
                    if (LoadSpellingList(_activeSpellingList))
                    {
                        CurrentSpellingWordIndex = 0;
                    }
                    else
                    {
                        AWSHelper.Instance.DownloadFile(GetSpellingListRemotePath(_activeSpellingList), null, this).Forget();
                    }
                }
            }
        }
        public void OnFileAvailable(string path, string content = null)
        {
            if (!string.IsNullOrEmpty(content))
            {
                var spellingList = NewtonJsonSerializer.ParseJSON<SpellingList>(content);
                if (spellingList != null)
                {
                    _spellingList.Add(spellingList);
                    if (spellingList.Title == _activeSpellingList)
                    {
                        LoadSpellingList(_activeSpellingList);
                    }
                }
            }
        }
        bool LoadSpellingList(string name)
        {
            var activeList = _spellingList.FirstOrDefault(o => o.Title == name);
            if (activeList != null)
            {
                SpellingWordList.Clear();
                foreach (var word in activeList.Words)
                {
                    SpellingWordList.Add(word);
                }
                return true;
            }

            return false;
        }

        public int CurrentSpellingWordIndex
        {
            get => _currentSpellingWordIndex;
            set
            {
                if (_currentSpellingWordIndex != value)
                {
                    _currentSpellingWordIndex = value;
                    if (_currentSpellingWordIndex >= 0 && _currentSpellingWordIndex < SpellingWordList.Count)
                    {
                        ZibaobaoLibContext.Instance.Speak(SpellingWordList[_currentSpellingWordIndex]);
                    }
                    OnPropertyChanged();
                }
            }
        }
    }
}