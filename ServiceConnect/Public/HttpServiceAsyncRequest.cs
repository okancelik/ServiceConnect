using System;
using System.IO;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using ServiceConnect.Object;


namespace ServiceConnect
{
    public class HttpServiceAsyncRequest
    {
        private string UserName, Password, TokenUrl, TokenString = String.Empty;

        private bool IsTokenBased = false;

        /// <summary>
        /// Not Authentication.
        /// </summary>
        public HttpServiceAsyncRequest() { }

        /// <summary>
        /// Basic Authentication.
        /// </summary>
        /// <param name="UserName"></param>
        /// <param name="Password"></param>
        public HttpServiceAsyncRequest(string UserName, string Password)
        {
            this.UserName = UserName;
            this.Password = Password;
        }

        /// <summary>
        /// Token Based Authentication.
        /// </summary>
        /// <param name="token"></param>
        public HttpServiceAsyncRequest(string token)
        {
            TokenManager.TokenClear();
            TokenString = token;
            IsTokenBased = true;
        }

        /// <summary>
        /// Token Based Authentication.
        /// </summary>
        /// <param name="UserName"></param>
        /// <param name="Password"></param>
        /// <param name="TokenUrl"></param>
        public HttpServiceAsyncRequest(string UserName, string Password, string TokenUrl)
        {
            TokenManager.TokenClear();

            this.UserName = UserName;
            this.Password = Password;
            this.TokenUrl = TokenUrl;

            IsTokenBased = true;
        }

        /// <summary>
        /// Token oluşturur.
        /// </summary>
        /// <returns></returns>
        private string GetToken()
        {
            if (TokenString == String.Empty)
                return TokenManager.GetToken(UserName, Password, TokenUrl);
            else
                return TokenString;
        }

        /// <summary>
        /// Credential oluşturur.
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        private CredentialCache GetCredential(string url)
        {
            CredentialCache credentialCache = new CredentialCache();
            credentialCache.Add(new Uri(url), "Basic", new NetworkCredential(UserName, Password));

            return credentialCache;
        }

        /// <summary>
        /// Basic Kod üretir.
        /// </summary>
        /// <returns></returns>
        private string GetBasic()
        {
            String encoded = Convert.ToBase64String(Encoding.GetEncoding("ISO-8859-1").GetBytes(UserName + ":" + Password));
            return "Basic " + encoded;
        }

        /// <summary>
        /// Parametre olarak gönderilen Url adresine Http Get isteğinde bulunur.
        /// Geriye istenilen obje türünde veri döndürür.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="requestUrl"></param>
        /// <returns></returns>
        public async Task<T> GET<T>(string requestUrl, string contentType = "application/json")
        {
            try
            {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(requestUrl);
                request.Method = WebRequestMethods.Http.Get;
                request.Accept = contentType;

                if (!IsTokenBased)
                    request.Credentials = GetCredential(requestUrl);
                else
                    request.Headers.Add("Authorization", GetToken());

                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                StreamReader reader = new StreamReader(response.GetResponseStream());
                string jsonData = reader.ReadToEnd();
                var obj = JsonConvert.DeserializeObject<T>(jsonData);
                reader.Close();
                response.Close();

                return obj;
            }
            catch (Exception)
            {
                TokenManager.TokenClear();

                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(requestUrl);
                request.Method = WebRequestMethods.Http.Get;
                request.Accept = "application/json";
                if (!IsTokenBased)
                    request.Credentials = GetCredential(requestUrl);
                else
                    request.Headers.Add("Authorization", GetToken());

                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                StreamReader reader = new StreamReader(response.GetResponseStream());
                string jsonData = reader.ReadToEnd();
                var obj = JsonConvert.DeserializeObject<T>(jsonData);
                reader.Close();
                response.Close();

                return obj;
            }
        }


        /// <summary>
        /// Parametre olarak gönderilen Url adresine Http Get isteğinde bulunur.
        /// Geriye Json formatında String Döndürür.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="requestUrl"></param>
        /// <returns></returns>
        public async Task<string> GET(string requestUrl, string contentType = "application/json")
        {


            try
            {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(requestUrl);
                request.Method = WebRequestMethods.Http.Get;
                request.Accept = contentType;

                if (!IsTokenBased)
                    request.Credentials = GetCredential(requestUrl);
                else
                    request.Headers.Add("Authorization", GetToken());

                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                StreamReader reader = new StreamReader(response.GetResponseStream());
                string jsonData = reader.ReadToEnd();
                reader.Close();
                response.Close();

                return jsonData;
            }
            catch (Exception)
            {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(requestUrl);
                request.Method = WebRequestMethods.Http.Get;
                request.Accept = contentType;

                TokenManager.TokenClear();

                if (!IsTokenBased)
                    request.Headers.Add("Authorization", GetBasic());
                else
                    request.Headers.Add("Authorization", GetToken());

                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                StreamReader reader = new StreamReader(response.GetResponseStream());
                string jsonData = reader.ReadToEnd();
                reader.Close();
                response.Close();

                return jsonData;
            }
        }


        /// <summary>
        /// Parametre olarak gönderilen Url adresine parametre olarak gönderilen objeyi json verisine çevirip Post eder.
        /// Geriye istenilen obje türünde veri döndürür.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="requestUrl"></param>
        /// <param name="responseObject"></param>
        /// <returns></returns>
        public async Task<T> POST<T>(string requestUrl, object responseObject, string contentType = "application/json")
        {
            try
            {
                string postData = JsonConvert.SerializeObject(responseObject);
                byte[] data = Encoding.UTF8.GetBytes(postData);

                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(requestUrl);
                request.Method = WebRequestMethods.Http.Post;
                request.ContentType = contentType;

                if (!IsTokenBased)
                    request.Credentials = GetCredential(requestUrl);
                else
                    request.Headers.Add("Authorization", GetToken());

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
                var obj = JsonConvert.DeserializeObject<T>(jsonData);
                reader.Close();
                response.Close();

                return obj;
            }
            catch (Exception)
            {
                string postData = JsonConvert.SerializeObject(responseObject);
                byte[] data = Encoding.UTF8.GetBytes(postData);

                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(requestUrl);
                request.Method = WebRequestMethods.Http.Post;
                request.ContentType = "application/json";

                TokenManager.TokenClear();

                if (!IsTokenBased)
                    request.Headers.Add("Authorization", GetBasic());
                else
                    request.Headers.Add("Authorization", GetToken());

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
                var obj = JsonConvert.DeserializeObject<T>(jsonData);
                reader.Close();
                response.Close();

                return obj;
            }
        }

        /// <summary>
        /// Parametre olarak gönderilen Url adresine parametre olarak gönderilen objeyi json verisine çevirip Post eder.
        /// Geriye istenilen obje türünde veri döndürür.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="requestUrl"></param>
        /// <param name="responseObject"></param>
        /// <returns></returns>
        public async Task<T> POST<T>(string requestUrl, string responseObject, string contentType = "application/json")
        {
            try
            {
                byte[] data = Encoding.UTF8.GetBytes(responseObject);

                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(requestUrl);
                request.Method = WebRequestMethods.Http.Post;
                request.ContentType = contentType;

                if (!IsTokenBased)
                    request.Credentials = GetCredential(requestUrl);
                else
                    request.Headers.Add("Authorization", GetToken());

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
                var obj = JsonConvert.DeserializeObject<T>(jsonData);
                reader.Close();
                response.Close();

                return obj;
            }
            catch (Exception)
            {
                byte[] data = Encoding.UTF8.GetBytes(responseObject);

                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(requestUrl);
                request.Method = WebRequestMethods.Http.Post;
                request.ContentType = contentType;

                TokenManager.TokenClear();

                if (!IsTokenBased)
                    request.Headers.Add("Authorization", GetBasic());
                else
                    request.Headers.Add("Authorization", GetToken());

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
                var obj = JsonConvert.DeserializeObject<T>(jsonData);
                reader.Close();
                response.Close();

                return obj;
            }
        }

        /// <summary>
        /// Parametre olarak gönderilen Url adresine parametre olarak gönderilen objeyi json verisine çevirip Post eder.
        /// Geriye Json formatında String Döndürür.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="requestUrl"></param>
        /// <param name="responseObject"></param>
        /// <returns></returns>
        public async Task<string> POST(string requestUrl, object responseObject, string contentType = "application/json")
        {
            try
            {
                string postData = JsonConvert.SerializeObject(responseObject);
                byte[] data = Encoding.UTF8.GetBytes(postData);

                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(requestUrl);
                request.Method = WebRequestMethods.Http.Post;
                request.ContentType = contentType;

                if (!IsTokenBased)
                    request.Credentials = GetCredential(requestUrl);
                else
                    request.Headers.Add("Authorization", GetToken());

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
                reader.Close();
                response.Close();

                return jsonData;
            }
            catch (Exception)
            {
                TokenManager.TokenClear();

                string postData = JsonConvert.SerializeObject(responseObject);
                byte[] data = Encoding.UTF8.GetBytes(postData);

                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(requestUrl);
                request.Method = WebRequestMethods.Http.Post;
                request.ContentType = "application/json";

                if (!IsTokenBased)
                    request.Credentials = GetCredential(requestUrl);
                else
                    request.Headers.Add("Authorization", GetToken());

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
                reader.Close();
                response.Close();

                return jsonData;
            }
        }

        /// <summary>
        /// Parametre olarak gönderilen Url adresine parametre olarak gönderilen objeyi json verisine çevirip Post eder.
        /// Geriye Json formatında String Döndürür.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="requestUrl"></param>
        /// <param name="responseObject"></param>
        /// <returns></returns>
        public async Task<string> POST(string requestUrl, string responseObject, string contentType = "application/json")
        {
            try
            {
                byte[] data = Encoding.UTF8.GetBytes(responseObject);

                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(requestUrl);
                request.Method = WebRequestMethods.Http.Post;
                request.ContentType = contentType;

                if (!IsTokenBased)
                    request.Credentials = GetCredential(requestUrl);
                else
                    request.Headers.Add("Authorization", GetToken());

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
                reader.Close();
                response.Close();

                return jsonData;
            }
            catch (Exception)
            {
                TokenManager.TokenClear();

                byte[] data = Encoding.UTF8.GetBytes(responseObject);

                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(requestUrl);
                request.Method = WebRequestMethods.Http.Post;
                request.ContentType = contentType;

                if (!IsTokenBased)
                    request.Headers.Add("Authorization", GetBasic());
                else
                    request.Headers.Add("Authorization", GetToken());

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
                reader.Close();
                response.Close();

                return jsonData;
            }
        }

        /// <summary>
        /// Parametre olarak gönderilen Url adresine parametre olarak gönderilen objeyi json verisine çevirip Put eder. 
        /// Güncellemede kullanılır.
        /// Geriye istenilen obje türünde veri döndürür.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="requestUrl"></param>
        /// <param name="responseObject"></param>
        /// <returns></returns>
        public async Task<T> PUT<T>(string requestUrl, object responseObject, string contentType = "application/json")
        {
            try
            {
                string postData = JsonConvert.SerializeObject(responseObject);
                byte[] data = Encoding.UTF8.GetBytes(postData);

                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(requestUrl);
                request.Method = WebRequestMethods.Http.Put;
                request.ContentType = contentType;

                if (!IsTokenBased)
                    request.Credentials = GetCredential(requestUrl);
                else
                    request.Headers.Add("Authorization", GetToken());

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
                var obj = JsonConvert.DeserializeObject<T>(jsonData);
                reader.Close();
                response.Close();

                return obj;
            }
            catch (Exception)
            {
                TokenManager.TokenClear();

                string postData = JsonConvert.SerializeObject(responseObject);
                byte[] data = Encoding.UTF8.GetBytes(postData);

                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(requestUrl);
                request.Method = WebRequestMethods.Http.Put;
                request.ContentType = "application/json";

                if (!IsTokenBased)
                    request.Credentials = GetCredential(requestUrl);
                else
                    request.Headers.Add("Authorization", GetToken());

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
                var obj = JsonConvert.DeserializeObject<T>(jsonData);
                reader.Close();
                response.Close();

                return obj;
            }
        }

        /// <summary>
        /// Parametre olarak gönderilen Url adresine parametre olarak gönderilen objeyi json verisine çevirip Put eder.
        /// Güncellemede kullanılır.
        /// Geriye Json formatında String Döndürür.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="requestUrl"></param>
        /// <param name="responseObject"></param>
        /// <returns></returns>
        public async Task<string> PUT(string requestUrl, object responseObject, string contentType = "application/json")
        {
            try
            {
                string postData = JsonConvert.SerializeObject(responseObject);
                byte[] data = Encoding.UTF8.GetBytes(postData);

                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(requestUrl);
                request.Method = WebRequestMethods.Http.Put;
                request.ContentType = contentType;

                if (!IsTokenBased)
                    request.Credentials = GetCredential(requestUrl);
                else
                    request.Headers.Add("Authorization", GetToken());

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
                reader.Close();
                response.Close();

                return jsonData;
            }
            catch (Exception)
            {
                TokenManager.TokenClear();

                string postData = JsonConvert.SerializeObject(responseObject);
                byte[] data = Encoding.UTF8.GetBytes(postData);

                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(requestUrl);
                request.Method = WebRequestMethods.Http.Put;
                request.ContentType = "application/json";

                if (!IsTokenBased)
                    request.Credentials = GetCredential(requestUrl);
                else
                    request.Headers.Add("Authorization", GetToken());

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
                reader.Close();
                response.Close();

                return jsonData;
            }
        }

        /// <summary>
        /// Parametre olarak gönderilen Url adresine parametre olarak gönderilen objeyi json verisine çevirip Put eder. 
        /// Güncellemede kullanılır.
        /// Geriye istenilen obje türünde veri döndürür.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="requestUrl"></param>
        /// <param name="responseObject"></param>
        /// <returns></returns>
        public async Task<T> PUT<T>(string requestUrl, string responseObject, string contentType = "application/json")
        {
            try
            {
                byte[] data = Encoding.UTF8.GetBytes(responseObject);

                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(requestUrl);
                request.Method = WebRequestMethods.Http.Put;
                request.ContentType = contentType;

                if (!IsTokenBased)
                    request.Credentials = GetCredential(requestUrl);
                else
                    request.Headers.Add("Authorization", GetToken());

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
                var obj = JsonConvert.DeserializeObject<T>(jsonData);
                reader.Close();
                response.Close();

                return obj;
            }
            catch (Exception)
            {
                TokenManager.TokenClear();

                string postData = JsonConvert.SerializeObject(responseObject);
                byte[] data = Encoding.UTF8.GetBytes(postData);

                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(requestUrl);
                request.Method = WebRequestMethods.Http.Put;
                request.ContentType = contentType;

                if (!IsTokenBased)
                    request.Headers.Add("Authorization", GetBasic());
                else
                    request.Headers.Add("Authorization", GetToken());

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
                var obj = JsonConvert.DeserializeObject<T>(jsonData);
                reader.Close();
                response.Close();

                return obj;
            }
        }

        /// <summary>
        /// Parametre olarak gönderilen Url adresine parametre olarak gönderilen objeyi json verisine çevirip Put eder.
        /// Güncellemede kullanılır.
        /// Geriye Json formatında String Döndürür.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="requestUrl"></param>
        /// <param name="responseObject"></param>
        /// <returns></returns>
        public async Task<string> PUT(string requestUrl, string responseObject, string contentType = "application/json")
        {
            try
            {
                byte[] data = Encoding.UTF8.GetBytes(responseObject);

                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(requestUrl);
                request.Method = WebRequestMethods.Http.Put;
                request.ContentType = contentType;

                if (!IsTokenBased)
                    request.Credentials = GetCredential(requestUrl);
                else
                    request.Headers.Add("Authorization", GetToken());

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
                reader.Close();
                response.Close();

                return jsonData;
            }
            catch (Exception)
            {
                TokenManager.TokenClear();

                byte[] data = Encoding.UTF8.GetBytes(responseObject);

                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(requestUrl);
                request.Method = WebRequestMethods.Http.Put;
                request.ContentType = contentType;

                if (!IsTokenBased)
                    request.Headers.Add("Authorization", GetBasic());
                else
                    request.Headers.Add("Authorization", GetToken());

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
                reader.Close();
                response.Close();

                return jsonData;
            }
        }

        /// <summary>
        /// Parametre olarak gönderilen Url adresine Http Delete isteğinde bulunur.
        /// Geriye istenilen obje türünde veri döndürür.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="requestUrl"></param>
        /// <returns></returns>
        public async Task<T> DELETE<T>(string requestUrl, string contentType = "application/json")
        {
            try
            {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(requestUrl);
                request.Method = "DELETE";
                request.Accept = contentType;

                if (!IsTokenBased)
                    request.Credentials = GetCredential(requestUrl);
                else
                    request.Headers.Add("Authorization", GetToken());

                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                StreamReader reader = new StreamReader(response.GetResponseStream());
                string jsonData = reader.ReadToEnd();
                var obj = JsonConvert.DeserializeObject<T>(jsonData);
                reader.Close();
                response.Close();

                return obj;
            }
            catch (Exception)
            {
                TokenManager.TokenClear();

                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(requestUrl);
                request.Method = "DELETE";
                request.Accept = "application/json";

                if (!IsTokenBased)
                    request.Credentials = GetCredential(requestUrl);
                else
                    request.Headers.Add("Authorization", GetToken());

                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                StreamReader reader = new StreamReader(response.GetResponseStream());
                string jsonData = reader.ReadToEnd();
                var obj = JsonConvert.DeserializeObject<T>(jsonData);
                reader.Close();
                response.Close();

                return obj;
            }
        }

        /// <summary>
        /// Parametre olarak gönderilen Url adresine Http Delete isteğinde bulunur.
        /// Geriye Json formatında String Döndürür.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="requestUrl"></param>
        /// <returns></returns>
        public async Task<string> DELETE(string requestUrl, string contentType = "application/json")
        {
            try
            {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(requestUrl);
                request.Method = "DELETE";
                request.Accept = contentType;

                if (!IsTokenBased)
                    request.Credentials = GetCredential(requestUrl);
                else
                    request.Headers.Add("Authorization", GetToken());

                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                StreamReader reader = new StreamReader(response.GetResponseStream());
                string jsonData = reader.ReadToEnd();
                reader.Close();
                response.Close();

                return jsonData;
            }
            catch (Exception)
            {
                TokenManager.TokenClear();

                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(requestUrl);
                request.Method = "DELETE";
                request.Accept = "application/json";

                if (!IsTokenBased)
                    request.Credentials = GetCredential(requestUrl);
                else
                    request.Headers.Add("Authorization", GetToken());

                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                StreamReader reader = new StreamReader(response.GetResponseStream());
                string jsonData = reader.ReadToEnd();
                reader.Close();
                response.Close();

                return jsonData;
            }
        }

        /// <summary>
        /// Manuel basit istekte Bulunur.
        /// </summary>
        /// <param name="requestUrl"></param>
        /// <param name="obj"></param>
        /// <returns></returns>
        public async Task<string> REQUEST(string requestUrl, Request obj)
        {
            TokenManager.TokenClear();
            if(obj==null)
                obj=new Request();

            byte[] data = null;

            if (obj.Body != null)
                data = Encoding.UTF8.GetBytes(obj.Body);

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(requestUrl);

            if (obj.Method != null)
                request.Method = obj.Method;
            if (obj.Content_Type != null)
                request.ContentType = obj.Content_Type;
            if (obj.Authorization != null)
                request.Headers.Add("Authorization", obj.Authorization);
            if (obj.Headers != null)
            {
                foreach (var item in obj.Headers)
                {
                    request.Headers.Add(item);
                }
            }

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

            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
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
        public async Task<T> REQUEST<T>(string requestUrl, Request obj)
        {
            TokenManager.TokenClear();
            if (obj == null)
                obj = new Request();

            byte[] data = null;

            if (obj.Body != null)
                data = Encoding.UTF8.GetBytes(obj.Body);

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(requestUrl);

            if (obj.Method != null)
                request.Method = obj.Method;
            if (obj.Content_Type != null)
                request.ContentType = obj.Content_Type;
            if (obj.Authorization != null)
                request.Headers.Add("Authorization", obj.Authorization);
            if (obj.Headers != null)
            {
                foreach (var item in obj.Headers)
                {
                    request.Headers.Add(item);
                }
            }

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

            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            StreamReader reader = new StreamReader(response.GetResponseStream());
            string jsonData = reader.ReadToEnd();
            var responseObject = JsonConvert.DeserializeObject<T>(jsonData);
            reader.Close();
            response.Close();

            return responseObject;
        }
    }
}
