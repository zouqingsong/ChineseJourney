using System;
using System.IO;
using System.Threading.Tasks;
using ChineseJourney.Common.Model;
using Xamarin.Forms;
using ZibaobaoLib;
using ZibaobaoLib.Helpers;
using ZibaobaoLib.Model;

namespace ChineseJourney.Common.Controller
{
    public class BaobaoGameController
    {
        public const string BookListName = "BaobaoBookList.txt";
        //public const string HttpResourceBase = "http://www.doitech.com/liufen/lesson/" + ResourceBase + "/";
        protected BaobaoGameController()
        {
            DataModel = new MasterViewModel(BookListName);
            DownloadController.Instance.OnFileAvailable += DataModel.Download_OnFileAvailable;
            var hanziList = HanziStrokeController.Instance.HanZi;
            //DownloadController.Instance.DownloadResource(DownloadController.BookListName);
        }
        public static void Invoke(Action action)
        {
            if (Device.IsInvokeRequired)
            {
                Device.BeginInvokeOnMainThread(action);
            }
            else
            {
                action();
            }
        }
        public static Task<T> Invoke<T>(Func<T> a)
        {
            var tcs = new TaskCompletionSource<T>();
            if (Device.IsInvokeRequired)
            {
                Device.BeginInvokeOnMainThread(() =>
                {
                    RunFunc(a, tcs);
                });
            }
            else
            {
                RunFunc(a, tcs);
            }
            return tcs.Task;
        }
        static void RunFunc<T>(Func<T> a, TaskCompletionSource<T> tcs)
        {
            try
            {
                var result = a();
                tcs.SetResult(result);
            }
            catch (Exception ex)
            {
                tcs.SetException(ex);
            }
        }
        public MasterViewModel DataModel { get;}
        static BaobaoGameController _instance;
        public static BaobaoGameController Instance => _instance?? (_instance = new BaobaoGameController());
    }
}