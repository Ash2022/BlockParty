namespace LongriverSDKNS
{
    [System.Serializable]
    public class GetBindTransferCodeResult
    {
        [UnityEngine.SerializeField]
        public string gameId;
        [UnityEngine.SerializeField]
        public long gameAccountId;
        public GetBindTransferCodeResult(string gameId, long gameAccountId)
        {
            this.gameId = gameId;
            this.gameAccountId = gameAccountId;
        }
    }
}

