using System;
using UnityEngine;

public class InputManager : MonoBehaviour
{
    public static InputManager Instance { get; private set; }

    public event Action<Vector2> OnTapEvent = delegate { };
    public event Action<Vector2> OnTouchMoveEvent = delegate { };
    public event Action<Vector2> OnDragEvent = delegate { };
    public event Action<Vector2> OnDragBeginEvent = delegate { };
    public event Action<Vector2> OnDragEndEvent = delegate { };

    public event Action<float> OnZoomEvent = delegate { };
    public event Action<Vector2> OnTwoFingerDragEvent = delegate { };

    public bool isTouching = false;
    private bool isDragging = false;
    public bool isBusy = false;
    private float touchStartTime;
    private Vector2 touchStartPos;

    public Vector3 worldPos;
    public Vector2 lastEndPos;

    [SerializeField] private float holdTimestamp = 0.3f;
    [SerializeField] private Camera mainCamera;

    private SpriteRenderer box;
    private float _boxBottomEdge;

    [SerializeField] private float upperEdge = 3f;
    [SerializeField] private float bottomPadding = 0.0f;

    private bool m_isGameStarted;
    private bool m_isMainMenu;
    private bool m_isGameHanging;
    private bool m_isGameShop;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        this.RegisterListener(EventID.StateGameSetup, _ => SetFlags(false, false, false, false));
        this.RegisterListener(EventID.StateMainMenu, _ => SetFlags(false, true,  false, false));
        this.RegisterListener(EventID.StateGameStarted, _ => SetFlags(true,  false, false, false));
        this.RegisterListener(EventID.StateGamePause, _ => SetFlags(false, false, false, false));
        this.RegisterListener(EventID.StateGamePopup, _ => SetFlags(false, false, false, false));
        this.RegisterListener(EventID.StateGameShop, _ => SetFlags(false, false, false, true));
        this.RegisterListener(EventID.StateGameHanging, _ => SetFlags(false, false, true,  false));
        this.RegisterListener(EventID.StateGameOver, _ => SetFlags(false, false, false, false));
    }

    private void SetFlags(bool started, bool mainMenu, bool hanging, bool shop)
    {
        m_isGameStarted = started;
        m_isMainMenu    = mainMenu;
        m_isGameHanging = hanging;
        m_isGameShop    = shop;
    }

    private void Update()
    {
        if (!m_isGameStarted && !m_isMainMenu && !m_isGameHanging)
            return;

#if UNITY_EDITOR
        Vector2 p = Input.mousePosition;
        Vector3 pWorld = mainCamera.ScreenToWorldPoint(new Vector3(p.x, p.y, -mainCamera.transform.position.z));

        if (IsAboveUpperEdge(pWorld) || IsBelowBoxEdge(pWorld)) return;

        HandleMouseInput(pWorld);
#else
        if (Input.touchCount == 0) return;

        Vector2 p = Input.GetTouch(0).position;
        Vector3 pWorld = mainCamera.ScreenToWorldPoint(new Vector3(p.x, p.y, -mainCamera.transform.position.z));

        if (IsAboveUpperEdge(pWorld) || IsBelowBoxEdge(pWorld)) return;

        HandleTouchInput(pWorld);
#endif
    }

    private bool IsAboveUpperEdge(Vector3 worldPos)
    {
        if (m_isGameHanging) return false;
        return worldPos.y > upperEdge;
    }

    private bool IsBelowBoxEdge(Vector3 worldPos)
    {
        if (m_isGameHanging) return false;
        if (box == null)
        {
            box = GameManager.Instance.m_levelController.box;
            if (box != null) _boxBottomEdge = box.bounds.min.y;
        }

        if (box == null) return false;

        return worldPos.y < _boxBottomEdge - bottomPadding;
    }

    private void HandlePointerBegin(Vector2 pos)
    {
        isTouching = true;
        isDragging = false;
        touchStartTime = Time.time;
        touchStartPos = pos;
    }

    private void HandlePointerMove(Vector2 pos)
    {
        if (!isTouching) return;

        OnTouchMoveEvent?.Invoke(pos);

        if (!isDragging && Time.time - touchStartTime >= holdTimestamp)
        {
            isDragging = true;
            OnDragBeginEvent?.Invoke(pos);
        }

        if (isDragging)
            OnDragEvent?.Invoke(pos);
    }

    private void HandlePointerEnd(Vector2 pos)
    {
        if (!isTouching) return;

        lastEndPos = pos;
        float duration = Time.time - touchStartTime;

        if (!isDragging && duration < holdTimestamp)
            OnTapEvent?.Invoke(pos);

        if (isDragging)
            OnDragEndEvent?.Invoke(pos);

        isTouching = false;
        isDragging = false;
    }

    private void HandleTouchInput(Vector3 pWorld)
    {
        int count = Input.touchCount;

        if (count == 0 || m_isGameShop) return;

        if (m_isMainMenu)
        {
            if (count == 2)
            {
                Touch t0 = Input.GetTouch(0);
                Touch t1 = Input.GetTouch(1);

                Vector2 p0Prev = t0.position - t0.deltaPosition;
                Vector2 p1Prev = t1.position - t1.deltaPosition;

                float prevMag = (p0Prev - p1Prev).magnitude;
                float currMag = (t0.position - t1.position).magnitude;

                OnZoomEvent?.Invoke((currMag - prevMag) * 0.25f);
                OnTwoFingerDragEvent?.Invoke((t0.deltaPosition + t1.deltaPosition) / 0.7f);
            }
            return;
        }

        if (count != 1) return;

        Touch touch = Input.GetTouch(0);

        if (box != null)
        {
            float half = box.bounds.size.x * 0.5f;
            float center = box.transform.position.x;
            pWorld.x = Mathf.Clamp(pWorld.x, center - half, center + half);
        }
        pWorld.z = transform.position.z;
        worldPos = pWorld;
        Vector2 pos = mainCamera.WorldToScreenPoint(pWorld);

        if (touch.phase == TouchPhase.Began)
            HandlePointerBegin(pos);
        else if (touch.phase == TouchPhase.Moved || touch.phase == TouchPhase.Stationary)
            HandlePointerMove(pos);
        else if (touch.phase == TouchPhase.Ended || touch.phase == TouchPhase.Canceled)
            HandlePointerEnd(pos);
    }

    private void HandleMouseInput(Vector3 pWorld)
    {
        if (m_isGameShop) return;

        if (m_isMainMenu)
        {
            float scroll = Input.mouseScrollDelta.y;
            if (Mathf.Abs(scroll) > 0.001f)
                OnZoomEvent?.Invoke(scroll * 3f);

            if (Input.GetMouseButton(1))
            {
                Vector2 delta = new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y")) * 3f;
                OnTwoFingerDragEvent?.Invoke(delta);
            }
            return;
        }

        if (box != null)
        {
            float half = box.bounds.size.x * 0.5f;
            float center = box.transform.position.x;
            pWorld.x = Mathf.Clamp(pWorld.x, center - half, center + half);
        }
        pWorld.z = transform.position.z;
        worldPos = pWorld;
        Vector2 pos = mainCamera.WorldToScreenPoint(pWorld);

        if (Input.GetMouseButtonDown(0))
            HandlePointerBegin(pos);
        else if (Input.GetMouseButton(0))
            HandlePointerMove(pos);
        else if (Input.GetMouseButtonUp(0))
            HandlePointerEnd(pos);
    }

}
