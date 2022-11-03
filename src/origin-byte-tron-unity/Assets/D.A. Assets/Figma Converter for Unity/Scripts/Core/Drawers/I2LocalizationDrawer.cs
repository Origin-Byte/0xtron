using DA_Assets.FCU.Model;
using System.IO;
using System.Threading.Tasks;
using UnityEngine;
using System.Text;
using DA_Assets.FCU.Config;
using System.Collections;
using System;
using DA_Assets.Shared.CodeHelpers;

#if I2LOC_EXISTS 
using I2.Loc;
#endif

namespace DA_Assets.FCU.Core.Drawers
{
    [Serializable]
    public class I2LocalizationDrawer : ControllerHolder<FigmaConverterUnity>
    {
        public IEnumerator Draw()
        {
#if I2LOC_EXISTS && UNITY_EDITOR
            if (languageSource == null)
            {
                languageSource = MonoBehaviour.FindObjectOfType<LanguageSource>();

                if (languageSource == null)
                {
                    GameObject _gameObject = UnityCodeHelpers.CreateEmptyGameObject();
                    _gameObject.name = FCU_Config.Instance.I2LocGameObjectName;
                    languageSource = _gameObject.AddComponent<LanguageSource>();
                }
            }

            ImportCSV(GetLocFilePath(), eSpreadsheetUpdateMode.Merge);
#endif
            yield break;
        }
        public IEnumerator AddI2Localizes()
        {
#if I2LOC_EXISTS && UNITY_EDITOR
            foreach (FObject text in controller.CanvasDrawer.TextDrawer.texts)
            {
                AddI2Localize(text);
                yield return new WaitForSecondsRealtime(FCU_Config.Instance.Delay001);
            }
#endif
            yield return null;
        }
#if I2LOC_EXISTS && UNITY_EDITOR
        [SerializeField] LanguageSource languageSource;
        private void AddI2Localize(FObject fobject)
        {
            fobject.GameObject.TryAddComponent(out I2.Loc.Localize i2l);

            i2l.Source = languageSource;

            string subStr = fobject.Characters.SubstringSafe(32);
            string newKey = subStr.ReplaceInvalidFileNameChars().ToLower();
            string lfp = GetLocFilePath();

            if (TextExistsInFile(lfp, newKey) == false)
            {
                string text = fobject.Characters;
                text = EscapeQuotesForExel(text);

                string newLine = $"{newKey};;;\"{fobject.Characters}\"\n";

                using (StreamWriter writer = new StreamWriter(lfp, true, Encoding.UTF8))
                {
                    writer.WriteLine(newLine);
                }
            }

            i2l.Term = newKey;
        }
        private string GetLocFilePath()
        {
            string path = $"{Application.dataPath}/{FCU_Config.Publisher}/{FCU_Config.ProductName}/{FCU_Config.Instance.LocalizationFileName}";
            CreateLocFile(path);
            return path;
        }
        private string EscapeQuotesForExel(string text)
        {
            return text.Replace("\"", "\"\"");
        }
        private void ImportCSV(string FileName, eSpreadsheetUpdateMode updateMode)
        {
            languageSource.mSource.Import_CSV(
                "",
                LocalizationReader.ReadCSVfile(FileName, Encoding.UTF8),
                updateMode,
                ';');

            languageSource.mSource.Awake();
        }
        private void CreateLocFile(string path)
        {
            if (File.Exists(path) == false)
            {
                FileStream oFileStream = new FileStream(path, FileMode.Create);
                oFileStream.Close();

                CheckLocHeader(path);
            }
            else
            {
                CheckLocHeader(path);
            }
        }

        private void CheckLocHeader(string path)
        {
            string fileHeader = "Key;Type;Desc;English";

            if (TextExistsInFile(path, fileHeader) == false)
            {
                string currentContent = File.ReadAllText(path);
                File.WriteAllText(path, $"{fileHeader}\n{currentContent}");
            }
        }
        private bool TextExistsInFile(string filePath, string text)
        {
            string[] lines = File.ReadAllLines(filePath);
            bool contains = false;
            Parallel.ForEach(lines, (line) =>
            {
                if (line.Contains(text))
                {
                    contains = true;
                }
            });

            return contains;
        }
#endif
    }
}
