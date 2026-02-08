using System.Collections.Generic;
using UnityEngine;
using System;

namespace LongriverSDKNS
{
#if UNITY_EDITOR
    public enum EditorChannel
    {
        Editor,
        Organic
    }
#endif
    public class LongriverSDK : MonoBehaviour
    {

        // -- static --
        static public string SDK_VERSION = "V0.0.81";
        static public string APP_JUMP_SECRET_KEY = "8C34BFB0F00F4631BD7897A84F19EFBE";
        static public LongriverSDK instance;

        private bool hasInit = false;
        private Action<InitSuccessResult> initSuccessDelegate;
        private InitSuccessResult saveInitSuccessResult;
        private Action<Dictionary<string, object>> appJumpDelegate;
#if UNITY_EDITOR
        public EditorChannel editorChannel = EditorChannel.Editor;
        public bool editorTestMockAutoLoginReturnSucc = true;
        public bool editorTestMockCheckLoginReturnSucc = true;
        public bool editorTestMockCheckLoginDeviceReturnSucc = true;
        public List<String> editorTestMockShopItemIdsList;
        public List<String> editorTestMockSubscriptionItemIdsList;
        public bool editorTestMockPaySucc = true;
#endif
        [UnityEngine.HideInInspector]
        [UnityEngine.SerializeField]
        private TextAsset configText = null;

        public Action<State> showPrivacyOptionsFormDelegate = null;

        public void Awake()
        {
            if (instance != null)
            {
                DestroyImmediate(gameObject);
            }
            else
            {
                DontDestroyOnLoad(gameObject);
                instance = this;
            }
        }
        public void Start()
        {
            Init();
        }

        public void Init()
        {
            this.BindCallbackManager();
            if (null == configText) {
                LongriverSDKConfig.GetConfig(this, RunAfterGetConfig);
            } else {
                this.RunAfterGetConfig(configText.text);
            }
        }

        public string GetSDKVersion()
        {
            return LongriverSDKBridgeFactory.instance.GetSDKVersion();
        }

        private void BindCallbackManager()
        {
            var obj = FindSubObject(gameObject);
            obj.AddComponent<LongriverSDKCallback>();
        }
        private GameObject FindSubObject(GameObject go)
        {
            var trans = go.transform.Find("NativeInternalObject");
            if (trans != null)
            {
                Debug.Log("find NativeInternalObject");
                return trans.gameObject;
            }
            else
            {
                Debug.Log("add NativeInternalObject");
                var obj = new GameObject();
                obj.name = "NativeInternalObject";
                obj.transform.SetParent(go.transform);
                return obj;
            }
        }

        private void RunAfterGetConfig(String inputConfig)
        {
           if (inputConfig != null)
           {
                Debug.Log($"config: {inputConfig}");
                LongriverSDKBridgeFactory.instance.initWithConfig(inputConfig);
           }
           else
           {
               Debug.Log("LongriverSDK fail to get any config from file or code");
           }
        }

        public void setInitSuccessDelegate(Action<InitSuccessResult> inputDelegate)
        {
            this.initSuccessDelegate = inputDelegate;
            if (hasInit)
            {
                this.initSuccessDelegate(this.saveInitSuccessResult);
            }
        }

        public void initSuccess(InitSuccessResult inputSuccessResult)
        {
            Debug.Log("SDK init success");
            hasInit = true;
            this.saveInitSuccessResult = inputSuccessResult;
            if (this.initSuccessDelegate != null)
            {
                this.initSuccessDelegate(saveInitSuccessResult);
            }
        }

        public void enableSimpleMode()
        {
            LongriverSDKBridgeFactory.instance.enableSimpleMode();
        }

        public void setUmpWithIp(bool enable)
        {
            LongriverSDKBridgeFactory.instance.setUmpWithIp(enable);
        }
        public void setAppJumpDelegate(Action<Dictionary<string, object>> appJumpDelegate)
        {
            this.appJumpDelegate = appJumpDelegate;
#if UNITY_IOS && !UNITY_EDITOR
            Action<string> block = (string openUrl) => 
            {
                Debug.Log($"deepLinkActivated: {openUrl}");
                if (openUrl.Contains(APP_JUMP_SECRET_KEY))
                {
                    string data = openUrl[(openUrl.IndexOf("data") + 5)..];
                    string dataJson = System.Uri.UnescapeDataString(data);
                    Debug.Log($"deepLinkActivated dataJson: {dataJson}");
                    Dictionary<string, object> dataDict = Json.Deserialize(dataJson) as Dictionary<string, object>;
                    this.appJumpCallback(dataDict);
                }
            };
            // longriversdk://8C34BFB0F00F4631BD7897A84F19EFBE?data=%7B%22uid%22:123867275589,%22level%22:129,%22services%20%22:%22s2%22,%22userName%22:%22zhangsan%22,%22transferCode%22:null%7D
            string openUrl = Application.absoluteURL;   // check cold launch
            if (!string.IsNullOrWhiteSpace(openUrl))
            {
                block(openUrl);
            }
            Application.deepLinkActivated += (openUrl) =>   // listener hot launch
            {
                block(openUrl);
            };
#endif
        }

        internal void appJumpCallback(Dictionary<string, object> dataDict)
        {
            this.appJumpDelegate?.Invoke(dataDict);
        }

        public int tryLaunchOrOpenAppStore(string packageName, string className, Dictionary<string, object> dataDict)
        {
            return LongriverSDKBridgeFactory.instance.tryLaunchOrOpenAppStore(packageName, className, dataDict);
        }

        public string GetUMPParameters()
        {
            return LongriverSDKBridgeFactory.instance.GetUMPParameters();
        }

        public bool IsPrivacyOptionsRequired()
        {
            return LongriverSDKBridgeFactory.instance.IsPrivacyOptionsRequired();
        }

        public void ShowPrivacyOptionsForm(Action<State> action)
        {
            this.showPrivacyOptionsFormDelegate = action;
            LongriverSDKBridgeFactory.instance.ShowPrivacyOptionsForm();
        }

        internal void CallbackShowPrivacyOptionsForm(State state)
        {
            if (null != this.showPrivacyOptionsFormDelegate)
            {
                this.showPrivacyOptionsFormDelegate.Invoke(state);
            }
        }

        public void SetDebugGeography(int index)
        {
            LongriverSDKBridgeFactory.instance.SetDebugGeography(index);
        }

        public void OnApplicationPause(bool isPaused)
        {
            if (hasInit)
            {
                if (isPaused)
                {
                    LongriverSDKBridgeFactory.instance.onPause();
                }
                else
                {
                    LongriverSDKBridgeFactory.instance.onResume();
                }
            }
        }

        public void SetAttributionInfoListener(Action<AttributionInfo> attributionInfoDelegate)
        {
            AttributionHelper.GetInstance().SetAttributionInfoListener(attributionInfoDelegate);
        }

        public AttributionInfo GetAttributionInfo()
        {
            return AttributionHelper.GetInstance().GetAttributionInfo();
        }
        public void Log(string eventName)
        {
            Log(eventName, null);
        }

        public void Log(string eventName, Dictionary<string, string> paramMap)
        {
            LongriverSDKBridgeFactory.instance.Log(eventName, paramMap);
        }
        public void logThirdLevel(int level)
        {
            LongriverSDKBridgeFactory.instance.logThirdLevel(level);
        }
        public void logThirdEvent(string eventName, Dictionary<string, string> paramMap)
        {
            LongriverSDKBridgeFactory.instance.logThirdEvent(eventName, paramMap);
        }

        public void logThirdEventWithCustomEvent(CustomEvent customEvent) 
        {
            LongriverSDKBridgeFactory.instance.logThirdEventWithCustomEvent(customEvent);
        }

        public void UploadEventToAllPlatform(string eventName, Dictionary<string, object> eventDatas)
        {
            LongriverSDKBridgeFactory.instance.UploadEventToAllPlatform(eventName, eventDatas);
        }

        public void UploadEventToPlus(string eventName, Dictionary<string, object> data)
        {
            LongriverSDKBridgeFactory.instance.UploadEventToPlus(eventName, data);
        }

        //helpful log
        public void LogPaySuccess(string store, string transactionID, string productID, DateTime purchaseDate, decimal price, string priceString, string currency)
        {
            Dictionary<string, string> param = new Dictionary<string, string>()
            {
                // GooglePlay or AppleAppStore
                {"store", store},
                {"transactionID", transactionID},
                {"productID", productID},
                {"purchaseDate", purchaseDate.ToString("yyyy-MM-dd HH:mm:ss")},
                {"currency", currency },
                {"priceString", priceString},
                {"price", price.ToString() },
            };
            Log("pay_success", param);
        }

        public void UploadRoleLogin(string server_id, string server_name, string role_id, string role_name)
        {
            LongriverSDKBridgeFactory.instance.UploadRoleLogin(server_id, server_name, role_id, role_name);
        }

        public StaticInfo GetStaticInfo()
        {
            return LongriverSDKBridgeFactory.instance.GetStaticInfo();
        }

        //onlineconfig
        private OnlineConfigResult onlineConfigResult = null;
        private List<Action<OnlineConfigResult>> onlineConfigActionList = new List<Action<OnlineConfigResult>>();
        public void fetchComplete(OnlineConfigResult r)
        {
            onlineConfigResult = r;
            foreach(Action<OnlineConfigResult> one in onlineConfigActionList)
            {
                one(r);
            }
        }
        public List<OnlineParamPair> getRemoteConfigSync()
        {
            if (onlineConfigResult != null && onlineConfigResult.isSuccess)
            {
                return onlineConfigResult.data;
            }
            else return null;
        }
        public void getRemoteConfigAsync(Action<OnlineConfigResult> action)
        {
            if(onlineConfigResult != null)
            {
                action(onlineConfigResult);
            }
            else
            {
                onlineConfigActionList.Add(action);
            }
        }

        private Action<String> shareFacebookAsyncSuccessAction = null;
        private Action<State> shareFacebookAsyncFailAction = null;
        //share
        public void shareFacebook(string url,byte[] image, Action<String> success, Action<State> fail)
        {
            shareFacebookAsyncSuccessAction = success;
            shareFacebookAsyncFailAction = fail;
            LongriverSDKBridgeFactory.instance.shareFacebookAsync(url, image);
        }
        public void shareFacebookAsyncSuccess()
        {
            shareFacebookAsyncSuccessAction("success");
        }
        public void shareFacebookAsyncFail(State s)
        {
            shareFacebookAsyncFailAction(s);
        }
    }
}


