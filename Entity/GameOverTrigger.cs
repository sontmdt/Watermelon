using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class GameOverTrigger : MonoBehaviour
{
    [SerializeField] private float allowedTime = 2f;
    [SerializeField] private float warningTime = 1f;
    [SerializeField] private float checkInterval = 0.05f;
    [SerializeField] private SpriteRenderer lineGameOver;
    [SerializeField] private Collider2D zone;

    private Dictionary<GameObject, float> ballTimerDict = new Dictionary<GameObject, float>();
    private Dictionary<GameObject, Color> originalColorDict = new Dictionary<GameObject, Color>();

    public bool gameOverTriggered = false;
    private Coroutine monitorRoutine;
    private bool isFlashingLine = false;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (GameManager.Instance.m_levelController.ballSpawner.totalVolume <= 100) return;
        if (!other.CompareTag("Ball")) return;

        if (!ballTimerDict.ContainsKey(other.gameObject))
            ballTimerDict[other.gameObject] = 0f;

        // Lưu màu gốc của ball
        if (Ball.ballMap.TryGetValue(other.transform, out var ball) && ball.bodySprite != null)
        {
            if (!originalColorDict.ContainsKey(other.gameObject))
                originalColorDict[other.gameObject] = ball.bodySprite.color;
        }

        if (monitorRoutine == null)
            monitorRoutine = StartCoroutine(CheckBalls());
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (!ballTimerDict.Remove(other.gameObject)) return;

        if (Ball.ballMap.TryGetValue(other.transform, out var ball) && ball.bodySprite != null)
        {
            if (originalColorDict.TryGetValue(other.gameObject, out var original))
                ball.bodySprite.color = original;
        }

        originalColorDict.Remove(other.gameObject);

        if (ballTimerDict.Count == 0)
        {
            MyAnimation.StopFlashLoop(lineGameOver);
            isFlashingLine = false;

            if (monitorRoutine != null)
            {
                StopCoroutine(monitorRoutine);
                monitorRoutine = null;
            }
        }
    }

    private IEnumerator CheckBalls()
    {
        WaitForSeconds wait = new WaitForSeconds(checkInterval);
        while (!gameOverTriggered && ballTimerDict.Count > 0)
        {
            while (GameManager.Instance.CurrentState != EventID.StateGameStarted)
                yield return null;

            bool shouldFlashLine = false;
            var keys = ballTimerDict.Keys.ToList();

            foreach (var ballGO in keys)
            {
                if (ballGO == null)
                {
                    ballTimerDict.Remove(ballGO);
                    originalColorDict.Remove(ballGO);
                    continue;
                }

                if (!Ball.ballMap.TryGetValue(ballGO.transform, out var ball)) continue;
                if (ball.bodySprite == null || ball.ballCollider == null) continue;

                var sr = ball.bodySprite;
                var ballCol = ball.ballCollider;

                if (!IsFullyInside(ballCol, zone))
                {
                    if (originalColorDict.TryGetValue(ballGO, out var original))
                        sr.color = original;

                    ballTimerDict[ballGO] = 0f;
                    continue;
                }

                float newTime = ballTimerDict[ballGO] + checkInterval;
                ballTimerDict[ballGO] = newTime;

                float remaining = allowedTime - newTime;
                if (remaining <= warningTime)
                {
                    shouldFlashLine = true;

                    float t = Mathf.PingPong(Time.time * 5f, 1f);
                    sr.color = Color.Lerp(originalColorDict[ballGO], Color.red, t);
                }

                if (newTime >= allowedTime)
                {
                    gameOverTriggered = true;
                    ballTimerDict.Clear();
                    GameManager.Instance.m_levelController.GameOver();
                    monitorRoutine = null;
                    yield break;
                }
            }

            yield return wait;

            if (shouldFlashLine && !isFlashingLine)
            {
                isFlashingLine = true;
                MyAnimation.FlashLoop(lineGameOver, Color.red, 0.5f);
            }
            else if (!shouldFlashLine && isFlashingLine)
            {
                isFlashingLine = false;
                MyAnimation.StopFlashLoop(lineGameOver);
            }
        }

        monitorRoutine = null;
    }

    private bool IsFullyInside(Collider2D ball, Collider2D zone)
    {
        Bounds b = ball.bounds;
        Bounds z = zone.bounds;

        Vector3[] corners = {
            new Vector3(b.min.x, b.min.y),
            new Vector3(b.min.x, b.max.y),
            new Vector3(b.max.x, b.min.y),
            new Vector3(b.max.x, b.max.y)
        };

        foreach (var c in corners)
        {
            if (!z.Contains(c))
                return false;
        }
        return true;
    }

    public void ResetTrigger()
    {
        gameOverTriggered = false;

        foreach (var kvp in originalColorDict)
        {
            if (kvp.Key != null && Ball.ballMap.TryGetValue(kvp.Key.transform, out var ball))
            {
                if (ball.bodySprite != null)
                    ball.bodySprite.color = kvp.Value;
            }
        }

        ballTimerDict.Clear();
        originalColorDict.Clear();

        if (monitorRoutine != null)
        {
            StopCoroutine(monitorRoutine);
            monitorRoutine = null;
        }

        isFlashingLine = false;
        MyAnimation.StopFlashLoop(lineGameOver);
    }
}
