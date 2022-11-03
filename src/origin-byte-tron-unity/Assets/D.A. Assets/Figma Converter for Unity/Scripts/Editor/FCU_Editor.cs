using DA_Assets.FCU.Config;
using DA_Assets.FCU.Core;
using DA_Assets.FCU.Extensions;
using DA_Assets.FCU.Model;
using DA_Assets.Shared;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using DA_Assets.Shared.CodeHelpers;

#pragma warning disable IDE0003

namespace DA_Assets.FCU
{
    [CustomEditor(typeof(FigmaConverterUnity)), CanEditMultipleObjects]
    internal class FCU_Editor : Editor
    {
        private FigmaConverterUnity fcu;
        private DA_GUI_Editor gui;
        private AssetFinder assetFinder = new AssetFinder();
        private void OnEnable()
        {
            fcu = (FigmaConverterUnity)target;

            CreateEditorGUI();

            fcu.Model.DynamicCoroutine = StartAndAddToPool;
            fcu.Model.StopAllCoroutines = StopAllCoroutines();

            StartRepaintingAssetUI();

            fcu.Model.GetGameViewSize = GameViewUtils.GetGameViewSize;
            fcu.Model.SetGameViewSize = GameViewUtils.SetGameViewSize;

            fcu.Model.DynamicCoroutine(fcu.Cacher.TryRestoreProjectFromCache());
            fcu.Model.DynamicCoroutine(this.AssetFinder.Init());

            fcu.CanvasDrawer.Init();
        }
        public AssetFinder AssetFinder
        {
            get
            {
                return assetFinder.SetController(fcu);
            }
        }
        public override void OnInspectorGUI()
        {
            gui.DrawGroup(new Group
            {
                GroupType = GroupType.Vertical,
                Style = CustomStyle.BG,
                Body = () =>
                {
                    DrawAssetHeader();
                    GUILayout.Space(gui.NORMAL_SPACE);
                    DrawMainSettings();

                    switch (fcu.Model.MainSettings.ImageComponent)
                    {
                        case ImageComponent.ProceduralImage:
                            DrawPUI_Settings();
                            break;
                        case ImageComponent.MPImage:
                            DrawMPImageSettings();
                            break;
                    }

                    switch (fcu.Model.MainSettings.TextComponent)
                    {
                        case TextComponent.TextMeshPro:
                            DrawTextMeshProSettings();
                            break;
                        default:
                            DrawDefaultTextSettings();
                            break;
                    }

                    DrawCustomPrefabs();

                    DrawAssetTools();
                    DrawDefines();
                    GUILayout.Space(gui.NORMAL_SPACE);

                    DrawFramesList();

                    GUILayout.Space(gui.NORMAL_SPACE);

                    if (fcu.IsAuthed() && fcu.IsProjectUrlExists())
                    {
                        if (gui.Button(new GUIContent(LocKey.label_download_project.Localize())))
                        {
                            fcu.DownloadProject_OnClick();
                        }
                    }

                    if (fcu.Model.SelectableFrames.Count() > 0)
                    {
                        GUILayout.Space(gui.NORMAL_SPACE);
                        if (gui.Button(new GUIContent(LocKey.label_import_frames.Localize())))
                        {
                            fcu.ImportSelectedFrames_OnClick();
                        }
                    }

                    Footer();
                }
            });

            if (fcu.Model.MainSettings.DebugMode)
            {
                GUILayout.Space(gui.NORMAL_SPACE);
                base.OnInspectorGUI();
            }
        }
        private void DrawAssetHeader()
        {
            gui.TopProgressBar(fcu.WebClient.PbarProgress);

            GUILayout.BeginVertical(gui.imgLogo, gui.GetStyle(CustomStyle.BOX_NO_FRAME));
            GUILayout.Space(gui.BIG_SPACE * 2f);
            GUILayout.EndVertical();

            gui.DrawGroup(new Group
            {
                GroupType = GroupType.Vertical,
                Style = CustomStyle.BOX_NO_FRAME,
                Body = () =>
                {
                    GUILayout.Space(gui.BIG_SPACE * -0.8f);
                    gui.DrawGroup(new Group
                    {
                        GroupType = GroupType.Horizontal,
                        Body = () =>
                        {
                            GUILayout.FlexibleSpace();

                            gui.Label(new GUIContent($"{Mathf.Round(fcu.WebClient.PbarBytes / 1024)} kB", LocKey.label_kilobytes.Localize()), 10, false);
                            GUILayout.Space(gui.SMALL_SPACE);
                            gui.Label(new GUIContent("—"), 10, false);

                            string userId = fcu.Model.MainSettings.CurrentFigmaUser.Id.SubstringSafe(10);
                            string userName = fcu.Model.MainSettings.CurrentFigmaUser.Handle;

                            if (string.IsNullOrWhiteSpace(userName) == false)
                            {
                                GUILayout.Space(gui.SMALL_SPACE);
                                gui.Label(new GUIContent(userName, LocKey.label_user_name.Localize()), 10, false);
                                GUILayout.Space(gui.SMALL_SPACE);
                                gui.Label(new GUIContent("—"), 10, false);
                            }
                            else if (string.IsNullOrWhiteSpace(userId) == false)
                            {
                                GUILayout.Space(gui.SMALL_SPACE);
                                gui.Label(new GUIContent(userId, LocKey.tooltip_user_id.Localize()), 10, false);
                                GUILayout.Space(gui.SMALL_SPACE);
                                gui.Label(new GUIContent("—"), 10, false);
                            }

                            GUILayout.Space(gui.SMALL_SPACE);
                            gui.Label(new GUIContent(fcu.Model.Guid, LocKey.tooltip_asset_instance_id.Localize()), 10, false);
                            GUILayout.Space(gui.SMALL_SPACE);
                            gui.Label(new GUIContent("—"), 10, false);
                            GUILayout.Space(gui.SMALL_SPACE);
                            gui.Label(new GUIContent(FCU_Config.Instance.ProductVersion, LocKey.tooltip_product_version.Localize()), 10, false);
                        }
                    });

                    string currentProjectName = fcu.Model.CurrentProject.Name;

                    if (currentProjectName != null)
                    {
                        gui.DrawGroup(new Group
                        {
                            GroupType = GroupType.Horizontal,
                            Body = () =>
                            {
                                GUILayout.FlexibleSpace();
                                gui.Label(new GUIContent(currentProjectName), 10, false);
                            }
                        });
                    }
                }
            });

        }
        private void ShowRecentProjectsPopup()
        {
            List<CacheMeta> recentProjects = fcu.Cacher.GetAll();

            foreach (CacheMeta project in recentProjects)
            {
                gui.Button(new GUIContent(LocKey.label_no_recent_projects.Localize()), true);
            }

            List<GUIContent> options = new List<GUIContent>();

            foreach (CacheMeta project in recentProjects)
            {
                options.Add(new GUIContent(project.Name));
            }

            if (options.Count() < 1)
            {
                options.Add(new GUIContent(LocKey.label_no_recent_projects.Localize()));
            }

            EditorUtility.DisplayCustomMenu(new Rect(0, 250, 0, 0), options.ToArray(), -1, (userData, ops, selected) =>
            {
                fcu.Model.MainSettings.ProjectUrl = recentProjects[selected].Url;
                fcu.Model.DynamicCoroutine(fcu.Cacher.TryRestoreProjectFromCache(recentProjects[selected].Url));

            }, null);
        }
        private void DrawMainSettings()
        {
            gui.DrawMenu(ref fcu.Model.SelectableHamburgerItems, new HamburgerItem
            {
                Id = FCU_MenuId.MAIN_SETTINGS_KEY.ToString(),
                GUIContent = new GUIContent(LocKey.label_main_settings.Localize(), ""),
                Body = () =>
                {
                    fcu.Model.MainSettings.Token = gui.TextField(
                        new GUIContent(LocKey.label_token.Localize(), LocKey.tooltip_token.Localize()),
                        fcu.Model.MainSettings.Token,
                        new GUIContent(LocKey.label_auth.Localize(), LocKey.tooltip_auth.Localize()), () =>
                    {
                        fcu.Auth_OnClick();
                    });

                    fcu.Model.MainSettings.ProjectUrl = gui.TextField(
                        new GUIContent(LocKey.label_project_url.Localize(), LocKey.tooltip_project_url.Localize()),
                        fcu.Model.MainSettings.ProjectUrl,
                        new GUIContent(gui.imgViewRecent, LocKey.tooltip_recent_projects.Localize()), () =>
                    {
                        ShowRecentProjectsPopup();
                    });

                    fcu.Model.MainSettings.ImageFormat = gui.EnumField(
                        new GUIContent(LocKey.label_images_format.Localize(), LocKey.tooltip_images_format.Localize()),
                        fcu.Model.MainSettings.ImageFormat);

                    fcu.Model.MainSettings.ImageScale = gui.EnumField(
                        new GUIContent(LocKey.label_images_scale.Localize(), LocKey.tooltip_images_scale.Localize()),
                        fcu.Model.MainSettings.ImageScale);

                    fcu.Model.MainSettings.ImageComponent = gui.EnumField(
                        new GUIContent(LocKey.label_image_component.Localize(), LocKey.tooltip_image_component.Localize()),
                        fcu.Model.MainSettings.ImageComponent);

                    fcu.Model.MainSettings.TextComponent = gui.EnumField(
                        new GUIContent(LocKey.label_text_component.Localize(), LocKey.tooltip_text_component.Localize()),
                        fcu.Model.MainSettings.TextComponent);

                    fcu.Model.MainSettings.ShadowComponent = gui.EnumField(
                        new GUIContent(LocKey.label_shadow_type.Localize(), LocKey.tooltip_shadow_type.Localize()),
                        fcu.Model.MainSettings.ShadowComponent);

                    fcu.Model.MainSettings.UseI2Localization = gui.Toggle(
                        new GUIContent(LocKey.label_use_i2localization.Localize(), LocKey.tooltip_use_i2localization.Localize()),
                        fcu.Model.MainSettings.UseI2Localization);

                    fcu.Model.MainSettings.ComponentDetectionType = gui.EnumField(
                        new GUIContent(LocKey.label_component_detection_type.Localize(), LocKey.tooltip_component_detection_type.Localize()),
                        fcu.Model.MainSettings.ComponentDetectionType);

                    FCU_Config.Instance.Use_UIManager = gui.Toggle(
                        new GUIContent(LocKey.label_ui_manager.Localize(), LocKey.tooltip_ui_manager.Localize()),
                        FCU_Config.Instance.Use_UIManager);

                    fcu.Model.MainSettings.RedownloadSprites = gui.Toggle(
                        new GUIContent(LocKey.label_redownload_sprites.Localize(), LocKey.tooltip_redownload_sprites.Localize()),
                        fcu.Model.MainSettings.RedownloadSprites);

                    FCU_Config.Instance.Https = gui.Toggle(
                        new GUIContent(LocKey.label_use_https.Localize(), LocKey.tooltip_use_https.Localize()),
                        FCU_Config.Instance.Https);

                    FCU_Config.Instance.CompressAssetsOnImport = gui.Toggle(
                        new GUIContent(LocKey.label_auto_disable_compress_assets_on_import.Localize(), LocKey.tooltip_auto_disable_compress_assets_on_import.Localize()),
                        FCU_Config.Instance.CompressAssetsOnImport);

                    fcu.Model.MainSettings.DebugMode = gui.Toggle(
                        new GUIContent(LocKey.label_debug_mode.Localize(), LocKey.tooltip_debug_mode.Localize()),
                        fcu.Model.MainSettings.DebugMode);

                    FCU_Config.Instance.Language = gui.EnumField(
                        new GUIContent(LocKey.label_language.Localize(), LocKey.tooltip_language.Localize()),
                        FCU_Config.Instance.Language, uppercase: true, itemNames: FCU_Config.Instance.AssetLanguageNames);
                }
            });
        }
        private void DrawFramesList()
        {
            if (fcu.Model.SelectableFrames.Count() > 0)
            {
                int framesCount = fcu.Model.SelectableFrames.Count();
                int framesToDownloadCount = fcu.Model.SelectableFrames.Where(x => x.Selected == true).Count();

                int index = fcu.Model.SelectableHamburgerItems.FindIndex(row => row.Id == FCU_MenuId.FRAMES_KEY.ToString());

                if (index == -1)
                {

                }
                else if (framesToDownloadCount == 0)
                {
                    fcu.Model.SelectableHamburgerItems[index].CheckBoxValue = false;
                }
                else if (framesToDownloadCount == framesCount)
                {
                    fcu.Model.SelectableHamburgerItems[index].CheckBoxValue = true;
                }
                else if (framesToDownloadCount != framesCount)
                {
                    fcu.Model.SelectableHamburgerItems[index].CheckBoxValue = false;
                    fcu.Model.SelectableHamburgerItems[index].CheckBoxValueTemp = false;
                }

                gui.DrawMenu(ref fcu.Model.SelectableHamburgerItems, new HamburgerItem
                {
                    Id = FCU_MenuId.FRAMES_KEY.ToString(),
                    GUIContent = new GUIContent(LocKey.label_frames_to_download.Localize(framesToDownloadCount, framesCount), ""),
                    AddCheckBox = true,
                    Body = () =>
                    {
                        var pages = fcu.Model.SelectableFrames.GroupBy(x => new { x.ParentId, x.ParentName });

                        foreach (var page in pages)
                        {
                            int currentPageFramesSelectedCount = fcu.Model.SelectableFrames
                                .Where(x => x.ParentId == page.Key.ParentId)
                                .Where(x => x.Selected == true)
                                .Count();

                            int _index = fcu.Model.SelectableHamburgerItems.FindIndex(row => row.Id == page.Key.ParentId);

                            if (_index == -1)
                            {

                            }
                            else if (currentPageFramesSelectedCount == 0)
                            {
                                fcu.Model.SelectableHamburgerItems[_index].CheckBoxValue = false;
                            }
                            else if (framesToDownloadCount == framesCount)
                            {
                                fcu.Model.SelectableHamburgerItems[_index].CheckBoxValue = true;
                            }

                            gui.DrawMenu(ref fcu.Model.SelectableHamburgerItems, new HamburgerItem
                            {
                                AddCheckBox = true,
                                GUIContent = new GUIContent($"{page.Key.ParentName} ({currentPageFramesSelectedCount}/{page.Count()})", ""),
                                Id = page.Key.ParentId,
                                DrawnInsideLayoutGroup = true,
                                Body = () =>
                                {
                                    for (int i = 0; i < page.Count(); i++)
                                    {
                                        page.ToList()[i].Selected = 
                                            gui.CheckBox(new GUIContent(page.ToList()[i].Name), page.ToList()[i].Selected);
                                    }
                                },
                                CheckBoxValueChanged = (menuId, value) =>
                                {
                                    for (int i = 0; i < fcu.Model.SelectableFrames.Count(); i++)
                                    {
                                        if (fcu.Model.SelectableFrames[i].ParentId == menuId)
                                        {
                                            fcu.Model.SelectableFrames[i].Selected = value;
                                        }
                                    }
                                }
                            });
                        }
                    },
                    CheckBoxValueChanged = (menuId, value) =>
                    {
                        for (int i = 0; i < fcu.Model.SelectableFrames.Count(); i++)
                        {
                            fcu.Model.SelectableFrames[i].Selected = value;
                        }
                    }
                });
            }
        }
        private void DrawPUI_Settings()
        {
#if PUI_EXISTS
            gui.DrawMenu(ref fcu.Model.SelectableHamburgerItems, new HamburgerItem
            {
                GUIContent = new GUIContent(LocKey.label_pui_settings.Localize()),
                Id = FCU_MenuId.PUI_SETTINGS_KEY.ToString(),
                Body = () =>
                {
                    fcu.Model.PuiSettings.Type = gui.EnumField(new GUIContent(LocKey.label_pui_type.Localize(), ""),
                        fcu.Model.PuiSettings.Type);

                    fcu.Model.PuiSettings.RaycastTarget = gui.Toggle(new GUIContent(LocKey.label_pui_raycast_target.Localize(), ""),
                        fcu.Model.PuiSettings.RaycastTarget);

                    fcu.Model.PuiSettings.ModifierType = gui.EnumField(new GUIContent(LocKey.label_pui_modifier_type.Localize(), ""),
                        fcu.Model.PuiSettings.ModifierType);

                    fcu.Model.PuiSettings.BorderWidth = gui.FloatField(new GUIContent(LocKey.label_pui_border_width.Localize(), ""),
                        fcu.Model.PuiSettings.BorderWidth);

                    fcu.Model.PuiSettings.FalloffDistance = gui.FloatField(new GUIContent(LocKey.label_pui_falloff_distance.Localize(), ""),
                        fcu.Model.PuiSettings.FalloffDistance);
                }
            });
#endif
        }
        private void DrawMPImageSettings()
        {

        }
        private void DrawDefaultTextSettings()
        {
            gui.DrawMenu(ref fcu.Model.SelectableHamburgerItems, new HamburgerItem
            {
                GUIContent = new GUIContent(LocKey.label_unity_text_settings.Localize(), ""),
                Id = FCU_MenuId.UNITY_TEXT_SETTINGS_KEY.ToString(),
                Body = () =>
                {
                    fcu.Model.UnityTextSettings.BestFit = gui.Toggle(new GUIContent(LocKey.label_best_fit.Localize(), LocKey.tooltip_best_fit.Localize()),
                        fcu.Model.UnityTextSettings.BestFit);

                    fcu.Model.UnityTextSettings.FontLineSpacing = gui.FloatField(new GUIContent(LocKey.label_line_spacing.Localize(), LocKey.tooltip_line_spacing.Localize()),
                        fcu.Model.UnityTextSettings.FontLineSpacing);

                    fcu.Model.UnityTextSettings.HorizontalWrapMode = gui.EnumField(new GUIContent(LocKey.label_horizontal_overflow.Localize(), LocKey.tooltip_horizontal_overflow.Localize()),
                        fcu.Model.UnityTextSettings.HorizontalWrapMode);

                    fcu.Model.UnityTextSettings.VerticalWrapMode = gui.EnumField(new GUIContent(LocKey.label_vertical_overflow.Localize(), LocKey.tooltip_vertical_overflow.Localize()),
                        fcu.Model.UnityTextSettings.VerticalWrapMode);

                    gui.SerializedPropertyField<FigmaConverterUnity>(serializedObject, x => x.model.unityTextSettings.Fonts);
                }
            });
        }
        private void DrawTextMeshProSettings()
        {
#if TMPRO_EXISTS
            gui.DrawMenu(ref fcu.Model.SelectableHamburgerItems, new HamburgerItem
            {
                GUIContent = new GUIContent(LocKey.label_textmeshpro_settings.Localize(), ""),
                Id = FCU_MenuId.TM_SETTINGS_KEY.ToString(),
                Body = () =>
                {
                    fcu.Model.TextMeshSettings.AutoSize = gui.Toggle(new GUIContent(LocKey.label_auto_size.Localize(), LocKey.tooltip_auto_size.Localize()),
                        fcu.Model.TextMeshSettings.AutoSize);

                    fcu.Model.TextMeshSettings.OverrideTags = gui.Toggle(new GUIContent(LocKey.label_override_tags.Localize(), LocKey.tooltip_override_tags.Localize()),
                        fcu.Model.TextMeshSettings.OverrideTags);

                    fcu.Model.TextMeshSettings.Wrapping = gui.Toggle(new GUIContent(LocKey.label_wrapping.Localize(), LocKey.tooltip_wrapping.Localize()),
                        fcu.Model.TextMeshSettings.Wrapping);

                    fcu.Model.TextMeshSettings.RichText = gui.Toggle(new GUIContent(LocKey.label_rich_text.Localize(), LocKey.tooltip_rich_text.Localize()),
                        fcu.Model.TextMeshSettings.RichText);

                    fcu.Model.TextMeshSettings.RaycastTarget = gui.Toggle(new GUIContent(LocKey.label_raycast_target.Localize(), LocKey.tooltip_raycast_target.Localize()),
                        fcu.Model.TextMeshSettings.RaycastTarget);

                    fcu.Model.TextMeshSettings.ParseEscapeCharacters = gui.Toggle(new GUIContent(LocKey.label_parse_escape_characters.Localize(), LocKey.tooltip_parse_escape_characters.Localize()),
                        fcu.Model.TextMeshSettings.ParseEscapeCharacters);

                    fcu.Model.TextMeshSettings.VisibleDescender = gui.Toggle(new GUIContent(LocKey.label_visible_descender.Localize(), LocKey.tooltip_visible_descender.Localize()),
                        fcu.Model.TextMeshSettings.VisibleDescender);

                    fcu.Model.TextMeshSettings.Kerning = gui.Toggle(new GUIContent(LocKey.label_kerning.Localize(), LocKey.tooltip_kerning.Localize()),
                        fcu.Model.TextMeshSettings.Kerning);

                    fcu.Model.TextMeshSettings.ExtraPadding = gui.Toggle(new GUIContent(LocKey.label_extra_padding.Localize(), LocKey.tooltip_extra_padding.Localize()),
                        fcu.Model.TextMeshSettings.ExtraPadding);

                    fcu.Model.TextMeshSettings.Overflow = gui.EnumField(new GUIContent(LocKey.label_overflow.Localize(), LocKey.tooltip_overflow.Localize()),
                        fcu.Model.TextMeshSettings.Overflow);

                    fcu.Model.TextMeshSettings.HorizontalMapping = gui.EnumField(new GUIContent(LocKey.label_horizontal_mapping.Localize(), LocKey.tooltip_horizontal_mapping.Localize()),
                        fcu.Model.TextMeshSettings.HorizontalMapping);

                    fcu.Model.TextMeshSettings.VerticalMapping = gui.EnumField(new GUIContent(LocKey.label_vertical_mapping.Localize(), LocKey.tooltip_vertical_mapping.Localize()),
                        fcu.Model.TextMeshSettings.VerticalMapping);

                    fcu.Model.TextMeshSettings.GeometrySorting = gui.EnumField(new GUIContent(LocKey.label_geometry_sorting.Localize(), LocKey.tooltip_geometry_sorting.Localize()),
                        fcu.Model.TextMeshSettings.GeometrySorting);

                    gui.SerializedPropertyField<FigmaConverterUnity>(serializedObject, x => x.model.textMeshSettings.Fonts);
                }
            });
#endif
        }
        private void DrawDefines()
        {

            gui.DrawMenu(ref fcu.Model.SelectableHamburgerItems, new HamburgerItem
            {
                GUIContent = new GUIContent(LocKey.label_additional_assets_config.Localize(), LocKey.tooltip_additional_assets_config.Localize()),
                Id = FCU_MenuId.ASSETS_CONFIG_KEY.ToString(),
                Body = () =>
                {
                    if (FCU_Config.Instance.AssemblyConfigs.IsEmpty())
                    {
                        return;
                    }

                    foreach (AssemblyConfig assemblyConfig in FCU_Config.Instance.AssemblyConfigs)
                    {
                        assemblyConfig.Enabled.Value = gui.Toggle(
                            new GUIContent(assemblyConfig.Name),
                            assemblyConfig.Enabled.Value,
                            new GUIContent("Path"), () =>
                            {
                                AssetPatcher.Patch(assemblyConfig);
                            });

                        this.AssetFinder.Refresh(assemblyConfig);
                    }
                }
            });
        }
        private void DrawAssetTools()
        {
            gui.DrawMenu(ref fcu.Model.SelectableHamburgerItems, new HamburgerItem
            {
                Id = FCU_MenuId.ASSET_TOOLS_KEY.ToString(),
                GUIContent = new GUIContent(LocKey.label_tools.Localize(), LocKey.tooltip_tools.Localize()),
                Body = () =>
                {
                    if (gui.Button(new GUIContent(
                        LocKey.label_destroy_fcu_meta.Localize(),
                        LocKey.tooltip_destroy_fcu_meta.Localize())))
                    {
                        fcu.Model.DynamicCoroutine(fcu.Tools.DestroyCurrentCanvasMetas_OnClick());
                    }

                    GUILayout.Space(gui.NORMAL_SPACE);

                    if (gui.Button(new GUIContent(
                        LocKey.label_clear_canvas.Localize(),
                        LocKey.tooltip_clear_canvas.Localize())))
                    {
                        fcu.Tools.DestroyCurrentCanvasChilds_OnClick();
                    }

                    GUILayout.Space(gui.NORMAL_SPACE);

                    if (gui.Button(new GUIContent(
                        LocKey.label_destroy_last_imported_frames.Localize(),
                        LocKey.tooltip_destroy_last_imported_frames.Localize())))
                    {
                        fcu.Tools.DestroyLastImportedFrames_OnClick();
                    }

                    GUILayout.Space(gui.NORMAL_SPACE);

                    if (gui.Button(new GUIContent(LocKey.label_write_to_support.Localize())))
                    {
                        Application.OpenURL(FCU_Config.Instance.TgLink);
                    }
                }
            });
        }
        private void Footer()
        {
            GUILayout.Space(gui.BIG_SPACE);

            if (gui.Button(new GUIContent(LocKey.label_made_by.Localize()), true, CustomStyle.BOX_NO_FRAME, 10))
            {
                Application.OpenURL(FCU_Config.Instance.TgLink);
            }
        }
        public IEnumerator StopAllCoroutines()
        {
            for (int i = 0; i < fcu.Model.CoroutinesPool.Count(); i++)
            {
                ((EditorCoroutine)fcu.Model.CoroutinesPool[i]).Stop();
                yield return new WaitForSecondsRealtime(FCU_Config.Instance.Delay001);
            }
        }
        private void CreateEditorGUI()
        {
            if (fcu.Model.EditorGUI == null)
            {
                fcu.Model.EditorGUI = CreateInstance<DA_GUI_Editor>();
            }

            gui = (DA_GUI_Editor)fcu.Model.EditorGUI;
        }
        private void StartRepaintingAssetUI()
        {
            EditorCoroutine repainter = ((EditorCoroutine)fcu.Model.AssetRepainter);

            if (repainter != null)
            {
                repainter.Stop();
            }

            fcu.Model.AssetRepainter = fcu.Model.DynamicCoroutine(RepaintAssetUI(fcu));
        }
        private IEnumerator RepaintAssetUI(FigmaConverterUnity fcu)
        {
            while (true)
            {
                if (fcu == null)
                {
                    break;
                }

                if (Selection.Contains(fcu.gameObject))
                {
                    this.Repaint();
                }

                while (true)
                {
                    if (fcu.WebClient.PbarProgress > 0)
                    {
                        break;
                    }

                    yield return new WaitForSecondsRealtime(FCU_Config.Instance.Delay01);
                }

                yield return new WaitForSecondsRealtime(FCU_Config.Instance.Delay001);
            }
        }
        public EditorCoroutine StartAndAddToPool(IEnumerator routine)
        {
            EditorCoroutine editorCoroutine = new EditorCoroutine(routine);
            fcu.Model.CoroutinesPool.Add(editorCoroutine);
            return editorCoroutine;
        }
        private void DrawCustomPrefabs()
        {
            gui.DrawMenu(ref fcu.Model.SelectableHamburgerItems, new HamburgerItem
            {
                Id = FCU_MenuId.CUSTOM_PREFABS_KEY.ToString(),
                GUIContent = new GUIContent(LocKey.label_custom_prefabs.Localize(), LocKey.tooltip_custom_prefabs.Localize()),
                Body = () =>
                {
                    gui.SerializedPropertyField<FigmaConverterUnity>(serializedObject, x => x.model.CustomPrefabs);
                }
            });
        }
        private enum FCU_MenuId
        {
            MAIN_SETTINGS_KEY,
            UNITY_TEXT_SETTINGS_KEY,
            GBUTTON_SETTINGS_KEY,
            GBUTTON_HOVER_COLOR_KEY,
            GBUTTON_CLICK_COLOR_KEY,
            GBUTTON_HOVER_SCALE_KEY,
            GBUTTON_CLICK_SCALE_KEY,
            GBUTTON_AUDIO_KEY,
            TM_SETTINGS_KEY,
            PUI_SETTINGS_KEY,
            MPUI_SETTINGS_KEY,
            FRAMES_KEY,
            ASSETS_CONFIG_KEY,
            ASSET_TOOLS_KEY,
            CUSTOM_PREFABS_KEY,
            DEBUG_TOOLS_KEY,
        }
    }
}
