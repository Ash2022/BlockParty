using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System;

namespace LongriverSDKNS
{
    [Serializable]
    public class StartPaymentResult
    {
        [UnityEngine.SerializeField]
        public long gameOrderId;
        [UnityEngine.SerializeField]
        public string transactionId;
        [UnityEngine.SerializeField]
        public string payload;
    }
}


