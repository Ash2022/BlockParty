namespace LongriverSDKNS
{
    [System.Serializable]
    public class BindTransferCodeResult
    {
        [UnityEngine.SerializeField]
        public string gameId;
        [UnityEngine.SerializeField]
        public long gameAccountId;
        public BindTransferCodeResult(string gameId, long gameAccountId)
        {
            this.gameId = gameId;
            this.gameAccountId = gameAccountId;
        }
    }
}

