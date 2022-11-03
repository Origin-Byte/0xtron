using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;

namespace DA_Assets.FCU.UI
{
    /// <summary>
    /// https://blog.csdn.net/qq_35711014/article/details/108111754
    /// </summary>
    public class DraggablePopup : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
    {
        [SerializeField] RectTransform popupTransform;
        private IEnumerator moveToPosCoroutine = null;
        private Vector2 halfSize;
        private float timePassed = 0f;
        private float updateEventTimeout = 1f;
        private Vector2 tempMainCanvasResolution;
        private bool inited = false;
        internal void Init()
        {
            tempMainCanvasResolution = mainCanvasResolution;

            popupTransform.sizeDelta = new Vector2(mainCanvasResolution.x, popupTransform.sizeDelta.y);
            halfSize = 0.5f * popupTransform.root.localScale.x * popupTransform.sizeDelta;

            OnEndDrag(null);
            inited = true;
        }
        private void Update()
        {
            if (inited == false)
            {
                return;
            }

            timePassed += Time.deltaTime;

            if (timePassed > updateEventTimeout)
            {
                halfSize = popupTransform.root.localScale.x * popupTransform.sizeDelta / 2;
                RefreshSizeAfterScreenResize();
            }
        }
        public void RefreshSizeAfterScreenResize()
        {
            if (mainCanvasResolution != null && tempMainCanvasResolution != mainCanvasResolution)
            {
                Init();
            }
        }
        public void OnBeginDrag(PointerEventData data)
        {
            if (moveToPosCoroutine != null)
            {
                StopCoroutine(moveToPosCoroutine);
                moveToPosCoroutine = null;
            }
        }
        public void OnDrag(PointerEventData data)
        {
            if (locked == false)
            {
                popupTransform.position = data.position;
            }
        }
        public void BlockDrag()
        {
            StopAllCoroutines();
            StartCoroutine(_BlockDrag());
        }
        private bool locked = false;
        private IEnumerator _BlockDrag()
        {
            locked = true;
            yield return new WaitForSeconds(1f);
            locked = false;
        }
        public void OnEndDrag(PointerEventData data)
        {
            Vector3 pos = popupTransform.position;

            int screenWidth = Screen.width;
            int screenHeight = Screen.height;

            float distToLeft = pos.x;
            float distToRight = Mathf.Abs(pos.x - screenWidth);
            float distToBottom = Mathf.Abs(pos.y);
            float distToTop = Mathf.Abs(pos.y - screenHeight);
            float horDistance = Mathf.Min(distToLeft, distToRight);
            float vertDistance = Mathf.Min(distToBottom, distToTop);

            if (horDistance < vertDistance)
            {
                if (distToLeft < distToRight)
                {
                    pos = new Vector3(halfSize.x, pos.y, 0f);
                }
                else
                {
                    pos = new Vector3(screenWidth - halfSize.x, pos.y, 0f);
                }

                pos.y = Mathf.Clamp(pos.y, halfSize.y, screenHeight - halfSize.y);
            }
            else
            {
                if (distToBottom < distToTop)
                {
                    pos = new Vector3(pos.x, halfSize.y, 0f);
                }
                else
                {
                    pos = new Vector3(pos.x, screenHeight - halfSize.y, 0f);
                }

                pos.x = Mathf.Clamp(pos.x, halfSize.x, screenWidth - halfSize.x);
            }

            if (moveToPosCoroutine != null)
            {
                StopCoroutine(moveToPosCoroutine);
            }

            moveToPosCoroutine = MoveToPosAnimation(pos);
            StartCoroutine(moveToPosCoroutine);
        }
        private IEnumerator MoveToPosAnimation(Vector3 targetPos)
        {
            float modifier = 0f;
            Vector3 initialPos = popupTransform.position;

            while (modifier < 1f)
            {
                modifier += 4f * Time.unscaledDeltaTime;
                popupTransform.position = Vector3.Lerp(initialPos, targetPos, modifier);
                yield return null;
            }
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