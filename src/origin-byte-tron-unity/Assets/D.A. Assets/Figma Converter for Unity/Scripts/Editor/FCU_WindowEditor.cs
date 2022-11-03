using DA_Assets;
using DA_Assets.FCU.Config;
using DA_Assets.FCU.Core;
using DA_Assets.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;

namespace DA_Assets.FCU
{
    public class FCU_WindowEditor : EditorWindow
    {
        private static Vector2 windowSize = new Vector2(300, 240);
        private Vector2 referenceResolution = new Vector2(0, 0);
        private CanvasResolution canvasResolution;
        private Orientation orientation;
        private DA_GUI_Editor gui;
        [MenuItem("Tools/" + FCU_Config.ProductName + "/Create " + FCU_Config.ProductName)]
        public static void ShowWindow()
        {
            FCU_WindowEditor ccw = GetWindow<FCU_WindowEditor>($"Create {FCU_Config.ProductName}");
            ccw.maxSize = windowSize;
            ccw.minSize = windowSize;

            ccw.position = new Rect(
                (Screen.currentResolution.width - windowSize.x * 2) / 2,
                (Screen.currentResolution.height - windowSize.y * 2) / 2,
                windowSize.x,
                windowSize.y);
        }

        private void OnEnable()
        {
            if (gui == null)
            {
                gui = CreateInstance<DA_GUI_Editor>();
            }
        }
        private void OnGUI()
        {
            gui.DrawGroup(new Group
            {
                GroupType = GroupType.Vertical,
                Style = CustomStyle.BG,
                Body = () =>
                {
                    GUILayout.Label($"You can create a GameObject with\n{FCU_Config.ProductName} script, Canvas,\nCanvas Scaler and Graphic Raycaster", EditorStyles.boldLabel);
                    EditorGUILayout.Space(gui.NORMAL_SPACE);

                    gui.DrawGroup(new Group
                    {
                        GroupType = GroupType.Horizontal,
                        Body = () =>
                        {
                            GUILayout.Label($"Reference Resolution", EditorStyles.boldLabel);
                            EditorGUILayout.Space(gui.NORMAL_SPACE);
                            canvasResolution = gui.EnumField(null, canvasResolution, false);
                        }
                    });
                    EditorGUILayout.Space(gui.NORMAL_SPACE);
                    gui.DrawGroup(new Group
                    {
                        GroupType = GroupType.Horizontal,
                        Body = () =>
                        {
                            GUILayout.Label($"Orientation", EditorStyles.boldLabel);
                            EditorGUILayout.Space(gui.NORMAL_SPACE);
                            orientation = gui.EnumField(null, orientation);
                        }
                    });

                    EditorGUILayout.Space(gui.NORMAL_SPACE);
                    referenceResolution = EditorGUILayout.Vector2Field("Your own value", referenceResolution);
                    EditorGUILayout.Space(gui.NORMAL_SPACE);

                    if (gui.Button(new GUIContent("Create Canvas"), true))
                    {
                        if (referenceResolution != new Vector2(0, 0))
                        {
                            switch (orientation)
                            {
                                case Orientation.Portrait:
                                    CanvasDrawer.TryInstantiateCanvas(new Vector2(referenceResolution.y, referenceResolution.x));
                                    break;
                                case Orientation.Landscape:
                                    CanvasDrawer.TryInstantiateCanvas(new Vector2(referenceResolution.x, referenceResolution.y));
                                    break;
                            }
                        }
                        else
                        {
                            string[] parsedResolution = canvasResolution.ToString().Replace("_", "").Split('x');

                            int x = Convert.ToInt32(parsedResolution[0]);
                            int y = Convert.ToInt32(parsedResolution[1]);

                            switch (orientation)
                            {
                                case Orientation.Portrait:
                                    CanvasDrawer.TryInstantiateCanvas(new Vector2(y, x));
                                    break;
                                case Orientation.Landscape:
                                    CanvasDrawer.TryInstantiateCanvas(new Vector2(x, y));
                                    break;
                            }

                        }

                        CanvasDrawer.TryInstantiateEventSystem();
                    }
                }
            });
        }
        private enum CanvasResolution
        {
            _812x375,
#if UNITY_ANDROID
            _1920x1080,
            _2160x1080,
            _2560x1440,
            _2960x1440,
#elif UNITY_IOS
            iPhone_1334x750,
            iPhoneXS_2436x1125,
            iPhoneXR_1792x828,
            iPhoneXSMax_2688x1242,
            iPad_2048x1536,
            iPadPro_2732x2048,
            iPadPro_2224x1668
#endif
        }
        private enum Orientation
        {
            Portrait,
            Landscape
        }
    }
}
