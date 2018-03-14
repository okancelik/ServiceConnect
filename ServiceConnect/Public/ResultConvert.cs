using System;
using System.Web.Script.Serialization;

// ReSharper disable once CheckNamespace
namespace ServiceConnect
{
    public static class ResultConvert
    {
        /// <summary>
        /// Json Formatındaki string veriyi objeye çevirir.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="json"></param>
        /// <returns></returns>
        public static T Deserialize<T>(string json)
        {
            var obj = new JavaScriptSerializer().Deserialize<T>(json);
            return obj;
        }

        /// <summary>
        /// Objeyi string json formatına çevirir.
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static string Serialize(dynamic obj)
        {
            try
            {
                return new JavaScriptSerializer().Serialize(obj);
            }
            catch (Exception)
            {
                return String.Empty;
            }
        }
    }
}
