using DA_Assets.FCU.Model;
using DA_Assets.Shared.CodeHelpers;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace DA_Assets.FCU.UI
{
    [Serializable]
    public class UIManager : MonoBehaviour
    {
        [SerializeField] float showHideSpeedSec = 0.25f;
        [SerializeField] bool showLock;

        [SerializeField] Transform buttonsContainer;
        [SerializeField] UIManagerButton buttonPrefab;
        [SerializeField] ResizePanel resizePanel;
        [SerializeField] DraggablePopup draggablePopup;

        [SerializeField] List<Window> windows;

        public IEnumerator Start()
        {
            windows = new List<Window>();

            List<FCU_Meta> frames = FindFramesInScene();

            yield return AddWindowScripts(frames);
            yield return InstantiateMenuButtons();

            resizePanel.Init();
            draggablePopup.Init();
        }
        private IEnumerator AddWindowScripts(List<FCU_Meta> frames)
        {
            foreach (FCU_Meta item in frames)
            {
                Window win = item.gameObject.AddComponent<Window>();

                win.IdNameInstanceId = new IdNameInstanceId
                {
                    Id = item.Id,
                    Name = item.FixedName,
                    InstanceId = item.FigmaConverterUnity.GetInstanceID()
                };

                windows.Add(win);
                yield return null;
            }
        }
        private List<FCU_Meta> FindFramesInScene()
        {
            List<FCU_Meta> frames = new List<FCU_Meta>();
            FigmaConverterUnity[] fcus = FindObjectsOfType<FigmaConverterUnity>();

            if (fcus.Length > 0)
            {
                frames = fcus
                    .Where(x => x.ProjectParser.FcuMetas.IsEmpty() == false)
                    .SelectMany(x => x.ProjectParser.FcuMetas.Where(x1 => x1 != null && x1.Tags.Contains(FCU_Tag.Frame)))
                    .GroupBy(x => x.Id)
                    .Select(x => x.First())
                    .ToList();
            }

            return frames;
        }
        public static bool IsExistsOnScene()
        {
            UIManager uiManager = FindObjectOfType<UIManager>();

            if (uiManager == null)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        private static UIManager _instance;
        public static UIManager Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = FindObjectOfType<UIManager>();
                }

                return _instance;
            }
            set
            {
                _instance = value;
            }
        }

        private IEnumerator InstantiateMenuButtons()
        {
            List<UIManagerButton> menuButtons = new List<UIManagerButton>();

            foreach (UIManagerButton btn in menuButtons)
            {
                btn.gameObject.Destroy();
            }

            foreach (Window win in windows)
            {
                if (win == null)
                    continue;

                UIManagerButton newButton = Instantiate(buttonPrefab, buttonsContainer);
                newButton.SetModel(win.IdNameInstanceId);
                menuButtons.Add(newButton);
                yield return null;
            }
        }
        public IEnumerator Show(IdNameInstanceId idNameGuid)
        {
            if (showLock)
            {
                yield break;
            }

            foreach (Window win in windows)
            {
                if (win == null)
                    continue;

                if (win.IdNameInstanceId.Equals(idNameGuid))
                {
                    showLock = true;

                    win.Canvas.sortingOrder = 999;
                    win.gameObject.SetActive(true);
                    yield return StartCoroutine(ShowAnimation(win));
                }
                else
                {
                    win.Canvas.sortingOrder = 0;
                }
            }

            foreach (Window win in windows)
            {
                if (win == null)
                    continue;

                if (win.IdNameInstanceId.Equals(idNameGuid))
                {
                    continue;
                }
                else
                {
                    CanvasGroup canvasGroup = GetCanvasGroup(win.gameObject);
                    canvasGroup.alpha = 0;
                    win.gameObject.SetActive(false);
                }
            }

            showLock = false;
        }
        private IEnumerator ShowAnimation(Window win)
        {
            CanvasGroup canvasGroup = GetCanvasGroup(win.gameObject);

            if (canvasGroup.alpha == 1)
            {
                yield break;
            }

            canvasGroup.alpha = 0;
            canvasGroup.blocksRaycasts = true;

            float elapsedTime = 0;
            float endAlpha = 1;
            float startAlpha = 0;

            while (elapsedTime < showHideSpeedSec)
            {
                canvasGroup.alpha = Mathf.Lerp(startAlpha, endAlpha, (elapsedTime / showHideSpeedSec));
                elapsedTime += Time.deltaTime;
                yield return null;
            }

            canvasGroup.alpha = endAlpha;
        }
        private CanvasGroup GetCanvasGroup(GameObject uiElement)
        {
            bool exists = uiElement.TryGetComponent(out CanvasGroup canvasGroup);

            if (exists == false)
            {
                canvasGroup = uiElement.AddComponent<CanvasGroup>();
                canvasGroup.alpha = 0f;
            }

            return canvasGroup;
        }
    }
}
