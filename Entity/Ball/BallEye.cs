using UnityEngine;
using System.Collections;

public class BallEyes : MonoBehaviour
{
    [SerializeField] private SpriteRenderer eyeSprite;
    [SerializeField] private Ball ball;

    private Vector3 eyeOrigin;
    private float radius = 0.2f;
    private eEyeMode mode = eEyeMode.FollowPlayer;

    private Coroutine followRoutine;
    private Vector3 currentTarget;
    private PlayerController playerController;
    private LevelController levelController;
    private void Awake()
    {
        eyeOrigin = eyeSprite.transform.localPosition;
        currentTarget = eyeOrigin;

        if (playerController == null) playerController = GameManager.Instance.m_levelController.m_playerController;
        playerController.OnMoveEvent += SetMode;
        if (levelController == null) levelController = GameManager.Instance.m_levelController;
    }

    private void OnDestroy()
    {
        if (GameManager.Instance != null)
        {
            if (levelController != null && playerController != null)
               playerController.OnMoveEvent -= SetMode;
        }

        if (followRoutine != null)
        {
            StopCoroutine(followRoutine);
            followRoutine = null;
        }
    }

    public void SetMode(eEyeMode newMode, Transform target)
    {
        if (ball.isFalling) return;

        mode = newMode;

        if (followRoutine != null)
        {
            StopCoroutine(followRoutine);
            followRoutine = null;
        }

        if (mode == eEyeMode.FollowFall && target != null && isActiveAndEnabled)
        {
            followRoutine = StartCoroutine(FollowTargetRoutine(target));
        }
        else if (target != null)
        {
            UpdateTarget(target.position);
        }
    }

    private IEnumerator FollowTargetRoutine(Transform target)
    {
        float updateInterval = 0.02f;
        while (mode == eEyeMode.FollowFall && target != null && gameObject.activeInHierarchy)
        {
            UpdateTarget(target.position);
            yield return new WaitForSeconds(updateInterval);
        }
    }

    private void UpdateTarget(Vector3 worldPos)
    {
        Vector3 localDir = transform.InverseTransformPoint(worldPos) - eyeSprite.transform.localPosition;
        localDir.Normalize();

        float x = eyeOrigin.x + Mathf.Clamp(localDir.x * radius, -radius, radius);
        float y = eyeOrigin.y + Mathf.Clamp(localDir.y * radius, -radius, radius);

        currentTarget = new Vector3(x, y, eyeOrigin.z);

        ApplyEyePosition();
    }

    private void ApplyEyePosition()
    {
        float speed = 5f;
        eyeSprite.transform.localPosition = Vector3.Lerp(
            eyeSprite.transform.localPosition,
            currentTarget,
            speed * Time.deltaTime
        );
    }
}
