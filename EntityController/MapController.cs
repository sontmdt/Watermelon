using UnityEngine;

public class MapController : MonoBehaviour
{
    [SerializeField] private Camera cam;
    [SerializeField] private float zoomSpeed = 10f;   
    [SerializeField] private float panSpeed = 0.07f;     
    [SerializeField] private float minZoom = 3f;
    [SerializeField] private float maxZoom = 15f;
    [SerializeField] private float smoothTime = 0.15f;   

    private Grid grid;
    private Vector3 camStartPos;
    private float camStartSize;

    private float targetZoom;
    private Vector3 targetPos;
    private Vector3 velocity = Vector3.zero; 

    private void OnDisable()
    {
        if (InputManager.Instance == null) return;
        InputManager.Instance.OnZoomEvent -= OnZoom;
        InputManager.Instance.OnTwoFingerDragEvent -= OnPan;
    }

    private void Start()
    {
        if (cam == null) cam = GameManager.Instance.m_camera;
        if (grid == null) grid = GameManager.Instance.m_grid;
        camStartPos = cam.transform.position;
        camStartSize = cam.orthographicSize;
        targetZoom = camStartSize;
        targetPos = camStartPos;

        InputManager.Instance.OnZoomEvent += OnZoom;
        InputManager.Instance.OnTwoFingerDragEvent += OnPan;
    }

    private void Update()
    {
        if (GameManager.Instance.CurrentState != EventID.StateMainMenu)
            return;
        cam.orthographicSize = Mathf.Lerp(cam.orthographicSize, targetZoom, Time.deltaTime * (1f / smoothTime));
        cam.transform.position = Vector3.SmoothDamp(cam.transform.position, targetPos, ref velocity, smoothTime);
        ClampCamera();
    }

    private void OnZoom(float delta)
    {
        if (GameManager.Instance.CurrentState != EventID.StateMainMenu) return;
        targetZoom = Mathf.Clamp(targetZoom - delta * zoomSpeed * Time.deltaTime, minZoom, maxZoom);
    }

    private void OnPan(Vector2 delta)
    {
        if (GameManager.Instance.CurrentState != EventID.StateMainMenu) return;
        Vector3 move = new Vector3(-delta.x * panSpeed, -delta.y * panSpeed, 0);
        targetPos += move;
    }
    private void ClampCamera()
    {
        if (grid == null) return;

        int width = 100;
        int height = 50;

        float halfHeight = cam.orthographicSize;
        float halfWidth = cam.aspect * halfHeight;

        Vector3 pos = cam.transform.position;
        pos.x = Mathf.Clamp(pos.x, halfWidth, width - halfWidth);
        pos.y = Mathf.Clamp(pos.y, halfHeight, height - halfHeight);

        cam.transform.position = pos;
    }
    public void ResetCamera()
    {
        cam.transform.position = camStartPos;
        cam.orthographicSize = camStartSize;
        targetZoom = camStartSize;
        targetPos = camStartPos;
    }
}
