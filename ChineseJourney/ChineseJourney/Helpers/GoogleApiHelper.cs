using System;
using System.Linq;
using System.Threading.Tasks;
using ChineseJourney.Common.Model;
using Xamarin.Auth;
using Xamarin.Auth.Presenters;
using Xamarin.Forms;
using ZibaobaoLib;
using ZibaobaoLib.Data;
using ZibaobaoLib.Helpers;

namespace ChineseJourney.Common.Helpers
{
    public class UserLoginEventArgs : EventArgs
    {
        public UserLoginEventArgs(BaobaoUser user)
        {
            User = user;
        }
        public BaobaoUser User { get; set; }
    }
    public class AuthenticationState
    {
        public static OAuth2Authenticator Authenticator;
    }
    public class GoogleApiHelper
    {
        OAuth2Authenticator _authenticator;

        public bool CanAutoLogin =>
            AccountStore.Create().FindAccountsForService(ZibaobaoLibContext.Instance.AppName).FirstOrDefault() != null;

        public void LoginUser(bool force=false)
        {
            var account = AccountStore.Create().FindAccountsForService(ZibaobaoLibContext.Instance.AppName).FirstOrDefault();
            if (!force && account != null)
            {
                LoginUser(account).Forget();
            }
            else
            {
                string clientId = null;
                string redirectUri = "";
                switch (Device.RuntimePlatform)
                {
                    case Device.Android:
                        clientId = Constants.AndroidClientId;
                        redirectUri = Constants.AndroidRedirectUrl;
                        break;
                    default:
                        clientId = Constants.iOSClientId;
                        redirectUri = Constants.iOSRedirectUrl;
                        break;
                }
                _authenticator = new OAuth2Authenticator(
                        clientId,
                        null,
                        Constants.Scope,
                        new Uri(Constants.AuthorizeUrl),
                        new Uri(redirectUri),
                        new Uri(Constants.AccessTokenUrl),
                        null,
                        true)
                    { AllowCancel = true };
                _authenticator.Completed += OnAuthCompleted;
                _authenticator.Error += OnAuthError;

                AuthenticationState.Authenticator = _authenticator;
                var presenter = new OAuthLoginPresenter();
                presenter.Login(_authenticator);
            }
        }
        void OnAuthError(object sender, AuthenticatorErrorEventArgs e)
        {
            var authenticator = sender as OAuth2Authenticator;
            if (authenticator != null)
            {
                authenticator.Completed -= OnAuthCompleted;
                authenticator.Error -= OnAuthError;
            }
            X1LogHelper.Error(e.Message);
        }

        async void OnAuthCompleted(object sender, AuthenticatorCompletedEventArgs e)
        {
           var authenticator = sender as OAuth2Authenticator;
            if (authenticator != null)
            {
                authenticator.Completed -= OnAuthCompleted;
                authenticator.Error -= OnAuthError;
            }
            
            if (e.IsAuthenticated)
            {
                AccountStore.Create().Save(e.Account, ZibaobaoLibContext.Instance.AppName);
                await LoginUser(e.Account);
            }
            /*
            if (Device.RuntimePlatform == Device.UWP)
            {
                Application.Current.MainPage.Navigation.PopModalAsync().Forget();
            }*/
        }

        private async Task LoginUser(Account account)
        {
            var request = new OAuth2Request("GET", new Uri(Constants.UserInfoUrl), null, account);
            var response = await request.GetResponseAsync();
            if (response != null)
            {
                string userJson = await response.GetResponseTextAsync();
                var user = NewtonJsonSerializer.ParseJSON<BaobaoUser>(userJson);
                OnUserLogin?.Invoke(this, new UserLoginEventArgs(user));
            }
        }

        public event EventHandler<UserLoginEventArgs> OnUserLogin;
    }
}
