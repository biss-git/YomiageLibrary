using System;
using System.Collections.Generic;
using System.Net;
using System.Reflection;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;

namespace Yomiage.API
{
    public static class ServerInfo
    {

        public static int ApiPort = 42503;

        public static string ApiPath = "api";


        public static string Execute(HttpListenerRequest req, HttpListenerResponse res, string reqBody)
        {
            string path = GetApiPath(req.RawUrl);
            if (path != "/server")
            {
                res.StatusCode = 404;
                return "";
            }
            switch (req.HttpMethod)
            {
                case "GET":
                    {
                        Assembly assembly = Assembly.GetExecutingAssembly();
                        AssemblyName asmName = assembly.GetName();
                        Version version = asmName.Version;
                        var dic = new Dictionary<string, string>();
                        dic.Add("version", version.ToString());
                        return JsonSerializer.Serialize(dic);
                    }
                // case "POST":
                //     return "";// (new CreateUserController(req, res, reqBody)).Execute();
                default:
                    res.StatusCode = 405;
                    return "";
            }
        }

        /// <summary>
        /// APIパスを取得する
        /// </summary>
        /// <param name="srcPath">URLパス</param>
        /// <returns>APIパス</returns>
        public static string GetApiPath(string srcPath)
        {
            string[] path = srcPath.Split('?');
            string condition = String.Format(@"^/{0}", ApiPath);
            return Regex.Replace(path[0], condition, "");
        }
    }
}
