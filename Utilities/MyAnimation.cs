using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.UI;
public static class MyAnimation
{
    public static void PlayMergeAnimation(Transform ball, Spawner<FX> fxSpawner)
    {
        FX newFX = fxSpawner.Spawn("MergeFX", ball.position, ball.rotation);
        newFX.gameObject.SetActive(true);
    }
    public static void PlayTouchAnimation(Vector3 touchPos, Spawner<FX> fxSpawner)
    {
        FX newFX = fxSpawner.Spawn("TouchFX", touchPos, Quaternion.identity);
        newFX.gameObject.SetActive(true);
    }
    public static void PlayDestroyBallAnimation(Transform ball, Spawner<FX> fxSpawner)
    {
        if (fxSpawner == null) return;

        FX newFX = fxSpawner.Spawn("DestroyFX", ball.transform.position, ball.transform.rotation);
        newFX.gameObject.SetActive(true);
    }
    public static void PlayDamBallAnimation(Vector3 damPos, Spawner<FX> fxSpawner, Transform hammer, Transform ball = null)
    {
        if (damPos.x >= 0)
        {
            hammer.rotation = Quaternion.Euler(0f, 180f, 0f);
            hammer.position = damPos + new Vector3(-1f, -1f, 0f);
        }
        else
        {
            hammer.rotation = Quaternion.identity;
            hammer.position = damPos + new Vector3(1f, -1f, 0f);
        }
        hammer.gameObject.SetActive(true);

        float swingAngle = -120f;
        float swingDuration = 0.2f;

        hammer.DOKill();
        InputManager.Instance.isBusy = true;
        hammer
            .DOLocalRotate(new Vector3(0, 0, swingAngle), swingDuration, RotateMode.LocalAxisAdd)
            .SetEase(Ease.OutQuad)
            .OnComplete(() =>
            {
                hammer
                    .DOLocalRotate(new Vector3(0, 0, -swingAngle), swingDuration, RotateMode.LocalAxisAdd)
                    .SetEase(Ease.InQuad)
                    .OnComplete(() =>
                    {
                        if (ball != null) PlayDestroyBallAnimation(ball, fxSpawner);
                        hammer.gameObject.SetActive(false);
                        InputManager.Instance.isBusy = false;
                    });
   
            });
    }
    public static void PlayUpgradeBallAnimation(Ball newBall)
    {
        Transform t = newBall.transform;

        Vector3 originalScale = t.localScale;

        newBall.gameObject.SetActive(false);
        t.localScale = Vector3.zero;

        DOVirtual.DelayedCall(0.1f, () =>
        {
            newBall.gameObject.SetActive(true);

            t.DOScale(originalScale, 0.25f)
                .SetEase(Ease.OutBack);

            t.DOPunchRotation(new Vector3(0, 0, 15f), 0.3f, 10, 0.5f);

            if (newBall.bodySprite != null)
            {
                var c = newBall.bodySprite.color;
                newBall.bodySprite.color = new Color(c.r, c.g, c.b, 0f);
                newBall.bodySprite.DOFade(1f, 0.15f);
            }
        });
    }


public static void AimAnimation(Ball ball)
    {
        if (ball == null) return;
        ball.isAimed = true;
        if (ball.nextType == eBallType.None && ServiceLocator.Instance.GetService<IBoosterContext>().ActiveBooster == eActiveBooster.Upgrade) return;
        SpriteRenderer aim = ball.aim;
        if (aim == null) return;

        aim.gameObject.SetActive(true);
        aim.DOKill(true);
        ball.DOKill(true);

        aim.transform.localRotation = Quaternion.identity;
        aim.transform.localScale = ball.aimScale;

        aim.transform.DORotate(new Vector3(0, 0, 360), 1.5f, RotateMode.LocalAxisAdd)
            .SetEase(Ease.Linear)
            .SetLoops(-1)
            .SetId("GamePlay");

        aim.transform.DOScale(ball.aimScale * 1.1f, 0.8f)
            .SetEase(Ease.InOutSine)
            .SetLoops(-1, LoopType.Yoyo)
            .SetId("GamePlay");

        if (aim != null)
        {
            aim.DOFade(0.4f, 0.6f)
                .SetEase(Ease.InOutSine)
                .SetLoops(-1, LoopType.Yoyo)
                .SetId("GamePlay");
        }

        SpriteRenderer ballSR = ball.bodySprite;
        if (ballSR != null)
        {
            ballSR.DOColor(new Color(1f, 0.8f, 0.8f, 1f), 0.4f)
                .SetEase(Ease.InOutSine)
                .SetLoops(-1, LoopType.Yoyo)
                .SetId("GamePlay");
        }
    }
    //public static void AimAllAnimation()
    //{
    //    if (InputManager.Instance.canDestroyBall || InputManager.Instance.canUpgradeBall)
    //    {
    //        List<Transform> ballList = GameManager.Instance.m_levelController.ballList;
    //        foreach (Transform ball in ballList)
    //        {
    //            AimAnimation(ball);
    //        }
    //    }
    //}
    public static List<eBallType> AimUpgradeBallAnimation(Dictionary<Transform, Ball> ballMap)
    {
        var ballTypes = (eBallType[])Enum.GetValues(typeof(eBallType));
        int[] counts = new int[ballTypes.Length];
        foreach (var t in ballMap.Values)
        {
            int index = (int)t.type; 
            counts[index]++;
        }
        List<eBallType> aimedBallTypes = new List<eBallType>();

        foreach (var t in ballMap.Values)
        {
            int index = (int)t.type;

            if (counts[index] >= 2)
            {
                AimAnimation(t);

                if (!aimedBallTypes.Contains(t.type))
                    aimedBallTypes.Add(t.type);
            }
        }

        return aimedBallTypes;
    }

    public static void StopAimAnimation(Ball ball)
    {
        if (ball == null) return;

        SpriteRenderer aim = ball.aim;

        DOTween.Kill(aim);
        DOTween.Kill(ball);
        DOTween.Kill(ball.bodySprite);
        DOTween.Kill(ball.aim);

        aim.transform.localRotation = Quaternion.identity;
        aim.gameObject.SetActive(false);

        Color c = aim.color;
        c.a = 1f;
        aim.color = c;

        ball.bodySprite.color = Color.white;
    }
    public static void StopAimAllAnimation(Dictionary<Transform, Ball> ballMap)
    {
        foreach (Ball ball in ballMap.Values)
        {
            if (ball == null) continue;

            StopAimAnimation(ball);
        }
    }

    //
    public static void PlayBeginSmailDestroyAnimation(Spotlight spotlight)
    {
        spotlight.m_spotlightCollider.enabled = true;
        PlayBeginDestroyAnimation(spotlight);
    }
    public static void PlayBeginDestroyAnimation(Spotlight spotlight)
    {
        spotlight.m_spotlight2d.DOKill();
        if (spotlight.m_spotlight2d == null) return;

        float baseFalloff = spotlight.m_spotlight2d.falloffIntensity;
        PlayHandTouchAnimation(GameManager.Instance.m_levelController.hand);
        Tween t = DOTween.To(() => spotlight.m_spotlight2d.falloffIntensity,
                             x => spotlight.m_spotlight2d.falloffIntensity = x,
                             baseFalloff * 7f,
                             0.8f)
            .SetEase(Ease.InOutSine)
            .SetLoops(-1, LoopType.Yoyo)
            .SetId("GamePlay");

        spotlight.StartCoroutine(WaitForAnyInput(() =>
        {
            if (t != null && t.IsActive()) t.Kill();
            spotlight.m_spotlight2d.falloffIntensity = baseFalloff;
        }));
    }
    public static void PlayHandTouchAnimation(Transform hand)
    {
        if (hand == null) return;

        SpriteRenderer handSprite = hand.GetComponentInChildren<SpriteRenderer>();
        if (handSprite == null) return;

        hand.DOKill();
        handSprite.DOKill();

        float moveDuration = 1.2f;
        float fadeDuration = 1.0f;
        float clickScale = 0.8f;
        float clickDuration = 0.2f;
        float disappearDuration = 0.5f;
        float delayBetweenLoops = 0.6f;

        Sequence seq = null;
        void PlayOnce()
        {
            hand.localPosition = new Vector3(3f, -3f, 0f);
            hand.localScale = Vector3.one;
            handSprite.color = new Color(1f, 1f, 1f, 0f);
            hand.gameObject.SetActive(true);

            seq = DOTween.Sequence();
            seq.Append(hand.DOLocalMove(Vector3.zero, moveDuration).SetEase(Ease.OutQuad));
            seq.Join(handSprite.DOFade(1f, fadeDuration).SetEase(Ease.InOutQuad));
            seq.Append(hand.DOScale(clickScale, clickDuration).SetEase(Ease.OutQuad));
            seq.Append(hand.DOScale(1f, clickDuration).SetEase(Ease.OutBack));
            seq.Append(handSprite.DOFade(0f, disappearDuration).SetEase(Ease.InQuad));
            seq.AppendInterval(delayBetweenLoops);
            seq.OnComplete(PlayOnce);
        }

        PlayOnce();

        GameManager.Instance.StartCoroutine(WaitForAnyInput(() =>
        {
            if (seq != null && seq.IsActive()) seq.Kill();
            hand.gameObject.SetActive(false);
        }));
    }
    private static IEnumerator WaitForAnyInput(System.Action onInput)
    {
        while (true)
        {
            bool hasInput =
                Input.GetMouseButtonDown(0) ||
                Input.touchCount > 0;

            if (hasInput)
            {
                onInput?.Invoke();
                yield break;
            }
            yield return null;
        }
    }
    //
    public static void PlayDropBomb(Transform bomb, Vector3 mousePos, TweenCallback onComplete = null)
    {
        bomb.gameObject.SetActive(true);
        mousePos.z = 0;

        Vector3 originalPosition = bomb.position;
        Vector3 originalScale = bomb.localScale;
        Quaternion originalRotation = bomb.rotation;

        bomb.position = mousePos + new Vector3(0, 6f, 0);
        bomb.localScale = Vector3.one * 2f;

        float duration = 2f;
        float finalScale = 0.5f;

        bomb.DOMove(mousePos, duration).SetEase(Ease.InQuad);
        bomb.DOScale(finalScale, duration).SetEase(Ease.InQuad);
        bomb.DORotate(new Vector3(0, 180f, 0), duration, RotateMode.FastBeyond360)
            .OnComplete(() =>
            {
                bomb.DOScale(finalScale * 1.2f, 0.1f)
                    .SetLoops(2, LoopType.Yoyo)
                    .OnComplete(() =>
                    {
                        bomb.position = originalPosition;
                        bomb.localScale = originalScale;
                        bomb.rotation = originalRotation;
                        bomb.gameObject.SetActive(false);
                        onComplete?.Invoke();
                    });
            });
    }
    public static Sequence PlayWaringLightAnimation(Light2D light, float duration = 0.3f, float intensity = 1.2f)
    {
        if (light == null) return null;

        light.DOKill();
        Color redSoft = new Color(1f, 0.25f, 0.25f, 0.35f);
        Color whiteColor = new Color(1f, 1f, 1f, 0.7f); 
        float baseIntensity = light.intensity;
        Sequence seq = DOTween.Sequence();
        seq.Append(
            DOTween.To(() => light.color, x => light.color = x, whiteColor, duration)
                  .SetEase(Ease.Linear)
        ).Join(DOTween.To(() => light.intensity, x => light.intensity = x, baseIntensity * intensity, duration))
        .Append(
            DOTween.To(() => light.color, x => light.color = x, redSoft, duration)
                  .SetEase(Ease.Linear)
        ).Join(DOTween.To(() => light.intensity, x => light.intensity = x, baseIntensity, duration))
        .SetLoops(-1, LoopType.Restart);
        return seq;
    }

    public static void Flash(SpriteRenderer flashSprite, Color flashColor, int flashCount = 3, float flashDuration = 0.2f)
    {
        flashSprite.gameObject.SetActive(true);
        flashSprite.color = new Color(flashColor.r, flashColor.g, flashColor.b, 0);

        flashSprite.DOKill();

        Sequence seq = DOTween.Sequence();
        for (int i = 0; i < flashCount; i++)
        {
            seq.Append(flashSprite.DOFade(1f, flashDuration * 0.5f));
            seq.Append(flashSprite.DOFade(0f, flashDuration * 0.5f));
        }

        seq.OnComplete(() => flashSprite.gameObject.SetActive(false));
    }
    public static Tween FlashLoop(SpriteRenderer flashSprite, Color flashColor, float flashDuration = 0.2f)
    {
        flashSprite.gameObject.SetActive(true);
        flashSprite.DOKill();

        Color originalColor = Color.white; 

        Tween t = flashSprite
            .DOColor(flashColor, flashDuration * 0.5f)
            .SetLoops(-1, LoopType.Yoyo)
            .SetEase(Ease.Linear)
            .SetId("GamePlay");
        flashSprite.color = originalColor;

        return t;
    }
    public static void StopFlashLoop(SpriteRenderer flashSprite)
    {
        flashSprite.DOKill();
        flashSprite.color = Color.white;
    }
    public static void PlayCancelZoneAnimation(Image zoneImage, Color normalColor, float scaleUp = 1.15f, float colorTweenDuration = 0.25f, float scaleTweenDuration = 0.25f)
    {
        if (zoneImage == null) return;
        Transform zoneTransform = zoneImage.transform;
        Color hoverColor = new Color(1f, 0.3f, 0.3f, 1f);
        zoneImage.DOKill();
        zoneTransform.DOKill();
        zoneImage.DOColor(hoverColor, colorTweenDuration);
        zoneTransform.DOScale(scaleUp, scaleTweenDuration).SetEase(Ease.OutBack);
    }
    public static void ResetCancelZoneAnimation(Image zoneImage, Color normalColor, float colorTweenDuration = 0.25f, float scaleTweenDuration = 0.25f)
    {
        if (zoneImage == null) return;
        Transform zoneTransform = zoneImage.transform;
        zoneImage.DOKill();
        zoneTransform.DOKill();
        zoneImage.DOColor(normalColor, colorTweenDuration);
        zoneTransform.DOScale(1f, scaleTweenDuration).SetEase(Ease.InOutSine);
    }
}
