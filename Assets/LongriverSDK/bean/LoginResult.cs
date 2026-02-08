using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace LongriverSDKNS
{
    [Serializable]
    public class LoginResult
    {
        [UnityEngine.SerializeField]
        public long gameAccountId;
        [UnityEngine.SerializeField]
        public string saveGameId;
        [UnityEngine.SerializeField]
        public long saveId;
        [UnityEngine.SerializeField]
        public string loginType;
        [UnityEngine.SerializeField]
        public string sessionToken;
        [UnityEngine.SerializeField]
        public bool isNew;
    }
}

