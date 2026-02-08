using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;


namespace LongriverSDKNS
{
    [Serializable]
    public class InitSuccessResult
    {
        [UnityEngine.SerializeField]
        public bool isSuccess;
        [UnityEngine.SerializeField]
        public string msg;

        static public InitSuccessResult FromJson(string str)
        {
            return JsonUtility.FromJson<InitSuccessResult>(str);
        }
        public string ToJson()
        {
            return JsonUtility.ToJson(this);
        }
    }
}


