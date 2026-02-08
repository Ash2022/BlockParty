using System;
namespace LongriverSDKNS
{
    [Serializable]
    public class SignOutResult
    {
        [UnityEngine.SerializeField]
        public int code;
        [UnityEngine.SerializeField]
        public string msg;
        public SignOutResult(int code, string msg) 
        {
            this.code = code;
            this.msg = msg;
        }
    }
}

