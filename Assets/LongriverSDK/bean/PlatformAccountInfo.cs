using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
namespace LongriverSDKNS
{
    [Serializable]
    public class PlatformAccountInfo
    {
        [UnityEngine.SerializeField]
        public string platform;
        [UnityEngine.SerializeField]
        public bool hasLinked;
        [UnityEngine.SerializeField]
        public string uniqeId;
        [UnityEngine.SerializeField]
        public string iconUrl;
        [UnityEngine.SerializeField]
        public string nickName;
    }
}

