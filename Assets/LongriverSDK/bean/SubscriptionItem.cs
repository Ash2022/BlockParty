using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
namespace LongriverSDKNS
{
    [Serializable]
    public class SubscriptionItem
    {
        [UnityEngine.SerializeField]
        public string originalTransactionId;
        [UnityEngine.SerializeField]
        public string itemId;
        [UnityEngine.SerializeField]
        public string productId;
        [UnityEngine.SerializeField]
        public bool isSubscribed;
        [UnityEngine.SerializeField]
        public long purchaseTime;
        [UnityEngine.SerializeField]
        public long expiredTime;
        [UnityEngine.SerializeField]
        public long leftTime;
        [UnityEngine.SerializeField]
        public bool isInFree;
        [UnityEngine.SerializeField]
        public bool isInIntroductory;
        [UnityEngine.SerializeField]
        public bool isCancel;
    }
}

