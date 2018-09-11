using System;
using System.IO;
using System.Threading.Tasks;
using Plugin.DownloadManager;
using Plugin.DownloadManager.Abstractions;
using ZibaobaoLib.Helpers;
using ZibaobaoLib.Model;

namespace ZibaobaoLib
{
    public class DownloadController
    {
        static DownloadController _instance;
        public static DownloadController Instance => _instance ?? (_instance = new DownloadController());

        public DownloadController()
        {
            var downloadManager = CrossDownloadManager.Current;
            downloadManager.PathNameForDownloadedFile = downloadFile =>
            {
                var fileName = Path.GetFileName(downloadFile.Url) ?? "";
                return Path.Combine(ZibaobaoLibContext.Instance.PersistentStorage.DownloadPath, fileName);
            };
        }

        public void DownloadResource(string uri)
        {
            var downloadManager = CrossDownloadManager.Current;
            var file = downloadManager.CreateDownloadFile(BaobaoModel.HttpResourceBase + uri);
            file.PropertyChanged += File_PropertyChanged;
            Task.Run(()=>downloadManager.Start(file));
        }

        void File_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            System.Diagnostics.Debug.WriteLine("[Property changed] " + e.PropertyName + " -> " + sender?.GetType().GetProperty(e.PropertyName)?.GetValue(sender, null));

            // Update UI text-fields
            var downloadFile = ((IDownloadFile)sender);
            if (downloadFile == null)
            {
                return;
            }
            // Update UI text-fields
            switch (e.PropertyName)
            {
                case nameof(IDownloadFile.Status):
                    X1LogHelper.Log($"{downloadFile.Url} [status]: {downloadFile.Status}");
                    break;
                case nameof(IDownloadFile.TotalBytesExpected):
                    X1LogHelper.Verbose($"{downloadFile.Url} [Total length]: {downloadFile.TotalBytesExpected}");
                    break;
                case nameof(IDownloadFile.TotalBytesWritten):
                    X1LogHelper.Verbose($"{downloadFile.Url} [downloaded]: {downloadFile.TotalBytesWritten}");
                    break;
            }

            // Update UI if download-status changed.
            if (e.PropertyName == "Status")
            {
                switch (((IDownloadFile)sender).Status)
                {
                    case DownloadFileStatus.COMPLETED:
                        {
                            X1LogHelper.Log($"{downloadFile.Url} [downloaded as ]: [{downloadFile.DestinationPathName}]");
                            var path = downloadFile.DestinationPathName;
                            if (path.StartsWith(BaobaoModel.LocalPathPrefix))
                            {
                                path = path.Substring(BaobaoModel.LocalPathPrefix.Length);
                            }
                            OnFileAvailable?.Invoke(this, new StringEventArgs(path));
                        }
                        break;
                }
            }
        }

        public event EventHandler<StringEventArgs> OnFileAvailable;
    }
}