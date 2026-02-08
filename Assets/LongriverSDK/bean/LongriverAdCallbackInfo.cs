using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace LongriverSDKNS {
    [Serializable]
    public class SimpleAdCallbackInfo
    {
        [UnityEngine.SerializeField]
        public string unitId;
        [UnityEngine.SerializeField]
        public CallbackInfo callbackInfo;
        public SimpleAdCallbackInfo()
        {

        }
    }
}

