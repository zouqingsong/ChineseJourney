using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using ZibaobaoLib.Helpers;

namespace ZibaobaoLib
{
    public class NewtonJsonSerializer
    {
        static readonly JsonSerializerSettings _settings = new JsonSerializerSettings
        {
            Formatting = Formatting.Indented,
            NullValueHandling = NullValueHandling.Ignore,
            DateTimeZoneHandling = DateTimeZoneHandling.RoundtripKind,
            DateFormatString = "yyyy-MM-dd HH:mm:ss.FFFFFFF",
            ReferenceLoopHandling = ReferenceLoopHandling.Serialize,
        };

        public static string ToJSON(object ob)
        {
            return JsonConvert.SerializeObject(ob, _settings);
        }

        /// <summary>
        /// Deserialize the given JSON string data (<paramref name="data"/>) into a
        ///   dictionary.
        /// </summary>
        /// <param name="data">JSON string.</param>
        /// <returns>Deserialized dictionary.</returns>
        public IDictionary<string, object> DeserializeData(string data)
        {
            var values = JsonConvert.DeserializeObject<Dictionary<string, object>>(data, _settings);

            return DeserializeData(values);
        }

        public static T ParseJSON<T>(string content)
        {
            try
            {
                return  !string.IsNullOrEmpty(content)?JsonConvert.DeserializeObject<T>(content, _settings):default(T);
            }
            catch (Exception ex)
            {
                X1LogHelper.Exception(ex);
            }
            return default(T);
        }

        /// <summary>
        /// Deserialize the given JSON object (<paramref name="data"/>) into a dictionary.
        /// </summary>
        /// <param name="data">JSON object.</param>
        /// <returns>Deserialized dictionary.</returns>
        private static IDictionary<string, object> DeserializeData(JObject data)
        {
            var dict = data.ToObject<Dictionary<String, Object>>();

            return DeserializeData(dict);
        }

        /// <summary>
        /// Deserialize any elements of the given data dictionary (<paramref name="data"/>) 
        ///   that are JSON object or JSON arrays into dictionaries or lists respectively.
        /// </summary>
        /// <param name="data">Data dictionary.</param>
        /// <returns>Deserialized dictionary.</returns>
        private static IDictionary<string, object> DeserializeData(IDictionary<string, object> data)
        {
            foreach (var key in data.Keys.ToArray())
            {
                var value = data[key];

                if (value is JObject)
                    data[key] = DeserializeData(value as JObject);

                if (value is JArray)
                    data[key] = DeserializeData(value as JArray);
            }

            return data;
        }

        /// <summary>
        /// Deserialize the given JSON array (<paramref name="data"/>) into a list.
        /// </summary>
        /// <param name="data">Data dictionary.</param>
        /// <returns>Deserialized list.</returns>
        private static IList<Object> DeserializeData(JArray data)
        {
            var list = data.ToObject<List<Object>>();

            for (int i = 0; i < list.Count; i++)
            {
                var value = list[i];

                if (value is JObject)
                {
                    list[i] = DeserializeData(value as JObject);
                }

                if (value is JArray)
                {
                    list[i] = DeserializeData(value as JArray);
                }
            }

            return list;
        }

    }
}