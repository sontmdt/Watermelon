public enum EventID
{
    None = 0,

    StateGameSetup,
    StateMainMenu,
    StateGameStarted,
    StateGamePause,
    StateGamePopup,
    StateGameShop,
    StateGameHanging,
    StateGameOver,

    // Gameplay data events
    OnCoinChanged,
    OnDiamondChanged,
    OnDestroyBoosterChanged,
    OnShakeBoosterChanged,
    OnUpgradeBoosterChanged,
    OnSmallDestroyBoosterChanged,
    OnScoreChanged,
    OnComboChanged,
    OnDropped,
    OnBall11Spawned,
    OnBall11SpawnedFirst,
}
