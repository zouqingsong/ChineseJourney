namespace ChineseJourney.Common.Model
{
    public static class Constants
    {
        // OAuth
        // For Google login, configure at https://console.developers.google.com/
        public static string AndroidClientId = "725892070485-74q9j2grh10e84qn9pcodmj763evrb05.apps.googleusercontent.com";
        public static string AndroidRedirectUrl = "com.googleusercontent.apps.725892070485-74q9j2grh10e84qn9pcodmj763evrb05:/oauth2redirect";

        public static string iOSClientId = "725892070485-9ogfjh1pmig9ivl6b25pdqnehg4b146c.apps.googleusercontent.com";
        public static string iOSRedirectUrl = "com.relaxingtech.chinesejourney:/oauth2redirect";

        // These values do not need changing
        public static string Scope = "https://www.googleapis.com/auth/userinfo.email";
        public static string AuthorizeUrl = "https://accounts.google.com/o/oauth2/auth";
        public static string AccessTokenUrl = "https://www.googleapis.com/oauth2/v3/token";
        public static string UserInfoUrl = "https://www.googleapis.com/oauth2/v2/userinfo";
    }
}
