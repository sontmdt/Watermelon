using UnityEngine;
using UnityEngine.UI;

public class GameTool : MonoBehaviour
{

    [SerializeField] private Button btnExit;
    [SerializeField] private Button btnCoin;
    [SerializeField] private Button btnDiamond;
    [SerializeField] private Button btnShake;
    [SerializeField] private Button btnUpgrade;
    [SerializeField] private Button btnDestroy;
    [SerializeField] private Button btnSmallDestroy;

    [SerializeField] private Button btnSpawmBall11;
    private void Awake()
    {
        btnExit.AddAnimatedListener(OnClickExit);
        btnCoin.AddAnimatedListener(OnClickCoin);
        btnDiamond.AddAnimatedListener(OnClickDiamond);
        btnShake.AddAnimatedListener(OnClickShake);
        btnUpgrade.AddAnimatedListener(OnClickUpgrade);
        btnDestroy.AddAnimatedListener(OnClickDestroy);
        btnSmallDestroy.AddAnimatedListener(OnClickSmallDestroy);
        btnSpawmBall11.AddAnimatedListener(OnSpawnBallBoss);
    }
    private void OnDestroy()
    {
        btnExit.onClick.RemoveAllListeners();
        btnCoin.onClick.RemoveAllListeners();
        btnDiamond.onClick.RemoveAllListeners();
        btnShake.onClick.RemoveAllListeners();
        btnUpgrade.onClick.RemoveAllListeners();
        btnDestroy.onClick.RemoveAllListeners();
        btnSmallDestroy.onClick.RemoveAllListeners();
        btnSpawmBall11.onClick.RemoveAllListeners();
    }
    private void OnClickExit()
    {
        this.gameObject.SetActive(false);
    }
    private void OnClickCoin()
    {
        GameManager.Instance.m_playerAssetController.AddAsset(eAssetType.Coin, 50);
    }
    private void OnClickDiamond()
    {
        GameManager.Instance.m_playerAssetController.AddAsset(eAssetType.Diamond, 100);
    }
    private void OnClickShake()
    {
        GameManager.Instance.m_playerAssetController.AddAsset(eAssetType.ShakeBooster, 1);
    }
    private void OnClickUpgrade()
    {
        GameManager.Instance.m_playerAssetController.AddAsset(eAssetType.UpgradeBooster, 1);
    }
    private void OnClickDestroy()
    {
        GameManager.Instance.m_playerAssetController.AddAsset(eAssetType.DestroyBooster, 1);
    }
    private void OnClickSmallDestroy()
    {
        GameManager.Instance.m_playerAssetController.AddAsset(eAssetType.SmallDestroyBooster, 1);
    }
    private BallSpawner ballSpawner;
    private LevelController levelController;
    private void OnSpawnBallBoss()
    {
        if (levelController == null) levelController = GameManager.Instance.m_levelController;
        if (ballSpawner == null) ballSpawner = levelController.ballSpawner;
        Ball newBall = ballSpawner.Spawn(eBallType.Ball11, new Vector3(0, 0, 0), this.transform.rotation);
        newBall.gameObject.SetActive(true);
        this.gameObject.SetActive(false);
        if (Ball.ballMap.TryGetValue(newBall.transform, out Ball ball11))
        {
            ball11.rb.bodyType = RigidbodyType2D.Kinematic;
            ball11.lightFX.PlayFX();
        }
    }
}
