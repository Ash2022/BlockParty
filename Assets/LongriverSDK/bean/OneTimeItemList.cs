using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace LongriverSDKNS
{
    [Serializable]
    public class OneTimeItemList
    {
        [UnityEngine.SerializeField]
        public List<OneTimeItem> oneTimeItems = new List<OneTimeItem>();
    }
}


