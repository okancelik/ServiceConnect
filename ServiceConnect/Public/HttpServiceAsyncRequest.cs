using System;
using System.IO;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using ServiceConnect.Object;

// ReSharper disable once CheckNamespace
namespace ServiceConnect
{
    public class HttpServiceAsyncRequest
    {
        // ReSharper disable once FieldCanBeMadeReadOnly.Local
        private string _userName;
        // ReSharper disable once FieldCanBeMadeReadOnly.Local
        private string _password;
        // ReSharper disable once FieldCanBeMadeReadOnly.Local
        private string _tokenUrl;
        // ReSharper disable once FieldCanBeMadeReadOnly.Local
        private string _tokenString = String.Empty;

        // ReSharper disable once FieldCanBeMadeReadOnly.Local
        private bool _isTokenBased;
        // ReSharper disable once FieldCanBeMadeReadOnly.Local
        private bool _isAuthorization = true;

        /// <summary>
        /// Not Authentication.
        /// </summary>
        public HttpServiceAsyncRequest()
        {
            _isAuthorization = false;
        }

        /// <summary>
        /// Basic Authentication.
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="password"></param>
        public HttpServiceAsyncRequest(string userName, string password)
        {
            _userName = userName;
            _password = password;
        }

        /// <summary>
        /// Token Based Authentication.
        /// </summary>
        /// <param name="token"></param>
        public HttpServiceAsyncRequest(string token)
        {
            _tokenString = token;
            _isTokenBased = true;
        }

        /// <summary>
        /// Token Based Authentication.
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="password"></param>
        /// <param name="tokenUrl"></param>
        public HttpServiceAsyncRequest(string userName, string password, string tokenUrl)
        {
            _userName = userName;
            _password = password;
            _tokenUrl = tokenUrl;

            _isTokenBased = true;
        }

        /// <summary>
        /// Token oluşturur.
        /// </summary>
        /// <returns></returns>
        private string GetToken()
        {
            if (_tokenString == String.Empty)
                return TokenManager.GetToken(_userName, _password, _tokenUrl);
            else
                return _tokenString;
        }


        /// <summary>
        /// Basic Kod üretir.
        /// </summary>
        /// <returns></returns>
        private string GetBasic()
        {
            String encoded = Convert.ToBase64String(Encoding.GetEncoding("ISO-8859-1").GetBytes(_userName + ":" + _password));
            return "Basic " + encoded;
        }

        /// <summary>
        /// Parametre olarak gönderilen Url adresine Http Get isteğinde bulunur.
        /// Geriye istenilen obje türünde veri döndürür.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="requestUrl"></param>
        /// <param name="timeOut"></param>
        /// <param name="contentType"></param>
        /// <returns></returns>
        // ReSharper disable once InconsistentNaming
        public async Task<T> GET<T>(string requestUrl, int timeOut = 0, string contentType = "application/json")
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(requestUrl);
            request.Method = WebRequestMethods.Http.Get;
            request.ContentType = contentType;

            if (timeOut > 0)
                request.Timeout = timeOut;

            if (_isAuthorization)
            {
                request.Headers.Add("Authorization", !_isTokenBased ? GetBasic() : GetToken());
            }

            WebResponse  response = await request.GetResponseAsync();
            // ReSharper disable once AssignNullToNotNullAttribute
            StreamReader reader =  new StreamReader(response.GetResponseStream());
            string jsonData = reader.ReadToEnd();
            var obj = ResultConvert.Deserialize<T>(jsonData);
            reader.Close();
            response.Close();

            return obj;
        }


        /// <summary>
        /// Parametre olarak gönderilen Url adresine Http Get isteğinde bulunur.
        /// Geriye Json formatında String Döndürür.
        /// </summary>
        /// <param name="requestUrl"></param>
        /// <param name="timeOut"></param>
        /// <param name="contentType"></param>
        /// <returns></returns>
        // ReSharper disable once InconsistentNaming
        public async Task<string> GET(string requestUrl, int timeOut = 0, string contentType = "application/json")
        {

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(requestUrl);
            request.Method = WebRequestMethods.Http.Get;
            request.Accept = contentType;

            if (timeOut > 0)
                request.Timeout = timeOut;

            if (_isAuthorization)
            {
                request.Headers.Add("Authorization", !_isTokenBased ? GetBasic() : GetToken());
            }

            WebResponse response = await request.GetResponseAsync();
            // ReSharper disable once AssignNullToNotNullAttribute
            StreamReader reader = new StreamReader(response.GetResponseStream());
            string jsonData = reader.ReadToEnd();
            reader.Close();
            response.Close();

            return jsonData;
        }


        /// <summary>
        /// Parametre olarak gönderilen Url adresine parametre olarak gönderilen objeyi json verisine çevirip Post eder.
        /// Geriye istenilen obje türünde veri döndürür.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="requestUrl"></param>
        /// <param name="responseObject"></param>
        /// <param name="timeOut"></param>
        /// <param name="contentType"></param>
        /// <returns></returns>
        // ReSharper disable once InconsistentNaming
        public async Task<T> POST<T>(string requestUrl, object responseObject, int timeOut = 0, string contentType = null)
        {
            string postData = ResultConvert.Serialize(responseObject);
            byte[] data = Encoding.UTF8.GetBytes(postData);

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(requestUrl);
            request.Method = WebRequestMethods.Http.Post;
            request.ContentType = contentType ?? "application/json";

            if (timeOut > 0)
                request.Timeout = timeOut;

            if (_isAuthorization)
            {
                request.Headers.Add("Authorization", !_isTokenBased ? GetBasic() : GetToken());
            }

            request.ContentLength = data.Length;

            using (Stream newStream = request.GetRequestStream())
            {
                newStream.Write(data, 0, data.Length);
                newStream.Flush();
                newStream.Close();
            }

            WebResponse  response = await request.GetResponseAsync();
            // ReSharper disable once AssignNullToNotNullAttribute
            StreamReader reader = new StreamReader(response.GetResponseStream());
            string jsonData = reader.ReadToEnd();
            var obj = ResultConvert.Deserialize<T>(jsonData);
            reader.Close();
            response.Close();

            return obj;
        }

        /// <summary>
        /// Parametre olarak gönderilen Url adresine parametre olarak gönderilen objeyi json verisine çevirip Post eder.
        /// Geriye istenilen obje türünde veri döndürür.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="requestUrl"></param>
        /// <param name="responseObject"></param>
        /// <param name="timeOut"></param>
        /// <param name="contentType"></param>
        /// <returns></returns>
        // ReSharper disable once InconsistentNaming
        public async Task<T> POST<T>(string requestUrl, string responseObject, int timeOut = 0, string contentType = null)
        {
            byte[] data = Encoding.UTF8.GetBytes(responseObject);

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(requestUrl);
            request.Method = WebRequestMethods.Http.Post;
            request.ContentType = contentType ?? "application/json";

            if (timeOut > 0)
                request.Timeout = timeOut;

            if (_isAuthorization)
            {
                request.Headers.Add("Authorization", !_isTokenBased ? GetBasic() : GetToken());
            }

            request.ContentLength = data.Length;

            using (Stream newStream = request.GetRequestStream())
            {
                newStream.Write(data, 0, data.Length);
                newStream.Flush();
                newStream.Close();
            }

            WebResponse  response = await request.GetResponseAsync();
            // ReSharper disable once AssignNullToNotNullAttribute
            StreamReader reader = new StreamReader(response.GetResponseStream());
            string jsonData = reader.ReadToEnd();
            var obj = ResultConvert.Deserialize<T>(jsonData);
            reader.Close();
            response.Close();

            return obj;
        }

        /// <summary>
        /// Parametre olarak gönderilen Url adresine parametre olarak gönderilen objeyi json verisine çevirip Post eder.
        /// Geriye Json formatında String Döndürür.
        /// </summary>
        /// <param name="requestUrl"></param>
        /// <param name="responseObject"></param>
        /// <param name="timeOut"></param>
        /// <param name="contentType"></param>
        /// <returns></returns>
        // ReSharper disable once InconsistentNaming
        public async Task<string> POST(string requestUrl, object responseObject, int timeOut = 0, string contentType = null)
        {
            string postData = ResultConvert.Serialize(responseObject);
            byte[] data = Encoding.UTF8.GetBytes(postData);

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(requestUrl);
            request.Method = WebRequestMethods.Http.Post;
            request.ContentType = contentType ?? "application/json";

            if (timeOut > 0)
                request.Timeout = timeOut;

            if (_isAuthorization)
            {
                request.Headers.Add("Authorization", !_isTokenBased ? GetBasic() : GetToken());
            }

            request.ContentLength = data.Length;

            using (Stream newStream = request.GetRequestStream())
            {
                newStream.Write(data, 0, data.Length);
                newStream.Flush();
                newStream.Close();
            }

            WebResponse  response = await request.GetResponseAsync();
            // ReSharper disable once AssignNullToNotNullAttribute
            StreamReader reader = new StreamReader(response.GetResponseStream());
            string jsonData = reader.ReadToEnd();
            reader.Close();
            response.Close();

            return jsonData;
        }

        /// <summary>
        /// Parametre olarak gönderilen Url adresine parametre olarak gönderilen objeyi json verisine çevirip Post eder.
        /// Geriye Json formatında String Döndürür.
        /// </summary>
        /// <param name="requestUrl"></param>
        /// <param name="responseObject"></param>
        /// <param name="timeOut"></param>
        /// <param name="contentType"></param>
        /// <returns></returns>
        // ReSharper disable once InconsistentNaming
        public async Task<string> POST(string requestUrl, string responseObject, int timeOut = 0, string contentType = null)
        {
            byte[] data = Encoding.UTF8.GetBytes(responseObject);

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(requestUrl);
            request.Method = WebRequestMethods.Http.Post;
            request.ContentType = contentType ?? "application/json";

            if (timeOut > 0)
                request.Timeout = timeOut;

            if (_isAuthorization)
            {
                request.Headers.Add("Authorization", !_isTokenBased ? GetBasic() : GetToken());
            }

            request.ContentLength = data.Length;

            using (Stream newStream = request.GetRequestStream())
            {
                newStream.Write(data, 0, data.Length);
                newStream.Flush();
                newStream.Close();
            }

            WebResponse  response = await request.GetResponseAsync();
            // ReSharper disable once AssignNullToNotNullAttribute
            StreamReader reader = new StreamReader(response.GetResponseStream());
            string jsonData = reader.ReadToEnd();
            reader.Close();
            response.Close();

            return jsonData;
        }

        /// <summary>
        /// Parametre olarak gönderilen Url adresine parametre olarak gönderilen objeyi json verisine çevirip Put eder. 
        /// Güncellemede kullanılır.
        /// Geriye istenilen obje türünde veri döndürür.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="requestUrl"></param>
        /// <param name="responseObject"></param>
        /// <param name="timeOut"></param>
        /// <param name="contentType"></param>
        /// <returns></returns>
        // ReSharper disable once InconsistentNaming
        public async Task<T> PUT<T>(string requestUrl, object responseObject, int timeOut = 0, string contentType = null)
        {
            string postData = ResultConvert.Serialize(responseObject);
            byte[] data = Encoding.UTF8.GetBytes(postData);

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(requestUrl);
            request.Method = WebRequestMethods.Http.Put;
            request.ContentType = contentType ?? "application/json";

            if (timeOut > 0)
                request.Timeout = timeOut;

            if (_isAuthorization)
            {
                request.Headers.Add("Authorization", !_isTokenBased ? GetBasic() : GetToken());
            }

            request.ContentLength = data.Length;

            using (Stream newStream = request.GetRequestStream())
            {
                newStream.Write(data, 0, data.Length);
                newStream.Flush();
                newStream.Close();
            }

            WebResponse  response = await request.GetResponseAsync();
            // ReSharper disable once AssignNullToNotNullAttribute
            StreamReader reader = new StreamReader(response.GetResponseStream());
            string jsonData = reader.ReadToEnd();
            var obj = ResultConvert.Deserialize<T>(jsonData);
            reader.Close();
            response.Close();

            return obj;
        }

        /// <summary>
        /// Parametre olarak gönderilen Url adresine parametre olarak gönderilen objeyi json verisine çevirip Put eder.
        /// Güncellemede kullanılır.
        /// Geriye Json formatında String Döndürür.
        /// </summary>
        /// <param name="requestUrl"></param>
        /// <param name="responseObject"></param>
        /// <param name="timeOut"></param>
        /// <param name="contentType"></param>
        /// <returns></returns>
        // ReSharper disable once InconsistentNaming
        public async Task<string> PUT(string requestUrl, object responseObject, int timeOut = 0, string contentType = null)
        {
            string postData = ResultConvert.Serialize(responseObject);
            byte[] data = Encoding.UTF8.GetBytes(postData);

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(requestUrl);
            request.Method = WebRequestMethods.Http.Put;
            request.ContentType = contentType ?? "application/json";

            if (timeOut > 0)
                request.Timeout = timeOut;

            if (_isAuthorization)
            {
                request.Headers.Add("Authorization", !_isTokenBased ? GetBasic() : GetToken());
            }

            request.ContentLength = data.Length;

            using (Stream newStream = request.GetRequestStream())
            {
                newStream.Write(data, 0, data.Length);
                newStream.Flush();
                newStream.Close();
            }

            WebResponse  response = await request.GetResponseAsync();
            // ReSharper disable once AssignNullToNotNullAttribute
            StreamReader reader = new StreamReader(response.GetResponseStream());
            string jsonData = reader.ReadToEnd();
            reader.Close();
            response.Close();

            return jsonData;
        }

        /// <summary>
        /// Parametre olarak gönderilen Url adresine parametre olarak gönderilen objeyi json verisine çevirip Put eder. 
        /// Güncellemede kullanılır.
        /// Geriye istenilen obje türünde veri döndürür.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="requestUrl"></param>
        /// <param name="responseObject"></param>
        /// <param name="timeOut"></param>
        /// <param name="contentType"></param>
        /// <returns></returns>
        // ReSharper disable once InconsistentNaming
        public async Task<T> PUT<T>(string requestUrl, string responseObject, int timeOut = 0, string contentType = null)
        {
            string postData = ResultConvert.Serialize(responseObject);
            byte[] data = Encoding.UTF8.GetBytes(postData);

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(requestUrl);
            request.Method = WebRequestMethods.Http.Put;
            request.ContentType = contentType ?? "application/json";

            if (timeOut > 0)
                request.Timeout = timeOut;

            if (_isAuthorization)
            {
                request.Headers.Add("Authorization", !_isTokenBased ? GetBasic() : GetToken());
            }

            request.ContentLength = data.Length;

            using (Stream newStream = request.GetRequestStream())
            {
                newStream.Write(data, 0, data.Length);
                newStream.Flush();
                newStream.Close();
            }

            WebResponse  response = await request.GetResponseAsync();
            // ReSharper disable once AssignNullToNotNullAttribute
            StreamReader reader = new StreamReader(response.GetResponseStream());
            string jsonData = reader.ReadToEnd();
            var obj = ResultConvert.Deserialize<T>(jsonData);
            reader.Close();
            response.Close();

            return obj;
        }

        /// <summary>
        /// Parametre olarak gönderilen Url adresine parametre olarak gönderilen objeyi json verisine çevirip Put eder.
        /// Güncellemede kullanılır.
        /// Geriye Json formatında String Döndürür.
        /// </summary>
        /// <param name="requestUrl"></param>
        /// <param name="responseObject"></param>
        /// <param name="timeOut"></param>
        /// <param name="contentType"></param>
        /// <returns></returns>
        // ReSharper disable once InconsistentNaming
        public async Task<string> PUT(string requestUrl, string responseObject, int timeOut = 0, string contentType = null)
        {
            byte[] data = Encoding.UTF8.GetBytes(responseObject);

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(requestUrl);
            request.Method = WebRequestMethods.Http.Put;
            request.ContentType = contentType ?? "application/json";

            if (timeOut > 0)
                request.Timeout = timeOut;

            if (_isAuthorization)
            {
                request.Headers.Add("Authorization", !_isTokenBased ? GetBasic() : GetToken());
            }

            request.ContentLength = data.Length;

            using (Stream newStream = request.GetRequestStream())
            {
                newStream.Write(data, 0, data.Length);
                newStream.Flush();
                newStream.Close();
            }

            WebResponse  response = await request.GetResponseAsync();
            // ReSharper disable once AssignNullToNotNullAttribute
            StreamReader reader = new StreamReader(response.GetResponseStream());
            string jsonData = reader.ReadToEnd();
            reader.Close();
            response.Close();

            return jsonData;
        }

        /// <summary>
        /// Parametre olarak gönderilen Url adresine Http Delete isteğinde bulunur.
        /// Geriye istenilen obje türünde veri döndürür.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="requestUrl"></param>
        /// <param name="timeOut"></param>
        /// <param name="contentType"></param>
        /// <returns></returns>
        // ReSharper disable once InconsistentNaming
        public async Task<T> DELETE<T>(string requestUrl, int timeOut = 0, string contentType = null)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(requestUrl);
            request.Method = "DELETE";
            request.ContentType = contentType ?? "application/json";

            if (timeOut > 0)
                request.Timeout = timeOut;

            if (_isAuthorization)
            {
                request.Headers.Add("Authorization", !_isTokenBased ? GetBasic() : GetToken());
            }

            WebResponse  response = await request.GetResponseAsync();
            // ReSharper disable once AssignNullToNotNullAttribute
            StreamReader reader = new StreamReader(response.GetResponseStream());
            string jsonData = reader.ReadToEnd();
            var obj = ResultConvert.Deserialize<T>(jsonData);
            reader.Close();
            response.Close();

            return obj;
        }

        /// <summary>
        /// Parametre olarak gönderilen Url adresine Http Delete isteğinde bulunur.
        /// Geriye Json formatında String Döndürür.
        /// </summary>
        /// <param name="requestUrl"></param>
        /// <param name="timeOut"></param>
        /// <param name="contentType"></param>
        /// <returns></returns>
        // ReSharper disable once InconsistentNaming
        public async Task<string> DELETE(string requestUrl, int timeOut = 0, string contentType = null)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(requestUrl);
            request.Method = "DELETE";
            request.ContentType = contentType ?? "application/json";

            if (timeOut > 0)
                request.Timeout = timeOut;

            if (_isAuthorization)
            {
                request.Headers.Add("Authorization", !_isTokenBased ? GetBasic() : GetToken());
            }

            WebResponse  response = await request.GetResponseAsync();
            // ReSharper disable once AssignNullToNotNullAttribute
            StreamReader reader = new StreamReader(response.GetResponseStream());
            string jsonData = reader.ReadToEnd();
            reader.Close();
            response.Close();

            return jsonData;
        }

        /// <summary>
        /// Manuel basit istekte Bulunur.
        /// </summary>
        /// <param name="requestUrl"></param>
        /// <param name="obj"></param>
        /// <returns></returns>
        // ReSharper disable once InconsistentNaming
        public async Task<string> REQUEST(string requestUrl, Request obj)
        {
            if (obj == null)
                obj = new Request();

            byte[] data = null;

            if (obj.Body != null)
                data = Encoding.UTF8.GetBytes(obj.Body);

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(requestUrl);

            if (obj.Method != null)
                request.Method = obj.Method;
            if (obj.ContentType != null)
                request.ContentType = obj.ContentType;
            else
                request.ContentType = "application/json";
            if (obj.Authorization != null)
                request.Headers.Add("Authorization", obj.Authorization);
            if (obj.Headers != null)
            {
                foreach (var item in obj.Headers)
                {
                    request.Headers.Add(item);
                }
            }

            if (obj.TimeOut > 0)
                request.Timeout = obj.TimeOut;

            if (data != null)
            {
                request.ContentLength = data.Length;
                using (Stream newStream = request.GetRequestStream())
                {
                    newStream.Write(data, 0, data.Length);
                    newStream.Flush();
                    newStream.Close();
                }
            }

            WebResponse  response = await request.GetResponseAsync();
            // ReSharper disable once AssignNullToNotNullAttribute
            StreamReader reader = new StreamReader(response.GetResponseStream());
            string jsonData = reader.ReadToEnd();
            reader.Close();
            response.Close();

            return jsonData;
        }

        /// <summary>
        /// Manuel basit istekte Bulunur.
        /// istenilen türde obje döndürür.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="requestUrl"></param>
        /// <param name="obj"></param>
        /// <returns></returns>
        // ReSharper disable once InconsistentNaming
        public async Task<T> REQUEST<T>(string requestUrl, Request obj)
        {
            if (obj == null)
                obj = new Request();

            byte[] data = null;

            if (obj.Body != null)
                data = Encoding.UTF8.GetBytes(obj.Body);

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(requestUrl);

            if (obj.Method != null)
                request.Method = obj.Method;
            if (obj.ContentType != null)
                request.ContentType = obj.ContentType;
            else
                request.ContentType = "application/json";
            if (obj.Authorization != null)
                request.Headers.Add("Authorization", obj.Authorization);
            if (obj.Headers != null)
            {
                foreach (var item in obj.Headers)
                {
                    request.Headers.Add(item);
                }
            }

            if (obj.TimeOut > 0)
                request.Timeout = obj.TimeOut;

            if (data != null)
            {
                request.ContentLength = data.Length;
                using (Stream newStream = request.GetRequestStream())
                {
                    newStream.Write(data, 0, data.Length);
                    newStream.Flush();
                    newStream.Close();
                }
            }

            WebResponse  response = await request.GetResponseAsync();
            // ReSharper disable once AssignNullToNotNullAttribute
            StreamReader reader = new StreamReader(response.GetResponseStream());
            string jsonData = reader.ReadToEnd();
            var responseObject = ResultConvert.Deserialize<T>(jsonData);
            reader.Close();
            response.Close();

            return responseObject;
        }
    }
}
