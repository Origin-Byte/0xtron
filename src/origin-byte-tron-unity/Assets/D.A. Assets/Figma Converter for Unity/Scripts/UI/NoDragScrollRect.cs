using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace DA_Assets.FCU.UI
{
    /// <summary>
    /// https://stackoverflow.com/a/31847675
    /// </summary>
    public class NoDragScrollRect : ScrollRect
    {
        public override void OnBeginDrag(PointerEventData eventData) { }
        public override void OnDrag(PointerEventData eventData) { }
        public override void OnEndDrag(PointerEventData eventData) { }
    }
}
