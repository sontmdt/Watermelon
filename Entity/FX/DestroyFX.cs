using System.Collections;
using UnityEngine;

public class DestroyFX : FX
{
    public Animator animator;
    private Coroutine autoDespawnRoutine;
    private static float cachedClipLength = -1f;

    private void Awake()
    {
        if (animator != null && cachedClipLength < 0)
        {
            var clips = animator.runtimeAnimatorController.animationClips;
            foreach (var clip in clips)
            {
                if (clip.name == "DestroyFX")
                {
                    cachedClipLength = clip.length;
                    break;
                }
            }
        }
    }

    private void OnEnable()
    {
        PlayDestroyFX();
    }

    private void PlayDestroyFX()
    {
        if (animator == null) return;

        animator.Play("DestroyFX", 0, 0f);

        float clipLength = cachedClipLength;

        if (autoDespawnRoutine != null)
            StopCoroutine(autoDespawnRoutine);

        InputManager.Instance.isBusy = true;
        autoDespawnRoutine = StartCoroutine(AutoDespawn(clipLength));
    }

    private IEnumerator AutoDespawn(float duration)
    {
        yield return new WaitForSeconds(duration);
        GameManager.Instance.m_levelController.fxSpawner.Despawn(this);

        InputManager.Instance.isBusy = false;
        autoDespawnRoutine = null;
    }

    private void OnDisable()
    {
        if (autoDespawnRoutine != null)
        {
            StopCoroutine(autoDespawnRoutine);
            autoDespawnRoutine = null;
            InputManager.Instance.isBusy = false;
        }
    }
}
