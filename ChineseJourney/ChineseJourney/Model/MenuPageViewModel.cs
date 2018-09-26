using System.Collections.ObjectModel;
using System.Windows.Input;
using ChineseJourney.Common.Helpers;
using ZibaobaoLib.Command;
using ZibaobaoLib.Data;
using ZibaobaoLib.Model;

namespace ChineseJourney.Common.Model
{
    public class MenuPageViewModel : BaobaoModel
    {
        GoogleApiHelper _googleApiHelper = new GoogleApiHelper();
        ICommand _loginCommand;
        BaobaoUser _user;

        public ObservableCollection<MasterMenuItem> MenuItems { get; set; }
        public MenuPageViewModel(string indexFileName) : base(indexFileName)
        {
            MenuItems = new ObservableCollection<MasterMenuItem>(new[]
            {
                new MasterMenuItem {Title = "Spelling", TargetType = typeof(HanziPage)},
                new MasterMenuItem {Title = "Exam", TargetType = typeof(QuestionPage)},
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