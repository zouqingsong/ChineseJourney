using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using ZibaobaoLib;
using ZibaobaoLib.Helpers;

namespace SamplyGame.Helpers
{
    public class DiskStorage : IPersistentStorage
    {
        #region Helpers
#if (WINDOWS_APP || WINDOWS_PHONE_APP || WINDOWS_UWP)
        public static string _personalPath = _CreateDirectory(Windows.Storage.ApplicationData.Current.LocalFolder.Path);
#elif (__IOS__ || __ANDROID__)
        static string _personalPath = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
#else
        static string _personalPath = Environment.GetFolderPath(Environment.SpecialFolder.CommonDocuments);
#endif

        public string DownloadPath
        {
            get { return string.IsNullOrEmpty(_download) ? PackagePath : _download; }
            set { _download = value; }
        }

        string _download;

        static string _documentPath => _CreateDirectory(Path.Combine(_personalPath, ZibaobaoLibContext.Instance.AppName));
        static string _dataPath => _CreateDirectory(Path.Combine(_documentPath, "data"));

        static string _CreateDirectory(string path)
        {
            try
            {
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }
            }
            catch (Exception ex)
            {
                X1LogHelper.Exception(ex);
            }
            return path;
        }
        static bool _Delete(string path)
        {
            try
            {
                if (Directory.Exists(path))
                {
                    new DirectoryInfo(path).Delete(true);
                }
                else if (File.Exists(path))
                {
                    File.Delete(path);
                }
                return true;
            }
            catch (Exception)
            {
            }
            return false;
        }

        #endregion
        public string PersonalPath => _personalPath;
        public string DocumentPath => _documentPath;
        public string DataPath => _dataPath;
        public string LogPath => _CreateDirectory(Path.Combine(_documentPath, "log"));

        private string _sharePath;
        public string SharePath
        {
            get
            {
                if (string.IsNullOrEmpty(_sharePath))
                {
                    return _CreateDirectory(Path.Combine(_documentPath, "share"));
                }
                return _sharePath;
            }
            set
            {
                if (!string.IsNullOrEmpty(value))
                {
                    _sharePath = value;
                }
            }
        }
        public string SettingPath => _CreateDirectory(Path.Combine(_dataPath, "settings"));
        public string ProfilePath => _CreateDirectory(Path.Combine(_dataPath, "profile"));
        public string DeviceDataPath => _CreateDirectory(Path.Combine(_dataPath, "devicedata"));
        public string PackagePath => _CreateDirectory(Path.Combine(_dataPath, "packages"));
        public string TempPath => _CreateDirectory(Path.Combine(_dataPath, "temp"));

        static DiskStorage()
        {
            _Delete(Path.Combine(_dataPath, "cache"));
            _Delete(Path.Combine(_dataPath, "temp"));
        }

        public string CreateDirectory(string path)
        {
            return _CreateDirectory(path);
        }

        public bool Exists(string path)
        {
            return Directory.Exists(path) || File.Exists(path);
        }

        public bool ResetData()
        {
            try
            {
                _Delete(Path.Combine(_dataPath, "cache"));
                _Delete(Path.Combine(_dataPath, "temp"));
                _Delete(Path.Combine(_dataPath, "entity"));
                _Delete(PersistentDataHelper.Instance.GetDataBasePath(true, PersistentDataHelper.DataDB));
                return true;
            }
            catch (Exception ex)
            {
                X1LogHelper.Exception(ex);
                return false;
            }
        }

        public bool Create(string path)
        {
            try
            {
                CreateDirectory(Path.GetDirectoryName(path));
            }
            catch (Exception)
            {
            }
            try
            {
                using (File.Create(path)) { }
                return true;
            }
            catch (Exception ex)
            {
                X1LogHelper.Exception(ex);
            }
            return false;
        }

        public DateTime GetFileCreationTime(string path)
        {
            return new FileInfo(path).CreationTime;
        }

        public void DeleteAllFiles(string folder)
        {
            foreach (var file in GetAllFiles(folder, "*.*"))
            {
                try
                {
                    Delete(file);
                }
                catch
                {
                }
            }
        }
        public void Copy(string sourceDirectory, string targetDirectory)
        {
            DirectoryInfo diSource = new DirectoryInfo(sourceDirectory);
            DirectoryInfo diTarget = new DirectoryInfo(targetDirectory);

            CopyAll(diSource, diTarget);
        }

        public static void CopyAll(DirectoryInfo source, DirectoryInfo target)
        {
            Directory.CreateDirectory(target.FullName);

            // Copy each file into the new directory.
            foreach (FileInfo fi in source.GetFiles())
            {
                Console.WriteLine(@"Copying {0}\{1}", target.FullName, fi.Name);
                fi.CopyTo(Path.Combine(target.FullName, fi.Name), true);
            }

            // Copy each subdirectory using recursion.
            foreach (DirectoryInfo diSourceSubDir in source.GetDirectories())
            {
                DirectoryInfo nextTargetSubDir =
                    target.CreateSubdirectory(diSourceSubDir.Name);
                CopyAll(diSourceSubDir, nextTargetSubDir);
            }
        }

        public void CopyFile(string source, string dest)
        {
            Delete(dest);
            try
            {
                File.Copy(source, dest);
            }
            catch (Exception ex)
            {
                X1LogHelper.Exception(ex);
            }
        }

        public bool Delete(string path)
        {
            return _Delete(path);
        }

        public List<string> GetAllFiles(string path, string filter)
        {
            return new DirectoryInfo(path).GetFiles(filter).Select(fileInfo => fileInfo.FullName).ToList();
        }

        public string GetLatestFile(string path, string filter)
        {
            var files = new DirectoryInfo(path).GetFiles(filter);
            FileInfo fileInfo = null;
            foreach (var file in files)
            {
                if (fileInfo == null || file.LastWriteTimeUtc > fileInfo.LastWriteTimeUtc)
                {
                    fileInfo = file;
                }
            }
            return fileInfo?.FullName ?? string.Empty;
        }


        public string GetEntityPath(string id)
        {
            return string.Empty;
            //return Path.Combine(PersistentDataHelper.Instance.EntityPath, id);
        }

        public string GetCachePath(string id)
        {
            return string.Empty;
            //return Path.Combine(PersistentDataHelper.Instance.CachePath, id);
        }
        public bool DeleteEntiry(string id)
        {
            try
            {
                var path = GetEntityPath(id);
                if (File.Exists(path))
                {
                    File.Delete(path);
                }
            }
            catch (Exception ex)
            {
                X1LogHelper.Error(ex.Message);
            }
            return false;
        }

        public bool SaveEntiry(string id, Stream data)
        {
            try
            {
                using (var stream = new FileStream(GetEntityPath(id), FileMode.Create))
                {
                    data.Position = 0;
                    data.CopyTo(stream);
                    return true;
                }
            }
            catch (Exception ex)
            {
                X1LogHelper.Error(ex.Message);
            }
            return false;
        }

        public Stream LoadEntiry(string id)
        {
            try
            {
                return LoadContent(GetEntityPath(id));
            }
            catch (Exception ex)
            {
                X1LogHelper.Error(ex.Message);
            }
            return null;
        }

        public Stream LoadCache(string id)
        {
            try
            {
                return LoadContent(GetCachePath(id));
            }
            catch { }
            return null;
        }

        public long GetFileLength(string path)
        {
            try
            {
                return new FileInfo(path).Length;
            }
            catch (Exception ex)
            {
                X1LogHelper.Error(ex.Message);
            }

            return 0;
        }
        public Stream LoadContent(string path)
        {
            try
            {
                if (File.Exists(path))
                {
                    return new FileStream(path, FileMode.Open, FileAccess.Read);
                }
            }
            catch (Exception ex)
            {
                X1LogHelper.Error(ex.Message);
            }
            return null;
        }
        public void SaveText(string path, string text)
        {
            try
            {

                File.WriteAllText(path, text);
            }
            catch (Exception ex)
            {
                X1LogHelper.Error(ex.Message);
            }
        }

        public bool SaveContent(string path, Stream data)
        {
            try
            {
                using (var stream = new FileStream(path, FileMode.Create))
                {
                    try
                    {
                        data.Position = 0;
                    }
                    catch (Exception ex)
                    {
                        X1LogHelper.Error(ex.Message);
                    }
                    data.CopyTo(stream);
                    return true;
                }
            }
            catch (Exception ex)
            {
                X1LogHelper.Exception(ex);
            }
            return false;
        }

        public string LoadText(string path)
        {
            try
            {
                if (File.Exists(path))
                {
                    return File.ReadAllText(path);
                }
            }
            catch (Exception ex)
            {
                X1LogHelper.Error(ex.Message);
            }
            return string.Empty;
        }
    }
}