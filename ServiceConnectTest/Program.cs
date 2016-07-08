using System.Collections.Generic;
using ServiceConnect;
using ServiceConnectTest.Models;

namespace ServiceConnectTest
{
    class Program
    {

        static  void Main(string[] args)
        {
            FillData();
        }

        private static void FillData()
        {
            //HttpServiceRequest req = new HttpServiceRequest();

            //RequestObject obj = new RequestObject();

            //obj.Method = "POST";
            //obj.Content_Type = "application/x-www-form-urlencoded";
            //obj.Body = "grant_type=password&username=test&password=1234";

            //string token = req.REQUEST("http://192.168.1.79/ExampleApiTokenBased/Token", obj);

            //HttpServiceRequest requ = new HttpServiceRequest(ConvertStringToToken(token));

            //string response = requ.GET("http://192.168.1.79/ExampleApiTokenBased/api/Product/Gets");

        }

        public static string ConvertStringToToken(string strToken)
        {
            Token tk = new Token();
            string[] baseToken = strToken.Replace('{', ' ').Trim().
                                          Replace('}', ' ').Trim().
                                          Replace('\"', ' ').Trim().Split(',');
            foreach (var item in baseToken)
            {
                if (item.Split(':')[0].Trim() == "access_token")
                {
                    tk.access_token = item.Split(':')[1];
                }
                else if (item.Split(':')[0].Trim() == "token_type")
                {
                    tk.token_type = item.Split(':')[1];
                }
                else if (item.Split(':')[0].Trim() == "expires_in")
                {
                    tk.expires_in = item.Split(':')[1];
                }
            }
            return tk.token_type + " " + tk.access_token;
        }
    }

    class Token
    {
        public string access_token { get; set; }
        public string token_type { get; set; }
        public string expires_in { get; set; }
    }
}

