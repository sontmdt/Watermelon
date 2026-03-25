using DG.Tweening;
using System.Collections.Generic;
using UnityEngine;

public class UpgradeBooster : MonoBehaviour, IBooster
{
    private List<Ball> selectedBalls = new List<Ball>();
    private eBallType currentBallType = eBallType.None;
    private Ball b;
    private Ball newBall;

    private LevelController levelController;
    private Camera mainCamera;
    private IBoosterContext _boosterCtx;

    private void Start()
    {
        if (levelController == null) levelController = GameManager.Instance.m_levelController;
        mainCamera = GameManager.Instance.m_camera;
        _boosterCtx = ServiceLocator.Instance.GetService<IBoosterContext>();
    }

    public void Effect()
    {
        Vector2 screenPos = Input.touchCount > 0 ? Input.GetTouch(0).position : (Vector2)Input.mousePosition;
        Vector2 worldPoint = mainCamera.ScreenToWorldPoint(screenPos);

        Collider2D hit = Physics2D.OverlapPoint(worldPoint);
        if (hit == null || !hit.CompareTag("Ball"))
            return;

        Ball ball = Ball.ballMap.TryGetValue(hit.transform, out var found) ? found : null;

        if (ball == null || ball.type == eBallType.Ball11)
            return;
        if (!levelController.ballUpgradeNameList.Contains(ball.type)) return;
        if (!selectedBalls.Contains(ball))
        {
            if (currentBallType != ball.type && currentBallType != eBallType.None) return;
            currentBallType = ball.type;
            ball.SetIntensity(0.1f);
            selectedBalls.Add(ball);
            ball.aim.gameObject.SetActive(true);
        }
        if (selectedBalls.Count < 2)
            return;

        BallSpawner ballSpawner = levelController.ballSpawner;

        newBall = ballSpawner.Spawn(
            selectedBalls[0].nextType,
            selectedBalls[0].transform.position,
            Quaternion.identity
        );
        newBall.gameObject.SetActive(true);
        Ball.ballMap.TryGetValue(newBall.transform, out b);

        foreach (Ball selectedBall in selectedBalls)
        {
            if (selectedBall.aim != null)
                selectedBall.aim.gameObject.SetActive(false);
            MyAnimation.StopAimAnimation(selectedBall);
            levelController.ballSpawner.Despawn(selectedBall);
        }

        MyAnimation.PlayMergeAnimation(b.transform, levelController.fxSpawner);

        var settings = GameManager.Instance.settings;
        switch (b.type)
        {
            case eBallType.Ball09:
                GameManager.Instance.m_playerAssetController.AddAsset(eAssetType.Coin, settings.coinRewardBall09);
                break;
            case eBallType.Ball10:
                GameManager.Instance.m_playerAssetController.AddAsset(eAssetType.Coin, settings.coinRewardBall10);
                break;
            case eBallType.Ball11:
                GameManager.Instance.m_playerAssetController.AddAsset(eAssetType.Coin, settings.coinRewardBall11);
                break;
        }

        if (b.type != eBallType.Ball11) MyAnimation.PlayUpgradeBallAnimation(b);
        else
        {
            b.rb.bodyType = RigidbodyType2D.Kinematic;
            b.lightFX.PlayFX();
            this.PostEvent(EventID.OnBall11Spawned);
        }
        Cancel();
        GameManager.Instance.m_playerAssetController.UseAsset(eAssetType.UpgradeBooster, 1);
    }

    public void Cancel()
    {
        MyAnimation.StopAimAllAnimation(Ball.ballMap);
        _boosterCtx.Deactivate();
        levelController.lightBall.gameObject.SetActive(false);
        InputManager.Instance.isBusy = false;

        if (selectedBalls.Count > 0)
        {
            selectedBalls[0].SetIntensity(0f);
        }
        selectedBalls.Clear();
        currentBallType = eBallType.None;

        if (newBall != null)
        {
            if (newBall.type == eBallType.Ball11) return;
        }
        DOVirtual.DelayedCall(0.2f, () =>
        {
            EventManager.Instance.PostEvent(EventID.StateGameStarted);
        }).SetId("Gameplay");
    }
}
