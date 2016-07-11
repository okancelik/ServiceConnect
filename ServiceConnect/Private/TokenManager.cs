using System;
using System.IO;
using System.Net;
using System.Text;

namespace ServiceConnect
{
    static class TokenManager
    {
        private static  int  Count;
        private static  Token obj;

        /// <summary>
        /// Token Alır.
        /// </summary>
        /// <returns></returns>
        public static string GetToken(string userName,string password,string tokenUrl)
        {
            if (Count <= 0)
            {
                byte[] data = Encoding.UTF8.GetBytes("grant_type=password&username=" + userName + "&password=" + password);
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(tokenUrl);
                request.Method = WebRequestMethods.Http.Post;
                request.ContentType = "application/json";
                request.ContentLength = data.Length;

                using (Stream newStream = request.GetRequestStream())
                {
                    newStream.Write(data, 0, data.Length);
                    newStream.Flush();
                    newStream.Close();
                }

                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                StreamReader reader = new StreamReader(response.GetResponseStream());
                string jsonData = reader.ReadToEnd();

                obj = new Token();
                obj = ConvertStringToToken(jsonData);

                reader.Close();
                response.Close();

                Count = Convert.ToInt32(obj.expires_in);
            }
            Count--;
            return obj.token_type + " " + obj.access_token;
        }

        /// <summary>
        /// String olarak gelen json formatındaki veriyi Token Objesine çevirir.
        /// </summary>
        /// <param name="strToken"></param>
        /// <returns></returns>
        public static Token ConvertStringToToken(string strToken)
        {
            Token tk = new Token();
            string[] baseToken = strToken.Replace('{',  ' ').Trim().
                                          Replace('}',  ' ').Trim().
                                          Replace('\"', ' ').Trim().Split(',');
            foreach (var item in baseToken)
            {
                if (item.Split(':')[0].Trim()      == "access_token")
                {
                    tk.access_token = item.Split(':')[1];
                }
                else if (item.Split(':')[0].Trim() == "token_type")
                {
                    tk.token_type   = item.Split(':')[1];
                }
                else if (item.Split(':')[0].Trim() == "expires_in")
                {
                    tk.expires_in   = item.Split(':')[1];
                }
            }
            return tk;
        }

        public static void TokenClear()
        {
            Count = 0;
            obj = new Token();
        }
    }
}
