using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System;

namespace LongriverSDKNS
{
    [Serializable]
    public class ShopItemResult
    {
        [UnityEngine.SerializeField]
        public List<ShopItem> items = new List<ShopItem>();
    }
}

