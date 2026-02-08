using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
namespace LongriverSDKNS
{
    [Serializable]
    public class PurchaseItems
    {
        [UnityEngine.SerializeField]
        public List<UnconsumeItem> unconsumeItems = new List<UnconsumeItem>();
    }
}

