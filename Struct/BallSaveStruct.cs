using UnityEngine;

[System.Serializable]
public struct BallSaveStruct
{
    public int idBall;
    public eBallType type;
    public Vector3 position;
    public Quaternion rotation;
}