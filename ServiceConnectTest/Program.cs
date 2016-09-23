using System;
using ServiceConnect;
using ServiceConnect.Object;


namespace ServiceConnectTest
{
    class Program
    {

        static void Main(string[] args)
        {
           // FillData();                  // Default 
            FillDataAsync();             // Async
            FillDataBasic();             // Basic Authentication
            FillDataAsyncBasic();        // Basic Authentication Async
            FillDataToken();             // Token Authentication
            FillDataAsyncToken();        // Token Authentication Async
            FillDataTokenManuel();       // Token Authentication Manuel & REQUEST Method
            FillDataAsyncTokenManuel();  // Token Authentication Async Manuel & Async REQUEST Method
        }


        private static void FillData()
        {
            HttpServiceRequest request = new HttpServiceRequest();
            string json = "{\"ID\":8,\"ProductName\":\"SSSSSSSSSSSSS\",\"Description\":\"YYYYYYYYYYY\",\"CategoryId\":1}";
            object response;


            // GET
            response = request.GET("http://192.168.1.74/ExampleApiNS/api/Products/Gets");

            // POST
            response = request.POST("http://192.168.1.74/ExampleApiNS/api/Products/Create", json);

            // PUT
            response = request.PUT("http://192.168.1.74/ExampleApiNS/api/Products/Update", json);

            // DELETE
            response = request.DELETE("http://192.168.1.74/ExampleApiNS/api/Products/Delete/8");
        }


        private async static void FillDataAsync()
        {
            HttpServiceAsyncRequest asyncRequest = new HttpServiceAsyncRequest();
            string json = "{\"ID\":8,\"ProductName\":\"PPPPPPPPPPPPPPPP\",\"Description\":\"YYYYYYYYYYY\",\"CategoryId\":1}";
            object response;

            // GET
            response = await asyncRequest.GET("http://192.168.1.74/ExampleApiNS/api/Products/Gets");

            // POST
            response = await asyncRequest.POST("http://192.168.1.74/ExampleApiNS/api/Products/Create", json);

            // PUT
            response = await asyncRequest.PUT("http://192.168.1.74/ExampleApiNS/api/Products/Update", json);

            // DELETE
            response = await asyncRequest.DELETE("http://192.168.1.74/ExampleApiNS/api/Products/Delete/7");
        }

        private static void FillDataBasic()
        {
            HttpServiceRequest request = new HttpServiceRequest("test", "1234");
            string json = "{\"ID\":7,\"ProductName\":\"XXXXXXXXXXX\",\"Description\":\"YYYYYYYYYYY\",\"CategoryId\":1}";
            object response;

            // GET
            response = request.GET("http://192.168.1.79/ExampleApis/api/Products/Gets");

            // POST
            response = request.POST("http://192.168.1.79/ExampleApis/api/Products/Create", json);

            // PUT
            response = request.PUT("http://192.168.1.79/ExampleApis/api/Products/Update", json);

            // DELETE
            response = request.DELETE("http://192.168.1.79/ExampleApis/api/Products/Delete/5");
        }

        private async static void FillDataAsyncBasic()
        {
            HttpServiceAsyncRequest asyncRequest = new HttpServiceAsyncRequest("test", "1234");
            string json = "{\"ID\":7,\"ProductName\":\"XXXXXXXXXXX\",\"Description\":\"YYYYYYYYYYY\",\"CategoryId\":1}";
            object response;

            // GET
            response = await asyncRequest.GET("http://192.168.1.79/ExampleApis/api/Products/Gets");

            // POST
            response = await asyncRequest.POST("http://192.168.1.79/ExampleApis/api/Products/Create", json);

            // PUT
            response = await asyncRequest.PUT("http://192.168.1.79/ExampleApis/api/Products/Update", json);

            // DELETE
            response = await asyncRequest.DELETE("http://192.168.1.79/ExampleApis/api/Products/Delete/5");
        }

        private static void FillDataToken()
        {
            HttpServiceRequest request = new HttpServiceRequest("test", "1234", "http://192.168.1.79/ExampleApiTokenBased/Token");
            string json = "{\"ID\":7,\"ProductName\":\"XXXXXXXXXXX\",\"Description\":\"YYYYYYYYYYY\",\"CategoryId\":1}";
            object response;

            // GET
            response = request.GET("http://192.168.1.79/ExampleApiTokenBased/api/Product/Gets");

            // POST
            response = request.POST("http://192.168.1.79/ExampleApiTokenBased/api/Product/Create", json);

            // PUT
            response = request.PUT("http://192.168.1.79/ExampleApiTokenBased/api/Product/Update", json);

            // DELETE
            response = request.DELETE("http://192.168.1.79/ExampleApiTokenBased/api/Product/Delete/5");
        }


        private static async void FillDataAsyncToken()
        {
            HttpServiceAsyncRequest asyncRequest = new HttpServiceAsyncRequest("test", "1234", "http://192.168.1.79/ExampleApiTokenBased/Token");
            string json = "{\"ID\":7,\"ProductName\":\"XXXXXXXXXXX\",\"Description\":\"YYYYYYYYYYY\",\"CategoryId\":1}";
            object response;

            // GET
            response = await asyncRequest.GET("http://192.168.1.79/ExampleApiTokenBased/api/Product/Gets");

            // POST
            response = await asyncRequest.POST("http://192.168.1.79/ExampleApiTokenBased/api/Product/Create", json);

            // PUT
            response = await asyncRequest.PUT("http://192.168.1.79/ExampleApiTokenBased/api/Product/Update", json);

            // DELETE
            response = await asyncRequest.DELETE("http://192.168.1.79/ExampleApiTokenBased/api/Product/Delete/5");
        }




        private static void FillDataTokenManuel()
        {
            HttpServiceRequest tokenRequest = new HttpServiceRequest();
            Request requestObject = new Request();

            requestObject.Method = "POST";
            requestObject.Content_Type = "application/x-www-form-urlencoded";
            requestObject.Body = "grant_type=password&username=test&password=1234";

            //REQUEST
            string token = tokenRequest.REQUEST("http://192.168.1.79/ExampleApiTokenBased/Token", requestObject);
            token = ConvertJsonToToken(token);

            HttpServiceRequest request = new HttpServiceRequest(token);
            string json = "{\"ID\":7,\"ProductName\":\"XXXXXXXXXXX\",\"Description\":\"YYYYYYYYYYY\",\"CategoryId\":1}";
            object response;

            // GET
            response = request.GET("http://192.168.1.79/ExampleApiTokenBased/api/Product/Gets");

            // POST
            response = request.POST("http://192.168.1.79/ExampleApiTokenBased/api/Product/Create", json);

            // PUT
            response = request.PUT("http://192.168.1.79/ExampleApiTokenBased/api/Product/Update", json);

            // DELETE
            response = request.DELETE("http://192.168.1.79/ExampleApiTokenBased/api/Product/Delete/5");
        }

        private static async void FillDataAsyncTokenManuel()
        {
            HttpServiceAsyncRequest tokenRequest = new HttpServiceAsyncRequest();
            Request requestObject = new Request();

            requestObject.Method = "POST";
            requestObject.Content_Type = "application/x-www-form-urlencoded";
            requestObject.Body = "grant_type=password&username=test&password=1234";

            //REQUEST
            string token = await tokenRequest.REQUEST("http://192.168.1.79/ExampleApiTokenBased/Token", requestObject);
            token = ConvertJsonToToken(token);

            HttpServiceAsyncRequest request = new HttpServiceAsyncRequest(token);
            string json = "{\"ID\":7,\"ProductName\":\"XXXXXXXXXXX\",\"Description\":\"YYYYYYYYYYY\",\"CategoryId\":1}";
            object response;

            // GET
            response = await request.GET("http://192.168.1.79/ExampleApiTokenBased/api/Product/Gets");

            // POST
            response = await request.POST("http://192.168.1.79/ExampleApiTokenBased/api/Product/Create", json);

            // PUT
            response = await request.PUT("http://192.168.1.79/ExampleApiTokenBased/api/Product/Update", json);

            // DELETE
            response = await request.DELETE("http://192.168.1.79/ExampleApiTokenBased/api/Product/Delete/5");
        }

        private static string ConvertJsonToToken(string strJson)
        {
            strJson = strJson.Replace('{', ' ').Trim().Replace('}', ' ').Trim().Replace('\"', ' ').Trim();
            string[] arrayStrJson = strJson.Split(',');
            string token = String.Empty;
            string tokenType = String.Empty;
            foreach (var item in arrayStrJson)
            {
                if (item.Split(':')[0].Trim() == "access_token")
                    token = item.Trim().Split(':')[1];
                if (item.Split(':')[0].Trim() == "token_type")
                    tokenType = item.Trim().Split(':')[1];
            }
            return tokenType + " " + token;
        }
    }

}

