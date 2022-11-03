//
//███████╗██╗░██████╗░███╗░░░███╗░█████╗░  ░█████╗░░█████╗░███╗░░██╗██╗░░░██╗███████╗██████╗░████████╗███████╗██████╗░
//██╔════╝██║██╔════╝░████╗░████║██╔══██╗  ██╔══██╗██╔══██╗████╗░██║██║░░░██║██╔════╝██╔══██╗╚══██╔══╝██╔════╝██╔══██╗
//█████╗░░██║██║░░██╗░██╔████╔██║███████║  ██║░░╚═╝██║░░██║██╔██╗██║╚██╗░██╔╝█████╗░░██████╔╝░░░██║░░░█████╗░░██████╔╝
//██╔══╝░░██║██║░░╚██╗██║╚██╔╝██║██╔══██║  ██║░░██╗██║░░██║██║╚████║░╚████╔╝░██╔══╝░░██╔══██╗░░░██║░░░██╔══╝░░██╔══██╗
//██║░░░░░██║╚██████╔╝██║░╚═╝░██║██║░░██║  ╚█████╔╝╚█████╔╝██║░╚███║░░╚██╔╝░░███████╗██║░░██║░░░██║░░░███████╗██║░░██║
//╚═╝░░░░░╚═╝░╚═════╝░╚═╝░░░░░╚═╝╚═╝░░╚═╝  ░╚════╝░░╚════╝░╚═╝░░╚══╝░░░╚═╝░░░╚══════╝╚═╝░░╚═╝░░░╚═╝░░░╚══════╝╚═╝░░╚═╝
//
//███████╗░█████╗░██████╗░  ██╗░░░██╗███╗░░██╗██╗████████╗██╗░░░██╗
//██╔════╝██╔══██╗██╔══██╗  ██║░░░██║████╗░██║██║╚══██╔══╝╚██╗░██╔╝
//█████╗░░██║░░██║██████╔╝  ██║░░░██║██╔██╗██║██║░░░██║░░░░╚████╔╝░
//██╔══╝░░██║░░██║██╔══██╗  ██║░░░██║██║╚████║██║░░░██║░░░░░╚██╔╝░░
//██║░░░░░╚█████╔╝██║░░██║  ╚██████╔╝██║░╚███║██║░░░██║░░░░░░██║░░░
//╚═╝░░░░░░╚════╝░╚═╝░░╚═╝  ░╚═════╝░╚═╝░░╚══╝╚═╝░░░╚═╝░░░░░░╚═╝░░░
//

using DA_Assets.FCU.Config;
using DA_Assets.FCU.Core;
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
using Console = DA_Assets.Shared.Console;

#pragma warning disable IDE0003

namespace DA_Assets.FCU
{
    [Serializable]
    public class FigmaConverterUnity : MonoBehaviour
    {
        [SerializeField] public FCU_Model model = new FCU_Model();
        [SerializeField] WebClient webClient = new WebClient();
        [SerializeField] ProjectParser projectParser = new ProjectParser();
        [SerializeField] CanvasDrawer canvasDrawer = new CanvasDrawer();
        [SerializeField] Tools tools = new Tools();
        [SerializeField] Cacher cacher = new Cacher();
        public void Auth_OnClick()
        {
            this.Model.DynamicCoroutine(Auth());
        }
        public void DownloadProject_OnClick()
        {
            this.Model.DynamicCoroutine(DownloadProject());
        }
        public void ImportSelectedFrames_OnClick()
        {
            this.Model.DynamicCoroutine(ImportSelectedFrames());
        }
        private IEnumerator Auth()
        {
            CoroutineResult<AuthResult> authResult = default;

            yield return this.Model.DynamicCoroutine(this.WebClient.Auth(x => authResult = x));

            if (authResult.Success)
            {
                this.Model.MainSettings.Token = authResult.Result.access_token;

                CoroutineResult<FigmaUser> userResult = default;

                yield return this.Model.DynamicCoroutine(this.WebClient.GetCurrentFigmaUser(x => userResult = x));

                if (userResult.Success)
                {
                    this.Model.MainSettings.CurrentFigmaUser = userResult.Result;
                    Console.Success(LocKey.log_auth_complete.Localize());
                }
                else
                {
                    Console.LogError(LocKey.log_cant_get_profile_info.Localize(authResult.Error.ParseError()));
                    this.StopImport();
                }
            }
            else
            {
                Console.LogError(LocKey.log_cant_auth.Localize(authResult.Error.ParseError()));
                this.StopImport();
            }
        }
        private IEnumerator DownloadProject()
        {
            this.Model.SelectableFrames = new List<SelectableItem>();

            if (string.IsNullOrWhiteSpace(this.Model.MainSettings.Token))
            {
                Console.LogError(LocKey.log_need_auth.Localize());
                yield break;
            }
            else if (string.IsNullOrWhiteSpace(this.Model.MainSettings.ProjectUrl))
            {
                Console.LogError(LocKey.log_incorrent_project_url.Localize());
                yield break;
            }

            CoroutineResult<FigmaProject> result = default;

            yield return this.Model.DynamicCoroutine(this.WebClient.DownloadProject(_result => result = _result));

            if (result.Success)
            {
                this.Model.CurrentProject = result.Result;

                Console.WriteLine(LocKey.log_project_downloaded.Localize());

                FillSelectableFramesArray(fromCache: false);
            }
            else
            {
                switch (result.Error.Status)
                {
                    case 403:
                        Console.LogError(LocKey.log_need_auth.Localize());
                        break;
                    case 404:
                        Console.LogError(LocKey.log_project_not_found.Localize());
                        break;
                    default:
                        Console.LogError(LocKey.log_unknown_error.Localize());
                        break;
                }

                this.StopImport();
            }
        }
        public void FillSelectableFramesArray(bool fromCache)
        {
            List<SelectableItem> newItems = new List<SelectableItem>();

            foreach (FObject page in this.Model.CurrentProject.Document.Children)
            {
                foreach (FObject frame in page.Children)
                {
                    if (frame.Type == FCU_Tag.Frame.ToUpper())
                    {
                        frame.SetTag(FCU_Tag.Frame);

                        newItems.Add(new SelectableItem
                        {
                            Id = frame.Id,
                            Name = frame.Name,
                            Selected = true,
                            ParentId = $"{page.Id}_{page.Name}",
                            ParentName = page.Name,
                        });
                    }
                }
            }

            var newItemsIds = newItems.Select(x => x.Id);
            var oldItemsIds = this.Model.SelectableFrames.Select(x => x.Id);
            bool equals = newItemsIds.SequenceEqual(oldItemsIds);

            if (equals)
            {
                for (int i = 0; i < this.Model.SelectableFrames.Count(); i++)
                {
                    this.Model.SelectableFrames[i].Name = newItems[i].Name;
                    this.Model.SelectableFrames[i].ParentId = newItems[i].ParentId;
                    this.Model.SelectableFrames[i].ParentName = newItems[i].ParentName;
                }
            }
            else
            {
                this.Model.SelectableFrames = newItems;
            }

            if (fromCache == false && this.Model.SelectableFrames.Count() == 1)
            {
                this.Model.SelectableFrames[0].Selected = true;
                this.Model.DynamicCoroutine(ImportSelectedFrames());
            }
            else if (this.Model.SelectableFrames.Count > 0)
            {
                Console.WriteLine(LocKey.log_frames_finded.Localize(), this.Model.SelectableFrames.Count);
            }
            else
            {
                Console.LogError(LocKey.log_frames_not_found.Localize());
            }
        }
        public IEnumerator ImportSelectedFrames()
        {
            List<FObject> selectedFrames = this.Model.CurrentProject.Document.Children
                .Select(x => x.Children)
                .FromChunks()
                .Where(fobject => this.Model.SelectableFrames
                .Where(si => si.Selected)
                .Select(si => si.Id)
                .Contains(fobject.Id))
                .ToList();

            FObject page = new FObject
            {
                FixedName = FCU_Tag.Page.ToString(),
                Children = selectedFrames,
                Tags = new List<FCU_Tag> { FCU_Tag.Page }
            };

            List<FObject> fobjects = new List<FObject>();

            this.Tools.ClearLastImportedFrames();

            yield return this.Model.DynamicCoroutine(this.ProjectParser.SetTags(page));

            this.Tools.CacheResolutionData();

            yield return this.Model.DynamicCoroutine(this.ProjectParser.InstantiateGameObjects(page, fobjects, null));
            yield return this.Model.DynamicCoroutine(this.CanvasDrawer.SetFigmaTransform(fobjects));

            this.Tools.RestoreResolutionData();

            yield return this.Model.DynamicCoroutine(this.ProjectParser.DestroyMissing(fobjects));
            yield return this.Model.DynamicCoroutine(this.ProjectParser.SetMutualFlag(fobjects));


            yield return this.Model.DynamicCoroutine(this.ProjectParser.SetAssetPaths(fobjects));
            yield return this.Model.DynamicCoroutine(this.WebClient.SetImageLinks(fobjects));
            yield return this.Model.DynamicCoroutine(this.WebClient.DownloadImages(fobjects));

#if UNITY_EDITOR
            UnityEditor.AssetDatabase.Refresh();
#endif
            yield return this.Model.DynamicCoroutine(this.CanvasDrawer.DrawToCanvas(fobjects));

            Console.Success(LocKey.log_import_complete.Localize());
        }
        public FCU_Model Model
        {
            get
            {
                return model.SetController(this);
            }
        }

        public WebClient WebClient
        {
            get
            {
                return webClient.SetController(this);
            }
        }

        public ProjectParser ProjectParser
        {
            get
            {
                return projectParser.SetController(this);
            }
        }
        public CanvasDrawer CanvasDrawer
        {
            get
            {
                canvasDrawer.SetController(this);
                canvasDrawer.TryInstantiateCanvas();
                CanvasDrawer.TryInstantiateEventSystem();
                return canvasDrawer;
            }
        }
        public Tools Tools
        {
            get
            {
                return tools.SetController(this);
            }
        }
        public Cacher Cacher
        {
            get
            {
                return cacher.SetController(this);
            }
        }
    }
}