using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace DA_Assets.FCU.UI
{
    /// <summary>
    /// https://unity3d.com/learn/tutorials/modules/intermediate/live-training-archive/panels-panes-windows
    /// </summary>
    public class ResizePanel : MonoBehaviour, IPointerDownHandler, IDragHandler
    {
        [SerializeField] RectTransform currentRect;
        [SerializeField] RectTransform resizeObjectRect;
        [SerializeField] RectTransform buttonsContainer;

        private Vector2 minSize;
        private Vector2 maxSize;
        private Vector2 currentPointerPosition;
        private Vector2 previousPointerPosition;
        private bool minSizeUpdated = false;
        internal void Init()
        {
            currentRect = GetComponent<RectTransform>();
        }
        private void UpdateMinSize()
        {
            if (minSizeUpdated == false)
            {
                Button[] buttons = buttonsContainer.GetComponentsInChildren<Button>();

                if (buttons.Length > 0)
                {
                    float btnResizeX = currentRect.sizeDelta.x;
                    float btnResizeY = currentRect.sizeDelta.y;
                    float firstButtonX = buttons[0].GetComponent<RectTransform>().sizeDelta.x;

                    minSize = new Vector2(btnResizeX + firstButtonX, btnResizeY);
                    maxSize = new Vector2(mainCanvasResolution.x, mainCanvasResolution.y / 2f);
                }
            }
        }

        public void OnPointerDown(PointerEventData data)
        {
            resizeObjectRect.SetAsLastSibling();

            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                resizeObjectRect,
                data.position,
                data.pressEventCamera, out previousPointerPosition);

            UpdateMinSize();
        }

        public void OnDrag(PointerEventData data)
        {
            if (resizeObjectRect == null)
                return;

            Vector2 sizeDelta = resizeObjectRect.sizeDelta;

            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                resizeObjectRect, data.position, data.pressEventCamera, out currentPointerPosition);
            Vector2 resizeValue = currentPointerPosition - previousPointerPosition;

            sizeDelta += new Vector2(resizeValue.x, -resizeValue.y);
            sizeDelta = new Vector2(Mathf.Clamp(sizeDelta.x, minSize.x, maxSize.x), Mathf.Clamp(sizeDelta.y, minSize.y, maxSize.y));

            resizeObjectRect.sizeDelta = sizeDelta;
            previousPointerPosition = currentPointerPosition;
        }
        private Vector2 mainCanvasResolution
        {
            get
            {
                return UIManager.Instance.GetComponent<Canvas>().GetComponent<RectTransform>().sizeDelta;
            }
        }
    }
}
