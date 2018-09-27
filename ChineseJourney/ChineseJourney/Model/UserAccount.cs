using System.Collections.Generic;
using System.Linq;
using Xamarin.Auth;
using ZibaobaoLib;

namespace ChineseJourney.Common.Model
{
    public class UserAccount
    {
        public const string KeyEmail = "email";
        public const string KeyName = "name";
        public UserAccount()
        {
            Account = AccountStore.Create().FindAccountsForService(ZibaobaoLibContext.Instance.AppName).FirstOrDefault() ??
                      new Account();
        }

        public void Reset()
        {
            Account = new Account();
            Save();
        }
        public string Email
        {
            get => Account.Properties.ContainsKey(KeyEmail)?Account.Properties[KeyEmail]:string.Empty;
            set => Account.Properties[KeyEmail] = value;
        }

        public string Name
        {
            get => Account.Properties.ContainsKey(KeyName) ? Account.Properties[KeyEmail] : string.Empty;
            set => Account.Properties[KeyName] = value;
        }

        public void Save()
        {
            AccountStore.Create().Save(Account, ZibaobaoLibContext.Instance.AppName);
        }

        public Account Account { get; set; }
    }
}