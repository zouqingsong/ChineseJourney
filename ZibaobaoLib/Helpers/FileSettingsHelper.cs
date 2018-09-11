using System;
using System.Collections.Generic;
using System.IO;

namespace ZibaobaoLib.Helpers
{
    public class FileSettingsHelper<T> where T:new()
    {
        static IPersistentStorage Storage => ZibaobaoLibContext.Instance.PersistentStorage;

        public static string FileName => typeof(T).Name + ".txt";

        public static string SettingsPath
        {
            get
            {
                Storage.CreateDirectory(Storage.SettingPath);
                var path = Path.Combine(Storage.SettingPath, FileName);
                if (!Storage.Exists(path))
                {
                    Storage.Create(path);
                }
                return path;
            }
        }

        public static string GetSettingFolder(string group)
        {
            string path = Path.Combine(Storage.SettingPath, group);
            Storage.CreateDirectory(path);
            return path;
        }

        public static List<string> GetAllFiles(string path, string filter)
        {
            return Storage.GetAllFiles(path, filter);
        }
        

        public static T LoadSetting(string settingPath=null)
        {
            T setting = default(T);
            try
            {
                if (string.IsNullOrEmpty(settingPath))
                {
                    settingPath = SettingsPath;
                }
                var settingContent = Storage.LoadText(settingPath);
                setting = NewtonJsonSerializer.ParseJSON<T>(settingContent);
            }
            catch (Exception)
            {
            }

            if (setting == null)
            {
                setting = new T();
            }
            return setting;
        }

        public static void SaveSetting(T setting, string settingPath = null)
        {
            if (string.IsNullOrEmpty(settingPath))
            {
                settingPath = SettingsPath;
            }
            Storage.SaveText(settingPath, NewtonJsonSerializer.ToJSON(setting));
        }

    }
}
