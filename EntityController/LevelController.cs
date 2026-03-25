using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class LevelController : MonoBehaviour
{
    public event Action<eBallType> OnMissionProgressChangedEvent = delegate { };
    public event Action OnAimAnimationEvent = delegate { };
    public PlayerController m_playerController;
    public GameObject m_environmet;
    public GameOverTrigger m_gameOverTrigger;

    public List<eBallType> m_missonList = new List<eBallType>();
    public Spotlight m_spotlight;

    public Light2D lightBall;
    public SpriteRenderer box;
    public Transform hand;
    public BallSpawner ballSpawner;
    public Spawner<FX> fxSpawner;
    public List<eBallType> ballUpgradeNameList;
    public List<Ball> ballDestroyList;

    public int numbleReviveByAds;
    public int numbleShakeBoosterByAds;
    public int numbleDestroyBoosterByAds;
    public int numbleSmallDestroyBoosterByAds;
    public int numbleUpgradeBoosterByAds;

    private IGameDataService GameData => ServiceLocator.Instance.GetService<IGameDataService>();
    private IBoosterContext _boosterCtx;

    private void Awake()
    {
        ServiceLocator.Instance.RegisterService<IScoreService>(new ScoreService());
        ServiceLocator.Instance.RegisterService<IRandomService>(new RandomService(GameManager.Instance.settings));
    }

    private void Start()
    {
        SetUp();
    }

    private void SetUp()
    {

        _boosterCtx = ServiceLocator.Instance.GetService<IBoosterContext>();
        EventManager.Instance.PostEvent(EventID.StateGameStarted);
        if (m_playerController) m_playerController.Setup();
        numbleReviveByAds = 1;
        numbleShakeBoosterByAds = 1;
        numbleDestroyBoosterByAds = 1;
        numbleSmallDestroyBoosterByAds = 1;
        numbleUpgradeBoosterByAds = 1;
        SetupMission();
    }

    private bool isGameOverRunning = false;

    public void GameOver()
    {
        if (isGameOverRunning) return;

        m_playerController.isDead = true;
        foreach (Ball ball in Ball.ballMap.Values)
        {
            ball.ChangeState(isDead: true);
            ball.rb.linearVelocity = Vector2.zero;
            ball.rb.angularVelocity = 0f;
            ball.rb.bodyType = RigidbodyType2D.Kinematic;
        }
        StartCoroutine(WaitForEnd());
    }

    private IEnumerator WaitForEnd()
    {
        yield return new WaitForSeconds(GameManager.Instance.settings.gameOverDelay);
        isGameOverRunning = true;
        int score = ServiceLocator.Instance.GetService<IScoreService>().CurrentScore;
        GameManager.Instance.m_helper.uiGameOver.UpdateResult(score);
        yield return null;
        if (numbleReviveByAds > 0)
        {
            _boosterCtx.Activate(eActiveBooster.ReviseAndDestroy);
            EventManager.Instance.PostEvent(EventID.StateGamePopup);
            yield return new WaitForSeconds(GameManager.Instance.settings.reviveDuration);
        }
        if (m_playerController.isDead == true)
        {
            EventManager.Instance.PostEvent(EventID.StateGameOver);
            _boosterCtx.Deactivate();
        }
        isGameOverRunning = false;
    }

    public void RestartGame()
    {
        ClearLevel();
        GameData.ResetLevel();
        GameData.Save();
        Loader.Load(eScene.InGame);
    }

    public void ClearLevel()
    {
        DOTween.KillAll();
        SetupMission();
        UpdateMission();
        ServiceLocator.Instance.GetService<IScoreService>().Reset();
        m_playerController.Clear();
        Destroy(this);
    }

    public void UpdateMission()
    {
        OnMissionProgressChangedEvent?.Invoke(m_missonList[m_missonList.Count - 1]);
    }

    public void StartAim()
    {
        OnAimAnimationEvent?.Invoke();
    }

    private void SetupMission()
    {
        m_missonList = new List<eBallType>()
        {
            eBallType.Ball01,
            eBallType.Ball02,
            eBallType.Ball03,
            eBallType.Ball04,
            eBallType.Ball05,
        };
    }
}
