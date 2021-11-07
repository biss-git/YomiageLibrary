using System;
using System.Net;
using System.Threading.Tasks;

namespace Yomiage.API
{
    public static class ApiServer
    {
        private static HttpListener listener;
        private static readonly ControllerMapper mapper = new ControllerMapper();



        /// <summary>
        /// APIサービスを起動する
        /// </summary>
        public static void Start(int portNumber)
        {
            if (listener?.IsListening == true) { return; }
            portNumber = Math.Clamp(portNumber, 10000, 60000);
            for (int i = portNumber; i < portNumber + 100; i++)
            {

                listener = new HttpListener();
                try
                {
                    ServerInfo.ApiPort = i;
                    string rootUrl = String.Format("http://localhost:{0}/{1}/", ServerInfo.ApiPort, ServerInfo.ApiPath);

                    listener.Prefixes.Clear();
                    listener.Prefixes.Add(rootUrl);
                    listener.Start();
                    break;
                }
                catch (Exception)
                {
                }
            }

            Task.Run(Tick);
        }

        public static void Stop()
        {
            try
            {
                listener?.Stop();
            }
            catch (Exception)
            {
            }
        }

        private static void Tick()
        {
            while (listener?.IsListening == true)
            {
                try
                {
                    IAsyncResult result = listener.BeginGetContext(OnRequested, listener);
                    result.AsyncWaitHandle.WaitOne();
                }
                catch (Exception)
                {
                }
            }
        }



        /// <summary>
        /// リクエスト時の処理を実行する
        /// </summary>
        /// <param name="result">結果</param>
        private static async void OnRequested(IAsyncResult result)
        {
            HttpListener clsListener = (HttpListener)result.AsyncState;
            if (!clsListener.IsListening)
            {
                return;
            }

            HttpListenerContext context = clsListener.EndGetContext(result);
            HttpListenerRequest req = context.Request;
            HttpListenerResponse res = context.Response;
            res.AddHeader("Access-Control-Allow-Origin", "*"); // どこからのリクエストでも受け取る
            res.AddHeader("Access-Control-Allow-Headers", "*"); // どこからのリクエストでも受け取る
            res.AddHeader("Access-Control-Allow-Methods", "*"); // どこからのリクエストでも受け取る
            try
            {
                await mapper.Execute(req, res);
            }
            catch (Exception)
            {
                // log.Error(ex.ToString());
            }
            finally
            {
                try
                {
                    if (null != res)
                    {
                        res.Close();
                    }
                }
                catch (Exception)
                {
                    // log.Error(clsEx.ToString());
                }
            }
        }


    }
}
