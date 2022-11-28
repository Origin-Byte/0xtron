using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using System.Linq.Expressions;

namespace DA_Assets.Shared.CodeHelpers
{
    /// <summary>
    /// D.A. Unity Code Helpers [10.2022]
    /// </summary>
    public static class UnityCodeHelpers
    {
        /// <summary>
        /// https://stackoverflow.com/a/2776689
        /// </summary>
        public static string Truncate(this string value, int maxLength)
        {
            return value?.Length > maxLength ? value.Substring(0, maxLength) : value;
        }
        public static string StringSha256Hash(this string text, int truncateLenght = 0)
        {
            if (string.IsNullOrEmpty(text))
            {
                return null;
            }
            else
            {
                string hash = BitConverter
                    .ToString(new System.Security.Cryptography.SHA256Managed()
                    .ComputeHash(System.Text.Encoding.UTF8.GetBytes(text)))
                    .Replace("-", string.Empty);

                if (truncateLenght > 0)
                {
                    hash = hash.Truncate(truncateLenght);
                }

                return hash;
            }
        }
        /// <summary>
        /// https://stackoverflow.com/a/6488922
        /// </summary>
        public static IEnumerable<T2> GetDuplicates<T1, T2>(this Dictionary<T1, List<T2>> data)
        {
            // find duplicates
            var dupes = new HashSet<T2>(
                            from list1 in data.Values
                            from list2 in data.Values
                            where list1 != list2
                            from item in list1.Intersect(list2)
                            select item);

            // return a list of the duplicates
            return dupes;
        }
        public static string[] GetFieldsArray<T>(Expression<Func<T, object>> pathExpression)
        {
            var getMemberNameFunc = new Func<Expression, MemberExpression>(expression => expression as MemberExpression);
            var memberExpression = getMemberNameFunc(pathExpression.Body);
            var names = new Stack<string>();

            while (memberExpression != null)
            {
                names.Push(memberExpression.Member.Name);
                memberExpression = getMemberNameFunc(memberExpression.Expression);
            }

            return names.ToArray();
        }


#if UNITY_EDITOR
        private delegate void GetWidthAndHeight(TextureImporter importer, ref int width, ref int height);
        private static GetWidthAndHeight getWidthAndHeightDelegate;
        /// <summary>
        /// Sets the maximum size of the texture based on its width and height.
        /// <para><see href="https://forum.unity.com/threads/getting-original-size-of-texture-asset-in-pixels.165295/"/></para>
        /// </summary>
        public static void FixTextureSize(this TextureImporter importer)
        {
            int[] maxTextureSizeValues = { 32, 64, 128, 256, 512, 1024, 2048, 4096, 8192, 16384 };

            if (getWidthAndHeightDelegate == null)
            {
                var method = typeof(TextureImporter).GetMethod("GetWidthAndHeight", BindingFlags.NonPublic | BindingFlags.Instance);
                getWidthAndHeightDelegate = Delegate.CreateDelegate(typeof(GetWidthAndHeight), null, method) as GetWidthAndHeight;
            }

            int width = 0;
            int height = 0;

            getWidthAndHeightDelegate(importer, ref width, ref height);

            int max = Mathf.Max(width, height);

            int defsize = 1024; //Default size

            for (int i = 0; i < maxTextureSizeValues.Length; i++)
            {
                if (maxTextureSizeValues[i] >= max)
                {
                    defsize = maxTextureSizeValues[i];
                    break;
                }
            }

            importer.maxTextureSize = defsize;
        }
#endif
        /// <summary>
        /// Returns the first attribute of the input type.
        /// </summary>
        public static T GetFirstAttributeOfType<T>(this Enum value) where T : Attribute
        {
            FieldInfo fi = value.GetType().GetField(value.ToString());
            object[] objs = fi.GetCustomAttributes(typeof(T), false);

            if (objs.Length > 0)
            {
                return (T)Convert.ChangeType(objs[0], typeof(T));
            }
            else
            {
                return default;
            }
        }

        public static Vector2 Round(this Vector2 vector2, int dp = 0)
        {
            float x = (float)System.Math.Round(vector2.x, dp);
            float y = (float)System.Math.Round(vector2.y, dp);

            return new Vector2(x, y);
        }



        public static List<object> GetMembersValues<T>(this T @object)
        {
            List<object> values = new List<object>();

            const BindingFlags bindingFlags = BindingFlags.Public | BindingFlags.Instance;

            MemberInfo[] memberInfos = typeof(T)
                .GetFields(bindingFlags)
                .Cast<MemberInfo>()
                .Concat(typeof(T)
                .GetProperties(bindingFlags))
                .ToArray();

            foreach (MemberInfo memberInfo in memberInfos)
            {

                try
                {
                    object value = memberInfo.GetValue(@object);
                    values.Add(value);
                }
                catch
                {
                    values.Add(null);
                }
            }

            return values;
        }

        public static object GetValue(this MemberInfo memberInfo, object forObject)
        {
            switch (memberInfo.MemberType)
            {
                case MemberTypes.Field:
                    return ((FieldInfo)memberInfo).GetValue(forObject);
                case MemberTypes.Property:
                    return ((PropertyInfo)memberInfo).GetValue(forObject);
                default:
                    throw new NotImplementedException();
            }
        }
        public static bool TryReadAllText(this string path, out string text)
        {
            try
            {
                text = File.ReadAllText(path);
                return true;
            }
            catch
            {
                text = null;
                return false;
            }
        }
        public static float ToFloat(this float? value)
        {
            return value.HasValue ? value.Value : 0;
        }
        /// <summary>
        /// Returns false if bool is null. Returns the true value if not null.
        /// </summary>
        public static bool ToBool(this bool? value)
        {
            return value.HasValue ? value.Value : false;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="gameObject"></param>
        /// <param name="component"></param>
        /// <returns>Returns whether a component of the input type has been added.</returns>
        public static bool TryAddComponent<T>(this GameObject gameObject, out T component) where T : UnityEngine.Component
        {
            if (gameObject.TryGetComponent(out component))
            {
                return false;
            }
            else
            {
                component = gameObject.AddComponent<T>();
                return true;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="gameObject"></param>
        /// <param name="graphic">Found or added graphic component.</param>
        /// <returns>Returns whether a component of the input type has been added.</returns>
        public static bool TryAddGraphic<T>(this GameObject gameObject, out T graphic) where T : Graphic
        {
            if (gameObject.TryGetComponent(out graphic))
            {
                return false;
            }
            else if (gameObject.TryGetComponent(out Graphic _graphic))
            {
                return false;
            }
            else
            {
                graphic = gameObject.AddComponent<T>();
                return true;
            }
        }
        /// <summary>
        /// Marks target object as dirty, but as an extension.
        /// </summary>
        /// <param name="target">The object to mark as dirty.</param>
        public static void SetDirty(this UnityEngine.Object target)
        {
#if UNITY_EDITOR
            EditorUtility.SetDirty(target);
#endif
        }

        public static GameObject CreateEmptyGameObject(Transform parent = null)
        {
            GameObject tempGO = new GameObject();
            GameObject emptyGO;

            if (parent == null)
            {
                emptyGO = UnityEngine.Object.Instantiate(tempGO);
            }
            else
            {
                emptyGO = UnityEngine.Object.Instantiate(tempGO, parent);
            }

            tempGO.Destroy();
            return emptyGO;
        }



        /// <summary>
        /// Checks for the presence of folders in the input path, and if they are not there, creates them.
        /// </summary>
        /// <param name="pathFolders">Full path to the destination folder, including the 'Assets' folder in begin, and destination in end.</param>
        public static string CreateAllPathFolders(params string[] path)
        {
#if UNITY_EDITOR
            for (int i = 0; i < path.Count(); i++)
            {
                if (i == 0)
                {
                    continue;
                }

                IEnumerable<string> parentFolders = path[i].GetBetweenElement(path);
                string parentPath = Path.Combine(parentFolders.ToArray());
                string newFolderPath = Path.Combine(parentPath, path[i]);

                if (AssetDatabase.IsValidFolder(newFolderPath) == false)
                {
                    AssetDatabase.CreateFolder(parentPath, path[i]);
                }
            }
#endif
            return Path.Combine(path);
        }

        public static IEnumerator WaitForFrames(this UnityEngine.Object @object, int count)
        {
            for (int i = 0; i < count; i++)
            {
                yield return new WaitForEndOfFrame();
            }
        }

        public static void CreateFolderIfNotExists(string path)
        {
            if (Directory.Exists(path) == false)
            {
                Directory.CreateDirectory(path);
            }
        }
        public static string ToLower(this Enum value)
        {
            return value.ToString().ToLower();
        }
        public static string ToUpper(this Enum value)
        {
            return value.ToString().ToUpper();
        }
        /// <summary>
        /// Truncates a string to a specific character only if the string really needs to be truncated. Does not cause an exception.
        /// </summary>
        public static string SubstringSafe(this string value, int length)
        {
            if (string.IsNullOrEmpty(value))
            {
                return value;
            }
            else if (value.Length > length)
            {
                return value.Substring(0, length);
            }
            else
            {
                return value;
            }
        }

        /// <summary>
        /// Makes random bool.
        /// <para><see href="https://forum.unity.com/threads/random-randomboolean-function.83220/#post-548687"/></para>
        /// </summary>
        public static bool RandomBool
        {
            get
            {
                return UnityEngine.Random.value > 0.5f;
            }
        }
        /// <summary>
        /// Removes all childs from Transform.
        /// <para><see href="https://www.noveltech.dev/unity-delete-children/"/></para>
        /// </summary>
        public static int ClearChilds(this Transform transform)
        {
            int childCount = transform.childCount;

            for (int i = childCount - 1; i >= 0; i--)
            {
                transform.GetChild(i).gameObject.Destroy();
            }

            return childCount;
        }
        /// <summary>
        /// Removes all HTML tags from string.
        /// <para><see href="https://stackoverflow.com/a/18154046"/></para>
        /// </summary>
        public static string RemoveHTML(this string text)
        {
            return Regex.Replace(text, "<.*?>", string.Empty);
        }
        /// <summary>
        /// Removing string between two strings.
        /// <para><see href="https://stackoverflow.com/q/51891661"/></para>
        /// </summary>
        public static string RemoveBetween(this string text, string startTag, string endTag)
        {
            Regex regex = new Regex(string.Format("{0}(.*?){1}", Regex.Escape(startTag), Regex.Escape(endTag)), RegexOptions.RightToLeft);
            string result = regex.Replace(text, startTag + endTag);
            return result;
        }
        /// <summary>
        /// Get part of string between two strings.
        /// <para><see href="https://stackoverflow.com/a/17252672"/></para>
        /// </summary>
        public static string GetBetween(this string text, string startTag, string endTag)
        {
            int pFrom = text.IndexOf(startTag) + startTag.Length;
            int pTo = text.LastIndexOf(endTag);
            string result = text.Substring(pFrom, pTo - pFrom);
            return result;
        }

        /// <summary>
        /// Remap value from these interval to another interval with saving proportion
        /// <para><see href="https://forum.unity.com/threads/re-map-a-number-from-one-range-to-another.119437/"/></para>
        /// </summary>
        /// <param name="value">Value for remapping</param>
        /// <param name="sourceMin"></param>
        /// <param name="sourceMax"></param>
        /// <param name="targetMin"></param>
        /// <param name="targetMax"></param>
        /// <returns></returns>
        public static float Remap(this float value, float sourceMin, float sourceMax, float targetMin, float targetMax)
        {
            return (value - sourceMin) / (sourceMax - sourceMin) * (targetMax - targetMin) + targetMin;
        }


        /// <summary>
        /// Destroying Unity GameObject, but as an extension.
        /// <para>Works in Editor and Playmode.</para>
        /// </summary>
        /// <summary>
        public static void Destroy(this UnityEngine.Object unityObject)
        {
#if UNITY_EDITOR
            UnityEngine.Object.DestroyImmediate(unityObject);
#else
            UnityEngine.Object.Destroy(unityObject);
#endif
        }
        /// <summary>
        /// Destroying script of Unity GameObject, but as an extension.
        /// <para>Works in Editor and Playmode.</para>
        /// </summary>
        public static void Destroy(this UnityEngine.Component unityComponent)
        {
#if UNITY_EDITOR
            UnityEngine.Object.DestroyImmediate(unityComponent);
#else
            UnityEngine.Object.Destroy(unityComponent);
#endif
        }
        /// <summary>
        /// <para>Example: "#ff000099".ToColor() red with alpha ~50%</para>
        /// <para>Example: "ffffffff".ToColor() white with alpha 100%</para>
        /// <para>Example: "00ff00".ToColor() green with alpha 100%</para>
        /// <para>Example: "0000ff00".ToColor() blue with alpha 0%</para>
        /// <para><see href="https://github.com/smkplus/KamaliDebug"/></para>
        /// </summary>
        public static Color ToColor(this string color)
        {
            if (color.StartsWith("#", StringComparison.InvariantCulture))
            {
                color = color.Substring(1); // strip #
            }

            if (color.Length == 6)
            {
                color += "FF"; // add alpha if missing
            }

            uint hex = Convert.ToUInt32(color, 16);
            float r = ((hex & 0xff000000) >> 0x18) / 255f;
            float g = ((hex & 0xff0000) >> 0x10) / 255f;
            float b = ((hex & 0xff00) >> 8) / 255f;
            float a = (hex & 0xff) / 255f;

            return new Color(r, g, b, a);
        }
        /// <summary>
        /// Simplified syntax for splitting string by string
        /// </summary>
        public static string[] Split(this string text, string separator)
        {
            return text.Split(new string[] { separator }, StringSplitOptions.None);
        }

        /// <summary>
        /// Replaces text in a file.
        /// <para><see href="https://stackoverflow.com/a/58377834/8265642"/></para>
        /// </summary>
        /// <param name="filePath">Path of the text file.</param>
        /// <param name="searchText">Text to search for.</param>
        /// <param name="replaceText">Text to replace the search text.</param>
        static public void ReplaceInFile(string filePath, string searchText, string replaceText)
        {
            StreamReader reader = new StreamReader(filePath);
            string content = reader.ReadToEnd();
            reader.Close();

            content = Regex.Replace(content, searchText, replaceText);

            StreamWriter writer = new StreamWriter(filePath);
            writer.Write(content);
            writer.Close();
        }

        public static string ReplaceSeparatorChars(this string value, string newChar = "")
        {
            string _value = value;

            string[] chars = new string[]
            {
                "_", "-" , " ", "+", "&"
            };

            foreach (string @char in chars)
            {
                _value = _value.Replace(@char, newChar);
            }

            return _value;
        }

        /// <summary>
        /// <para><see href="https://forum.unity.com/threads/easy-text-format-your-debug-logs-rich-text-format.906464/"/></para>
        /// </summary>
        public static string TextBold(this string str) => "<b>" + str + "</b>";
        public static string TextColor(this string str, string clr) => string.Format("<color={0}>{1}</color>", clr, str);
        public static string TextItalic(this string str) => "<i>" + str + "</i>";
        public static string TextSize(this string str, int size) => string.Format("<size={0}>{1}</size>", size, str);
    }
}