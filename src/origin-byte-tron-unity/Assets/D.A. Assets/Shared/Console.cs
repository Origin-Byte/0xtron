using DA_Assets.Shared.CodeHelpers;
using System;
using UnityEditor;

namespace DA_Assets.Shared
{
    public static class Console
    {
        public static string redColor = "red";
        public static string blackColor = "black";
        public static string whiteColor = "white";
        public static string violetColor = "#8b00ff";
        public static string orangeColor = "#ffa500";
        public static void LogError(Exception ex)
        {
            LogError(ex.ToString());
        }
        public static void LogError(string log, params object[] args)
        {
            LogError(string.Format(log, args));
        }
        public static void LogError(string log)
        {
            UnityEngine.Debug.LogError(log);
        }
        public static void LogWarning(string log)
        {
            UnityEngine.Debug.LogWarning(log.TextColor(orangeColor).TextBold());
        }
        public static void WriteLine(string log, params object[] args)
        {
            WriteLine(string.Format(log, args));
        }
        public static void WriteLine(string log)
        {
            string color = whiteColor;
#if UNITY_EDITOR
            color = EditorGUIUtility.isProSkin ? whiteColor : blackColor;
#endif
            UnityEngine.Debug.Log(log.TextColor(color).TextBold());
        }
        public static void Success(string log)
        {
            UnityEngine.Debug.Log(log.TextColor(violetColor).TextBold());
        }
    }
}