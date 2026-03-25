using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIGameOver : MenuBase
{
    private IGameDataService GameData => ServiceLocator.Instance.GetService<IGameDataService>();
    [SerializeField] private Button btnRestart;
    [SerializeField] private Button btnExit;
    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private TextMeshProUGUI highestScoreText;

    private AnimationUI animationUI;
    private AnimationUI AnimUI => animationUI ? animationUI : animationUI = GetComponent<AnimationUI>();

    private bool _initialized;

    public override void Setup()
    {
        if (!_initialized)
        {
            btnRestart.AddAnimatedListener(OnClickRestart);
            btnExit.AddAnimatedListener(OnClickExit);
            _initialized = true;
        }
    }

    public override void Show() => AnimUI.Show();

    public override void Hide() => AnimUI.Hide();

    private void OnClickRestart()
    {
        AnimUI.Hide(() =>
        {
            EventManager.Instance.PostEvent(EventID.StateGameSetup);
            GameManager.Instance.m_levelController.RestartGame();
        });
    }

    private void OnClickExit()
    {
        GameData.SaveAllBallState();
        EventManager.Instance.PostEvent(EventID.StateGameSetup);
        AnimUI.Hide(() => Loader.Load(eScene.Menu));
    }

    public void UpdateResult(int currentScore)
    {
        scoreText.SetText("Score: {0}", currentScore);
        highestScoreText.SetText("Highest: {0}", GameData.data.playerData.highestScore);
    }
}
