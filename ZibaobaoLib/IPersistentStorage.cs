using System;
using System.Collections.Generic;
using System.IO;

namespace ZibaobaoLib
{
    public interface IPersistentStorage
    {
        List<string> GetAllFiles(string path, string filter);

        DateTime GetFileCreationTime(string path);

        long GetFileLength(string path);
        string GetLatestFile(string path, string filter);
        string GetEntityPath(string id);
        bool DeleteEntiry(string id);
        bool SaveEntiry(string id, Stream data);
        Stream LoadEntiry(string id);
        string CreateDirectory(string path);
        bool Exists(string path);
        bool Delete(string path);
        bool Create(string path);
        void CopyFile(string source, string dest);
        string PersonalPath { get; }
        string DocumentPath { get; }
        string SettingPath { get; }
        string ProfilePath { get; }
        string TempPath { get; }
        string DeviceDataPath { get; }
        string DataPath { get; }

        string DownloadPath { get; set; }

        string SharePath { get; }
        string PackagePath { get; }
        string LogPath { get; }
        void SaveText(string path, string text);
        bool SaveContent(string path, Stream data);
        string LoadText(string path);
        bool ResetData();
        string GetCachePath(string id);
        Stream LoadCache(string id);

        Stream LoadContent(string path);
        void Copy(string sourceDirectory, string targetDirectory);
        void DeleteAllFiles(string folder);
    }
}
