using DA_Assets.FCU.Config;
using DA_Assets.FCU.Core;
using DA_Assets.FCU.Model;
using DA_Assets.Shared.CodeHelpers;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using Console = DA_Assets.Shared.Console;

#if JSON_NET_EXISTS
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
#endif

namespace DA_Assets.FCU.Extensions
{
    public static class FCU_Extensions
    {
        public static void StopImport(this FigmaConverterUnity fcu)
        {
            fcu.Model.DynamicCoroutine(fcu.Model.StopAllCoroutines);
        }
        public static string GetMetaId(this FObject fobject, FigmaConverterUnity fcu)
        {
            switch (fcu.Model.MainSettings.ComponentDetectionType)
            {
                case ComponentDetectionType.ById:
                    return fobject.Id;
                case ComponentDetectionType.ByName:
                    return fobject.Hierarchy;
            }

            return null;
        }
        public static IEnumerator WriteLog(this Request request, string text)
        {
            string logPath = Path.Combine(Directory.GetParent(Application.dataPath).FullName, "Logs");
            UnityCodeHelpers.CreateFolderIfNotExists(logPath);

            FileInfo[] fileInfos = new DirectoryInfo(logPath).GetFiles($"*.*");

            if (fileInfos.Length >= Config.FCU_Config.Instance.MaxLogFilesCount)
            {
                foreach (FileInfo file in fileInfos)
                {
                    try
                    {
                        file.Delete();
                    }
                    catch
                    {

                    }
                }
            }

            string logFileName = $"{DateTime.Now.ToString(Config.FCU_Config.Instance.DateTimeFormat)}_{Config.FCU_Config.Instance.WebLogFileName}";
            string logFilePath = Path.Combine(logPath, logFileName);

            if (text.IsJsonValid())
            {
#if JSON_NET_EXISTS
                JToken parsedJson = JToken.Parse(text);
                text = parsedJson.ToString(Formatting.Indented);
#endif
            }

            text = $"{request.Query}\n{text}";

            File.WriteAllText(logFilePath, text);

            yield return null;
        }
        public static bool IsEmpty(this FigmaProject figmaProject)
        {
            try
            {
                if (figmaProject.Equals(default) ||
                    figmaProject.Document == null ||
                    figmaProject.Document == default ||
                    figmaProject.Document.Children == null ||
                    figmaProject.Document.Children == default ||
                    figmaProject.Document.Children.Count() < 1)
                {
                    return true;
                }
            }
            catch (Exception ex)
            {
                Console.LogError(ex);
                return true;
            }

            return false;
        }
        public static float GetImageScale(this ImageScale imageScale)
        {
            switch (imageScale)
            {
                case ImageScale.X_0_5:
                    return 0.5f;
                case ImageScale.X_0_75:
                    return 0.75f;
                case ImageScale.X_1_0:
                    return 1f;
                case ImageScale.X_1_5:
                    return 1.5f;
                case ImageScale.X_2_0:
                    return 2.0f;
                case ImageScale.X_3_0:
                    return 3.0f;
                case ImageScale.X_4_0:
                    return 4.0f;
                default:
                    return 4.0f;
            }
        }
        public static string GetImageFormat(this ImageFormat imageFormat)
        {
            return imageFormat.ToString().ToLower();
        }
        public static bool ContainsButtonState(this string fixedName, out ButtonState buttonState)
        {
            ButtonState[] allTags = Enum.GetValues(typeof(ButtonState))
              .Cast<ButtonState>()
              .ToArray();

            foreach (ButtonState tag in allTags)
            {
                if (fixedName.ToLower().Contains(tag.ToLower()))
                {
                    buttonState = tag;
                    return true;
                }
            }

            buttonState = ButtonState.Default;
            return false;
        }
        public static void Log(this FigmaConverterUnity fcu, object log)
        {
            if (fcu.Model.MainSettings.DebugMode)
            {
                Debug.Log($"FCU {fcu.Model.Guid}: {log}");
            }
        }
        public static string ParseError(this FigmaError figmaError)
        {
            return $"Error: {figmaError.Error}\nStatus: {figmaError.Status}";
        }
        public static bool IsAuthed(this FigmaConverterUnity fcu)
        {
            if (string.IsNullOrWhiteSpace(fcu.Model.MainSettings.Token))
            {
                return false;
            }
            else
            {
                return true;
            }
        }
        public static bool IsProjectUrlExists(this FigmaConverterUnity fcu)
        {
            if (string.IsNullOrWhiteSpace(fcu.Model.MainSettings.ProjectUrl))
            {
                return false;
            }
            else
            {
                return true;
            }
        }
        public static void SetFixedName(this FObject fobject)
        {
            string newName = fobject.Name.ReplaceInvalidFileNameChars(FCU_Config.Instance.RealTagSeparator);

            if (newName.Length == 0)
            {
                if (fobject.Parent == null)
                {
                    newName = $"unnamed - {fobject.Id}";
                }
                else
                {
                    newName = $"unnamed - {fobject.Parent.Id} {fobject.Id}";
                }
            }

            fobject.FixedName = newName;
        }
        public static string GetHash(this FObject fobject)
        {
            string final = "";

            final += fobject.FixedName;
            final += fobject.Size.Round();
            final += fobject.Type;
            final += fobject.GetFigmaRotationAngle();
            final += fobject.Fills.GetHash();
            final += fobject.Strokes.GetHash();

            return final.StringSha256Hash(8).ToLower();
        }
        public static string GetHash(this List<Fill> fills)
        {
            string final = "";

            foreach (var item in fills)
            {
                final += item.BlendMode;
                final += item.Opacity;
                final += item.Type;
                final += item.ScaleMode;
                final += item.ImageRef;
                final += item.Color.ToString();
                final += item.Visible;

                if (item.GradientStops != null)
                {
                    foreach (var item2 in item.GradientStops)
                    {
                        final += item2.Color.ToString();
                        final += item2.Position.ToString();
                    }
                }

                if (item.GradientHandlePositions != null)
                {
                    foreach (var item2 in item.GradientHandlePositions)
                    {
                        final += item2.ToString();
                    }
                }
            }

            return final.StringSha256Hash();
        }
        public static int GetHash(this List<Stroke> strokes)
        {
            string final = "";

            foreach (var item in strokes)
            {
                final += item.BlendMode;
                final += item.Type;
                final += item.Color.ToString();
            }

            return final.GetHashCode();
        }
    }
}