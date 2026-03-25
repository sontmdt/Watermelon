using UnityEngine;

public abstract class MenuBase : MonoBehaviour
{
    [SerializeField] private EventID _eventID;
    public EventID eventID => _eventID;

    public virtual void Setup() { }
    public virtual void Show()  { gameObject.SetActive(true); }
    public virtual void Hide()  { gameObject.SetActive(false); }
}
