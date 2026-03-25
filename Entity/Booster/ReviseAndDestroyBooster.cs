using DG.Tweening;
using UnityEngine;

public class ReviseAndDestroyBooster : MonoBehaviour, IBooster
{
    private LevelController levelController;
    private IBoosterContext _boosterCtx;

    private void Start()
    {
        if (levelController == null) levelController = GameManager.Instance.m_levelController;
        _boosterCtx = ServiceLocator.Instance.GetService<IBoosterContext>();
    }

    public void Cancel()
    {
        throw new System.NotImplementedException();
    }

    public void Effect()
    {
        DOVirtual.DelayedCall(0.5f, () =>
        {
            levelController.m_playerController.isDead = false;
            levelController.m_gameOverTrigger.ResetTrigger();
            EventManager.Instance.PostEvent(EventID.StateGameStarted);
            levelController.m_playerController.DisableSpawn();

            Transform[] balls = new Transform[Ball.ballMap.Count];
            Ball.ballMap.Keys.CopyTo(balls, 0);

            for (int i = balls.Length - 1; i >= 0; i--)
            {
                Transform ballTransform = balls[i];
                if (ballTransform == null || ballTransform.position.y <= 0) continue;
                if (!Ball.ballMap.TryGetValue(ballTransform, out Ball ballObj)) continue;
                MyAnimation.PlayDestroyBallAnimation(ballTransform, levelController.fxSpawner);
                GameManager.Instance.m_levelController.ballSpawner.Despawn(ballTransform.GetComponent<Ball>());
            }

            foreach (var kvp in Ball.ballMap)
            {
                Ball b = kvp.Value;
                if (b == null) continue;
                b.ChangeState(isDead: false);
                b.rb.constraints = RigidbodyConstraints2D.None;
                b.rb.bodyType = RigidbodyType2D.Dynamic;
            }

            levelController.numbleReviveByAds -= 1;

            DOVirtual.DelayedCall(0.2f, () =>
            {
                levelController.m_playerController.EnableSpawn();
                _boosterCtx.Deactivate();
            });
        }).SetId("Gameplay");
    }
}
