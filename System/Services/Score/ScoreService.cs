using System;
using DG.Tweening;

public class ScoreService : IScoreService, IDisposable
{
    public int CurrentScore { get; private set; }

    public float comboDuration = 1f;
    public int currentCombo = 0;
    public float comboMultiplier = 1f;

    private Tween _comboTimer;

    public void Initialize(int startScore)
    {
        CurrentScore = startScore;
        EventManager.Instance.PostEvent(EventID.OnScoreChanged, CurrentScore);
    }

    public void AddScore(eBallType ballName)
    {
        int ballScore = GetBallScore(ballName);
        if (ballScore == 0) return;

        int total = UnityEngine.Mathf.RoundToInt(ballScore * comboMultiplier);
        CurrentScore += total;
        EventManager.Instance.PostEvent(EventID.OnScoreChanged, CurrentScore);
    }

    private int GetBallScore(eBallType ballName)
    {
        if (ballName == eBallType.Ball01) return 2;
        if (ballName == eBallType.Ball02) return 4;
        if (ballName == eBallType.Ball03) return 8;
        if (ballName == eBallType.Ball04) return 16;
        if (ballName == eBallType.Ball05) return 32;
        if (ballName == eBallType.Ball06) return 64;
        if (ballName == eBallType.Ball07) return 128;
        if (ballName == eBallType.Ball08) return 256;
        if (ballName == eBallType.Ball09) return 512;
        if (ballName == eBallType.Ball10) return 1024;
        if (ballName == eBallType.Ball11) return 2048;
        return 0;
    }

    public void AddCombo()
    {
        currentCombo++;
        comboMultiplier = GetMultiplier(currentCombo);
        EventManager.Instance.PostEvent(EventID.OnComboChanged, (currentCombo, comboMultiplier));

        _comboTimer?.Kill();
        _comboTimer = DOVirtual.DelayedCall(comboDuration, ResetCombo).SetUpdate(true);
    }

    private float GetMultiplier(int combo)
    {
        if (combo <= 1) return 1f;
        if (combo == 2) return 1.1f;
        if (combo == 3) return 1.2f;
        return 1.3f;
    }

    private void ResetCombo()
    {
        currentCombo = 0;
        comboMultiplier = 1f;
        EventManager.Instance.PostEvent(EventID.OnComboChanged, (currentCombo, comboMultiplier));
    }

    public void Reset()
    {
        _comboTimer?.Kill();
        CurrentScore = 0;
        currentCombo = 0;
        comboMultiplier = 1f;
        EventManager.Instance.PostEvent(EventID.OnScoreChanged, CurrentScore);
        EventManager.Instance.PostEvent(EventID.OnComboChanged, (currentCombo, comboMultiplier));
    }

    public void Dispose()
    {
        _comboTimer?.Kill();
    }
}
