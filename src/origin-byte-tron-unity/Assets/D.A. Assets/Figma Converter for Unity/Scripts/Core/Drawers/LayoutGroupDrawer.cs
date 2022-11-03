using DA_Assets.FCU.Config;
using DA_Assets.FCU.Extensions;
using DA_Assets.FCU.Model;
using DA_Assets.Shared;
using DA_Assets.Shared.CodeHelpers;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using Console = DA_Assets.Shared.Console;

namespace DA_Assets.FCU.Core.Drawers
{
    [Serializable]
    public class LayoutGroupDrawer : ControllerHolder<FigmaConverterUnity>
    {
        public IEnumerator Draw(FObject fobject, Return<MonoBehaviour> @return = null, FObject customDrawFObject = null)
        {
            MonoBehaviour result = null;

            foreach (FCU_Tag tag in fobject.Tags)
            {
                switch (tag)
                {
                    case FCU_Tag.HorLayoutGroup:
                    case FCU_Tag.VertLayoutGroup:
                        result = DrawHorizontalOrVerticalLayoutGroup(fobject);
                        break;
                    case FCU_Tag.GridLayoutGroup:
                        result = DrawGridLayoutGroup(fobject);
                        break;
                    case FCU_Tag.LayoutElement:
                        result = DrawLayoutElement(fobject);
                        break;
                }
            }

            @return?.Invoke(new CoroutineResult<MonoBehaviour>
            {
                Result = result
            });

            yield break;
        }
        private LayoutElement DrawLayoutElement(FObject fobject)
        {
            fobject.GameObject.TryAddComponent(out LayoutElement layoutElement);

            layoutElement.preferredWidth = fobject.Size.x;
            layoutElement.preferredHeight = fobject.Size.y;

            return layoutElement;
        }
        private HorizontalOrVerticalLayoutGroup DrawHorizontalOrVerticalLayoutGroup(FObject fobject)
        {
            HorizontalOrVerticalLayoutGroup hvGroup = null;

            foreach (FCU_Tag tag in fobject.Tags)
            {
                switch (tag)
                {
                    case FCU_Tag.HorLayoutGroup:
                        fobject.GameObject.TryAddComponent(out HorizontalLayoutGroup hvGroup1);
                        hvGroup = hvGroup1;
                        break;
                    case FCU_Tag.VertLayoutGroup:
                        fobject.GameObject.TryAddComponent(out VerticalLayoutGroup hvGroup2);
                        hvGroup = hvGroup2;
                        break;
                }
            }

            hvGroup.spacing = fobject.ItemSpacing;
            hvGroup.padding = new RectOffset
            {
                bottom = (int)Mathf.Round(fobject.PaddingBottom),
                top = (int)Mathf.Round(fobject.PaddingTop),
                left = (int)Mathf.Round(fobject.PaddingLeft),
                right = (int)Mathf.Round(fobject.PaddingRight)
            };

            hvGroup.childControlWidth = true;
            hvGroup.childControlHeight = true;
            hvGroup.childForceExpandWidth = true;
            hvGroup.childForceExpandHeight = true;
            hvGroup.childAlignment = fobject.GetChildAligment();

            return hvGroup;
        }
        private GridLayoutGroup DrawGridLayoutGroup(FObject fobject)
        {
            GridLayoutGroup glGroup;

            if (fobject.GameObject.TryGetComponent(out glGroup) == false)
            {
                glGroup = fobject.GameObject.AddComponent<GridLayoutGroup>();
            }

            glGroup.childAlignment = TextAnchor.MiddleCenter;

            try
            {
                string[] nameParts = fobject.FixedName.Split(FCU_Config.Instance.RealTagSeparator);
                string[] spacingArray = nameParts[nameParts.Length - 1].Split("x");
                string[] cellSizeArray = nameParts[nameParts.Length - 2].Split("x");

                int spacingX = Convert.ToInt32(spacingArray[0]);
                int spacingY = Convert.ToInt32(spacingArray[1]);

                int cellSizeX = Convert.ToInt32(cellSizeArray[0]);
                int cellSizeY = Convert.ToInt32(cellSizeArray[1]);

                glGroup.spacing = new Vector2(spacingX, spacingY);
                glGroup.cellSize = new Vector2(cellSizeX, cellSizeY);
            }
            catch (Exception ex)
            {
                Console.LogError(ex);
            }

            return glGroup;
        }
    }
}
