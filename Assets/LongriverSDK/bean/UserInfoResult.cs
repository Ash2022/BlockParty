using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace LongriverSDKNS
{
    [Serializable]
    public class UserInfoResult
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
        public List<PlatformAccountInfo> loginInfo = new List<PlatformAccountInfo>();
    }
}

