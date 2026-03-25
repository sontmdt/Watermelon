using UnityEngine;
using UnityEngine.UI;

public class CancelZone : MonoBehaviour
{
    private bool isDragging = false;
    private RectTransform rect;
    [SerializeField] private Image zoneImage;
    private Color normalColor;
    private IBoosterContext _boosterCtx;

    private void Awake()
    {
        rect = GetComponent<RectTransform>();
        if (zoneImage == null)
            zoneImage = GetComponent<Image>();
        normalColor = zoneImage.color;
        _boosterCtx = ServiceLocator.Instance.GetService<IBoosterContext>();
    }

    private void OnEnable()
    {
        InputManager.Instance.OnDragBeginEvent += OnDragBegin;
        InputManager.Instance.OnDragEvent += OnDrag;
        InputManager.Instance.OnDragEndEvent += OnDragEnd;
    }

    private void OnDisable()
    {
        InputManager.Instance.OnDragBeginEvent -= OnDragBegin;
        InputManager.Instance.OnDragEvent -= OnDrag;
        InputManager.Instance.OnDragEndEvent -= OnDragEnd;
    }

    private void OnDragBegin(Vector2 pos)
    {
        if (InputManager.Instance.isBusy) return;
        isDragging = true;
    }

    private void OnDrag(Vector2 pos)
    {
        if (InputManager.Instance.isBusy)
        {
            if (_boosterCtx.InCancelZone)
            {
                _boosterCtx.InCancelZone = false;
                MyAnimation.ResetCancelZoneAnimation(zoneImage, normalColor);
            }
            return;
        }

        bool inside = RectTransformUtility.RectangleContainsScreenPoint(rect, pos, GameManager.Instance.m_camera);

        if (inside && !_boosterCtx.InCancelZone)
        {
            _boosterCtx.InCancelZone = true;
            MyAnimation.PlayCancelZoneAnimation(zoneImage, normalColor, 2);
        }
        else if (!inside && _boosterCtx.InCancelZone)
        {
            _boosterCtx.InCancelZone = false;
            OnHoverExitCancelZone();
        }
    }

    private void OnDragEnd(Vector2 pos)
    {
        if (!InputManager.Instance.isBusy && _boosterCtx.InCancelZone && isDragging)
        {
            OnReleaseInsideZone();
        }

        isDragging = false;
        _boosterCtx.InCancelZone = false;
        MyAnimation.ResetCancelZoneAnimation(zoneImage, normalColor);
    }

    private void OnHoverExitCancelZone()
    {
        MyAnimation.ResetCancelZoneAnimation(zoneImage, normalColor);
    }

    private void OnReleaseInsideZone()
    {
        var active = _boosterCtx.ActiveBooster;
        if (active == eActiveBooster.Destroy || active == eActiveBooster.SmallDestroy)
            GameManager.Instance.SwitchColorScreen();

        _boosterCtx.Deactivate();
        EventManager.Instance.PostEvent(EventID.StateGameStarted);
        MyAnimation.ResetCancelZoneAnimation(zoneImage, normalColor);
    }
}
