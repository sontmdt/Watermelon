using DG.Tweening;
using System.Collections.Generic;
using UnityEngine;

public class LightFX : MonoBehaviour
{
    private static readonly int IntensityID = Shader.PropertyToID("_Intensity");
    private MaterialPropertyBlock _mpb;

    [SerializeField] private List<Transform> lightList;
    [SerializeField] private SpriteRenderer lightCircle;
    [SerializeField] private Ball ball;

    private void Awake() => _mpb = new MaterialPropertyBlock();

    private void SetCircleIntensity(float value)
    {
        _mpb.SetFloat(IntensityID, value);
        lightCircle.SetPropertyBlock(_mpb);
    }

    public void PlayFX()
    {
        gameObject.SetActive(true);
        EventManager.Instance.PostEvent(EventID.StateGameHanging);
        float durationRotate = 6f;
        float durationYScale = 0.3f;
        float durationXScale = 2f;
        float delayBetweenLights = 0.5f;

        lightCircle.transform.localScale = new Vector3(1.1f, 1.1f, 0f);
        SetCircleIntensity(1.4f);
        Shake();
        transform.DORotate(new Vector3(0, 0, 180), durationRotate, RotateMode.LocalAxisAdd)
            .SetEase(Ease.OutQuad)
            .OnComplete(() =>
            {
                gameObject.SetActive(false);
                ball.rb.bodyType = RigidbodyType2D.Dynamic;
                EventManager.Instance.PostEvent(EventID.StateGameStarted);
            });

        for (int i = 0; i < lightList.Count; i++)
        {
            Transform light = lightList[i];
            light.localScale = Vector3.zero;

            light.DOScaleY(12f, durationYScale)
                 .SetEase(Ease.OutExpo)
                 .SetDelay(i * delayBetweenLights);

            light.DOScaleX(1f, durationXScale)
                 .SetEase(Ease.OutQuad)
                 .SetDelay(i * delayBetweenLights);
        }

        float totalDelay = (lightList.Count - 1) * delayBetweenLights;
        float remainingTime = durationRotate - totalDelay;
        if (remainingTime > 0)
        {
            DOVirtual.Float(1.4f, 5f, remainingTime, value =>
            {
                SetCircleIntensity(value);
            }).SetDelay(totalDelay);
        }
    }
    public void Shake(float duration = 0.2f, float strength = 0.5f, int vibrato = 100, int loop = 25)
    {
        Camera cam = GameManager.Instance.m_camera;
        cam.transform.DOShakePosition(duration, strength, vibrato)
                     .SetLoops(loop, LoopType.Restart)
                     .SetEase(Ease.InOutSine);
    }
}
