using System;
using System.Linq;

#if JSON_NET_EXISTS
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
#endif

namespace DA_Assets.Shared.CodeHelpers
{
    public static class JsonExtensions
    {
        /// <summary>
        /// <para><see href="https://stackoverflow.com/a/14977915"/></para>
        /// </summary>
        public static bool IsJsonValid(this string json)
        {
#if JSON_NET_EXISTS
            if (string.IsNullOrWhiteSpace(json))
            {
                return false;
            }

            json = json.Trim();

            if ((json.StartsWith("{") && json.EndsWith("}")) || //For object
                (json.StartsWith("[") && json.EndsWith("]"))) //For array
            {
                try
                {
                    JToken obj = JToken.Parse(json);
                    return true;
                }
                catch (JsonReaderException)
                {
                    //Exception in parsing json
                    return false;
                }
                catch (Exception) //some other exception
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
#else
            return false;
#endif
        }

        public static bool TryParseJson<T>(this string json, out T genericType)
        {
#if JSON_NET_EXISTS
            try
            {
                T @object = JsonConvert.DeserializeObject<T>(json, JsonSerializerSettings);

                T defaultInstance = (T)Activator.CreateInstance(typeof(T));
                string defaultJson = JsonConvert.SerializeObject(defaultInstance);
                defaultInstance = JsonConvert.DeserializeObject<T>(defaultJson, JsonSerializerSettings);

                bool equal = Enumerable.SequenceEqual(@object.GetMembersValues(), defaultInstance.GetMembersValues());

                if (equal)
                {
                    throw new Exception("Object is default.");
                }
                else if (@object == null)
                {
                    throw new Exception("Object is null.");
                }

                genericType = @object;
                return true;
            }
            catch
            {
                genericType = default;
                return false;
            }
#else
            genericType = default;
            return false;
#endif
        }

#if JSON_NET_EXISTS
        public static JsonSerializerSettings JsonSerializerSettings { get; } = new JsonSerializerSettings
        {
            NullValueHandling = NullValueHandling.Ignore,
            Formatting = Formatting.Indented,
            ReferenceLoopHandling = ReferenceLoopHandling.Serialize,
            Error = (sender, error) =>
            {
                error.ErrorContext.Handled = true;
            }
        };
#endif
    }
}