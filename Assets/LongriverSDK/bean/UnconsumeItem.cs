using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
namespace LongriverSDKNS
{
    [Serializable]
    public class UnconsumeItem
    {
        [UnityEngine.SerializeField]
        public long gameOrderId;
        [UnityEngine.SerializeField]
        public string itemId;
        [UnityEngine.SerializeField]
        public long createTime;
        [UnityEngine.SerializeField]
        public long purchaseTime;
        [UnityEngine.SerializeField]
        public int status;
    }
}

