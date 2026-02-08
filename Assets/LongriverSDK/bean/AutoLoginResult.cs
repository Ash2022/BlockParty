using System;
namespace LongriverSDKNS
{
    [Serializable]
    public class AutoLoginResult
    {
        [UnityEngine.SerializeField]
        public string gameId;
        [UnityEngine.SerializeField]
        public long gameAccountId;
        [UnityEngine.SerializeField]
        public string saveGameId;
        [UnityEngine.SerializeField]
        public long saveId; 
        [UnityEngine.SerializeField]
        public string loginType;
        [UnityEngine.SerializeField]
        public bool isNew;

        public AutoLoginResult(string gameId, long gameAccountId, string saveGameId, long saveId, string loginType, bool isNew)
        {
            this.gameId = gameId;
            this.gameAccountId = gameAccountId;
            this.saveGameId = saveGameId;
            this.saveId = saveId;
            this.loginType = loginType;
            this.isNew = isNew;
        }
    }
}

