using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Web.Script.Serialization;

namespace Client
{
    public static class ApiClient
    {
        // Укажите IP вашего Node.js сервера
        public static string ServerUrl = "http://2.26.61.185:4444/api/odp";
        public static string CurrentSid = null;
        public static string CurrentPhone = null;

        public static Dictionary<string, object> Execute(Dictionary<string, object> payload)
        {
            var serializer = new JavaScriptSerializer();

            // Автоматически подмешиваем SID ко всем запросам, если он есть
            if (CurrentSid != null && !payload.ContainsKey("sid"))
                payload["sid"] = CurrentSid;

            string jsonPayload = serializer.Serialize(payload);

            var request = (HttpWebRequest)WebRequest.Create(ServerUrl);
            request.Method = "POST";
            request.ContentType = "application/x-www-form-urlencoded";

            string postData = "request=" + Uri.EscapeDataString(jsonPayload);
            byte[] byteArray = Encoding.UTF8.GetBytes(postData);
            request.ContentLength = byteArray.Length;

            using (var dataStream = request.GetRequestStream())
            {
                dataStream.Write(byteArray, 0, byteArray.Length);
            }

            using (var response = (HttpWebResponse)request.GetResponse())
            using (var stream = response.GetResponseStream())
            using (var reader = new StreamReader(stream))
            {
                string responseString = reader.ReadToEnd();
                return serializer.Deserialize<Dictionary<string, object>>(responseString);
            }
        }
    }
}