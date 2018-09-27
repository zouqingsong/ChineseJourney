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
        UserAccount _userAccount;

        public ObservableCollection<MasterMenuItem> MenuItems { get; set; }
        public MenuPageViewModel(string indexFileName) : base(indexFileName)
        {
            _userAccount = new UserAccount();
            MenuItems = new ObservableCollection<MasterMenuItem>(new[]
            {
                new MasterMenuItem {Title = "Spelling", TargetType = typeof(HanziPage)},
                new MasterMenuItem {Title = "Exam", TargetType = typeof(QuestionPage)},
            });
            _googleApiHelper.OnUserLogin += _googleApiHelper_OnUserLogin;
        }

        void _googleApiHelper_OnUserLogin(object sender, UserLoginEventArgs e)
        {
            _userAccount.Account = e.Account;
            _userAccount.Name = e.User.name;
            _userAccount.Email = e.User.email;
            _userAccount.Save();
            OnPropertyChanged(nameof(UserNameString));
        }

        public string UserNameString => !IsAccountValid ? "[login]" : $"{_userAccount.Email} [logout]";

        public bool IsAccountValid => !string.IsNullOrEmpty(_userAccount.Email);

        public ICommand LoginCommand => _loginCommand ?? (_loginCommand = new Command(() =>
        {
            bool isAccountValid = IsAccountValid;
            _userAccount.Reset();
            OnPropertyChanged(nameof(UserNameString));
            if (!isAccountValid)
            {
                _googleApiHelper.LoginUser(true);
            }
        }));
    }
}