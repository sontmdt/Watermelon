using UnityEngine;

// ── Data models ───────────────────────────────────────────────────────────────

[System.Serializable]
public class ObjectSaveData
{
    public System.Collections.Generic.List<ObjectSaveStruct> objectSaveData = new System.Collections.Generic.List<ObjectSaveStruct>();

    public int GetObjectLevel(string id)
    {
        var found = objectSaveData.Find(u => u.idObject == id);
        return found.idObject == null ? 0 : found.currentLevel;
    }

    public void SetObjectLevel(string id, int level)
    {
        var index = objectSaveData.FindIndex(u => u.idObject == id);
        if (index >= 0)
            objectSaveData[index] = new ObjectSaveStruct { idObject = id, currentLevel = level };
        else
            objectSaveData.Add(new ObjectSaveStruct { idObject = id, currentLevel = level });
    }
}

[System.Serializable]
public class PlayerSaveData
{
    public int coin = 1000;
    public int diamond = 100;
    public int highestScore;
    public int numbleDestroyBooster = 2;
    public int numbleShakeBooster = 2;
    public int numbleUpgradeBooster = 2;
    public int numbleSmallDestroyBooster = 2;
}

[System.Serializable]
public class LevelSavaData
{
    public System.Collections.Generic.List<BallSaveStruct> ballSavaData = new System.Collections.Generic.List<BallSaveStruct>();
    public eBallType ball;
    public eBallType nextBall;
    public int currentPoint;

    public BallSaveStruct GetBallData(int id) => ballSavaData.Find(u => u.idBall == id);

    public void SetBallData(int id, eBallType type, Vector3 position, Quaternion rotation)
    {
        ballSavaData.Add(new BallSaveStruct
        {
            idBall = id,
            type = type,
            position = position,
            rotation = rotation
        });
    }

    public void ClearLevelData()
    {
        ballSavaData.Clear();
        ball = eBallType.None;
        nextBall = eBallType.None;
        currentPoint = 0;
    }
}

[System.Serializable]
public class GameSaveData
{
    public ObjectSaveData objectUpgradeData = new ObjectSaveData();
    public PlayerSaveData playerData = new PlayerSaveData();
    public LevelSavaData levelSaveData = new LevelSavaData();
}

// ── Service ───────────────────────────────────────────────────────────────────

public class GameDataService : MonoBehaviour, IGameDataService, IInitializable
{
    public GameSaveData data { get; private set; } = new GameSaveData();

    private ISaveService SaveSvc => ServiceLocator.Instance.GetService<ISaveService>();

    private bool _isDirty;

    public void Initialize() => Load();

    public void MarkDirty() => _isDirty = true;

    public void Save()
    {
        SaveSvc?.Save<GameSaveData>("GameSave", data);
        _isDirty = false;
    }

    private void OnApplicationPause(bool paused)
    {
        if (paused && _isDirty) Save();
    }

    private void OnApplicationQuit()
    {
        if (_isDirty) Save();
    }

    private void Load()
    {
        var svc = SaveSvc;
        if (svc != null)
            data = svc.Load<GameSaveData>("GameSave");
        else
            data = new GameSaveData();
    }

    public void SaveAllBallState()
    {
        var level = GameManager.Instance.m_levelController;
        if (level == null) return;

        var player = level.m_playerController;
        if (player != null)
        {
            data.levelSaveData.ball = player.ballType != eBallType.None ? player.ballType : eBallType.None;
            data.levelSaveData.nextBall = player.nextBallType;
            data.levelSaveData.currentPoint = ServiceLocator.Instance.GetService<IScoreService>().CurrentScore;
        }

        data.levelSaveData.ballSavaData.Clear();

        int t = 0;
        if (GameManager.Instance.m_levelController == null) return;

        foreach (var kvp in Ball.ballMap)
        {
            Transform ballTransform = kvp.Key;
            Ball b = kvp.Value;
            if (ballTransform == null || b == null) continue;
            data.levelSaveData.SetBallData(t, b.type, ballTransform.position, ballTransform.rotation);
            t++;
        }
        Save();
    }

    public void LoadLevel(BallSpawner ballSpawner)
    {
        LevelController levelController = GameManager.Instance.m_levelController;
        ServiceLocator.Instance.GetService<IScoreService>().Initialize(data.levelSaveData.currentPoint);
        foreach (var savedBall in data.levelSaveData.ballSavaData)
        {
            Ball ball = ballSpawner.Spawn(savedBall.type, savedBall.position, savedBall.rotation);
            if (levelController.m_missonList.Count < (int)ball.type)
            {
                levelController.m_missonList.Add(ball.type);
                levelController.UpdateMission();
            }
            if (ball == null) continue;

            ball.gameObject.SetActive(true);
            ball.rb.bodyType = RigidbodyType2D.Kinematic;
        }

        foreach (var kvp in Ball.ballMap)
        {
            Ball ball = kvp.Value;
            if (ball == null) continue;
            ball.rb.bodyType = RigidbodyType2D.Dynamic;
            ball.DisableTrail();
        }
    }

    public void ResetLevel()
    {
        data.levelSaveData.ClearLevelData();
        Save();
    }
}
