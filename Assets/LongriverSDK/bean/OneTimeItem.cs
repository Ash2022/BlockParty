using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace LongriverSDKNS
{
    [Serializable]
    public class OneTimeItem
    {
        [UnityEngine.SerializeField]
        public string itemId;
        [UnityEngine.SerializeField]
        public string productId;
        [UnityEngine.SerializeField]
        public long purchaseTime;
    }
}


