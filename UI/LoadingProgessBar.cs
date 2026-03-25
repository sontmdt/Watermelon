using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class LoadingProgressBar : MonoBehaviour
{
    [SerializeField] private Image fullLoadingImg;
    [SerializeField] private Image loadingImg;
    [SerializeField] private float smoothTime = 0.25f;

    private float currentProgress = 0f;
    private Tween currentTween;
    private void Start()
    {
        loadingImg.fillAmount = 0f;
    }

    private void Update()
    {
        if (loadingImg == null) return; 

        float targetProgress = Loader.GetLoadingProgress();

        if (targetProgress > currentProgress + 0.01f)
        {
            currentTween?.Kill();
            currentTween = loadingImg
                .DOFillAmount(targetProgress, smoothTime)
                .SetEase(Ease.InOutSine)
                .SetLink(gameObject); 
            currentProgress = targetProgress;
        }

        if (targetProgress >= 0.9f)
        {
            currentTween?.Kill();
            loadingImg
                .DOFillAmount(1f, 0.5f)
                .SetEase(Ease.OutCubic)
                .SetLink(gameObject);
        }
    }

    private void OnDestroy()
    {
        currentTween?.Kill();
    }
}
