using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using ChineseJourney.Common.Helpers;
using ZibaobaoLib.Command;
using ZibaobaoLib.Data;
using ZibaobaoLib.Model;

namespace ChineseJourney.Common.Model
{
    public class MasterViewModel : BaobaoModel
    {
        GoogleApiHelper _googleApiHelper = new GoogleApiHelper();
        ICommand _loginCommand;
        private BaobaoUser _user;

        public ObservableCollection<MasterMenuItem> MenuItems { get; set; }
        public MasterViewModel(string indexFileName) : base(indexFileName)
        {
            MenuItems = new ObservableCollection<MasterMenuItem>(new[]
            {
                new MasterMenuItem { Id = 0, Title = "Spelling", TargetType = typeof(HanziPage)},
                new MasterMenuItem { Id = 1, Title = "Exam", TargetType = typeof(QuestionPage)},
                new MasterMenuItem { Id = 2, Title = "Page 3" },
                new MasterMenuItem { Id = 3, Title = "Page 4" },
                new MasterMenuItem { Id = 4, Title = "Settings" },
            });

            _googleApiHelper.OnUserLogin += _googleApiHelper_OnUserLogin;
            if (_googleApiHelper.CanAutoLogin)
            {
                _googleApiHelper.LoginUser();
            }
        }

        void _googleApiHelper_OnUserLogin(object sender, UserLoginEventArgs e)
        {
            _user = e.User;
            OnPropertyChanged(nameof(UserNameString));
        }

        public string UserNameString => _user != null ? $"{_user.name}[{_user.email}]" : "not login";

        public ICommand LoginCommand => _loginCommand ?? (_loginCommand = new Command(() =>
        {
            _googleApiHelper.LoginUser(true);
        }));
    }
}