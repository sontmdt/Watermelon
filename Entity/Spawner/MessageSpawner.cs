//using UnityEngine;

//public class MessageSpawner : Spawner
//{
//    public Transform SpawnMessage(string prefabName, Vector3 spawnPos = default, Quaternion spawnRot = default)
//    {
//        Transform prefab = this.GetPrefabByName(prefabName);
//        currentHolder = this.GetHolderByName(prefabName);
//        if (prefab == null) return null;

//        return SpawnMessage(prefab, spawnPos, spawnRot);
//    }

//    public Transform SpawnMessage(Transform prefab, Vector3 spawnPos = default, Quaternion spawnRot = default)
//    {
//        Transform newPrefab = this.GetPrefabFromPool(prefab);
//        newPrefab.SetParent(currentHolder, false);

//        RectTransform rect = newPrefab as RectTransform;
//        if (rect != null)
//        {
//            Vector2 screenPos;

//            if (spawnPos == default)
//            {
//                if (Input.touchCount > 0)
//                {
//                    screenPos = Input.GetTouch(0).position;
//                }
//                else
//                {
//                    screenPos = Input.mousePosition;
//                }
//            }
//            else
//            {
//                screenPos = Camera.main.WorldToScreenPoint(spawnPos);
//            }
//            Vector2 canvasPos;
//            RectTransformUtility.ScreenPointToLocalPointInRectangle(
//                currentHolder as RectTransform,
//                screenPos,
//                currentHolder.GetComponentInParent<Canvas>().worldCamera,
//                out canvasPos
//            );

//            rect.anchoredPosition = canvasPos;
//            rect.localRotation = spawnRot;
//            rect.localScale = prefab.localScale;
//        }
//        else
//        {
//            newPrefab.position = spawnPos;
//            newPrefab.rotation = spawnRot;
//            newPrefab.localScale = prefab.localScale;
//        }

//        return newPrefab;
//    }


//}
