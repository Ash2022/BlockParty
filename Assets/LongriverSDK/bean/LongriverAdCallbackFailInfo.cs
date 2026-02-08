using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace LongriverSDKNS
{
    [Serializable]
    public class SimpleAdCallbackFailInfo
    {
        [UnityEngine.SerializeField]
        public string unitId;
        [UnityEngine.SerializeField]
        public string code;
        [UnityEngine.SerializeField]
        public string message;
        public SimpleAdCallbackFailInfo()
        {

        }
    }
}

