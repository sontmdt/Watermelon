using DG.Tweening;
using System.Collections.Generic;
using UnityEngine;

public class SmallDestroyBooster : MonoBehaviour, IBooster
{
    private List<Ball> ballList;
    public Transform bomb;
    private LevelController levelController;
    private Camera mainCamera;
    private IBoosterContext _boosterCtx;
    private UIGameHanging _uiGameHanging;

    private void Start()
    {
        if (levelController == null) levelController = GameManager.Instance.m_levelController;
        mainCamera = GameManager.Instance.m_camera;
        _boosterCtx = ServiceLocator.Instance.GetService<IBoosterContext>();
        _uiGameHanging = Object.FindObjectOfType<UIGameHanging>(true);
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

        _boosterCtx.Deactivate();
        GameManager.Instance.StopSpotlightTracking();

        ballList = new List<Ball>(levelController.ballDestroyList);
        levelController.ballDestroyList.Clear();
        levelController.m_spotlight.m_spotlightCollider.enabled = false;
        _uiGameHanging?.HideForEffect();
        levelController.m_spotlight.StartWarningLight();
        DOVirtual.DelayedCall(0.3f, () =>
        {
            MyAnimation.PlayDropBomb(bomb, mousePos, () =>
            {
                for (int i = ballList.Count - 1; i >= 0; i--)
                {
                    Ball ball = ballList[i];
                    if (ball == null) continue;
                    MyAnimation.PlayMergeAnimation(ball.transform, levelController.fxSpawner);
                    levelController.ballSpawner.Despawn(ball);
                }
                GameManager.Instance.m_playerAssetController.UseAsset(eAssetType.SmallDestroyBooster, 1);
                OutCurrentState();
                levelController.m_spotlight.EndWarningLight();
            });
        }).SetId("Gameplay");
    }

    private void OutCurrentState()
    {
        _boosterCtx.Deactivate();
        EventManager.Instance.PostEvent(EventID.StateGameStarted);
        GameManager.Instance.SwitchColorScreen();
        InputManager.Instance.isBusy = false;
    }
}
