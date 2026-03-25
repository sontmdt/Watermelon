using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner<T> : MonoBehaviour where T : MonoBehaviour
{
    [Header("Spawner")]
    [SerializeField] protected List<Transform> holderList;
    protected Transform currentHolder;

    [Header("Prefabs & Pool")]
    public List<T> componentList;
    [SerializeField] protected List<T> poolList;

    protected virtual void Reset()
    {
        LoadComponents();
        LoadHolder();
    }

    protected virtual void Start()
    {
        LoadComponents();
        LoadHolder();
    }

    protected void LoadHolder()
    {
        if (holderList.Count > 0) return;

        Transform holderParent = transform.Find("Holder");
        if (holderParent == null) return;

        foreach (Transform holder in holderParent)
        {
            holderList.Add(holder);
        }
    }

    protected void LoadComponents()
    {
        if (componentList.Count > 0) return;

        Transform prefabParent = transform.Find("Prefabs");
        if (prefabParent == null) return;

        foreach (Transform child in prefabParent)
        {
            T comp = child.GetComponent<T>();
            if (comp != null)
                componentList.Add(comp);
        }

        HidePrefabs();
    }

    protected void HidePrefabs()
    {
        foreach (T prefab in componentList)
            prefab.gameObject.SetActive(false);
    }

    public virtual T Spawn(string prefabName, Vector3 spawnPos, Quaternion spawnRot, bool isMerge = false)
    {
        T prefab = GetPrefabByName(prefabName);
        currentHolder = GetHolderByName(prefabName);

        if (prefab == null)
        {
            Debug.LogWarning("Prefab not found: " + prefabName);
            return null;
        }

        return Spawn(prefab, spawnPos, spawnRot, isMerge);
    }

    public virtual T Spawn(T prefab, Vector3 spawnPos, Quaternion spawnRot, bool isMerge = false)
    {
        T newInstance = GetPrefabFromPool(prefab);

        newInstance.transform.SetParent(currentHolder, false);
        newInstance.transform.localPosition = spawnPos;
        newInstance.transform.localRotation = spawnRot;
        newInstance.transform.localScale = prefab.transform.localScale;

        newInstance.gameObject.SetActive(true);
        return newInstance;
    }

    protected Transform GetHolderByName(string prefabName)
    {
        foreach (Transform holder in holderList)
        {
            if (holder.name == prefabName) return holder;
        }
        return null;
    }

    protected T GetPrefabByName(string prefabName)
    {
        foreach (T prefab in componentList)
        {
            if (prefab.name == prefabName) return prefab;
        }
        return null;
    }

    protected T GetPrefabFromPool(T prefab)
    {
        for (int i = 0; i < poolList.Count; i++)
        {
            T pooled = poolList[i];
            if (pooled == null) continue;

            if (pooled.name == prefab.name)
            {
                poolList.RemoveAt(i);
                return pooled;
            }
        }

        T newInstance = Instantiate(prefab);
        newInstance.name = prefab.name;
        return newInstance;
    }

    public virtual void Despawn(T obj, float timeDelay = 0f)
    {
        if (obj == null) return;

        poolList.Add(obj);

        if (timeDelay <= 0f)
        {
            obj.gameObject.SetActive(false);
        }
        else
        {
            StartCoroutine(DespawnDelay(obj, timeDelay));
        }
    }

    private IEnumerator DespawnDelay(T obj, float delay)
    {
        yield return new WaitForSeconds(delay);

        if (obj != null)
            obj.gameObject.SetActive(false);
    }

}
