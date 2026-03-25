using UnityEngine;
using UnityEngine.EventSystems;

public class Hack : MonoBehaviour, IPointerClickHandler
{
    public GameObject tool;

    [Header("Settings")]
    public int requiredClicks = 4;
    public float maxDelayBetweenClicks = 0.5f;

    private int clickCount;
    private float lastClickTime;

    public void OnPointerClick(PointerEventData eventData)
    {
        if (Time.time - lastClickTime <= maxDelayBetweenClicks)
            clickCount++;
        else
            clickCount = 1;

        lastClickTime = Time.time;

        if (clickCount >= requiredClicks)
        {
            if (tool != null)
                tool.SetActive(true);

            clickCount = 0;
        }
    }
}
