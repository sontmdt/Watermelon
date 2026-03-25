using DG.Tweening;
using UnityEngine;

public class ShakeBooster : MonoBehaviour, IBooster
{
    public float angle = 20f;
    public float duration = 0.7f;
    public int vibrato = 10;

    private IBoosterContext _boosterCtx;

    private void Start()
    {
        _boosterCtx = ServiceLocator.Instance.GetService<IBoosterContext>();
    }

    public void Cancel()
    {
        throw new System.NotImplementedException();
    }

    public void Effect()
    {
        InputManager.Instance.isBusy = true;
        DOVirtual.DelayedCall(0.5f, () =>
        {
            Transform box = GameManager.Instance.m_levelController.m_environmet.transform;
            box.DOPunchRotation(new Vector3(0, 0, angle), duration, vibrato, 1f).OnComplete(() =>
            {
                InputManager.Instance.isBusy = false;
                _boosterCtx.Deactivate();
                EventManager.Instance.PostEvent(EventID.StateGameStarted);
            });
            GameManager.Instance.m_playerAssetController.UseAsset(eAssetType.ShakeBooster, 1);
        }).SetId("Gameplay");
    }
}
