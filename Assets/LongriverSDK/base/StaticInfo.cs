using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Runtime.InteropServices;
namespace LongriverSDKNS
{
    [Serializable]
    public class StaticInfo
    {
        [UnityEngine.SerializeField]
        public string gameName="";
        [UnityEngine.SerializeField]
        public string pn = "";
        [UnityEngine.SerializeField]
        public string appVersion = "";
        [UnityEngine.SerializeField]
        public string deviceid = "";
        [UnityEngine.SerializeField]
        public string platform = "";
        [UnityEngine.SerializeField]
        public string idfa = "";
        [UnityEngine.SerializeField]
        public string uid = "";
        [UnityEngine.SerializeField]
        public string sessionId = "";
        [UnityEngine.SerializeField]
        public string idfv = "";
        [UnityEngine.SerializeField]
        public string android_id = "";
        [UnityEngine.SerializeField]
        public string band = "";
        [UnityEngine.SerializeField]
        public string model = "";
        [UnityEngine.SerializeField]
        public string deviceName = "";
        [UnityEngine.SerializeField]
        public string systemVersion = "";
        [UnityEngine.SerializeField]
        public string network = "";
        [UnityEngine.SerializeField]
        public int isVpn = -1;
        [UnityEngine.SerializeField]
        public int isProxy = -1;

        private StaticInfo() { }

        public string toJson()
        {
            return JsonUtility.ToJson(this);
        }

        static public StaticInfo fromJson(string str)
        {
            return JsonUtility.FromJson<StaticInfo>(str);
        }

    }
}

