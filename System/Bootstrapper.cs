using UnityEngine;

public class Bootstrapper : MonoBehaviour
{
    [SerializeField] private AudioService audioServicePrefab;

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);

        // Audio
        if (audioServicePrefab != null)
        {
            var audio = Instantiate(audioServicePrefab);
            ServiceLocator.Instance.RegisterService<IAudioService>(audio);
        }
        else
        {
            Debug.LogWarning("[Bootstrapper] audioServicePrefab not assigned.");
        }

        // Save
        var save = new GameObject("FileSaveService").AddComponent<FileSaveService>();
        ServiceLocator.Instance.RegisterService<ISaveService>(save);

        // Game data
        var gameData = new GameObject("GameDataService").AddComponent<GameDataService>();
        ServiceLocator.Instance.RegisterService<IGameDataService>(gameData);

        // Booster context
        ServiceLocator.Instance.RegisterService<IBoosterContext>(new BoosterContext());
    }
}
