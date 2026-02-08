namespace LongriverSDKNS
{
    [System.Serializable]
    public class GenerateTransferCodeResult
    {
        [UnityEngine.SerializeField]
        public string transferCode;
        public GenerateTransferCodeResult(string transferCode)
        {
            this.transferCode = transferCode;
        }
    }
}

