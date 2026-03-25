using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
public static class Loader
{
    private class LoadingMonoBehaviour : MonoBehaviour { }
    private static event Action onLoaderCallBack;
    private static AsyncOperation loadingAsyncOperation;
    public static void Load(eScene scene, Action onLoaded = null)
    {
        onLoaderCallBack = () =>
        {
            GameObject loadingGameObject = new GameObject("Loading Game Object");
            loadingGameObject.AddComponent<LoadingMonoBehaviour>().StartCoroutine(LoadSceneAsync(scene));
        };
        SceneManager.LoadScene(eScene.Loading.ToString());
    }
    //private static IEnumerator LoadSceneAsync(eScene scene)
    //{
    //    yield return null;
    //    loadingAsyncOperation = SceneManager.LoadSceneAsync(scene.ToString());
    //    while (!loadingAsyncOperation.isDone)
    //    {
    //        yield return null;
    //    }
    //}
    private static IEnumerator LoadSceneAsync(eScene scene)
    {
        yield return null;
        loadingAsyncOperation = SceneManager.LoadSceneAsync(scene.ToString());
        loadingAsyncOperation.allowSceneActivation = false;

        float minLoadTime = 0.5f;
        float timer = 0f;

        while (!loadingAsyncOperation.isDone)
        {
            timer += Time.deltaTime;
            if (loadingAsyncOperation.progress >= 0.9f && timer >= minLoadTime)
            {
                loadingAsyncOperation.allowSceneActivation = true;
            }

            yield return null;
        }
    }

    public static float GetLoadingProgress()
    {
        if (loadingAsyncOperation != null) return loadingAsyncOperation.progress;
        else return 1f;
    }
    public static void LoaderCallBack()
    {
        if (onLoaderCallBack != null)
        {
            onLoaderCallBack();
            onLoaderCallBack = null;
        }
    }
}
