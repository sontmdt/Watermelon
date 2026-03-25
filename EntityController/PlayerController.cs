using DG.Tweening;
using System;
using System.Collections;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public GameObject player;
    public eBallType ballType;
    public event Action<eEyeMode, Transform> OnMoveEvent = delegate { };
    [SerializeField] private SpriteRenderer[] balls;
    public SpriteRenderer ball;
    public eBallType nextBallType;
    public bool isDead = false;

    [SerializeField] public BoosterDispatcher boosterDispatcher;
    [SerializeField] private float spawnDelay = 0.4f;

    private bool canSpawn = true;
    private float _spawnCooldown = 0f;

    private BallSpawner ballSpawner;
    private IRandomService randomService;
    private IScoreService scoreService;

    private InputManager inputManager;
    private GameManager gameManager;
    private LevelController levelController;
    private Camera mainCamera;
    private AudioService _audio;
    private IBoosterContext _boosterCtx;

    public void Setup()
    {
        if (gameManager == null) gameManager = GameManager.Instance;
        if (inputManager == null) inputManager = InputManager.Instance;
        if (levelController == null) levelController = gameManager.m_levelController;
        mainCamera = gameManager.m_camera;
        if (_audio == null) _audio = ServiceLocator.Instance.GetService<IAudioService>() as AudioService;
        _boosterCtx = ServiceLocator.Instance.GetService<IBoosterContext>();

        if (inputManager != null)
        {
            inputManager.OnTapEvent += Tap;
            inputManager.OnTouchMoveEvent += TouchMove;
            inputManager.OnDragEndEvent += DragEnd;
        }

        boosterDispatcher?.Setup();

        if (ballSpawner == null) ballSpawner = levelController.ballSpawner;
        if (balls == null) balls = GetComponentsInChildren<SpriteRenderer>();
        if (randomService == null) randomService = ServiceLocator.Instance.GetService<IRandomService>();
        if (scoreService == null) scoreService = ServiceLocator.Instance.GetService<IScoreService>();

        var saveData = ServiceLocator.Instance.GetService<IGameDataService>().data.levelSaveData;
        if (saveData.ball != eBallType.None)
            ballType = saveData.ball;
        else ballType = randomService.RandomNextBall();

        ball = balls[(int)ballType - 1];
        ball.gameObject.SetActive(true);

        if (saveData.nextBall != eBallType.None)
            nextBallType = saveData.nextBall;
        else nextBallType = randomService.RandomNextBall();

        EventManager.Instance.PostEvent(EventID.OnDropped, nextBallType);
    }

    private void Update()
    {
        if (canSpawn) return;
        _spawnCooldown += Time.deltaTime;
        if (_spawnCooldown >= spawnDelay)
        {
            canSpawn = true;
            _spawnCooldown = 0f;
        }
    }

    private void Tap(Vector2 screenPos)
    {
        if (isDead) return;
        if (_boosterCtx.IsActive)
        {
            boosterDispatcher.Execute(_boosterCtx.ActiveBooster);
            return;
        }
        if (inputManager.isBusy || !canSpawn) return;
        if (gameManager.CurrentState == EventID.StateGameHanging) return;
        MyAnimation.PlayTouchAnimation(inputManager.worldPos, levelController.fxSpawner);
        TouchMove(screenPos);
        SpawnBall();
    }

    private void TouchMove(Vector2 screenPos)
    {
        if (!canSpawn) return;
        Vector3 pos = transform.position;
        transform.position = new Vector3(inputManager.worldPos.x, pos.y, pos.z);
        OnMoveEvent?.Invoke(eEyeMode.FollowPlayer, transform);
    }

    private void DragEnd(Vector2 pos)
    {
        if (isDead) return;
        if (_boosterCtx.IsActive)
        {
            boosterDispatcher.Execute(_boosterCtx.ActiveBooster);
            return;
        }
        if (inputManager.isBusy) return;
        if (gameManager.CurrentState == EventID.StateGameHanging) return;
        MyAnimation.PlayTouchAnimation(inputManager.worldPos, levelController.fxSpawner);
        SpawnBall();
    }

    public void SpawnBall()
    {
        if (isDead || !canSpawn) return;

        eBallType prefabName = ballType;

        if (ballType == nextBallType)
            randomService.countConsecutiveSpawn = 2;

        randomService.UpdateConsecutiveSpawn(ballType);

        Ball spawnedBall = ballSpawner.Spawn(prefabName, transform.position, transform.rotation);
        spawnedBall.gameObject.SetActive(true);

        spawnedBall.isFalling = true;
        StartCoroutine(DisableFalling(spawnedBall));

        spawnedBall.ActivateTrail();
        OnMoveEvent?.Invoke(eEyeMode.FollowFall, spawnedBall.transform);

        scoreService.AddScore(prefabName);

        ballType = nextBallType;
        nextBallType = randomService.RandomNextBall();

        UpdateNextBall(ballType);
        EventManager.Instance.PostEvent(EventID.OnDropped, nextBallType);

        _audio?.PlayDropSound();

        canSpawn = false;
        _spawnCooldown = 0f;
    }

    public void DisableSpawn()
    {
        canSpawn = false;
        _spawnCooldown = 0f;
    }

    public void EnableSpawn()
    {
        canSpawn = true;
        _spawnCooldown = 0f;
    }

    private WaitForSeconds fallDelay = new WaitForSeconds(0.4f);
    private WaitForSeconds showBallDelay = new WaitForSeconds(0.4f);
    private IEnumerator DisableFalling(Ball b)
    {
        yield return fallDelay;
        if (b != null)
            b.isFalling = false;
    }

    private Coroutine showBallCoroutine;

    private void UpdateNextBall(eBallType ballName)
    {
        if (isDead) return;

        if (showBallCoroutine != null)
        {
            StopCoroutine(showBallCoroutine);
            showBallCoroutine = null;
        }

        if (ball != null)
            ball.gameObject.SetActive(false);

        ball = balls[(int)ballName - 1];
        showBallCoroutine = StartCoroutine(ShowBallDelay(ball));
    }

    private IEnumerator ShowBallDelay(SpriteRenderer targetBall)
    {
        yield return showBallDelay;

        if (!isDead && targetBall != null)
            targetBall.gameObject.SetActive(true);

        showBallCoroutine = null;
    }

    public void Shake()            => boosterDispatcher?.Shake();
    public void SmallDestroy()     => boosterDispatcher?.SmallDestroy();
    public void ReviseAndDestroy() => boosterDispatcher?.ReviseAndDestroy();

    internal void Clear()
    {
        if (inputManager != null)
        {
            inputManager.OnTapEvent -= Tap;
            inputManager.OnTouchMoveEvent -= TouchMove;
            inputManager.OnDragEndEvent -= DragEnd;
        }
    }
}
