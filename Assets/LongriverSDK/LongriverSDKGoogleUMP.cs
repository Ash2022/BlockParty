using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LongriverSDKNS
{
    public class LongriverSDKGoogleUMP : MonoBehaviour
    {
        protected static LongriverSDKGoogleUMP Singleton = null;
        static public  LongriverSDKGoogleUMP instance
        {
            get
            {
                if (null == Singleton)
                {
                    Singleton = UnityEngine.Object.FindFirstObjectByType<LongriverSDKGoogleUMP>();
                    if (null == Singleton) {
                        GameObject go = new GameObject("LongriverSDKGoogleUMP");
                        Singleton = go.AddComponent<LongriverSDKGoogleUMP>();
                        DontDestroyOnLoad(go);
                    }
                }
                return Singleton;
            }
        }
        void Awake() { if (null == Singleton) Singleton = this; else Destroy(gameObject); }

        // Start is called before the first frame update
        void Start()
        {
            
        }

        private Action googleUMPInitSDK = null;
        private Action googleUMPInitSuccess = null;
        private Action<int, string> googleUMPInitFailed = null;
        private Action googleUMPPrivacySuccess = null;
        private Action<int, string> googleUMPPrivacyFailed = null;

#if UNITY_ANDROID && !UNITY_EDITOR
        static private readonly string className = "com.longriversdk.longriverandroidsdk.GoogleUMPHelper"; 
        public class Void {}
        static internal T CallStatic<T>(string className, string methodName, params object[] args)
        {
            try {
                UnityEngine.Debug.LogFormat($"call android method {methodName};");
                using(UnityEngine.AndroidJavaClass jc = new UnityEngine.AndroidJavaClass(className)) 
                {
                    if (typeof(T) == typeof(Void))
                    {
                        jc.CallStatic(methodName, args);
                        return (T) default;
                    }
                    else
                    {
                        return jc.CallStatic<T>(methodName, args);
                    }
                }
            } catch (System.Exception e) {
                UnityEngine.Debug.LogException(e);
                return (T) default;
            }
        }

        public class GoogleUMPInitListener : UnityEngine.AndroidJavaProxy 
        {
            public GoogleUMPInitListener() : base("com.longriversdk.longriverandroidsdk.GoogleUMPHelper$GoogleUMPInitListener") {}
            public void onInitSDK() 
            {
                UnityMainThreadDispatcher.Instance().Enqueue(() => {
                    if (null != instance.googleUMPInitSDK)
                    {
                        instance.googleUMPInitSDK.Invoke();
                    }
                });
            }
            public void onUMPGatherSuccess() 
            {
                UnityMainThreadDispatcher.Instance().Enqueue(() => {
                    if (null != instance.googleUMPInitSuccess)
                    {
                        instance.googleUMPInitSuccess.Invoke();
                    }
                });
                
            }
            public void onUMPGatherFailed(int errorCode, string errorMsg) 
            {
                UnityMainThreadDispatcher.Instance().Enqueue(() => {
                    if (null != instance.googleUMPInitFailed)
                    {
                        instance.googleUMPInitFailed.Invoke(errorCode, errorMsg);
                    }
                });
            }
        }

        public class GoogleUMPPrivacyListener : UnityEngine.AndroidJavaProxy
        {
            public GoogleUMPPrivacyListener() : base("com.longriversdk.longriverandroidsdk.GoogleUMPHelper$GoogleUMPPrivacyListener") {}
            public void onSuccess() 
            {
                UnityMainThreadDispatcher.Instance().Enqueue(() => {
                    if (null != instance.googleUMPPrivacySuccess)
                    {
                        instance.googleUMPPrivacySuccess.Invoke();
                    }
                });
                
            }
            public void onFailed(int errorCode, string errorMsg) 
            {
                UnityMainThreadDispatcher.Instance().Enqueue(() => {
                    if (null != instance.googleUMPPrivacyFailed) 
                    {
                        instance.googleUMPPrivacyFailed.Invoke(errorCode, errorMsg);
                    }
                });
            }
        }

        public void SetEuropean(bool isEuropean)
        {
            CallStatic<Void>(className, "setEuropean", isEuropean);
        }

        public void SetForceTesting(bool isForceTesting)
        {
            CallStatic<Void>(className, "setForceTesting", isForceTesting);
        }

        public void SetDebugGeography(bool debugGeography)
        {
            CallStatic<Void>(className, "setDebugGeography", debugGeography);
        }

        public void AddTestDeviceHashedId(string deviceHashedId)
        {
            CallStatic<Void>(className, "addTestDeviceHashedId", deviceHashedId);
        }

        public void DoInit(Action initSDK, Action success, Action<int, string> fail) 
        {
            this.googleUMPInitSDK = initSDK;
            using(UnityEngine.AndroidJavaClass up = new UnityEngine.AndroidJavaClass("com.unity3d.player.UnityPlayer")) 
            {
                using (UnityEngine.AndroidJavaObject currentActivity = up.GetStatic<UnityEngine.AndroidJavaObject>("currentActivity"))
                {
                    CallStatic<Void>(className, "doInit", currentActivity, new GoogleUMPInitListener());
                }
            }
        }

        public void ShowPrivacyOptionsForm(Action success, Action<int, string> fail) 
        {
            using(UnityEngine.AndroidJavaClass up = new UnityEngine.AndroidJavaClass("com.unity3d.player.UnityPlayer")) 
            {
                using (UnityEngine.AndroidJavaObject currentActivity = up.GetStatic<UnityEngine.AndroidJavaObject>("currentActivity"))
                {
                    CallStatic<Void>(className, "showPrivacyOptionsForm", currentActivity, new GoogleUMPPrivacyListener());
                }
            }
        }
        public string GetUMPParameters() 
        {
            return CallStatic<string>(className, "getUMPParameters");
        }
        public bool isPrivacyOptionsRequired() 
        { 
            return CallStatic<bool>(className, "isPrivacyOptionsRequired");
        }
#elif UNITY_IOS && !UNITY_EDITOR
        public void SetEuropean(bool isEuropean) {}
        public void SetForceTesting(bool isForceTesting) {}
        public void SetDebugGeography(bool debugGeography) {}
        public void AddTestDeviceHashedId(string deviceHashedId) {}
        public void DoInit(Action initSDK, Action success, Action<int, string> fail) 
        {
            success.Invoke();
            initSDK.Invoke();
        }
        public void ShowPrivacyOptionsForm(Action success, Action<int, string> fail) 
        {
            success.Invoke();
        }
        public string GetUMPParameters() { return "{}"; }
        public bool isPrivacyOptionsRequired() { return true; }
#else
        public void SetEuropean(bool isEuropean) {}
        public void SetForceTesting(bool isForceTesting) {}
        public void SetDebugGeography(bool debugGeography) {}
        public void AddTestDeviceHashedId(string deviceHashedId) {}
        public void DoInit(Action initSDK, Action success, Action<int, string> fail) 
        {
            success.Invoke();
            initSDK.Invoke();
        }
        public void ShowPrivacyOptionsForm(Action success, Action<int, string> fail) 
        {
            success.Invoke();
        }
        public string GetUMPParameters() { return "{}"; }
        public bool isPrivacyOptionsRequired() { return true; }
#endif
    }
}

