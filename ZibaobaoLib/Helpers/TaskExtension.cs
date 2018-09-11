using System;
using System.Threading.Tasks;

namespace ZibaobaoLib.Helpers
{
    public static class TaskExtension
    {
        public static async void RunForget(this Task task)
        {
            try
            {
                await task;
            }
            catch (Exception e)
            {
                X1LogHelper.Exception(e);
            }
        }

        public static void Forget(this Task task)
        {
        }
    }

}