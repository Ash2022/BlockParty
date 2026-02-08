using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LongriverSDKNS
{
    [Serializable]
    public class State
    {
        [UnityEngine.SerializeField]
        public int code;
        [UnityEngine.SerializeField]
        public string msg;
        public State(int code, string msg)
        {
            this.code = code;
            this.msg = msg;
        }
    }
}

