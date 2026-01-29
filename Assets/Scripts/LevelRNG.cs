public static class LevelRNG
{
    private static System.Random _rng;

    /// <summary>
    /// Initialize the RNG with the given seed, only once per level load.
    /// </summary>
    public static void InitLevelRNG(int seed)
    {
        _rng = new System.Random(seed);
    }

    /// <summary>
    /// Provides a public accessor for the System.Random instance.
    /// All random calls in the game will use this instance.
    /// </summary>
    public static System.Random Rng
    {
        get
        {
            if (_rng == null)
            {
                // fallback: if never init, do some default
                _rng = new System.Random();
            }
            return _rng;
        }
    }
}
