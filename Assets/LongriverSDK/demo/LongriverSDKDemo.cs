using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using Newtonsoft.Json;
namespace LongriverSDKNS
{
    public class LongriverSDKDemo : MonoBehaviour
    {
        private int count;
        private List<string> logs = new List<string>() { };
        int maxLogsCount = 30;

        //int startPaymentIndex = 0;
        int buttonWith = 300;
        int buttonHeight = 80;
        string myGameContentVersion = "1.0";

        static public bool onFront = true;
        [UnityEngine.SerializeField]
        public bool hasTestWeb = false;

        int screenWidth;
        int screenHeight;
        private Vector2 btnScrollPosition;
        private Vector2 logScrollPosition;
        private int totalScrollHeight = 1000;

        private ShopItemResult shopItemResult = null;

        private string transferCode = null;

        private bool mUseVersion1 = false;

        private bool mUseShopItemFromServer = false;

        public class DemoWebAdListener : LongriverSDKWebAdListener 
        {
            private LongriverSDKDemo demo;
            public DemoWebAdListener(LongriverSDKDemo demo)
            {
                this.demo = demo;
            }

            public void onWebAdClose()
            {
                demo.log("onWebAdClose - ");
            }

            public void onWebAdLoaded()
            {
                demo.log("onWebAdLoaded - ");
            }

            public void onWebAdLoadFailed(string errorMsg)
            {
                demo.log($"onWebAdLoadFailed - {errorMsg}");
            }

            public void onWebAdPlayFailed(string errorMsg)
            {
                demo.log($"onWebAdPlayFailed - {errorMsg}");
            }

            public void onWebAdPlayStart()
            {
                demo.log("onWebAdPlayStart - ");
            }

            public void onWebAdReward(string msg)
            {
                demo.log($"onWebAdReward - {msg}");
            }
        }

        public class DemoAppOpenListener : LongriverSDKAppOpenAdListener {
            private LongriverSDKDemo demo;
            public DemoAppOpenListener(LongriverSDKDemo demo)
            {
                this.demo = demo;
            }
            /***
            * 广告超时
            */
            public void onAppOpenAdTimeout(string unitId)
            {
                demo.log("appopen ad timeout");
            }
            /***
            * 广告展示
            */
            public void onAppOpenAdShow(string unitId, CallbackInfo callbackInfo)
            {
                demo.log($"appopen ad show {callbackInfo.GetRevenue()}");
            }
            /***
            * 广告关闭
            */
            public void onAppOpenAdClose(string unitId, CallbackInfo callbackInfo)
            {
                demo.log("appopen ad close");
            }
            /***
            * 广告点击
            */
            public void onAppOpenAdClick(string unitId, CallbackInfo callbackInfo)
            {
                demo.log("appopen ad click");
            }

            public void onAppOpenAdLoaded(string unitId, CallbackInfo callbackInfo)
            {
                demo.log("appopen ad loaded");
            }

            public void onAppOpenAdLoadFailed(string unitId, int errorCode, string errorMsg)
            {
                demo.log("appopen ad load failed");
            }
        }

        public class DemoRewardListener : LongriverSDKRewardedVideoListener
        {
            private LongriverSDKDemo demo;
            public DemoRewardListener(LongriverSDKDemo demo)
            {
                this.demo = demo;
            }
            public void onRewardedVideoAdPlayStart(string adEntry, CallbackInfo callbackInfo)
            {
                demo.log($"reward video start {callbackInfo.GetRevenue()}");
            }
            public void onRewardedVideoAdPlayFail(string adEntry, string code, string message)
            {
                demo.log("reward video play fail");
            }
            public void onRewardedVideoAdPlayClicked(string adEntry, CallbackInfo callbackInfo)
            {
                demo.log("reward video click");
            }

            public void onRewardedVideoAdPlayClosed(string unitId, CallbackInfo callbackInfo)
            {
                demo.log("reward video close");
            }

            public void onReward(string unitId, CallbackInfo callbackInfo)
            {
                demo.log("give reward in onReward");
            }

            public void onRewardedVideoAdLoaded(string unitId, CallbackInfo callbackInfo)
            {
                demo.log("reward video loaded");
            }

            public void onRewardedVideoAdLoadFailed(string unitId, int errorCode, string errorMsg)
            {
                demo.log("reward video load failed");
            }
        }

        public class DemoInterListener : LongriverSDKInterstitialAdListener
        {
            private LongriverSDKDemo demo;
            public DemoInterListener(LongriverSDKDemo demo)
            {
                this.demo = demo;
            }
            public void onInterstitialAdShow(string adEntry, CallbackInfo callbackInfo)
            {
                demo.log($"Interstitial ad show {callbackInfo.GetRevenue()}");
            }
            /***
             * 广告关闭
             * @param unitId 广告位id
             */
            public void onInterstitialAdClose(string adEntry, CallbackInfo callbackInfo)
            {
                demo.log("Interstitial close");
            }
            /***
             * 广告点击
             * @param unitId 广告位id
             */
            public void onInterstitialAdClick(string adEntry, CallbackInfo callbackInfo)
            {
                demo.log("Interstitial ad click");
            }

            public void onInterstitialAdLoaded(string unitId, CallbackInfo callbackInfo)
            {
                demo.log("Interstitial ad loaded");
            }

            public void onInterstitialAdLoadFailed(string unitId, int errorCode, string errorMsg)
            {
                demo.log("Interstitial ad loaded failed");
            }

            public void onInterstitialAdPlayFail(string unitId, int errorCode, string errorMsg)
            {
                demo.log("Interstitial ad play failed");
            }
        }

        public class DemoBannerListener : LongriverSDKBannerAdListener
        {
            private LongriverSDKDemo demo;
            public DemoBannerListener(LongriverSDKDemo demo)
            {
                this.demo = demo;
            }
            public void onAdClick(string unitId, CallbackInfo callbackInfo)
            {
                demo.log("banner ad click");
            }

            public void onAdImpress(string unitId, CallbackInfo callbackInfo)
            {
                demo.log($"banner ad impress {callbackInfo.GetRevenue()}");
            }

            public void onAdLoaded(string unitId, CallbackInfo callbackInfo)
            {
                demo.log("banner ad loaded");
            }

            public void onAdLoadFailed(string unitId, int errorCode, string errorMsg)
            {
                demo.log("banner ad load failed");
            }
        }


        public class DemoPurchaseListener : IPurchaseItemsListener
        {
            private LongriverSDKDemo demo;
            public DemoPurchaseListener(LongriverSDKDemo demo)
            {
                this.demo = demo;
            }

            public void getOneTimeItems(OneTimeItemList oneTimeItemList)
            {
                demo.log("find getOneTimeItems callback " + JsonUtility.ToJson(oneTimeItemList));
            }

            public void getPurchaseItems(PurchaseItems purchaseItems)
            {
                foreach(var one in purchaseItems.unconsumeItems)
                {
                    demo.log("find unconsume item" + one.itemId + " " + one.gameOrderId+" and ready to consume");
                    LongriverSDKUserPayment.instance.consumeItem(one.gameOrderId);
                    demo.log("success to unconsume item" + one.itemId + " " + one.gameOrderId);
                }
            }

            public void getSubscriptionItems(SubscriptionData subscriptionItems)
            {
                demo.log("find subscription callback "+JsonUtility.ToJson(subscriptionItems));
            }
        }
#if DEBUG || DEVELOPMENT_BUILD

#if UNITY_IOS && !UNITY_EDITOR
        [System.Runtime.InteropServices.DllImport("__Internal")]
        private static extern void abort();
        public void TriggerNativeCrash() 
        {
            abort();
        }
#elif UNITY_ANDROID && !UNITY_EDITOR
        public void TriggerNativeCrash() 
        {
            AndroidJavaClass unityPlayer = new("com.unity3d.player.UnityPlayer");
            AndroidJavaObject activityObj = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
            AndroidJavaClass testCrashObj = new("com.library.test.TestCrash");
            testCrashObj.CallStatic("bridgeAddButton", activityObj, "TriggerNativeCrash");
        }
#else
        public void TriggerNativeCrash() {}
#endif

#endif // - #if DEBUG || DEVELOPMENT_BUILD
        void Start()
        {
            screenWidth = UnityEngine.Screen.width;
            screenHeight = UnityEngine.Screen.height;
#if !UNITY_EDITOR
    		buttonWith = screenWidth / 2 ;
    		buttonHeight = screenHeight / 13;
#endif
            btnScrollPosition = Vector2.zero;
            logScrollPosition = Vector2.zero;

            //the sdkOrderIds should save in the local or cloud
            HashSet<string> hasSendedSdkOrders = new HashSet<string>();

            // enable simple mode
            // LongriverSDK.instance.enableSimpleMode();

            LongriverSDK.instance.SetAttributionInfoListener(AttationInfoCallback);
            //success callback
            LongriverSDK.instance.setInitSuccessDelegate(InitCallback);
#if UNITY_IOS && !UNITY_EDITOR           
            string openUrl = Application.absoluteURL;
            log($"cold launch openUrl: {openUrl} - {string.IsNullOrWhiteSpace(openUrl)}");
#endif
            LongriverSDK.instance.setAppJumpDelegate(AppJumpCallback);

            //ad listener
            if (this.hasTestWeb)
            {
                LongriverSDKAd.instance.SetlongriverSDKWebAdListener(new DemoWebAdListener(this));
            }
            LongriverSDKAd.instance.SetLongriverSDKAppOpenAdListener(new DemoAppOpenListener(this));
            LongriverSDKAd.instance.SetLongriverSDKRewardedVideoListener(new DemoRewardListener(this));
            LongriverSDKAd.instance.SetLongriverSDKInterstitialAdListener(new DemoInterListener(this));
            LongriverSDKAd.instance.SetLongriverSDKBannerAdListener(new DemoBannerListener(this));
            LongriverSDKAd.instance.SetAdvertisingActiveListenter((string action) => {
                log("try show appopen ad");
                LongriverSDKAd.instance.ShowAppOpenWithTimeout(5.0F);
            });

            //item listener
            LongriverSDKUserPayment.instance.setIPurchaseItemsListener(new DemoPurchaseListener(this));
            LongriverSDKUserPayment.instance.setTransactionStatusListener((State state) => {
                log("transaction status code: " + state.code);
            });

            //online config
            LongriverSDK.instance.getRemoteConfigAsync((OnlineConfigResult r) =>
            {
                log("get online config complete "+ JsonUtility.ToJson(r));
            });
        }

        void OnGUI()
        {
            if (!onFront)
            {
                //if it is not on front stop draw the gui
                //Debug.Log("no on front return");
                return;
            }
            while (logs.Count > maxLogsCount)
            {
                logs.RemoveAt(0);
            }

            GUIStyle fontStyle = new GUIStyle();
            fontStyle.normal.background = null;
            fontStyle.normal.textColor = new Color(1, 1, 1);
            fontStyle.fontSize = 30;
            fontStyle.wordWrap = true;

            GUIStyle btnStyle = new GUIStyle(GUI.skin.button);
            btnStyle.alignment = TextAnchor.MiddleCenter;
            btnStyle.fontSize = 28;
            btnStyle.normal.textColor = Color.white;
            btnStyle.wordWrap = true;


            string l = "";
            for (int i = logs.Count - 1; i >= 0; i--)
            {
                l += logs[i] + "\n*************\n";
            }

            logScrollPosition = GUI.BeginScrollView(new Rect(5, 0, Screen.width - 5, Screen.height / 4), logScrollPosition, new Rect(5, 0, Screen.width - 10, 2000), false, false);
            GUI.Label(new Rect(5, 0, Screen.width - 15, Screen.height / 4), l, fontStyle);
            GUI.EndScrollView();

            //string playerItemStr = "player packages:\n";
            //foreach (var pair in playerPackages)
            //{
            //    playerItemStr += pair.Key + " : " + pair.Value + " \n";
            //}

            int y = Screen.height / 4 + 5;

            //GUI.Label(new Rect(buttonWith + 10, y + 10, Screen.width - buttonWith - 20, 150), playerItemStr, fontStyle);

            btnScrollPosition = GUI.BeginScrollView(new Rect(0, y, Screen.width, Screen.height - Screen.height / 4), btnScrollPosition, new Rect(0, y, Screen.width, totalScrollHeight), false, false);

            //y += buttonHeight + 10;
            //if (GUI.Button(new Rect(0, y, buttonWith, buttonHeight), "localtest", btnStyle))
            //{
            //    var ssss = "{\"isSuccess\":true,\"values\":{\"top5price\":\"5,4,3,2,1\"}}";
            //    var r = JsonUtility.FromJson<OnlineConfigResult>(ssss);
            //    Debug.Log(r.isSuccess +" "+ r.values.Count);
            //}
            y += buttonHeight + 10;
            if (GUI.Button(new Rect(0, y, buttonWith, buttonHeight), "try launch or open AppStore", btnStyle))
            {
                log("call try launch or open AppStore");
#if UNITY_IOS && !UNITY_EDITOR
                string appURLScheme = "beachsdk"; // com.simplesdk.test
                string appStoreURL = "itms-apps://itunes.apple.com/app/id284882215";
                int ret = LongriverSDK.instance.tryLaunchOrOpenAppStore(appURLScheme, appStoreURL, new Dictionary<string, object>() {
                    {"key_string", "value_string"},
                    {"key_int", 123},
                    {"key_float", 123.5F},
                });
#else
                string packageName = "com.survive.silentcastle";
                // string className = "com.survive.silentcastle.MainActivity";
                string className = "com.brick.sdk.library.kotlin.test.activitys.TestActivity";
                int ret = LongriverSDK.instance.tryLaunchOrOpenAppStore(packageName, className, new Dictionary<string, object>() {
                    {"key_string", "value_string"},
                    {"key_int", 123},
                    {"key_float", 123.5F},
                });
#endif
                log($"result: {ret}");
            }

            y += buttonHeight + 10;
            if (GUI.Button(new Rect(0, y, buttonWith, buttonHeight), "SetUseVersion1", btnStyle))
            {
                mUseVersion1 = !mUseVersion1;
                log($"call SetUseVersion1: {mUseVersion1}");
                LongriverSDKUserPayment.instance.SetUseVersion1(mUseVersion1);
            }

            y += buttonHeight + 10;
            if (GUI.Button(new Rect(0, y, buttonWith, buttonHeight), "SetUseShopItemFromServer", btnStyle))
            {
                mUseShopItemFromServer = !mUseShopItemFromServer;
                log($"call SetUseShopItemFromServer: {mUseShopItemFromServer}");
                LongriverSDKUserPayment.instance.SetUseShopItemFromServer(mUseShopItemFromServer);
            }
#if DEBUG || DEVELOPMENT_BUILD
            y += buttonHeight + 10;
            if (GUI.Button(new Rect(0, y, buttonWith, buttonHeight), "TriggerNativeCrash", btnStyle))
            {
                log("call TriggerNativeCrash");
                TriggerNativeCrash();
            }
#endif
            y += buttonHeight + 10;
            if (GUI.Button(new Rect(0, y, buttonWith, buttonHeight), "GetUMPParameters", btnStyle))
            {
                log("call get ump parameters");
                string umpJson = LongriverSDK.instance.GetUMPParameters();
                Dictionary<string, object> umpDict = Json.Deserialize(umpJson) as Dictionary<string, object>;
                bool isEuropean = umpDict.ContainsKey("isEuropean") && (bool) umpDict["isEuropean"];
                bool isGDPR = umpDict.ContainsKey("isGDPR") && (bool) umpDict["isGDPR"];
                bool canRequestAds = umpDict.ContainsKey("canRequestAds") && (bool) umpDict["canRequestAds"];
                bool canShowPersonalizedGoogleAds = umpDict.ContainsKey("canShowPersonalizedGoogleAds") && (bool) umpDict["canShowPersonalizedGoogleAds"];
                bool canShowGoogleAds = umpDict.ContainsKey("canShowGoogleAds") && (bool) umpDict["canShowGoogleAds"];
                bool canShowLimitedGoogleAds = umpDict.ContainsKey("canShowLimitedGoogleAds") && (bool) umpDict["canShowLimitedGoogleAds"];
                log($"isEuropean: {isEuropean}");
                log($"isGDPR: {isGDPR}");
                log($"canRequestAds: {canRequestAds}");
                log($"canShowPersonalizedGoogleAds: {canShowPersonalizedGoogleAds}");
                log($"canShowGoogleAds: {canShowGoogleAds}");
                log($"canShowLimitedGoogleAds: {canShowLimitedGoogleAds}");
            }

            y += buttonHeight + 10;
            if (GUI.Button(new Rect(0, y, buttonWith, buttonHeight), "IsPrivacyOptionsRequired", btnStyle))
            {
                log("call IsPrivacyOptionsRequired");
                bool isRequired = LongriverSDK.instance.IsPrivacyOptionsRequired();
                log($"isRequired: {isRequired}");
            }

            y += buttonHeight + 10;
            if (GUI.Button(new Rect(0, y, buttonWith, buttonHeight), "ShowPrivacyOptionsForm", btnStyle))
            {
                log("call show privacy options form");
                LongriverSDK.instance.ShowPrivacyOptionsForm((State state) => {
                    if (state.code == 200 )
                    {
                        log($"privacy options - success - {state.code} - {state.msg}");
                    }
                    else
                    {
                        log($"privacy options - failed - {state.code} - {state.msg}");
                    }
                });
            }

            y += buttonHeight + 10;
            if (GUI.Button(new Rect(0, y, buttonWith, buttonHeight), "SetDebugGeography", btnStyle))
            {
                log("call SetDebugGeography");
                // [0:disable |1:eea |2:not-eea]
                LongriverSDK.instance.SetDebugGeography(1);
            }

            y += buttonHeight + 10;
            if (GUI.Button(new Rect(0, y, buttonWith, buttonHeight), "logThirdEventWithCustomEvent", btnStyle))
            {
                log("call logThirdEventWithCustomEvent");
                CustomEvent customEvent = new CustomEvent();
                customEvent.eventName = "eventName";
                customEvent.partnerParameter = new Dictionary<string, object>() {
                    {"k1", "v1"}
                };
                customEvent.revenue = new Dictionary<string, object>() {
                    {"k2", 0.1}
                };
                LongriverSDK.instance.logThirdEventWithCustomEvent(customEvent);
            }
            y += buttonHeight + 10;
            if (GUI.Button(new Rect(0, y, buttonWith, buttonHeight), "Little Game Start", btnStyle))
            {
                log("call Little Game Start");
                LongriverSDK.instance.UploadEventToPlus("littleGameStart", null);
            }
            y += buttonHeight + 10;
            if (GUI.Button(new Rect(0, y, buttonWith, buttonHeight), "Little Game End", btnStyle))
            {
                log("call Little Game End");
                LongriverSDK.instance.UploadEventToPlus("littleGameEnd", null);
            }

            if (this.hasTestWeb)
            {
                y += buttonHeight + 10;
                if (GUI.Button(new Rect(0, y, buttonWith, buttonHeight), "isWebAdReady", btnStyle))
                {
                    bool isReady = LongriverSDKAd.instance.IsWebAdReady();
                    log($"call is web ad ready: {isReady}");
                }

                y += buttonHeight + 10;
                if (GUI.Button(new Rect(0, y, buttonWith, buttonHeight), "ShowUrl", btnStyle))
                {
                    log("call show url");
                    LongriverSDKAd.instance.ShowUrl();
                }
            }

            y += buttonHeight + 10;
            if (GUI.Button(new Rect(0, y, buttonWith, buttonHeight), "showAppOpenAdWithTimeout", btnStyle))
            {
                log("call show appopen ad");
                LongriverSDKAd.instance.ShowAppOpenWithTimeout(5.0F);
            }

            y += buttonHeight + 10;
            if (GUI.Button(new Rect(0, y, buttonWith, buttonHeight), "showAppOpenAdWithTimeout", btnStyle))
            {
                log("call show appopen ad");
                LongriverSDKAd.instance.ShowAppOpenWithTimeout(5.0F);
            }

            y += buttonHeight + 10;
            if (GUI.Button(new Rect(0, y, buttonWith, buttonHeight), "hasReward", btnStyle))
            {
                var result = LongriverSDKAd.instance.HasReward("");
                log("call hasRewardedVideo " + result);
            }

            y += buttonHeight + 10;
            if (GUI.Button(new Rect(0, y, buttonWith, buttonHeight), "showRewardVideoAd", btnStyle))
            {
                log("call showRewardVideoAd ");
                LongriverSDKAd.instance.ShowReward("");
            }

            y += buttonHeight + 10;
            if (GUI.Button(new Rect(0, y, buttonWith, buttonHeight), "hasReward no_google", btnStyle))
            {
                var result = LongriverSDKAd.instance.HasReward("no_google");
                log("call hasRewardedVideo no_google" + result);
            }

            y += buttonHeight + 10;
            if (GUI.Button(new Rect(0, y, buttonWith, buttonHeight), "showRewardVideoAd no_google", btnStyle))
            {
                log("call showRewardVideoAd no_google");
                LongriverSDKAd.instance.ShowReward("no_google");
            }


            y += buttonHeight + 10;
            if (GUI.Button(new Rect(0, y, buttonWith, buttonHeight), "hasInterstitial", btnStyle))
            {
                var result = LongriverSDKAd.instance.HasInterstitial("");
                log("call hasInterstitial " + result + " ");
            }

            y += buttonHeight + 10;
            if (GUI.Button(new Rect(0, y, buttonWith, buttonHeight), "showInterstitialAd", btnStyle))
            {
                log("call showInterstitialAd ");
                LongriverSDKAd.instance.ShowInterstitial("");
            }
            y += buttonHeight + 10;
            if (GUI.Button(new Rect(0, y, buttonWith, buttonHeight), "hasInterstitial no_google", btnStyle))
            {
                var result = LongriverSDKAd.instance.HasInterstitial("no_google");
                log("call hasInterstitial no_google" + result + " ");
            }

            y += buttonHeight + 10;
            if (GUI.Button(new Rect(0, y, buttonWith, buttonHeight), "showInterstitialAd no_google", btnStyle))
            {
                log("call showInterstitialAd no_google");
                LongriverSDKAd.instance.ShowInterstitial("no_google");
            }

            y += buttonHeight + 10;
            if (GUI.Button(new Rect(0, y, buttonWith, buttonHeight), "set banner background color", btnStyle))
            {
                log("set banner background color");
                LongriverSDKAd.instance.SetBannerBackgroundColor(0.5F, 0.2F, 0.8F, 1.0F);
            }

            y += buttonHeight + 10;
            if (GUI.Button(new Rect(0, y, buttonWith, buttonHeight), "showBannerAd top", btnStyle))
            {
                log("call showBannerAd top");
                LongriverSDKAd.instance.ShowOrReShowBanner(BannerPos.TOP);
            }

            y += buttonHeight + 10;
            if (GUI.Button(new Rect(0, y, buttonWith, buttonHeight), "showBannerAd bottom", btnStyle))
            {
                log("call showBannerAd bottom");
                LongriverSDKAd.instance.ShowOrReShowBanner(BannerPos.BOTTOM);
            }

            y += buttonHeight + 10;
            if (GUI.Button(new Rect(0, y, buttonWith, buttonHeight), "showBannerAd top safe", btnStyle))
            {
                log("call showBannerAd top");
                LongriverSDKAd.instance.ShowOrReShowBanner(BannerPos.SAFE_TOP);
            }

            y += buttonHeight + 10;
            if (GUI.Button(new Rect(0, y, buttonWith, buttonHeight), "showBannerAd bottom safe", btnStyle))
            {
                log("call showBannerAd bottom");
                LongriverSDKAd.instance.ShowOrReShowBanner(BannerPos.SAFE_BOTTOM);
            }

            y += buttonHeight + 10;
            if (GUI.Button(new Rect(0, y, buttonWith, buttonHeight), "hideBannerAd", btnStyle))
            {
                log("call dismissBannerAd ");
                LongriverSDKAd.instance.HideBanner();
            }

            y += buttonHeight + 10;
            if (GUI.Button(new Rect(0, y, buttonWith, buttonHeight), "removeBannerAd", btnStyle))
            {
                log("call dismissBannerAd ");
                LongriverSDKAd.instance.RemoveBanner();
            }

            y += buttonHeight + 10;
            if (GUI.Button(new Rect(0, y, buttonWith, buttonHeight), "testLog", btnStyle))
            {
                log("call testLog ");
                Dictionary<string, string> paramsDic = new Dictionary<string, string>();
                paramsDic.Add("paramA", "valueA");
                paramsDic.Add("paramB", "valueB");
                LongriverSDK.instance.Log("unityLogTest", paramsDic);
            }

            y += buttonHeight + 10;
            if (GUI.Button(new Rect(0, y, buttonWith, buttonHeight), "ad_load", btnStyle))
            {
                string eventName = "ad_load";
                LongriverSDK.instance.Log(eventName, new Dictionary<string, string>(){
                    {"mediationType", "xxx"},   // Admob
                    {"unitId", "xxx"},          // Admob UnitId
                    {"adtype", "xxx"},          // reward | interstitial | banner | appopen | native
                });
            }
            y += buttonHeight + 10;
            if (GUI.Button(new Rect(0, y, buttonWith, buttonHeight), "ad_load_fail", btnStyle))
            {
                string eventName = "ad_load_fail";
                LongriverSDK.instance.Log(eventName, new Dictionary<string, string>(){
                    {"mediationType", "xxx"},   // Admob
                    {"unitId", "xxx"},          // Admob UnitId
                    {"adtype", "xxx"},          // reward | interstitial | banner | appopen | native
                    {"code", "0"},              // errorCode
                    {"message", "xxx"},         // errorMsg
                }); 
            }
            y += buttonHeight + 10;
            if (GUI.Button(new Rect(0, y, buttonWith, buttonHeight), "ad_show", btnStyle))
            {
                string eventName = "ad_show";
                LongriverSDK.instance.Log(eventName, new Dictionary<string, string>(){
                    {"network_firm_id", "xxx"},     // Name of the advertising sub-channel
                    {"network_placement_id", "xxx"},// Ads ID of the advertising sub-channel
                    {"mediationType", "xxx"},       // Admob
                    {"unitId", "xxx"},              // Admob UnitId
                    {"adsource_id", "xxx"},         // Same value as unitId
                    {"adtype", "xxx"},              // reward | interstitial | banner | appopen | native
                    {"revenue", "0.1"},             // revenue(ecpm)
                });
            }
            y += buttonHeight + 10;
            if (GUI.Button(new Rect(0, y, buttonWith, buttonHeight), "ad_click", btnStyle))
            {
                string eventName = "ad_click";
                LongriverSDK.instance.Log(eventName, new Dictionary<string, string>(){
                    {"network_firm_id", "xxx"},         // Name of the advertising sub-channel
                    {"network_placement_id", "xxx"},   // Ads ID of the advertising sub-channel
                    {"mediationType", "xxx"},           // Admob
                    {"unitId", "xxx"},                  // Admob UnitId
                    {"adsource_id", "xxx"},             // Same value as unitId
                    {"adtype", "xxx"},                  // reward | interstitial | banner | appopen | native
                });
            }
            y += buttonHeight + 10;
            if (GUI.Button(new Rect(0, y, buttonWith, buttonHeight), "purchase_event", btnStyle))
            {
                decimal price = 0.1m;
                string currency = "USD";
                CustomEvent customEvent = new CustomEvent();
                customEvent.eventTag = "purchase";
                customEvent.logs = new Dictionary<string, object>() {
                    {"currency", currency},
                    {"price", price},
                    {"productId", "xxx"},
#if UNITY_ANDROID
                    {"transacationId", "GPA.3387-3098-5515-70766"},
#elif UNITY_IOS
                    {"transacationId", "2000000714738310"},
#else
                    {"transacationId", "xxxxx"},
#endif
                };
                LongriverSDK.instance.logThirdEventWithCustomEvent(customEvent);
            }
            y += buttonHeight + 10;
            if (GUI.Button(new Rect(0, y, buttonWith, buttonHeight), "addToCart_event", btnStyle))
            {
                decimal price = 0.1m;
                string currency = "USD";
                CustomEvent customEvent = new CustomEvent();
                customEvent.eventTag = "add_to_cart";
                customEvent.logs = new Dictionary<string, object>() {
                    {"currency", currency},
                    {"price", price},
                    {"productId", "xxx"},
                };
                LongriverSDK.instance.logThirdEventWithCustomEvent(customEvent);
            }
            y += buttonHeight + 10;
            if (GUI.Button(new Rect(0, y, buttonWith, buttonHeight), "initCheckout_event", btnStyle))
            {
                decimal price = 0.1m;
                string currency = "USD";
                CustomEvent customEvent = new CustomEvent();
                customEvent.eventTag = "init_checkout";
                customEvent.logs = new Dictionary<string, object>() {
                    {"currency", currency},
                    {"price", price},
                    {"productId", "xxx"},
                };
                LongriverSDK.instance.logThirdEventWithCustomEvent(customEvent);
            }
            y += buttonHeight + 10;
            if (GUI.Button(new Rect(0, y, buttonWith, buttonHeight), "subscription_event", btnStyle))
            {
                decimal price = 0.1m;
                string currency = "USD";
                CustomEvent customEvent = new CustomEvent();
                customEvent.eventTag = "subscription";
                customEvent.logs = new Dictionary<string, object>() {
                    {"currency", currency},
                    {"price", price},
                    {"orderId", "xxx"},
                };
                LongriverSDK.instance.logThirdEventWithCustomEvent(customEvent);
            }

            y += buttonHeight + 10;
            if (GUI.Button(new Rect(0, y, buttonWith, buttonHeight), "UploadEventToAllPlatform", btnStyle))
            {
                string eventName = "test_event_name";
                Dictionary<string, object> eventDatas = new() {
                    // aliyun & firebase
                    {"test_event_key1"      , "test_event_value1"},
                    {"test_event_key2"      , 2},
                    {"test_event_key3"      , 0.333D},
                    {"test_event_key4"      , true},
                    // adjust & aliyun & firebase
                    {"eventToken"           , "9qq8a4"},
                    {"amount"               , 0.99m},
                    {"currency"             , "USD"},
#if UNITY_ANDROID
                    {"deduplicationId"      , "GPA.3387-3098-5515-70766"},
                    {"transactionId"        , "GPA.3387-3098-5515-70766"},
#elif UNITY_IOS
                    {"deduplicationId"      , "2000000714738310"},
                    {"transactionId"        , "2000000714738310"},
#endif
                    {"productId"            , "item_id1"},
                    // adjust optional
                    {"callbackId"           , "test_callback_id"},
                    {"partnerParameters"    , new Dictionary<string, object>() {{ "test_partner_key1", "test_partner_value1" }}},
                    {"callbackParameters"   , new Dictionary<string, object>() {{ "test_callback_key1", "test_callback_value1" }}},
                };
                LongriverSDK.instance.UploadEventToAllPlatform(eventName, eventDatas);
            }

            y += buttonHeight + 10;
            if (GUI.Button(new Rect(0, y, buttonWith, buttonHeight), "online", btnStyle))
            {
                log("call online config");
                var re = LongriverSDK.instance.getRemoteConfigSync();
                if( re != null)
                {   
                    log("get online config " + re.Count);
                }
                else
                {
                    log("get online config null");
                }
            }

            y += buttonHeight + 10;
            if (GUI.Button(new Rect(0, y, buttonWith, buttonHeight), "pay success", btnStyle))
            {
                log("call testLog ");
                LongriverSDK.instance.LogPaySuccess("google play", "transactionID", "productID", DateTime.Now,
                    (decimal)0.99, "$ 0.99", "USD");

                string filePath = "Assets/StreamingAssets/LongriverSDKConfig";
                if (System.IO.File.Exists(filePath))
                {
                    string configContent = System.IO.File.ReadAllText("Assets/StreamingAssets/LongriverSDKConfig");
                    UnityEngine.Debug.Log($"configContent -> {configContent}");
                }
            }

            y += buttonHeight + 10;
            if (GUI.Button(new Rect(0, y, buttonWith, buttonHeight), "print attribution info", btnStyle))
            {
                var info = LongriverSDK.instance.GetAttributionInfo();
                if (info != null)
                {
                    log("info " + info.ToJson());
                }
                else
                {
                    log("info is null");
                }
            }
            y += buttonHeight + 10;
            if (GUI.Button(new Rect(0, y, buttonWith, buttonHeight), "pring loadding status", btnStyle))
            {
                var info = LongriverSDKAd.instance.GetLoadingStatusSummary();
                if (info != null)
                {
                    log("info " + Json.Serialize(info) );
                }
                else
                {
                    log("info is null");
                }
            }

            y += buttonHeight + 10;
            if (GUI.Button(new Rect(0, y, buttonWith, buttonHeight), "pring statics info", btnStyle))
            {
                var info = LongriverSDK.instance.GetStaticInfo();
                if (info != null)
                {
                    log("info " + info.toJson());
                }
                else
                {
                    log("info is null");
                }
            }

            y += buttonHeight + 10;
            if (GUI.Button(new Rect(0, y, buttonWith, buttonHeight), "autologin", btnStyle))
            {
                LongriverSDKUserPayment.instance.autoLoginAsync(true, delegate (AutoLoginResult r) {
                    log("autologin success " + JsonUtility.ToJson(r));

                }, delegate (State s) {
                    log("autologin fail " + JsonUtility.ToJson(s));
                });
            }

            y += buttonHeight + 10;
            if (GUI.Button(new Rect(0, y, buttonWith, buttonHeight), "check login", btnStyle))
            {
                LongriverSDKUserPayment.instance.checkLoginAsync(delegate(CheckLoginResult r) {
                    log("check login success " + JsonUtility.ToJson(r));

	            }, delegate(State s ) {
                    log("check login fail " + JsonUtility.ToJson(s));
                });
            }

            y += buttonHeight + 10;
            if (GUI.Button(new Rect(0, y, buttonWith, buttonHeight), "login with device", btnStyle))
            {
                LongriverSDKUserPayment.instance.loginWithTypeAsync(LOGIN_TYPE.DEVICE, delegate (LoginResult r) {
                    log("login success " + JsonUtility.ToJson(r));

                }, delegate (State s) {
                    log("login fail " + JsonUtility.ToJson(s));
                });
            }

            y += buttonHeight + 10;
            if (GUI.Button(new Rect(0, y, buttonWith, buttonHeight), "login with facebook", btnStyle))
            {
                LongriverSDKUserPayment.instance.loginWithTypeAsync(LOGIN_TYPE.FACEBOOK, delegate (LoginResult r) {
                    log("login success " + JsonUtility.ToJson(r));

                }, delegate (State s) {
                    log("login fail " + JsonUtility.ToJson(s));
                });
            }

            y += buttonHeight + 10;
            if (GUI.Button(new Rect(0, y, buttonWith, buttonHeight), "login with google play or game center", btnStyle))
            {
                LOGIN_TYPE t = LOGIN_TYPE_HELPER.GetLoginTypeWithSyste();
                LongriverSDKUserPayment.instance.loginWithTypeAsync(t, delegate (LoginResult r) {
                    log("login success " + JsonUtility.ToJson(r));

                }, delegate (State s) {
                    log("login fail " + JsonUtility.ToJson(s));
                });
            }

            y += buttonHeight + 10;
            if (GUI.Button(new Rect(0, y, buttonWith, buttonHeight), "login with apple_sign", btnStyle))
            {
                LongriverSDKUserPayment.instance.loginWithTypeAsync(LOGIN_TYPE.APPLE_SIGN, delegate (LoginResult r) {
                    log("login success " + JsonUtility.ToJson(r));

                }, delegate (State s) {
                    log("login fail " + JsonUtility.ToJson(s));
                });
            }

            y += buttonHeight + 10;
            if (GUI.Button(new Rect(0, y, buttonWith, buttonHeight), "bind with device", btnStyle))
            {
                LongriverSDKUserPayment.instance.bindWithTypeAsync(LOGIN_TYPE.DEVICE, delegate (UserInfoResult r) {
                    log("bind success " + JsonUtility.ToJson(r));

                }, delegate (State s) {
                    log("bind fail " + JsonUtility.ToJson(s));
                });
            }

            y += buttonHeight + 10;
            if (GUI.Button(new Rect(0, y, buttonWith, buttonHeight), "bind with facebook", btnStyle))
            {
                LongriverSDKUserPayment.instance.bindWithTypeAsync(LOGIN_TYPE.FACEBOOK, delegate (UserInfoResult r) {
                    log("bind success " + JsonUtility.ToJson(r));

                }, delegate (State s) {
                    log("bind fail " + JsonUtility.ToJson(s));
                });
            }

            y += buttonHeight + 10;
            if (GUI.Button(new Rect(0, y, buttonWith, buttonHeight), "bind with google play or game center", btnStyle))
            {
                LOGIN_TYPE t = LOGIN_TYPE_HELPER.GetLoginTypeWithSyste();
                LongriverSDKUserPayment.instance.bindWithTypeAsync(t, delegate (UserInfoResult r) {
                    log("bind success " + JsonUtility.ToJson(r));

                }, delegate (State s) {
                    log("bind fail " + JsonUtility.ToJson(s));
                });
            }

            y += buttonHeight + 10;
            if (GUI.Button(new Rect(0, y, buttonWith, buttonHeight), "unbind with device", btnStyle))
            {
                LongriverSDKUserPayment.instance.unbindWithTypeAsync(LOGIN_TYPE.DEVICE, delegate (UserInfoResult r) {
                    log("unbind success " + JsonUtility.ToJson(r));

                }, delegate (State s) {
                    log("unbind fail " + JsonUtility.ToJson(s));
                });
            }

            y += buttonHeight + 10;
            if (GUI.Button(new Rect(0, y, buttonWith, buttonHeight), "unbind with facebook", btnStyle))
            {
                LongriverSDKUserPayment.instance.unbindWithTypeAsync(LOGIN_TYPE.FACEBOOK, delegate (UserInfoResult r) {
                    log("unbind success " + JsonUtility.ToJson(r));

                }, delegate (State s) {
                    log("unbind fail " + JsonUtility.ToJson(s));
                });
            }

            y += buttonHeight + 10;
            if (GUI.Button(new Rect(0, y, buttonWith, buttonHeight), "unbind with google play or game center", btnStyle))
            {
                LOGIN_TYPE t = LOGIN_TYPE_HELPER.GetLoginTypeWithSyste();
                LongriverSDKUserPayment.instance.unbindWithTypeAsync(t, delegate (UserInfoResult r) {
                    log("unbind success " + JsonUtility.ToJson(r));

                }, delegate (State s) {
                    log("unbind fail " + JsonUtility.ToJson(s));
                });
            }
            y += buttonHeight + 10;
            if (GUI.Button(new Rect(0, y, buttonWith, buttonHeight), "get accountid and token", btnStyle))
            {
                log("gameAccountId is " + LongriverSDKUserPayment.instance.getGameAccountId());
                log("sessionToekn is " + LongriverSDKUserPayment.instance.getSessionToken());
            }

            y += buttonHeight + 10;
            if (GUI.Button(new Rect(0, y, buttonWith, buttonHeight), "get user info", btnStyle))
            {
                log("gameAccountId is " + LongriverSDKUserPayment.instance.getGameAccountId());
                LongriverSDKUserPayment.instance.getUserInfoAsync(delegate (UserInfoResult r) {
                    log("get user info success " + JsonUtility.ToJson(r));

                }, delegate (State s) {
                    log("get user info fail " + JsonUtility.ToJson(s));
                });
            }

            y += buttonHeight + 10;
            if (GUI.Button(new Rect(0, y, buttonWith, buttonHeight), "logout", btnStyle))
            {
                LongriverSDKUserPayment.instance.Logout();
            }

            y += buttonHeight + 10;
            if (GUI.Button(new Rect(0, y, buttonWith, buttonHeight), "delete account", btnStyle))
            {
                LongriverSDKUserPayment.instance.DeleteAccount(delegate (SignOutResult result) {
                    log("delete account success, " + JsonUtility.ToJson(result));
                }, delegate (State state) {
                    log("delete account failed, " + JsonUtility.ToJson(state));
                });
            }

            y += buttonHeight + 10;
            if (GUI.Button(new Rect(0, y, buttonWith, buttonHeight), "get bind transfer code", btnStyle))
            {
                LongriverSDKUserPayment.instance.GetBindTransferCode(delegate (GetBindTransferCodeResult result) {
                    log("get bind transfer code success, " + JsonUtility.ToJson(result));
                }, delegate (State state) {
                    log("get bind transfer code fail, " + JsonUtility.ToJson(state));
                });
            }

            y += buttonHeight + 10;
            if (GUI.Button(new Rect(0, y, buttonWith, buttonHeight), "generate transfer code", btnStyle))
            {
                LongriverSDKUserPayment.instance.GenerateTransferCode(delegate (GenerateTransferCodeResult result) {
                    log("generate transfer code success, " + JsonUtility.ToJson(result));
                    transferCode = result.transferCode;
                }, delegate(State state) {
                    log("generate transfer code fail, " + JsonUtility.ToJson(state));
                });
            }

            y += buttonHeight + 10;
            if (GUI.Button(new Rect(0, y, buttonWith, buttonHeight), "bind transfer code", btnStyle))
            {
                if (null != transferCode) {
                    LongriverSDKUserPayment.instance.BindTransferCode(transferCode, delegate (BindTransferCodeResult result) {
                        log("bind transfer code success, " + JsonUtility.ToJson(result));
                    }, delegate(State state) {
                        log("bind transfer code fail, " + JsonUtility.ToJson(state));
                    });
                }
            }

            y += buttonHeight + 10;
            if (GUI.Button(new Rect(0, y, buttonWith, buttonHeight), "bind transfer code and relogin", btnStyle))
            {
                if (null != transferCode) {
                    LongriverSDKUserPayment.instance.BindTransferCodeAndReLogin(transferCode, delegate (LoginResult result) {
                        log("bind transfer code and relogin success, " + JsonUtility.ToJson(result));
                    }, delegate(State state) {
                        log("bind transfer code and relogin fail, " + JsonUtility.ToJson(state));
                    });
                }
            }

            y += buttonHeight + 10;
            if (GUI.Button(new Rect(0, y, buttonWith, buttonHeight), "getItemAsync", btnStyle))
            {
                LongriverSDKUserPayment.instance.getShopItemsAsync(delegate (ShopItemResult r) {
                    log("getShopItemsAsync[success] - ");
                    shopItemResult = r;
                    List<ShopItem> shopItems = r.items;
                    for (int i = 0, len = shopItems.Count; i < len; i++)
                    {
                        ShopItem shopItem = shopItems[i];
                        log($"getShopItemsAsync[{i}] - itemId:{shopItem.itemId}, itemType:{shopItem.itemType}, formattedPrice:{shopItem.formattedPrice}, currency:{shopItem.currency}, doublePrice:{shopItem.doublePrice}, stringPrice:{shopItem.stringPrice}, price:{shopItem.price}, usdPrice:{shopItem.usdPrice}");
                    }
                }, delegate (State s) {

                    log("getShopItemsAsync[failed] - " + JsonUtility.ToJson(s));
                });
            }

            y += buttonHeight + 10;
            if (GUI.Button(new Rect(0, y, buttonWith, buttonHeight), "getItem", btnStyle))
            {
                var result = LongriverSDKUserPayment.instance.getShopItems();
                if (result != null) { 
                    log("getShopItems[success] - ");
                    shopItemResult = result;
                    List<ShopItem> shopItems = result.items;
                    for (int i = 0, len = shopItems.Count; i < len; i++)
                    {
                        ShopItem shopItem = shopItems[i];
                        log($"getShopItems[{i}] - itemId:{shopItem.itemId}, itemType:{shopItem.itemType}, formattedPrice:{shopItem.formattedPrice}, currency:{shopItem.currency}, doublePrice:{shopItem.doublePrice}, stringPrice:{shopItem.stringPrice}, price:{shopItem.price}, usdPrice:{shopItem.usdPrice}");
                    }
                }
                else
                {
                    log("item is null try later ");
                }
            }

            if(shopItemResult!= null && shopItemResult.items !=null)
            {
                foreach(var one in shopItemResult.items)
                {
                    y += buttonHeight + 10;
                    if (GUI.Button(new Rect(0, y, buttonWith, buttonHeight), "buy item "+one.itemId+" "+one.formattedPrice, btnStyle))
                    {
                        string cpOrderId = System.Guid.NewGuid().ToString();
                        Dictionary<string, string> extraInfo = new Dictionary<string, string>() {{"key1", "value1"},};
                        LongriverSDKUserPayment.instance.startPaymentWithEnvIdAndExtraInfo(one.itemId, cpOrderId, "", extraInfo, delegate (StartPaymentResult r) {
                            log("payment is success " + JsonUtility.ToJson(r));
                        }, delegate (State s) {
                            log("payment is fail " + JsonUtility.ToJson(s));
                        });
                    }
                }
            }

            y += buttonHeight + 10;
            if (GUI.Button(new Rect(0, y, buttonWith, buttonHeight), "buy item item_id1", btnStyle))
            {
                LongriverSDKUserPayment.instance.oneStepPay("item_id1", "", delegate (StartPaymentResult r)
                {
                    log("payment is success " + JsonUtility.ToJson(r));

                }, delegate (State s)
                {
                    log("payment is fail " + JsonUtility.ToJson(s));
                });
            }

            y += buttonHeight + 10;
            if (GUI.Button(new Rect(0, y, buttonWith, buttonHeight), "query subscription", btnStyle))
            {
                LongriverSDKUserPayment.instance.querySubscriptionAsync(delegate (SubscriptionData r) {
                    log("querySubscriptionAsync success " + JsonUtility.ToJson(r));

                }, delegate (State s) {
                    log("querySubscriptionAsync  fail " + JsonUtility.ToJson(s));
                });
            }

            y += buttonHeight + 10;
            if (GUI.Button(new Rect(0, y, buttonWith, buttonHeight), "query onetimeitem", btnStyle))
            {
                LongriverSDKUserPayment.instance.queryOneTimeItemAsync(delegate (OneTimeItemList r) {
                    log("queryOneTimeItemAsync success " + JsonUtility.ToJson(r));

                }, delegate (State s) {
                    log("queryOneTimeItemAsync  fail " + JsonUtility.ToJson(s));
                });
            }

            y += buttonHeight + 10;
            if (GUI.Button(new Rect(0, y, buttonWith, buttonHeight), "restore purchase", btnStyle))
            {
                log("sdk start restore");
                LongriverSDKUserPayment.instance.restorePurchases();
            }

            y += buttonHeight + 10;
            if (GUI.Button(new Rect(0, y, buttonWith, buttonHeight), "restore purchase with callback", btnStyle))
            {
                log("start restore with callback");
                LongriverSDKUserPayment.instance.restorePurchasesWithCallback((msg) => {
                    log($"restore success with - {msg}");
                }, (errorMsg) => {
                    log($"restore fail with - {errorMsg}");
                });
            }

            y += buttonHeight + 10;
            if (GUI.Button(new Rect(0, y, buttonWith, buttonHeight), "restore purchase with sync and callback", btnStyle))
            {
                log("start restore with sync and callback");
                LongriverSDKUserPayment.instance.restorePurchasesWithSyncAndCallback(true, (msg) => {
                    log($"restore success with - {msg}");
                }, (errorMsg) => {
                    log($"restore fail with - {errorMsg}");
                });
            }

            y += buttonHeight + 10;
            if (GUI.Button(new Rect(0, y, buttonWith, buttonHeight), "share facebook url", btnStyle))
            {
                log("share facebook url");
                LongriverSDK.instance.shareFacebook("http://www.baidu.com", null, delegate(string r){
                    log("shareFacebook success " );
                }, delegate (State s) {
                    log("shareFacebook  fail " + JsonUtility.ToJson(s));
                });
            }

            y += buttonHeight + 10;
            if (GUI.Button(new Rect(0, y, buttonWith, buttonHeight), "share facebook image", btnStyle))
            {
                //Sprite a = Resources.Load<Sprite>("test");
                //byte[] x = a.texture.EncodeToPNG();
                //Debug.Log("byte " + x.Length);
                //Debug.Log(Convert.ToBase64String(x));
                string imageBase64 = "iVBORw0KGgoAAAANSUhEUgAAACAAAAARCAIAAAAzPjmrAAAE+UlEQVQ4ESWVyW4dxxWG69TcM+9AUaJImYyswaOMLALbCWDAq+wC5HXyDHmPrLMykG1sIEACBPEg2JRE2qJIXvFOfW9PNaduAjQK6Oq/vjrndP2nAP/+z4HgQAEwA4xBUg+IUkoo1RgQIx4HRAAIQTsZjmOUIUJ3ryQgSlBchYECpjiqMI846nmUE08iChiNQ8D/R5D4NeI8+x8OdvMoQhmB3XychN0GsFsXeQ9SlHCYabAhYgBHTQCB7ZiFioSRII21NHBMGP7s/i7Of9yCiyoSZWAxxHgjBlEXcTHGmGWM1ROEAEnqDqT904f5aRG+nqGYikbo7zdD06vRppb1KnN6moZ3DzgNbJc1Je6TA3MyTb9b2xvFO48UQaWEbXDWAw6xbISD84jk2H02Rs+qQQR70i3ucfKHnBqnGg2yby9mmxcv32rnGnBrCO1NjI1ii8PrWj3Lw+/u0y+PyWLQrcMak++3sHDhgbQnE/lj4xJC3rT9Hw/Rs2yguk0yIXAYFOoB1z2qm77ZbE/L8PlvDw8rooyardS676mQWMqwJPlfW/qX5zBKXMqhYLQS8Lw1H5b4031XlvZc47POCy4uzfBBkpfTPco4j3+Vy0WPzmOFpP3NgX0yCVlw7aK+fNvsjflJOaIP0+1RTuayMkAes9ABfmH5ZSBUo7sVkIx8a7BdkmvPPrmD3i9pD/lPEPQAd0BMOahWT7F/RDqsmim2Zk1gr5icnKbHsOnsXkYo+8/fVok8ffqo2L9TCJZNy18hjwlJMBxwGDM/TRBieOaTeILL+CAiwrDPoErZ2/nix/Pl2Zutauv7KSokrUbZ/akvM2U8TpKkKvdoCV3f+ctvf7Dh30lZVodHhWQfPT4aF2nioGTs8X7l8vI9xjHPdg4xHdV+lMgklX1x9+zW/muxEJ63EKYYHzpZ0fH+dDKJrDxHJB7yfGygenmz5kgXtru4eQlGLy9njz5+LKf7GPmLrk5H+Eqh9+75Xx9P8ny0dUVDRW3gm1/mX71W5d39CqwISqbEkPSy9/Vci0wlCiQBmozL2cJjJhBhDcGWIU7oL7eD/e5icmRvxXT9shtYtxb50ZU//bmbpIyJaGJ4fbm8brUBWnFhHTKEDVky5FKUBQhZW/TD1dZhT1+smgAozal2WBESLS7lno4+wHR+czOI5mKgJK9cgX5G8OqqNiF6EawzGYap4AH5VfBVkUou5zjrBjgL7qEPm1Y9n8ckDe2GGiGidVDWC8aPHxweVJNVE42JTAgHZNvpuV0HiSprsk7em+l8o0zcwwDCCeIytixiDKexB2kjWXq9NP88f52AR8F165pKzhjP6o1VKtCMc5k610tJ39RGG3y1CpXIKrEYttelhyfV9TIdn6/yRJSzFnedNTY2SdEHOwRHB0v6ZVNv7e1cDYuC9yVxtGL5MMS6SyuDlFm7VZ4h50wqswFcr6kz3KBy1tSZ6ms135+8OCIV4PF0OlpZphTdqJznKA1u/uqMbBdFt3G6CX0N2ASiKHLrYIPpTZYkMngJWZrkjSV790Z09UbwV6fj5eHY3myRdjUjam1xs2mRu74zQmWAVouHSTQG+MZrvXFEQRFbIwneOhdCMPDoi6eEyvXWImDBoXdPiuN3Ro7TRbdmtkvQbZ77zsVLorRqKblqFTQK8xynUkVM7L2CGeIVQrFCLtY5NncE3mi0aajW9L/yrp7wvsDabwAAAABJRU5ErkJggg==";
                byte[] ba = Convert.FromBase64String(imageBase64);
                log("share facebook image bytes-length:"+ba.Length + "; base64String-length: " + imageBase64.Length);
                LongriverSDK.instance.shareFacebook(null, ba, delegate (string r) {
                    log("shareFacebook success ");
                }, delegate (State s) {
                    log("queryOneTimeItemAsync  fail " + JsonUtility.ToJson(s));
                });
            }

            GUI.EndScrollView();
            totalScrollHeight = y + 100;


            //		//supp
            //		if (showRward) {
            //			if (currentTs > rewardTs) {
            //				showRward = false;
            //			} else {
            //				GUI.Box (new Rect (0, 0, Screen.width, Screen.height, "showing reward"));
            //				if (GUI.Button (new Rect (100, 100, 100, 100), "clickAd")) );
            //					log ("click Ad");
            //					Mopub
            //				}
            //			}
            //		}
            //		//show all buy items

        }

        public void AttationInfoCallback(AttributionInfo info)
        {
            log("get Attribution  " + info.ToJson());
        }
        public void InitCallback(InitSuccessResult result)
        {
            log("sdk init success  " + result.ToJson()+" and test auto login");
            string sdkVersion = LongriverSDK.instance.GetSDKVersion();
            log($"sdk version: {sdkVersion}");
            LongriverSDKUserPayment.instance.autoLoginAsync(true, delegate (AutoLoginResult r) {
                log("autologin success " + JsonUtility.ToJson(r));
                LongriverSDK.instance.UploadRoleLogin("s2", "server2", "n2", "name2");
            }, delegate (State s) {
                log("autologin fail " + JsonUtility.ToJson(s));
            });
        }
        public void AppJumpCallback(Dictionary<string, object> dataDict)
        {
            foreach (var item in dataDict)
            {
                log($"app jump - {item.Key}:{item.Value}");
            }
        }
        public void log(string s)
        {
            UnityEngine.Debug.Log(s);
            logs.Add(s);
        }
        void Update()
        {
            if (Input.touchCount > 0)
            {

                Touch touch = Input.touches[0];
                if (touch.phase == TouchPhase.Moved)
                {
                    if (touch.position.y < Screen.height / 4 * 3)
                    {
                        btnScrollPosition.y += touch.deltaPosition.y;
                    }
                    else
                    {
                        logScrollPosition.x -= touch.deltaPosition.x;
                        logScrollPosition.y += touch.deltaPosition.y;
                    }
                }
            }
        }

    }

}

