using UnityEngine;

public enum eScreenRatioType : byte
{
    Square,  
    Standard, 
    Long  
}

public static class ScreenRatioHelper
{
    public static float GetUnitSize()
    {
        eScreenRatioType type = GetScreenRatioType();
        float worldHeight = GameManager.Instance.m_camera.orthographicSize * 2f;

        switch (type)
        {
            case eScreenRatioType.Square:
                return worldHeight / 90f;   
            case eScreenRatioType.Standard:
                return worldHeight / 100f; 
            case eScreenRatioType.Long:
                Debug.Log(worldHeight/180f);
                return worldHeight / 110f;
        }
        return 0.1f;
    }

    public static eScreenRatioType GetScreenRatioType()
    {
        float worldHeight = GameManager.Instance.m_camera.orthographicSize * 2f;
        float worldWidth = worldHeight * GameManager.Instance.m_camera.aspect;
        float aspect = Mathf.Max(worldWidth / worldHeight, worldHeight / worldWidth);

        if (aspect <= 1.7f)
            return eScreenRatioType.Square;
        else if (aspect <= 16f / 9f + 0.01f) 
            return eScreenRatioType.Standard;
        else
            return eScreenRatioType.Long;
    }



}
