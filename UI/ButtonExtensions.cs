using DG.Tweening;
using System;
using UnityEngine;
using UnityEngine.UI;

public static class ButtonExtensions
{
    public static void AddAnimatedListener(this Button btn, Action callback)
    {
        if (btn == null) return;
        btn.onClick.AddListener(() =>
        {
            if (DOTween.IsTweening(btn.transform)) return;
            PressAnimation(btn.transform, callback);
        });
    }

    private static void PressAnimation(Transform target, Action onComplete,
        float duration = 0.2f, float scaleAmount = 0.9f, Ease ease = Ease.OutBack)
    {
        if (target == null) return;
        target.DOKill();
        Vector3 originalScale = target.localScale;
        InputManager.Instance.isBusy = true;
        Sequence seq = DOTween.Sequence();
        seq.Append(target.DOScale(originalScale * scaleAmount, duration / 2f).SetEase(Ease.OutQuad));
        seq.Append(target.DOScale(originalScale, duration / 2f).SetEase(ease));
        seq.OnComplete(() =>
        {
            if (target == null || !target.gameObject.activeInHierarchy) return;
            onComplete?.Invoke();
            InputManager.Instance.isBusy = false;
        });
        seq.SetId(target);
        seq.SetUpdate(true);
        seq.SetLink(target.gameObject, LinkBehaviour.KillOnDestroy);
    }
}
