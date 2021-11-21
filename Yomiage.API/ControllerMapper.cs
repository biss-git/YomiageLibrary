using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Net;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Yomiage.API
{
    class ControllerMapper
    {
        private const string CONTENT_TYPE_JSON = "application/json";

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public ControllerMapper()
        {
        }

        /// <summary>
        /// 実行する
        /// </summary>
        /// <param name="req">リクエスト情報</param>
        /// <param name="res">レスポンス情報</param>
        public async Task Execute(HttpListenerRequest req, HttpListenerResponse res)
        {
            StreamReader reader = null;
            StreamWriter writer = null;
            string resBoby = null;

            try
            {
                res.StatusCode = (int)HttpStatusCode.OK;
                res.ContentType = CONTENT_TYPE_JSON;
                res.ContentEncoding = Encoding.UTF8;

                reader = new StreamReader(req.InputStream);
                writer = new StreamWriter(res.OutputStream);
                string reqBody = reader.ReadToEnd();

                resBoby = await ExecuteController(req, res, reqBody);
            }
            catch (Exception)
            {
                /* ～エラー処理～ */
            }
            finally
            {
                try
                {
                    writer.Write(resBoby);
                    writer.Flush();

                    if (null != reader)
                    {
                        reader.Close();
                    }
                    if (null != writer)
                    {
                        writer.Close();
                    }
                }
                catch (Exception)
                {
                    // log.Error(clsEx.ToString());
                }
            }
        }

        /// <summary>
        /// APIコントローラを実行する
        /// </summary>
        /// <param name="req">リクエスト情報</param>
        /// <param name="res">レスポンス情報</param>
        /// <param name="reqBody">リクエストボディ</param>
        /// <returns>レスポンス文字列</returns>
        private async Task<string> ExecuteController(HttpListenerRequest req, HttpListenerResponse res, string reqBody)
        {
            string path = ServerInfo.GetApiPath(req.RawUrl);

            if (path.StartsWith("/command"))
            {
                res.StatusCode = 500;
                // return await VoiceroidProcess.Execute(req, res, reqBody);

                switch (req.HttpMethod)
                {
                    case "GET":
                        res.StatusCode = 405;
                        return "";
                    case "POST":
                        try
                        {
                            var command = JsonSerializer.Deserialize<UnicoeCommand>(reqBody);
                            if (command.command == "play" &&
                                CommandService.PlayVoice != null)
                            {
                                command.success = CommandService.PlayVoice(command.TalkText);
                                if (command.success == true)
                                {
                                    res.StatusCode = 200;
                                }
                                return JsonSerializer.Serialize(command);
                            }
                            if (command.command == "stop" &&
                                CommandService.StopAction != null)
                            {
                                CommandService.StopAction();
                                await Task.Delay(100); // ちょっと待たないと上手く再生できないときがある。
                                command.success = true;
                                res.StatusCode = 200;
                                return JsonSerializer.Serialize(command);
                            }
                        }
                        catch (Exception)
                        {

                        }
                        return "";
                    case "OPTIONS":
                        res.StatusCode = 200;
                        return "";
                    default:
                        res.StatusCode = 405;
                        return "";
                }

            }
            else if (path.StartsWith("/server"))
            {
                return ServerInfo.Execute(req, res, reqBody);
            }
            else
            {
                res.StatusCode = 404;
            }
            return "";
        }


    }

}
