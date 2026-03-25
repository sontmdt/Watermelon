using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class UIGamePause : MenuBase
{
    private IGameDataService GameData => ServiceLocator.Instance.GetService<IGameDataService>();
    [SerializeField] private Button btnPlay;
    [SerializeField] private Button btnRestart;
    [SerializeField] private Button btnHome;

    [SerializeField] private Slider musicVolume;
    [SerializeField] private Slider effectVolume;

    private AnimationUI animationUI;
    private AnimationUI AnimUI => animationUI ? animationUI : animationUI = GetComponent<AnimationUI>();

    private bool _initialized;

    private void OnEnable()
    {
        var audio = ServiceLocator.Instance.GetService<IAudioService>();
        if (audio == null) return;
        musicVolume.value = audio.GetMusicVolume();
        effectVolume.value = audio.GetSoundVolume();
        musicVolume.onValueChanged.AddListener(audio.SetMusicVolume);
        effectVolume.onValueChanged.AddListener(audio.SetSoundVolume);
    }

    private void OnDisable()
    {
        var audio = ServiceLocator.Instance.GetService<IAudioService>();
        if (audio == null) return;
        musicVolume.onValueChanged.RemoveListener(audio.SetMusicVolume);
        effectVolume.onValueChanged.RemoveListener(audio.SetSoundVolume);
    }

    public override void Setup()
    {
        if (!_initialized)
        {
            btnPlay.AddAnimatedListener(OnClickPlay);
            btnRestart.AddAnimatedListener(OnClickRestart);
            btnHome.AddAnimatedListener(OnClickExit);
            _initialized = true;
        }
    }

    public override void Show() => AnimUI.Show();

    public override void Hide() => AnimUI.Hide();

    private void OnClickPlay() => this.PostEvent(EventID.StateGameStarted);

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
}
