using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.SceneManagement;
using UnityEngine.Tilemaps;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public GameSettings settings;
    public Helper m_helper;
    public Camera m_camera;
    public Light2D m_light;
    public float m_unitSize = 0.5f;

    public SpriteRenderer lightOverLay;
    public MapController m_mapController;
    public LevelController m_levelController { get; private set; }
    public GameObject currentLevel;
    public PlayerAssetController m_playerAssetController;
    public UpgradeDatabase upgradeDatabase;

    public Grid m_grid { get; private set; }
    public Tilemap m_tilemap { get; private set; }

    public EventID CurrentState { get; private set; }

    private AudioService _audio;
    private IGameDataService GameData => ServiceLocator.Instance.GetService<IGameDataService>();

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
        if (m_playerAssetController == null) m_playerAssetController = GetComponentInChildren<PlayerAssetController>();
        Camera.main.backgroundColor = Color.black;
        if (m_helper == null) m_helper = FindFirstObjectByType<Helper>();

        this.RegisterListener(EventID.StateGameSetup, OnStateGameSetup);
        this.RegisterListener(EventID.StateMainMenu, OnStateMainMenu);
        this.RegisterListener(EventID.StateGameStarted, OnStateGameStarted);
        this.RegisterListener(EventID.StateGamePause, OnStateGamePause);
        this.RegisterListener(EventID.StateGamePopup, OnStateGamePopup);
        this.RegisterListener(EventID.StateGameShop, OnStateGameShop);
        this.RegisterListener(EventID.StateGameHanging, OnStateGameHanging);
        this.RegisterListener(EventID.StateGameOver, OnStateGameOver);

        EventManager.Instance.PostEvent(EventID.StateGameSetup);
        m_unitSize = ScreenRatioHelper.GetUnitSize();
    }

    private void Start()
    {
        _audio = ServiceLocator.Instance.GetService<IAudioService>() as AudioService;
        SceneManager.sceneLoaded += OnSceneLoaded;
        Loader.Load(eScene.InGame);
        _audio?.PlayOpeningMusic();
        upgradeDatabase.SyncFromSave();
    }

    [ContextMenu("Reset All Upgrades")]
    private void ResetAll()
    {
        upgradeDatabase.ResetAllUpgrades();
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
        EventManager.Instance.RemoveListener(EventID.StateGameSetup, OnStateGameSetup);
        EventManager.Instance.RemoveListener(EventID.StateMainMenu, OnStateMainMenu);
        EventManager.Instance.RemoveListener(EventID.StateGameStarted, OnStateGameStarted);
        EventManager.Instance.RemoveListener(EventID.StateGamePause, OnStateGamePause);
        EventManager.Instance.RemoveListener(EventID.StateGamePopup, OnStateGamePopup);
        EventManager.Instance.RemoveListener(EventID.StateGameShop, OnStateGameShop);
        EventManager.Instance.RemoveListener(EventID.StateGameHanging, OnStateGameHanging);
        EventManager.Instance.RemoveListener(EventID.StateGameOver, OnStateGameOver);
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        DOTween.KillAll();

        if (scene.name == "InGame")
        {
            if (m_levelController == null)
            {
                GameObject obj = Instantiate(currentLevel, Vector3.zero, Quaternion.identity);
                m_levelController = obj.GetComponent<LevelController>();
            }
            m_unitSize = ScreenRatioHelper.GetUnitSize();
            if (m_helper != null) m_helper.Setup();
            EventManager.Instance.PostEvent(EventID.StateGameStarted);
        }
        if (scene.name == "Menu")
        {
            m_levelController.ClearLevel();
            EventManager.Instance.PostEvent(EventID.StateMainMenu);
            if (m_grid == null) m_grid = FindFirstObjectByType<Grid>();
            if (m_tilemap == null && m_grid != null) m_tilemap = m_grid.GetComponentInChildren<Tilemap>();
        }
    }

    private void OnApplicationPause(bool pause)
    {
        if (pause) GameData.SaveAllBallState();
    }

    private void OnApplicationQuit()
    {
        DOTween.KillAll();
        GameData.SaveAllBallState();
        if (CurrentState == EventID.StateGameOver) GameData.ResetLevel();
    }

    // ── State event handlers ──────────────────────────────────────────────

    private void OnStateGameSetup(object _)
    {
        CurrentState = EventID.StateGameSetup;
        ResumeGame();
    }

    private void OnStateMainMenu(object _)
    {
        CurrentState = EventID.StateMainMenu;
        _audio?.StopGameMusic();
        _audio?.PlayOpeningMusic();
        ResumeGame();
    }

    private void OnStateGameStarted(object _)
    {
        var prev = CurrentState;
        CurrentState = EventID.StateGameStarted;
        StopSpotlightTracking();
        if (prev != EventID.StateGamePause && prev != EventID.StateGamePopup && prev != EventID.StateGameHanging)
            _audio?.PlayInGameMusic();
        ResumeGame();
    }

    private void OnStateGamePause(object _)
    {
        CurrentState = EventID.StateGamePause;
        PauseGame();
    }

    private void OnStateGamePopup(object _)
    {
        CurrentState = EventID.StateGamePopup;
        var ctx = ServiceLocator.Instance.GetService<IBoosterContext>();
        if (ctx.ActiveBooster == eActiveBooster.ReviseAndDestroy)
            DOTween.Pause("GamePlay");
        else
            ResumeGame();
    }

    private void OnStateGameShop(object _)
    {
        CurrentState = EventID.StateGameShop;
        ResumeGame();
    }

    private void OnStateGameHanging(object _)
    {
        CurrentState = EventID.StateGameHanging;
        DOTween.Play("GamePlay");
        Time.timeScale = 1f;

        var ctx = ServiceLocator.Instance.GetService<IBoosterContext>();
        switch (ctx.ActiveBooster)
        {
            case eActiveBooster.Upgrade:
                m_levelController.lightBall.gameObject.SetActive(true);
                m_levelController.ballUpgradeNameList =
                    new List<eBallType>(MyAnimation.AimUpgradeBallAnimation(Ball.ballMap));
                break;

            case eActiveBooster.Destroy:
                SwitchColorScreen();
                if (isColorActive) MyAnimation.PlayBeginDestroyAnimation(m_levelController.m_spotlight);
                StartSpotlightTracking(trackTarget: true);
                break;

            case eActiveBooster.SmallDestroy:
                SwitchColorScreen();
                if (isColorActive) MyAnimation.PlayBeginSmailDestroyAnimation(m_levelController.m_spotlight);
                StartSpotlightTracking(trackTarget: false);
                break;
        }
    }

    private void OnStateGameOver(object _)
    {
        CurrentState = EventID.StateGameOver;
        _audio?.StopGameMusic();
        _audio?.PlayGameOverMusic();
        PauseGame();
    }

    private void PauseGame()
    {
        DOTween.Pause("GamePlay");
        Time.timeScale = 0f;
    }

    private void ResumeGame()
    {
        DOTween.Play("GamePlay");
        Time.timeScale = 1f;
    }

    // ── Color screen toggle ───────────────────────────────────────────────

    private bool isColorActive = false;
    public void SwitchColorScreen()
    {
        isColorActive = !isColorActive;
        m_levelController.m_spotlight.transform.position = Vector3.zero;
        m_light.gameObject.SetActive(isColorActive);
        m_levelController.m_spotlight.gameObject.SetActive(isColorActive);
    }

    // ── Spotlight tracking ────────────────────────────────────────────────

    private Coroutine _spotlightCoroutine;
    private Ball _hoverTarget;

    private void StartSpotlightTracking(bool trackTarget)
    {
        StopSpotlightTracking();
        _spotlightCoroutine = StartCoroutine(TrackSpotlight(trackTarget));
    }

    public void StopSpotlightTracking()
    {
        if (_spotlightCoroutine != null)
        {
            StopCoroutine(_spotlightCoroutine);
            _spotlightCoroutine = null;
        }
        if (_hoverTarget != null)
        {
            _hoverTarget.OnHoverExit();
            _hoverTarget = null;
        }
    }

    private IEnumerator TrackSpotlight(bool trackTarget)
    {
        while (true)
        {
            if (InputManager.Instance.isTouching)
            {
                Vector3 world = InputManager.Instance.worldPos;
                m_levelController.m_spotlight.transform.position = world;

                if (trackTarget)
                {
                    Collider2D col = Physics2D.OverlapPoint(world);
                    if (col != null && Ball.ballMap.TryGetValue(col.transform, out Ball target))
                    {
                        if (target != _hoverTarget)
                        {
                            _hoverTarget?.OnHoverExit();
                            _hoverTarget = target;
                            _hoverTarget.OnHoverEnter();
                        }
                    }
                    else if (_hoverTarget != null)
                    {
                        _hoverTarget.OnHoverExit();
                        _hoverTarget = null;
                    }
                }
            }

            yield return null;
        }
    }
}
