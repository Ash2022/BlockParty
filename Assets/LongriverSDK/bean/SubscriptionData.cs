using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
namespace LongriverSDKNS
{
    [Serializable]
    public class SubscriptionData
    {
        [UnityEngine.SerializeField]
        public List<SubscriptionItem> subscriptionItems = new List<SubscriptionItem>();
    }
}

