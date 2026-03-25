using DG.Tweening;
using UnityEngine;

public class BallSpawner : Spawner<Ball>
{
    private LevelController levelController;
    public float totalVolume = 0f;
    protected override void Start()
    {
        base.Start();
        ServiceLocator.Instance.GetService<IGameDataService>().LoadLevel(this);
    }
    public Ball Spawn(eBallType ballType, Vector3 spawnPos, Quaternion spawnRot, bool isMerge = false)
    {
        Ball ball = GetPrefabByType(ballType);
        currentHolder = GetHolderByType(ballType);
        if (ball == null) return null;

        return Spawn(ball, spawnPos, spawnRot, isMerge);
    }
    public override Ball Spawn(Ball ball, Vector3 spawnPos, Quaternion spawnRot, bool isMerge = false)
    {
        Ball newBall = GetPrefabFromPool(ball);

        totalVolume += GetVolume(newBall);
        
        newBall.transform.SetParent(currentHolder, false);
        newBall.transform.localPosition = spawnPos;
        newBall.transform.localRotation = spawnRot;

        if (isMerge)
        {
            newBall.transform.localScale = Vector3.zero;
            newBall.transform.DOScale(ball.transform.localScale, 0.2f);
        }
        else
        {
            newBall.transform.localScale = ball.transform.localScale;
            newBall.transform.parent = this.currentHolder;
        }
        return newBall;
    }
    public override void Despawn(Ball ball, float timeDelay = 0f)
    {
        base.Despawn(ball, timeDelay);
        totalVolume -= GetVolume(ball);
    }
    protected Transform GetHolderByType(eBallType ballType)
    {
        foreach (Transform holder in holderList)
        {
            if (holder.name == ballType.ToString()) return holder;
        }
        return null;
    }

    protected Ball GetPrefabByType(eBallType ballType)
    {
        foreach (Ball prefab in componentList)
        {
            if (prefab.type == ballType) return prefab;
        }
        return null;
    }
    private float GetVolume(Ball ball)
    {
        switch (ball.type)
        {
            case eBallType.Ball01:
                return 1;
            case eBallType.Ball02:
                return 2;
            case eBallType.Ball03:
                return 3;
            case eBallType.Ball04:
                return 4;
            case eBallType.Ball05:
                return 5;
            case eBallType.Ball06:
                return 6;
            case eBallType.Ball07:
                return 7;
            case eBallType.Ball08:
                return 8;
            case eBallType.Ball09:
                return 9;
            case eBallType.Ball10:
                return 10;
            case eBallType.Ball11:
                return 11;
        }
        return 0;
    }
}
