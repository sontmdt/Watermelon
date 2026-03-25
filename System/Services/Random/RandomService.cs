public class RandomService : IRandomService
{
    private int[] c;
    private int[] w;
    private readonly GameSettings _settings;

    public RandomService(GameSettings settings)
    {
        _settings = settings;
        c = new int[5] { 1, 1, 1, 1, 1 };
        var weights = _settings.ballSpawnWeights;
        w = weights != null && weights.Length == 5 ? (int[])weights.Clone() : new int[5] { 10, 15, 20, 25, 30 };
    }

    public int countConsecutiveSpawn { get; set; }

    public eBallType RandomNextBall()
    {
        int sum = 0;
        Shuffle(w);
        for (int i = 0; i < 5; i++)
            sum += w[i] * c[i];

        int value = UnityEngine.Random.Range(0, sum);
        int currentSum = 0;
        for (int i = 0; i < 5; i++)
        {
            currentSum += w[i] * c[i];
            if (value < currentSum) return (eBallType)(i + 1);
        }
        return eBallType.None;
    }

    public void UpdateConsecutiveSpawn(eBallType ballName)
    {
        if (countConsecutiveSpawn >= 2)
        {
            int index = (int)ballName;
            if (index >= 0 && index < c.Length) c[index - 1] = 0;
            countConsecutiveSpawn = 0;
        }
        else
        {
            for (int i = 0; i < c.Length; i++)
                c[i] = 1;
        }
    }

    private static void Shuffle(int[] array)
    {
        System.Random rnd = new System.Random();
        for (int i = array.Length - 1; i > 0; i--)
        {
            int j = rnd.Next(i + 1);
            int temp = array[i];
            array[i] = array[j];
            array[j] = temp;
        }
    }
}
