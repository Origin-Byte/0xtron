using DA_Assets.FCU.Model;
using DA_Assets.Shared.CodeHelpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;

namespace DA_Assets.Shared
{
    public class DA_GUI_Editor : Editor
    {
        public GUISkin guiSkin;

        public Texture2D imgLogo;
        public Texture2D imgViewRecent;
        public Texture2D imgExpandClosed;
        public Texture2D imgExpandOpened;
        public Texture2D iconFAQ;
        public Texture2D iconSupport;
        public int SMALL_SPACE = 5, FIELD_SPACE2 = 6, NORMAL_SPACE = 15, BIG_SPACE = 30;
        #region BASE_GUI_COMPONENTS
        /// <summary>
        /// Method to simplify work with GUI-groups.
        /// </summary>
        public void DrawGroup(Group group)
        {
            switch (group.GroupType)
            {
                case GroupType.Horizontal:
                    if (group.Style != CustomStyle.None)
                    {
                        GUILayout.BeginHorizontal(GetStyle(group.Style));
                    }
                    else
                    {
                        GUILayout.BeginHorizontal();
                    }

                    group.Body.Invoke();
                    GUILayout.EndHorizontal();
                    break;
                case GroupType.Vertical:
                    if (group.Style != CustomStyle.None)
                    {
                        GUILayout.BeginVertical(GetStyle(group.Style));
                    }
                    else
                    {
                        GUILayout.BeginVertical();
                    }

                    group.Body.Invoke();
                    GUILayout.EndVertical();
                    break;
            }
        }
        public bool CheckBox(GUIContent label, bool value)
        {
            bool _value;

            GUILayout.BeginHorizontal(GetStyle(CustomStyle.BOX_NO_FRAME1));

            int width = Screen.width - 121;

            GUILayout.Label(label, GetStyle(CustomStyle.HCB_BUTTON3), GUILayout.MaxWidth(width));

            Rect rect = GUILayoutUtility.GetRect(0, 0, 25, 25);

            _value = EditorGUI.Toggle(
                rect,
                value,
                EditorStyles.toggle);

            GUILayout.EndHorizontal();

            return _value;
        }
        public void DrawMenu(ref List<HamburgerItem> buffer, HamburgerItem menu)
        {
            int index = buffer.FindIndex(x => x.Id == menu.Id);

            if (index < 0)
            {
                buffer.Add(menu);
            }

            index = buffer.FindIndex(x => x.Id == menu.Id);

            bool value;

            GUILayout.BeginHorizontal(GetStyle(CustomStyle.BOX_FRAME2));

            Texture2D t2d;

            if (buffer[index].State)
            {
                t2d = imgExpandClosed;
            }
            else
            {
                t2d = imgExpandOpened;
            }

            GUILayout.Box(t2d, GUILayout.Width(20), GUILayout.Width(20));

            int width = 0;

            if (menu.AddCheckBox)
            {
                if (menu.DrawnInsideLayoutGroup)
                {
                    width = Screen.width - 141;
                }
                else
                {
                    width = Screen.width - 111;
                }

                if (GUILayout.Button(menu.GUIContent, GetStyle(CustomStyle.HCB_BUTTON2), GUILayout.MaxWidth(width)))
                {
                    buffer[index].State = !buffer[index].State;
                }

                GUILayout.Space(8);
                Rect rect = GUILayoutUtility.GetRect(0, 0, 25, 25);
                rect.x += 0;
                rect.y += 2;

                buffer[index].CheckBoxValue = EditorGUI.Toggle(
                    rect,
                    buffer[index].CheckBoxValue,
                    EditorStyles.toggle);

                if (menu.CheckBoxValueChanged != null)
                {
                    if (buffer[index].CheckBoxValue != buffer[index].CheckBoxValueTemp)
                    {
                        buffer[index].CheckBoxValueTemp = buffer[index].CheckBoxValue;
                        menu.CheckBoxValueChanged.Invoke(menu.Id, buffer[index].CheckBoxValue);
                    }
                }
            }
            else
            {
                width = Screen.width - 79;

                if (GUILayout.Button(menu.GUIContent, GetStyle(CustomStyle.HCB_BUTTON), GUILayout.MaxWidth(width)))
                {
                    buffer[index].State = !buffer[index].State;
                }
            }

            GUILayout.EndHorizontal();

            value = buffer[index].State;

            GUILayout.BeginHorizontal();
            GUILayout.Space(NORMAL_SPACE);
            GUILayout.BeginVertical();

            if (value)
            {
                GUILayout.Space(NORMAL_SPACE);
                menu.Body.Invoke();
                GUILayout.Space(NORMAL_SPACE);
            }

            GUILayout.EndVertical();
            GUILayout.Space(NORMAL_SPACE);
            GUILayout.EndHorizontal();
        }
        public void TopProgressBar(float value)
        {
            Rect position = GUILayoutUtility.GetRect(0, SMALL_SPACE);
            position.y -= NORMAL_SPACE;
            position.width = Screen.width;
            position.x -= (NORMAL_SPACE + 4);

            GUIStyle pbarBG = GetStyle(CustomStyle.BOX_NO_FRAME);
            GUIStyle pbarBody = GetStyle(CustomStyle.PBAR_BAR);

            int controlId = GUIUtility.GetControlID(nameof(TopProgressBar).GetHashCode(), FocusType.Keyboard);

            if (Event.current.GetTypeForControl(controlId) == EventType.Repaint)
            {
                if (value > 0.0f)
                {
                    Rect barRect = new Rect(position);
                    barRect.width *= value;
                    pbarBody.Draw(barRect, false, false, false, false);
                }
            }
        }
        public Vector2 Vector2Field(GUIContent label, Vector2 currentValue)
        {
            DrawGroup(new Group
            {
                GroupType = GroupType.Horizontal,
                Body = () =>
                {
                    GUILayout.Label(label, guiSkin.label, GUILayout.Width(EditorGUIUtility.labelWidth));
                    currentValue.x = EditorGUILayout.FloatField(currentValue.x, guiSkin.customStyles[4]);
                    GUILayout.Space(SMALL_SPACE);
                    currentValue.y = EditorGUILayout.FloatField(currentValue.y, guiSkin.customStyles[4]);
                }
            });

            return currentValue;
        }

        public T EnumField<T>(GUIContent label, T @enum, bool uppercase = true, string[] itemNames = null)
        {
            if (itemNames == null)
            {
                itemNames = Enum.GetNames(@enum.GetType());
            }

            if (uppercase)
            {
                for (int i = 0; i < itemNames.Length; i++)
                {
                    itemNames[i] = Regex.Replace(itemNames[i], "(\\B[A-Z])", "$1").ToUpper();
                }
            }
            else
            {
                for (int i = 0; i < itemNames.Length; i++)
                {
                    itemNames[i] = itemNames[i].Replace("_", " ");
                }
            }

            int result = 0;

            GUILayout.Space(FIELD_SPACE2);

            DrawGroup(new Group
            {
                GroupType = GroupType.Horizontal,
                Body = () =>
                {
                    if (label != null)
                    {
                        Label(label);
                    }

                    int _result = EditorGUILayout.Popup(System.Convert.ToInt32(@enum), itemNames, GetStyle(CustomStyle.ENUM));
                    result = _result;
                }
            });

            GUILayout.Space(FIELD_SPACE2);

            return (T)(object)result;
        }
        public bool Toggle(GUIContent label, bool toggleValue, GUIContent btnLabel = null, Action buttonClick = null)
        {
            int option = toggleValue ? 1 : 0;

            GUILayout.Space(FIELD_SPACE2);

            DrawGroup(new Group
            {
                GroupType = GroupType.Horizontal,
                Body = () =>
                {
                    Label(label);

                    option = EditorGUILayout.Popup(option, new string[]
                    {
                        "DISABLED",
                        "ENABLED"
                    }, GetStyle(CustomStyle.TOGGLE));

                    if (buttonClick != null)
                    {
                        GUILayout.Space(SMALL_SPACE * 2);

                        if (GUILayout.Button(btnLabel, GetStyle(CustomStyle.SMALL_BUTTON_WITH_TEXT)))
                        {
                            buttonClick.Invoke();
                        }
                    }
                }
            });

            GUILayout.Space(FIELD_SPACE2);

            return option == 1;
        }
        public string TextField(GUIContent label, string currentValue, GUIContent btnLabel = null, Action buttonClick = null)
        {
            GUILayout.Space(FIELD_SPACE2);

            DrawGroup(new Group
            {
                GroupType = GroupType.Horizontal,
                Body = () =>
                {
                    Label(label);
                    currentValue = EditorGUILayout.TextField(currentValue, GetStyle(CustomStyle.TEXT_FIELD));

                    if (buttonClick != null)
                    {
                        GUILayout.Space(NORMAL_SPACE - 5);

                        GUIStyle style;

                        if (btnLabel.image == null)
                        {
                            style = GetStyle(CustomStyle.SMALL_BUTTON_WITH_TEXT);
                        }
                        else
                        {
                            style = GetStyle(CustomStyle.SMALL_BUTTON_WITH_IMG);
                        }

                        if (GUILayout.Button(btnLabel, style))
                        {
                            buttonClick.Invoke();
                        }
                    }
                }
            });

            GUILayout.Space(FIELD_SPACE2);

            return currentValue;
        }
        public void Label(GUIContent label, int customSize = 0, bool widthOption = true)
        {
            GUIStyle style = GetStyle(CustomStyle.LABEL);

            int oldSize = 12;

            if (customSize != 0)
            {
                style.fontSize = customSize;
            }

            if (widthOption)
            {
                GUILayout.Label(label, style, GUILayout.Width(EditorGUIUtility.labelWidth));
            }
            else
            {
                GUILayout.Label(label, style);
            }

            style.fontSize = oldSize;
        }
        public float FloatField(GUIContent label, float currentValue)
        {
            GUILayout.Space(FIELD_SPACE2);

            DrawGroup(new Group
            {
                GroupType = GroupType.Horizontal,
                Body = () =>
                {
                    Label(label);
                    currentValue = EditorGUILayout.FloatField(currentValue, GetStyle(CustomStyle.FLOAT));
                }
            });

            GUILayout.Space(FIELD_SPACE2);

            return currentValue;
        }
        public bool HamburgerButton(GUIContent label, ref bool groupFoldState)
        {
            if (Button(label, false))
            {
                groupFoldState = !groupFoldState;
            }

            return groupFoldState;
        }
        public bool Button(GUIContent label, bool halfSize = true, CustomStyle customStyle = CustomStyle.BOX_FRAME, int customFontSize = 0)
        {
            bool clicked = false;

            DrawGroup(new Group
            {
                GroupType = GroupType.Horizontal,
                Body = () =>
                {
                    GUILayout.FlexibleSpace();
                    float inspectorWidth = Screen.width;

                    if (halfSize)
                    {
                        inspectorWidth /= 2;
                    }

                    GUIStyle style = GetStyle(customStyle);

                    int oldSize = style.fontSize;

                    if (customFontSize != 0)
                    {
                        style.fontSize = customFontSize;
                    }

                    clicked = GUILayout.Button(label, style, GUILayout.MaxWidth(inspectorWidth));
                    style.fontSize = oldSize;
                    GUILayout.FlexibleSpace();
                }
            });

            return clicked;
        }
        public void SerializedPropertyField<T>(SerializedObject so, Expression<Func<T, object>> pathExpression)
        {
            string[] fields = UnityCodeHelpers.GetFieldsArray(pathExpression);

            DrawGroup(new Group
            {
                GroupType = GroupType.Horizontal,
                Body = () =>
                {
                    GUILayout.Space(NORMAL_SPACE - 1);

                    DrawGroup(new Group
                    {
                        GroupType = GroupType.Vertical,
                        Body = () =>
                        {
                            GUI.backgroundColor = Color.gray;

                            SerializedProperty property = so.FindProperty(fields[0]);

                            if (fields.Length > 1)
                            {
                                for (int i = 1; i < fields.Length; i++)
                                {
                                    SerializedProperty rprop = property.FindPropertyRelative(fields[i]);

                                    if (rprop != null)
                                    {

                                        property = rprop;
                                    }
                                    else
                                    {

                                        break;
                                    }
                                }
                            }

                            so.Update();
                            EditorGUILayout.PropertyField(property, true);
                            so.ApplyModifiedProperties();

                            GUI.backgroundColor = Color.white;
                        }
                    });
                }
            });
        }
        #endregion
        public GUIStyle GetStyle(CustomStyle customStyle)
        {
            GUIStyle style;

            switch (customStyle)
            {
                case CustomStyle.TEXT_FIELD:
                case CustomStyle.TOGGLE:
                case CustomStyle.ENUM:
                case CustomStyle.FLOAT:
                    style = GetCustomStyle(CustomStyle.TEXT_FIELD.ToString());
                    break;
                default:
                    style = GetCustomStyle(customStyle.ToString());
                    break;
            }

            return style;
        }
        private GUIStyle GetCustomStyle(string styleName)
        {
            foreach (GUIStyle style in guiSkin.customStyles)
            {
                if (style.name == styleName)
                {
                    return style;
                }
            }

            Debug.Log("Custom style not found");
            return guiSkin.button;
        }
    }
}
public enum CustomStyle
{
    None,
    BG,
    LABEL,
    BOX_FRAME,
    BOX_FRAME2,
    BOX_NO_FRAME,
    BOX_NO_FRAME1,
    PBAR_BAR,
    TEXT_FIELD,
    TOGGLE,
    ENUM,
    FLOAT,
    SMALL_BUTTON_WITH_IMG,
    HCB_BUTTON,
    HCB_BUTTON2,
    HCB_BUTTON3,
    SMALL_BUTTON_WITH_TEXT
}
public enum GroupType
{
    Horizontal,
    Vertical,
}
public struct Group
{
    public GUIContent GUIContent { get; set; }
    public GroupType GroupType { get; set; }
    public Action Body { get; set; }
    public CustomStyle Style { get; set; }
}
