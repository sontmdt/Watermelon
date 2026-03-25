using DG.Tweening;
using System.Collections;
using UnityEngine;

public class BallMerge : MonoBehaviour
{
    [SerializeField] private Ball ball;
    private BallSpawner ballSpawner;
    private LevelController levelController;
    private PlayerAssetController playerAssetController;
    private AudioService _audio;
    private IScoreService _scoreService;
    private WaitForSeconds _mergeDelay;
    private GameSettings _settings;

    private void Awake()
    {
        ballSpawner = GameManager.Instance.m_levelController.ballSpawner;
        levelController = GameManager.Instance.m_levelController;
        playerAssetController = GameManager.Instance.m_playerAssetController;
        _audio = ServiceLocator.Instance.GetService<IAudioService>() as AudioService;
        _scoreService = ServiceLocator.Instance.GetService<IScoreService>();
        _settings = GameManager.Instance.settings;
        _mergeDelay = new WaitForSeconds(_settings.mergeDelay);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (!Ball.ballMap.TryGetValue(collision.transform, out Ball otherBall))
            return;

        if (!otherBall.IsSameType(ball))
            return;

        if (otherBall.isMerging || ball.isMerging || ball.IsMaxType())
            return;

        otherBall.isMerging = true;
        ball.isMerging = true;

        StartCoroutine(DelayedMerge(otherBall));
    }
    private IEnumerator DelayedMerge(Ball other)
    {
        yield return _mergeDelay;

        if (other == null || ball == null)
            yield break;

        Merge(other);
    }


    private void Merge(Ball other)
    {
        Vector3 spawnPos = (transform.position + other.transform.position) / 2f;

        if (ball.nextType != eBallType.None)
        {
            if (levelController.m_missonList.Count < (int)ball.nextType)
            {
                levelController.m_missonList.Add(ball.nextType);
                levelController.UpdateMission();
            }

            Ball newBall = ballSpawner.Spawn(ball.nextType, spawnPos, Quaternion.identity, true);
            newBall.gameObject.SetActive(true);

            if (ball.nextType == eBallType.Ball11)
            {
                if (Ball.ballMap.TryGetValue(newBall.transform, out Ball ball11))
                {
                    ball11.rb.bodyType = RigidbodyType2D.Kinematic;
                    ball11.lightFX.PlayFX();
                }
                this.PostEvent(EventID.OnBall11Spawned);
            }

            MyAnimation.PlayMergeAnimation(other.transform, levelController.fxSpawner);
            MyAnimation.PlayMergeAnimation(ball.transform, levelController.fxSpawner);

            ballSpawner.Despawn(other);
            ballSpawner.Despawn(ball);

            _scoreService.AddCombo();
            _scoreService.AddScore(newBall.type);

            switch (ball.nextType)
            {
                case eBallType.Ball09:
                    playerAssetController.AddAsset(eAssetType.Coin, _settings.coinRewardBall09);
                    break;
                case eBallType.Ball10:
                    playerAssetController.AddAsset(eAssetType.Coin, _settings.coinRewardBall10);
                    break;
                case eBallType.Ball11:
                    playerAssetController.AddAsset(eAssetType.Coin, _settings.coinRewardBall11);
                    break;
            }
            _audio?.PlayMergeSound();
        }
    }

    //public void ExplosionForceOptimized(Vector2 center, float radius, float force)
    //{
    //    Collider2D[] hits = Physics2D.OverlapCircleAll(center, radius);

    //    int maxHits = 10; 
    //    int count = Mathf.Min(hits.Length, maxHits);

    //    for (int i = 0; i < count; i++)
    //    {
    //        Collider2D hit = hits[i];
    //        if (hit.attachedRigidbody != null && hit.CompareTag("Ball"))
    //        {
    //            Vector2 dir = ((Vector2)hit.transform.position - center).normalized;
    //            hit.attachedRigidbody.AddForce(dir * force, ForceMode2D.Impulse);
    //        }
    //    }
    //}

}
