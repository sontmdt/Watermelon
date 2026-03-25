using System.Collections;
using UnityEngine;

public class TouchFX : FX
{
    [SerializeField] private Animator animator;

    private Coroutine autoDespawnRoutine;

    private float cachedClipLength = -1f;
    private WaitForSeconds cachedWait;

    private void Awake()
    {
        if (animator == null)
            return;

        if (cachedClipLength >= 0f)
            return;

        var controller = animator.runtimeAnimatorController;
        if (controller == null)
            return;

        var clips = controller.animationClips;
        for (int i = 0; i < clips.Length; i++)
        {
            if (clips[i].name == "TouchFX")
            {
                cachedClipLength = clips[i].length;
                cachedWait = new WaitForSeconds(cachedClipLength);
                break;
            }
        }
    }

    private void OnEnable()
    {
        PlayFX();
    }

    private void PlayFX()
    {
        if (animator == null)
            return;

        animator.Play("TouchFX", 0, 0f);

        if (autoDespawnRoutine != null)
        {
            StopCoroutine(autoDespawnRoutine);
            autoDespawnRoutine = null;
        }

        autoDespawnRoutine = StartCoroutine(AutoDespawn());
    }

    private IEnumerator AutoDespawn()
    {
        if (cachedWait != null)
            yield return cachedWait;
        else
            yield return null;

        GameManager.Instance.m_levelController.fxSpawner.Despawn(this);
        autoDespawnRoutine = null;
    }

    private void OnDisable()
    {
        if (autoDespawnRoutine != null)
        {
            StopCoroutine(autoDespawnRoutine);
            autoDespawnRoutine = null;
        }
    }
}
