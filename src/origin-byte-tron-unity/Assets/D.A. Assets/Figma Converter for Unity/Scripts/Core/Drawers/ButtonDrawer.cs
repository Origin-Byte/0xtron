using DA_Assets.FCU.Config;
using DA_Assets.FCU.Extensions;
using DA_Assets.FCU.Model;
using DA_Assets.FCU.UI;
using DA_Assets.Shared;
using DA_Assets.Shared.CodeHelpers;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace DA_Assets.FCU.Core.Drawers
{
    [Serializable]
    public class ButtonDrawer : ControllerHolder<FigmaConverterUnity>
    {
        [SerializeField] List<FCU_Meta> buttons;
        public void Init()
        {
            buttons = new List<FCU_Meta>();
        }
        public IEnumerator Draw(FObject fobject)
        {
            List<FObject> btnChilds = new List<FObject>();

            foreach (FObject child in fobject.Children)
            {
                if (child.FixedName.ContainsButtonState(out ButtonState buttonState))
                {
                    btnChilds.Add(child);
                }
            }

            bool equivalent = btnChilds.GroupBy(x => x.ContainsTag(FCU_Tag.Text) == false && x.DownloadableFile).Count() == 1;
            bool defaultButtonExists = fobject.GameObject.TryGetComponent(out Button btn);

            if (btnChilds.Count() > 0 && equivalent && defaultButtonExists == false)
            {
                yield return controller.Model.DynamicCoroutine(DrawFCU_Button(fobject));
            }
            else
            {
                yield return controller.Model.DynamicCoroutine(DrawDefaultButton(fobject));
            }

            buttons.Add(fobject.Meta);
        }

        public IEnumerator SetTargetGraphics()
        {
            foreach (FCU_Meta btnMeta in buttons)
            {
                FCU_Meta[] childMetas = btnMeta.GetComponentsInChildren<FCU_Meta>().Skip(1).ToArray();

                if (childMetas.Length == 0)
                {
                    continue;
                }

                if (btnMeta.ButtonComponent == ButtonComponent.Default)
                {
                    Button btn = btnMeta.gameObject.GetComponent<Button>();

                    SetButtonTargetGraphic(btn, childMetas);
                }
                else if (btnMeta.ButtonComponent == ButtonComponent.FCU_Button)
                {
                    FCU_Button fcuBtn = btnMeta.gameObject.GetComponent<FCU_Button>();

                    Set_FCU_ButtonTargetGraphic(fcuBtn, childMetas);
                }

                yield return new WaitForSecondsRealtime(FCU_Config.Instance.Delay001);
            }

            buttons.Clear();
        }
        private void SetButtonTargetGraphic(Button btn, FCU_Meta[] metas)
        {
            bool exists = metas.First().TryGetComponent(out Graphic gr1);

            //If the first element of the hierarchy can be used as a target graphic.
            if (exists)
            {
                btn.targetGraphic = gr1;
            }
            else
            {
                //If there is at least some image, assign it to the targetGraphic.
                foreach (FCU_Meta meta in metas)
                {
                    if (meta.TryGetComponent(out Image gr2))
                    {
                        btn.targetGraphic = gr2;
                        return;
                    }
                }

                //If there is at least some graphic, assign it to the targetGraphic.
                foreach (FCU_Meta meta in metas)
                {
                    if (meta.TryGetComponent(out Graphic gr3))
                    {
                        btn.targetGraphic = gr3;
                        return;
                    }
                }

                //If there is a graphic on the button itself, assign it to the targetGraphic.
                if (btn.TryGetComponent(out Graphic gr4))
                {
                    btn.targetGraphic = gr4;
                }
            }
        }
        private void Set_FCU_ButtonTargetGraphic(FCU_Button fcuBtn, FCU_Meta[] childMetas)
        {
            foreach (var childMeta in childMetas)
            {
                if (childMeta.FixedName.ContainsButtonState(out ButtonState buttonState) == false)
                {
                    continue;
                }

                if (childMeta.Tags.Contains(FCU_Tag.Image))
                {
                    if (buttonState == ButtonState.Default)
                    {
                        fcuBtn.targetGraphic = childMeta.GetComponent<Graphic>();
                    }

                    if (childMeta.IsDownloadable)
                    {
                        fcuBtn.transition = Selectable.Transition.SpriteSwap;

                        SpriteState spriteState = fcuBtn.spriteState;

                        switch (buttonState)
                        {
                            case ButtonState.Hover:
                                spriteState.highlightedSprite = childMeta.GetComponent<Image>()?.sprite;
                                childMeta.gameObject.Destroy();
                                break;
                            case ButtonState.Pressed:
                                spriteState.pressedSprite = childMeta.GetComponent<Image>()?.sprite;
                                childMeta.gameObject.Destroy();
                                break;
                            case ButtonState.Selected:
                                spriteState.selectedSprite = childMeta.GetComponent<Image>()?.sprite;
                                childMeta.gameObject.Destroy();
                                break;
                            case ButtonState.Disabled:
                                spriteState.disabledSprite = childMeta.GetComponent<Image>()?.sprite;
                                childMeta.gameObject.Destroy();
                                break;
                        }

                        fcuBtn.spriteState = spriteState;
                    }
                    else
                    {
                        fcuBtn.transition = Selectable.Transition.ColorTint;

                        ColorBlock colorBlock = fcuBtn.colors;

                        switch (buttonState)
                        {
                            case ButtonState.Hover:
                                colorBlock.highlightedColor = childMeta.GetComponent<Image>().color;
                                childMeta.gameObject.Destroy();
                                break;
                            case ButtonState.Pressed:
                                childMeta.gameObject.Destroy();
                                break;
                            case ButtonState.Selected:
                                colorBlock.selectedColor = childMeta.GetComponent<Image>().color;
                                childMeta.gameObject.Destroy();
                                break;
                            case ButtonState.Disabled:
                                colorBlock.disabledColor = childMeta.GetComponent<Image>().color;
                                childMeta.gameObject.Destroy();
                                break;
                        }

                        fcuBtn.colors = colorBlock;
                    }
                }
                else if (childMeta.Tags.Contains(FCU_Tag.Text))
                {
                    if (buttonState == ButtonState.Default)
                    {
                        fcuBtn.buttonText = childMeta.GetComponent<Text>();
                    }
                }

                if (buttonState != ButtonState.Default)
                {
                    try
                    {
                        childMeta.gameObject.Destroy();
                    }
                    catch
                    {

                    }
                }
            }
        }
        public IEnumerator DrawDefaultButton(FObject fobject)
        {
            controller.Log($"InstantiateButton | Button | {fobject.FixedName}");
            fobject.GameObject.TryAddComponent(out Button btn);
            yield return null;
        }

        public IEnumerator DrawFCU_Button(FObject fobject)
        {
            controller.Log($"InstantiateButton | FCU_Button | {fobject.FixedName}");

            fobject.GameObject.TryAddComponent(out FCU_Button btn);

            fobject.Meta.ButtonComponent = ButtonComponent.FCU_Button;

            foreach (FObject child in fobject.Children)
            {
                if (child.FixedName.ContainsButtonState(out ButtonState buttonState))
                {
                    if (child.ContainsTag(FCU_Tag.Text))
                    {
                        switch (buttonState)
                        {
                            case ButtonState.Default:
                                btn.textDefaultColor = child.GetTextColor();
                                fobject.Meta.Text = child.Meta;
                                break;
                            case ButtonState.Hover:
                                btn.textHoverColor = child.GetTextColor();
                                break;
                            case ButtonState.Pressed:
                                btn.textPressedColor = child.GetTextColor();
                                break;
                            case ButtonState.Selected:
                                btn.textSelectedColor = child.GetTextColor();
                                break;
                            case ButtonState.Disabled:
                                btn.textDisabledColor = child.GetTextColor();
                                break;
                        }
                    }
                }
            }

            yield break;
        }
    }
}
