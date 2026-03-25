using UnityEngine;
using UnityEngine.EventSystems;
public class HoverArea : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public bool IsHovering { get; private set; } = false;
    public void OnPointerEnter(PointerEventData eventData)
    {
        IsHovering = true;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        IsHovering = false;
    }
}
