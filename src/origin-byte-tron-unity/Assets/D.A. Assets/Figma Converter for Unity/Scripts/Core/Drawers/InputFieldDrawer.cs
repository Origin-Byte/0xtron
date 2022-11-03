using DA_Assets.FCU.Config;
using DA_Assets.FCU.Extensions;
using DA_Assets.FCU.Model;
using DA_Assets.Shared;
using DA_Assets.Shared.CodeHelpers;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace DA_Assets.FCU.Core.Drawers
{
    [Serializable]
    public class InputFieldDrawer : ControllerHolder<FigmaConverterUnity>
    {
        private List<FCU_Meta> inputFields;

        public void Init()
        {
            inputFields = new List<FCU_Meta>();
        }
        public IEnumerator Draw(FObject fobject)
        {
            switch (controller.Model.MainSettings.TextComponent)
            {
                case TextComponent.UnityText:
                    fobject.GameObject.TryAddComponent(out InputField inputField);
                    break;
#if TMPRO_EXISTS
                case TextComponent.TextMeshPro:
                    fobject.GameObject.TryAddComponent(out TMP_InputField tmpInputField);
                    break;
#endif
            }

            inputFields.Add(fobject.Meta);

            yield break;
        }

        public IEnumerator SetTargetGraphics()
        {
            switch (controller.Model.MainSettings.TextComponent)
            {
                case TextComponent.UnityText:
                    yield return SetTargetGraphicsInputFields();
                    break;
#if TMPRO_EXISTS
                case TextComponent.TextMeshPro:
                    yield return SetTargetGraphicsTmpInputFields();
                    break;
#endif
            }

            inputFields.Clear();
        }

        private IEnumerator SetTargetGraphicsInputFields()
        {
            foreach (FCU_Meta fieldMeta in inputFields)
            {
                InputField inputField = fieldMeta.GetComponent<InputField>();

                InputFieldModel ifm = GetGraphics(fieldMeta);

                inputField.targetGraphic = ifm.Background;
                inputField.placeholder = ifm.Placeholder;

                inputField.textComponent = (Text)ifm.TextComponent;
                inputField.textComponent.supportRichText = false;

                inputField.enabled = false;
                yield return new WaitForSecondsRealtime(FCU_Config.Instance.Delay001);
                inputField.enabled = true;
            }
        }
        private IEnumerator SetTargetGraphicsTmpInputFields()
        {
            foreach (FCU_Meta fieldMeta in inputFields)
            {
                TMP_InputField inputField = fieldMeta.GetComponent<TMP_InputField>();

                InputFieldModel ifm = GetGraphics(fieldMeta);

                inputField.targetGraphic = ifm.Background;
                inputField.placeholder = ifm.Placeholder;
                inputField.textComponent = (TextMeshProUGUI)ifm.TextComponent;

                inputField.enabled = false;
                yield return new WaitForSecondsRealtime(FCU_Config.Instance.Delay001);
                inputField.enabled = true;
            }
        }
        private InputFieldModel GetGraphics(FCU_Meta fieldMeta)
        {
            FCU_Meta[] childMetas = fieldMeta.GetComponentsInChildren<FCU_Meta>().Skip(1).ToArray();

            InputFieldModel ifm = new InputFieldModel
            {
                TextComponent = null,
                Background = null,
                Placeholder = null
            };

            foreach (FCU_Meta meta in childMetas)
            {
                bool exists = meta.TryGetComponent(out Graphic graphic);

                if (exists == false)
                {
                    continue;
                }

                if (ifm.Placeholder == null)
                {
                    if (meta.Tags.Contains(FCU_Tag.Placeholder))
                    {
                        ifm.Placeholder = graphic;
                        break;
                    }
                }
            }

            foreach (FCU_Meta meta in childMetas)
            {
                bool exists = meta.TryGetComponent(out Graphic graphic);

                if (exists == false)
                {
                    continue;
                }

                if (ifm.TextComponent == null)
                {
                    if (meta.Tags.Contains(FCU_Tag.Text) && graphic != ifm.Placeholder)
                    {
                        ifm.TextComponent = graphic;
                        break;
                    }
                }
            }

            foreach (FCU_Meta meta in childMetas)
            {
                bool exists = meta.TryGetComponent(out Graphic graphic);

                if (exists == false)
                {
                    continue;
                }

                if (ifm.Background == null && graphic != ifm.Placeholder && graphic != ifm.TextComponent)
                {
                    if (meta.Tags.Contains(FCU_Tag.Image))
                    {
                        ifm.Background = graphic;
                        break;
                    }
                }
            }

            if (ifm.Background == null)
            {
                bool exists = fieldMeta.TryGetComponent(out Graphic graphic);

                if (exists)
                {
                    ifm.Background = graphic;
                }
            }

            return ifm;
        }
    }
    public struct InputFieldModel
    {
        public Graphic Background;
        public Graphic TextComponent;
        public Graphic Placeholder;
    }
}