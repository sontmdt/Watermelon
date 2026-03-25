using System;
using System.Linq;
using UnityEngine;

public class Utils : MonoBehaviour
{
    public static eBallType RandomType()
    {
        eBallType[] types = ((eBallType[])Enum.GetValues(typeof(eBallType)))
            .Where(t => t != eBallType.None) 
            .ToArray();

        int randomIndex = UnityEngine.Random.Range(0, types.Length);
        return types[randomIndex];
    }

    public static eBallType RandomBallExcept(eBallType[] exceptTypes)
    {
        eBallType[] types = Enum.GetValues(typeof(eBallType))
            .Cast<eBallType>()
            .Where(t => t != eBallType.None)
            .Except(exceptTypes)
            .ToArray();

        int randomIndex = UnityEngine.Random.Range(0, types.Length);
        return types[randomIndex];
    }
}
