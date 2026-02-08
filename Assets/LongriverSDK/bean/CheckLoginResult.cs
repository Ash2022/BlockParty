using System;

namespace LongriverSDKNS
{
    [Serializable]
    public class CheckLoginResult
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

        public CheckLoginResult(string gameId, long gameAccountId, string saveGameId, long saveId, string loginType)
        {
            this.gameId = gameId;
            this.gameAccountId = gameAccountId;
            this.saveGameId = saveGameId;
            this.saveId = saveId;
            this.loginType = loginType;
        }
    }
}

