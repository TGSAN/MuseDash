using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Assets.Scripts.UI
{
    public class UIScrollRect : ScrollRect
    {
        public event Callback<PointerEventData> onBeginDrag, onEndDrag, onDrag, onScroll;

        public override void OnBeginDrag(PointerEventData eventData)
        {
            base.OnBeginDrag(eventData);
            if (onBeginDrag != null) onBeginDrag.Invoke(eventData);
        }

        public override void OnEndDrag(PointerEventData eventData)
        {
            base.OnEndDrag(eventData);
            if (onEndDrag != null) onEndDrag.Invoke(eventData);
        }

        public override void OnDrag(PointerEventData eventData)
        {
            base.OnEndDrag(eventData);
            if (onDrag != null) onDrag.Invoke(eventData);
        }

        public override void OnScroll(PointerEventData eventData)
        {
            base.OnScroll(eventData);
            if (onScroll != null) onScroll.Invoke(eventData);
        }
    }
}