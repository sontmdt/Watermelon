using DG.Tweening;
using UnityEngine;

public class DestroyBooster : MonoBehaviour, IBooster
{
    public Transform hammer;
    private LevelController levelController;
    private Camera mainCamera;
    private IBoosterContext _boosterCtx;

    private void Start()
    {
        if (levelController == null) levelController = GameManager.Instance.m_levelController;
        mainCamera = GameManager.Instance.m_camera;
        _boosterCtx = ServiceLocator.Instance.GetService<IBoosterContext>();
    }

    public void Cancel()
    {
        OutCurrentState();
    }

    public void Effect()
    {
        InputManager.Instance.isBusy = true;
        Vector3 mousePos = mainCamera.ScreenToWorldPoint(
            Input.touchCount > 0 ? (Vector3)Input.GetTouch(0).position : Input.mousePosition
        );
        mousePos.z = 0;

        if (_boosterCtx.InCancelZone)
        {
            OutCurrentState();
            return;
        }

        RaycastHit2D hit = Physics2D.Raycast(mousePos, Vector2.zero);
        bool hitBall = false;

        if (hit.collider != null && hit.collider.CompareTag("Ball"))
        {
            hitBall = true;
            MyAnimation.PlayDamBallAnimation(hit.collider.transform.position, levelController.fxSpawner, hammer, hit.collider.transform);
            if (Ball.ballMap.TryGetValue(hit.collider.transform, out Ball otherBall))
            {
                levelController.ballSpawner.Despawn(otherBall, 0.5f);
            }
        }

        if (!hitBall)
        {
            MyAnimation.PlayDamBallAnimation(mousePos, levelController.fxSpawner, hammer, null);
        }

        GameManager.Instance.m_playerAssetController.UseAsset(eAssetType.DestroyBooster, 1);
        OutCurrentState();
    }

    private void OutCurrentState()
    {
        _boosterCtx.Deactivate();
        EventManager.Instance.PostEvent(EventID.StateGameStarted);
        GameManager.Instance.SwitchColorScreen();
        InputManager.Instance.isBusy = false;
    }
}
