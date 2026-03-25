using UnityEngine;

[CreateAssetMenu(fileName = "GameSettings", menuName = "Settings/GameSettings")]
public class GameSettings : ScriptableObject
{
    [Header("Gameplay Timing")]
    public float mergeDelay = 0.05f;
    public float reviveDuration = 5f;
    public float gameOverDelay = 0.5f;

    [Header("Coin Rewards")]
    public int coinRewardBall09 = 10;
    public int coinRewardBall10 = 20;
    public int coinRewardBall11 = 30;

    [Header("Ball Spawn Weights (Ball01 - Ball05)")]
    public int[] ballSpawnWeights = new int[5] { 10, 15, 20, 25, 30 };
}
