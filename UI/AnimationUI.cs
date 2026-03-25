using DG.Tweening;
using UnityEngine;

public class AnimationUI : MonoBehaviour
{
    [SerializeField] private float showDuration = 0.2f;
    [SerializeField] private float hideDuration = 0.2f;
    [SerializeField] private Ease showEase = Ease.OutBack;
    [SerializeField] private Ease hideEase = Ease.InBack;

    public void Show()
    {
        Vector3 targetScale = transform.localScale;
        if (targetScale == Vector3.zero) targetScale = Vector3.one;
        transform.DOKill();
        transform.localScale = Vector3.zero;
        gameObject.SetActive(true);
        transform.DOScale(targetScale, showDuration)
            .SetEase(showEase)
            .SetId(transform)
            .SetLink(gameObject)
            .SetUpdate(true);
    }

    public Tween Hide(TweenCallback onComplete = null)
    {
        transform.DOKill();
        InputManager.Instance.isBusy = true;
        return transform.DOScale(Vector3.zero, hideDuration)
            .SetEase(hideEase)
            .SetId(transform)
            .SetLink(gameObject)
            .SetUpdate(true)
            .OnComplete(() =>
            {
                gameObject.SetActive(false);
                InputManager.Instance.isBusy = false;
                onComplete?.Invoke();
            });
    }
}
