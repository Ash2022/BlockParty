using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace LongriverSDKNS
{
    [Serializable]
    public class ShopItem
    {
        [UnityEngine.SerializeField]
        public string itemId;
        [UnityEngine.SerializeField]
        public string itemType;
        [UnityEngine.SerializeField]
        public int price;
        [UnityEngine.SerializeField]
        public string currency;
        [UnityEngine.SerializeField]
        public string formattedPrice;
        [UnityEngine.SerializeField]
        public double usdPrice;
        [UnityEngine.SerializeField]
        public double doublePrice;
        [UnityEngine.SerializeField]
        public string stringPrice;
    }
}

