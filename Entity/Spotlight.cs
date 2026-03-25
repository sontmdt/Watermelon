using DG.Tweening;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class Spotlight : MonoBehaviour
{
    public Light2D m_spotlight2d;
    public CircleCollider2D m_spotlightCollider;

    private float intensity;
    private Color color = Color.white;
    private float inner = 0.3f;
    private float outer = 1f;
    private void OnEnable()
    {
        intensity = 1f; 
        color = m_spotlight2d.color;
    }
    private void OnDisable()
    {
        m_spotlightCollider.enabled = false;
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (Ball.ballMap.TryGetValue(collision.transform, out Ball ball))
        {
            if (!GameManager.Instance.m_levelController.ballDestroyList.Contains(ball))
            {
                if (ball.type == eBallType.Ball01 || ball.type == eBallType.Ball02 || ball.type == eBallType.Ball03)
                {
                    GameManager.Instance.m_levelController.ballDestroyList.Add(ball);

                    ball.OnHoverEnter();
                }
            }
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (Ball.ballMap.TryGetValue(collision.transform, out Ball ball))
        {
            if (GameManager.Instance.m_levelController.ballDestroyList.Contains(ball))
            {
                GameManager.Instance.m_levelController.ballDestroyList.Remove(ball);
                ball.OnHoverExit();
            }
        }
    }


    private Sequence seq;
    public void StartWarningLight()
    {
        m_spotlight2d.intensity = 1f;
        m_spotlight2d.pointLightInnerRadius = 10f;
        m_spotlight2d.pointLightOuterRadius = 10f;
        seq = MyAnimation.PlayWaringLightAnimation(m_spotlight2d);
    }
    public void EndWarningLight()
    {
        m_spotlight2d.intensity = intensity;
        m_spotlight2d.color = color;
        m_spotlight2d.pointLightInnerRadius = inner;
        m_spotlight2d.pointLightOuterRadius = outer;

        seq.Kill();
    }
}
