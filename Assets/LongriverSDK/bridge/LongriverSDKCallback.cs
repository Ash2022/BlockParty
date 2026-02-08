using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
namespace LongriverSDKNS
{
    [DoNotRename]
    public class LongriverSDKCallback : MonoBehaviour
    {
        static public LongriverSDKCallback instance = null;
        public void Awake()
        {
            instance = this;
        }
        public void sdkInitSuccess(string str)
        {
            UnityMainThreadDispatcher.Instance().Enqueue(() => {
                Debug.Log("LongriverSDKCallback sdkInitSuccess " + str);
                InitSuccessResult info = InitSuccessResult.FromJson(str);
                LongriverSDK.instance.initSuccess(info);
            });
        }

        public void onAppJumpCallback(string str)
        {
            UnityMainThreadDispatcher.Instance().Enqueue(() => {
                Debug.Log("LongriverSDKCallback onAppJumpCallback " + str);
                Dictionary<string, object> dataDict = Json.Deserialize(str) as Dictionary<string, object>;
                LongriverSDK.instance.appJumpCallback(dataDict);
            });
        }

        public void setAttributionInfo(string str)
        {
            UnityMainThreadDispatcher.Instance().Enqueue(() => {
                Debug.Log("LongriverSDKCallback setAttributionInfo " + str);
                if (!string.IsNullOrWhiteSpace(str)) 
                {
                    PlayerPrefs.SetString("longriver_unity_attr_on", str);
                    PlayerPrefs.Save();
                }
                AttributionInfo info = AttributionInfo.FromJson(str);
                AttributionHelper.GetInstance().SetAttributionInfo(info);
            });
        }

        public void onShowPrivacyOptionsForm(string s)
        {
            UnityMainThreadDispatcher.Instance().Enqueue(() => {
                Debug.Log($"LongriverSDKCallback onShowPrivacyOptionsForm {s}");
                var r = JsonUtility.FromJson<State>(s);
                LongriverSDK.instance.CallbackShowPrivacyOptionsForm(r);
            });
        }

        //private CallbackInfo GetCallbackInfoFromDict(Dictionary<string, object> dict)
        //{
        //    if (dict.ContainsKey("callbackInfo"))
        //    {
        //        return JsonCallbackInfo.fromDict((Dictionary<string, object>)dict["callbackInfo"]);
        //    }
        //    else return null;
           
        //}
        //********************* ad **********************
        public void onWebAdLoaded()
        {
            Debug.Log("LongriverSDKCallback onWebAdLoaded - ");
            LongriverSDKWebAdListener listener = LongriverSDKAd.instance.getLongriverSDKWebAdListener();
            if (listener != null)
            {
                listener.onWebAdLoaded();
            }
        }
        public void onWebAdLoadFailed(string s)
        {
            Debug.Log($"LongriverSDKCallback onWebAdLoadFailed - {s}");
            LongriverSDKWebAdListener listener = LongriverSDKAd.instance.getLongriverSDKWebAdListener();
            if (listener != null)
            {
                listener.onWebAdLoadFailed(s);
            }
        }
        public void onWebAdClose()
        {
            Debug.Log("LongriverSDKCallback onWebAdClose - ");
            LongriverSDKWebAdListener listener = LongriverSDKAd.instance.getLongriverSDKWebAdListener();
            if (listener != null)
            {
                listener.onWebAdClose();
            }
        }
        public void onWebAdPlayStart()
        {
            Debug.Log("LongriverSDKCallback onWebAdPlayStart - ");
            LongriverSDKWebAdListener listener = LongriverSDKAd.instance.getLongriverSDKWebAdListener();
            if (listener != null)
            {
                listener.onWebAdPlayStart();
            }
        }
        public void onWebAdPlayFailed(string s)
        {
            Debug.Log($"LongriverSDKCallback onWebAdPlayFailed - {s}");
            LongriverSDKWebAdListener listener = LongriverSDKAd.instance.getLongriverSDKWebAdListener();
            if (listener != null)
            {
                listener.onWebAdPlayFailed(s);
            }
        }
        public void onWebAdReward(string s)
        {
            Debug.Log($"LongriverSDKCallback onWebAdReward - {s}");
            LongriverSDKWebAdListener listener = LongriverSDKAd.instance.getLongriverSDKWebAdListener();
            if (listener != null)
            {
                listener.onWebAdReward(s);
            }
        }

        public void onAdActive(string msg)
        {
            Debug.LogFormat("LongriverSDKCallback onAdActive ----- {0};", msg);
            System.Action<string> action = LongriverSDKAd.instance.GetAdvertisingActiveListenter();
            if (null != action)
            {
                action(msg);
            }
        }

        public void onAdImpress(string str)
        {
            Debug.Log("LongriverSDKCallback onAdImpress " + str);
            SimpleAdCallbackInfo info = JsonUtility.FromJson<SimpleAdCallbackInfo>(str);
            //Dictionary<string,object> dict =  (Dictionary<string, object>)Json.Deserialize(str);
            //string unitId = (string)dict["unitId"];
            //CallbackInfo info = GetCallbackInfoFromDict(dict);
            LongriverSDKBannerAdListener listener = LongriverSDKAd.instance.GetLongriverSDKBannerAdListener();
            if(listener!= null)
            {
                listener.onAdImpress(info.unitId, info.callbackInfo);
            }
        }

        public void onAdClick(string str)
        {
            Debug.Log("LongriverSDKCallback onAdClick " + str);
            //Dictionary<string, object> dict = (Dictionary<string, object>)Json.Deserialize(str);
            //string unitId = (string)dict["unitId"];
            //CallbackInfo info = GetCallbackInfoFromDict(dict);
            SimpleAdCallbackInfo info = JsonUtility.FromJson<SimpleAdCallbackInfo>(str);
            LongriverSDKBannerAdListener listener = LongriverSDKAd.instance.GetLongriverSDKBannerAdListener();
            if (listener != null)
            {
                listener.onAdClick(info.unitId, info.callbackInfo);
            }
        }

        public void onAdLoaded(string str)
        {
            Debug.Log("LongriverSDKCallback onAdLoaded " + str);
            SimpleAdCallbackInfo info = JsonUtility.FromJson<SimpleAdCallbackInfo>(str);
            LongriverSDKBannerAdListener listener = LongriverSDKAd.instance.GetLongriverSDKBannerAdListener();
            if (listener != null)
            {
                listener.onAdLoaded(info.unitId, info.callbackInfo);
            }
        }

        public void onAdLoadFailed(string str)
        {
            Debug.Log("LongriverSDKCallback onAdLoadFailed " + str);
            LongriverAdFailedInfo info = JsonUtility.FromJson<LongriverAdFailedInfo>(str);
            LongriverSDKBannerAdListener listener = LongriverSDKAd.instance.GetLongriverSDKBannerAdListener();
            if (listener != null)
            {
                listener.onAdLoadFailed(info.unitId, info.errorCode, info.errorMsg);
            }
        }


        public void onAppOpenAdTimeout(string msg)
        {
            Debug.Log("LongriverSDKCallback onAppOpenAdTimeout " + msg);
            SimpleAdCallbackInfo info = JsonUtility.FromJson<SimpleAdCallbackInfo>(msg);
            LongriverSDKAppOpenAdListener listener = LongriverSDKAd.instance.GetLongriverSDKAppOpenAdListener();
            if (listener != null)
            {
                listener.onAppOpenAdTimeout(info.unitId);
            }
        }

        public void onAppOpenAdShow(string msg)
        {
            Debug.Log("LongriverSDKCallback onAppOpenAdShow " + msg);
            SimpleAdCallbackInfo info = JsonUtility.FromJson<SimpleAdCallbackInfo>(msg);
            LongriverSDKAppOpenAdListener listener = LongriverSDKAd.instance.GetLongriverSDKAppOpenAdListener();
            if (listener != null)
            {
                listener.onAppOpenAdShow(info.unitId, info.callbackInfo);
            }
        }

        public void onAppOpenAdClose(string msg)
        {
            Debug.Log("LongriverSDKCallback onAppOpenAdClose " + msg);
            SimpleAdCallbackInfo info = JsonUtility.FromJson<SimpleAdCallbackInfo>(msg);
            LongriverSDKAppOpenAdListener listener = LongriverSDKAd.instance.GetLongriverSDKAppOpenAdListener();
            if (listener != null)
            {
                listener.onAppOpenAdClose(info.unitId, info.callbackInfo);
            }
        }

        public void onAppOpenAdClick(string msg)
        {
            Debug.Log("LongriverSDKCallback onAppOpenAdClick " + msg);
            SimpleAdCallbackInfo info = JsonUtility.FromJson<SimpleAdCallbackInfo>(msg);
            LongriverSDKAppOpenAdListener listener = LongriverSDKAd.instance.GetLongriverSDKAppOpenAdListener();
            if (listener != null)
            {
                listener.onAppOpenAdClick(info.unitId, info.callbackInfo);
            }
        }

        public void onAppOpenAdPlayFail(string msg)
        {
            Debug.Log("LongriverSDKCallback onAppOpenAdPlayFail " + msg);
        }

        public void onAppOpenAdLoaded(string msg)
        {
            Debug.Log("LongriverSDKCallback onAppOpenAdLoaded " + msg);
            SimpleAdCallbackInfo info = JsonUtility.FromJson<SimpleAdCallbackInfo>(msg);
            LongriverSDKAppOpenAdListener listener = LongriverSDKAd.instance.GetLongriverSDKAppOpenAdListener();
            if (listener != null)
            {
                listener.onAppOpenAdLoaded(info.unitId, info.callbackInfo);
            }

        }

        public void onAppOpenAdLoadFailed(string str) 
        {
            Debug.Log("LongriverSDKCallback onAppOpenAdLoadFailed " + str);
            LongriverAdFailedInfo info = JsonUtility.FromJson<LongriverAdFailedInfo>(str);
            LongriverSDKAppOpenAdListener listener = LongriverSDKAd.instance.GetLongriverSDKAppOpenAdListener();
            if (listener != null)
            {
                listener.onAppOpenAdLoadFailed(info.unitId, info.errorCode, info.errorMsg);
            }
        }

        public void onInterstitialAdShow(string str)
        {
            Debug.Log("LongriverSDKCallback onInterstitialAdShow " + str);
            //Dictionary<string, object> dict = (Dictionary<string, object>)Json.Deserialize(str);
            //string unitId = (string)dict["unitId"];
            //CallbackInfo info = GetCallbackInfoFromDict(dict);
            SimpleAdCallbackInfo info = JsonUtility.FromJson<SimpleAdCallbackInfo>(str);
            LongriverSDKInterstitialAdListener listener = LongriverSDKAd.instance.GetLongriverSDKInterstitialAdListener();
            if (listener != null)
            {
                listener.onInterstitialAdShow(info.unitId, info.callbackInfo);
            }
        }
         
        public void onInterstitialAdClose(string str)
        {
            Debug.Log("LongriverSDKCallback onInterstitialAdClose " + str);
            //Dictionary<string, object> dict = (Dictionary<string, object>)Json.Deserialize(str);
            //string unitId = (string)dict["unitId"];
            //CallbackInfo info = GetCallbackInfoFromDict(dict);
            SimpleAdCallbackInfo info = JsonUtility.FromJson<SimpleAdCallbackInfo>(str);
            LongriverSDKInterstitialAdListener listener = LongriverSDKAd.instance.GetLongriverSDKInterstitialAdListener();
            if (listener != null)
            {
                listener.onInterstitialAdClose(info.unitId, info.callbackInfo);
            }
        }
        public void onInterstitialAdClick(string str)
        {
            Debug.Log("LongriverSDKCallback onInterstitialAdClick " + str);
            //Dictionary<string, object> dict = (Dictionary<string, object>)Json.Deserialize(str);
            //string unitId = (string)dict["unitId"];
            //CallbackInfo info = GetCallbackInfoFromDict(dict);
            SimpleAdCallbackInfo info = JsonUtility.FromJson<SimpleAdCallbackInfo>(str);
            LongriverSDKInterstitialAdListener listener = LongriverSDKAd.instance.GetLongriverSDKInterstitialAdListener();
            if (listener != null)
            {
                listener.onInterstitialAdClick(info.unitId,info.callbackInfo);
            }
        }

        public void onInterstitialAdLoaded(string str) 
        {
            Debug.Log("LongriverSDKCallback onInterstitialAdLoaded " + str);
            SimpleAdCallbackInfo info = JsonUtility.FromJson<SimpleAdCallbackInfo>(str);
            LongriverSDKInterstitialAdListener listener = LongriverSDKAd.instance.GetLongriverSDKInterstitialAdListener();
            if (listener != null)
            {
                listener.onInterstitialAdLoaded(info.unitId,info.callbackInfo);
            }
        }

        public void onInterstitialAdLoadFailed(string str)
        {
            Debug.Log("LongriverSDKCallback onInterstitialAdLoadFailed " + str);
            LongriverAdFailedInfo info = JsonUtility.FromJson<LongriverAdFailedInfo>(str);
            LongriverSDKInterstitialAdListener listener = LongriverSDKAd.instance.GetLongriverSDKInterstitialAdListener();
            if (listener != null)
            {
                listener.onInterstitialAdLoadFailed(info.unitId, info.errorCode, info.errorMsg);
            }
        }

        public void onInterstitialAdPlayFail(string str)
        {
            Debug.Log("LongriverSDKCallback onInterstitialAdPlayFail " + str);
            LongriverAdFailedInfo info = JsonUtility.FromJson<LongriverAdFailedInfo>(str);
            LongriverSDKInterstitialAdListener listener = LongriverSDKAd.instance.GetLongriverSDKInterstitialAdListener();
            if (listener != null)
            {
                listener.onInterstitialAdPlayFail(info.unitId, info.errorCode, info.errorMsg);
            }
        }

        public void onRewardedVideoAdPlayStart(string str)
        {
            Debug.Log("LongriverSDKCallback onRewardedVideoAdPlayStart " + str);
            //Dictionary<string, object> dict = (Dictionary<string, object>)Json.Deserialize(str);
            //string unitId = (string)dict["unitId"];
            //CallbackInfo info = GetCallbackInfoFromDict(dict);
            SimpleAdCallbackInfo info = JsonUtility.FromJson<SimpleAdCallbackInfo>(str);
            LongriverSDKRewardedVideoListener listener = LongriverSDKAd.instance.GetLongriverSDKRewardedVideoListener();
            if (listener != null)
            {
                listener.onRewardedVideoAdPlayStart(info.unitId, info.callbackInfo);
            }
        }

        public void onRewardedVideoAdPlayFail(string str)
        {
            Debug.Log("LongriverSDKCallback onRewardedVideoAdPlayFail " + str);

            //Dictionary<string, object> dict = (Dictionary<string, object>)Json.Deserialize(str);
            //string unitId = (string)dict["unitId"];
            //string code = (string)dict["code"];
            //string message = (string)dict["message"];
            SimpleAdCallbackFailInfo info = JsonUtility.FromJson<SimpleAdCallbackFailInfo>(str);
            LongriverSDKRewardedVideoListener listener = LongriverSDKAd.instance.GetLongriverSDKRewardedVideoListener();
            if (listener != null)
            {
                listener.onRewardedVideoAdPlayFail(info.unitId,info.code,info.message);
            }
        }
        public void onRewardedVideoAdPlayClosed(string str)
        {
            Debug.Log("LongriverSDKCallback onRewardedVideoAdPlayClosed " + str);
            //Dictionary<string, object> dict = (Dictionary<string, object>)Json.Deserialize(str);
            //string unitId = (string)dict["unitId"];
            //CallbackInfo info = GetCallbackInfoFromDict(dict);
            SimpleAdCallbackInfo info = JsonUtility.FromJson<SimpleAdCallbackInfo>(str);
            //SimpleAdCallbackInfo info = JsonUtility.FromJson<SimpleAdCallbackInfo>(str);
            LongriverSDKRewardedVideoListener listener = LongriverSDKAd.instance.GetLongriverSDKRewardedVideoListener();
            if (listener != null)
            {
                listener.onRewardedVideoAdPlayClosed(info.unitId, info.callbackInfo);
            }
        }
        public void onRewardedVideoAdPlayClicked(string str)
        {
            Debug.Log("LongriverSDKCallback onRewardedVideoAdPlayClicked "+str);
            //Dictionary<string, object> dict = (Dictionary<string, object>)Json.Deserialize(str);
            //string unitId = (string)dict["unitId"];
            //CallbackInfo info = GetCallbackInfoFromDict(dict);
            SimpleAdCallbackInfo info = JsonUtility.FromJson<SimpleAdCallbackInfo>(str);
            LongriverSDKRewardedVideoListener listener = LongriverSDKAd.instance.GetLongriverSDKRewardedVideoListener();
            if (listener != null)
            {
                listener.onRewardedVideoAdPlayClicked(info.unitId, info.callbackInfo);
            }
        }

        public void onRewardedVideoAdLoaded(string str)
        {
            Debug.Log("LongriverSDKCallback onRewardedVideoAdLoaded "+str);
            SimpleAdCallbackInfo info = JsonUtility.FromJson<SimpleAdCallbackInfo>(str);
            LongriverSDKRewardedVideoListener listener = LongriverSDKAd.instance.GetLongriverSDKRewardedVideoListener();
            if (listener != null)
            {
                listener.onRewardedVideoAdLoaded(info.unitId, info.callbackInfo);
            }
        }

        public void onRewardedVideoAdLoadFailed(string str)
        {
            Debug.Log("LongriverSDKCallback onRewardedVideoAdLoadFailed "+str);
            LongriverAdFailedInfo info = JsonUtility.FromJson<LongriverAdFailedInfo>(str);
            LongriverSDKRewardedVideoListener listener = LongriverSDKAd.instance.GetLongriverSDKRewardedVideoListener();
            if (listener != null)
            {
                listener.onRewardedVideoAdLoadFailed(info.unitId, info.errorCode, info.errorMsg);
            }
        }

        /***
         * 发放奖励
         */
        public void onReward(string str)
        {
            Debug.Log("LongriverSDKCallback onReward " + str);
            SimpleAdCallbackInfo info = JsonUtility.FromJson<SimpleAdCallbackInfo>(str);
            //Dictionary<string, object> dict = (Dictionary<string, object>)Json.Deserialize(str);
            //string unitId = (string)dict["unitId"];
            //CallbackInfo info = GetCallbackInfoFromDict(dict);
            LongriverSDKRewardedVideoListener listener = LongriverSDKAd.instance.GetLongriverSDKRewardedVideoListener();
            if (listener != null)
            {
                listener.onReward(info.unitId, info.callbackInfo);
            }
        }

        //********************* pay **********************
        public void onSignOutSuccess(string s)
        {
            UnityMainThreadDispatcher.Instance().Enqueue(() => {
                var r = JsonUtility.FromJson<SignOutResult>(s);
                if (null != LongriverSDKUserPayment.instance && null != LongriverSDKUserPayment.instance.signOutSuccess) 
                {
                    LongriverSDKUserPayment.instance.signOutSuccess(r);
                }
            });
        }

        public void onSignOutFailed(string s)
        {
            UnityMainThreadDispatcher.Instance().Enqueue(() => {
                var r = JsonUtility.FromJson<State>(s);
                if (null != LongriverSDKUserPayment.instance && null != LongriverSDKUserPayment.instance.signOutFailed) 
                {
                    LongriverSDKUserPayment.instance.signOutFailed(r);
                }
            });
        }

        public void autoLoginSuccess(string s)
        {
            UnityMainThreadDispatcher.Instance().Enqueue(() => {
                var r = JsonUtility.FromJson<AutoLoginResult>(s);
                LongriverSDKUserPayment.instance.autoLoginAsyncSuccess(r);
            });
        }

        public void autoLoginFail(string s)
        {
            UnityMainThreadDispatcher.Instance().Enqueue(() => {
                var r = JsonUtility.FromJson<State>(s);
                LongriverSDKUserPayment.instance.autoLoginAsyncFail(r);
            });
        }

        public void checkLoginSuccess(string s)
        {
            UnityMainThreadDispatcher.Instance().Enqueue(() => {
                var r = JsonUtility.FromJson<CheckLoginResult>(s);
                LongriverSDKUserPayment.instance.checkLoginAsyncSuccess(r);
            });
        }

        public void checkLoginFail(string s)
        {
            UnityMainThreadDispatcher.Instance().Enqueue(() => {
                var r = JsonUtility.FromJson<State>(s);
                LongriverSDKUserPayment.instance.checkLoginAsyncFail(r);
            });
        }

        public void loginWithTypeAsyncSuccess(string s)
        {
            UnityMainThreadDispatcher.Instance().Enqueue(() => {
                var r = JsonUtility.FromJson<LoginResult>(s);
                LongriverSDKUserPayment.instance.loginWithTypeAsyncSuccess(r);
            });
        }

        public void loginWithTypeAsyncFail(string s)
        {
            UnityMainThreadDispatcher.Instance().Enqueue(() => {
                var r = JsonUtility.FromJson<State>(s);
                LongriverSDKUserPayment.instance.loginWithTypeAsyncFail(r);
            });
        }

        public void bindWithTypeAsyncSuccess(string s)
        {
            UnityMainThreadDispatcher.Instance().Enqueue(() => {
                var r = JsonUtility.FromJson<UserInfoResult>(s);
                LongriverSDKUserPayment.instance.bindWithTypeAsyncSuccess(r);
            });
        }

        public void bindWithTypeAsyncFail(string s)
        {
            UnityMainThreadDispatcher.Instance().Enqueue(() => {
                var r = JsonUtility.FromJson<State>(s);
                LongriverSDKUserPayment.instance.bindWithTypeAsyncFail(r);
            });
        }

        public void unbindWithTypeAsyncSuccess(string s)
        {
            UnityMainThreadDispatcher.Instance().Enqueue(() => {
                var r = JsonUtility.FromJson<UserInfoResult>(s);
                LongriverSDKUserPayment.instance.unbindWithTypeAsyncSuccess(r);
            });
        }

        public void unbindWithTypeAsyncFail(string s)
        {
            UnityMainThreadDispatcher.Instance().Enqueue(() => {
                var r = JsonUtility.FromJson<State>(s);
                LongriverSDKUserPayment.instance.unbindWithTypeAsyncFail(r);
            });
        }

        public void getUserInfoAsyncSuccess(string s)
        {
            UnityMainThreadDispatcher.Instance().Enqueue(() => {
                var r = JsonUtility.FromJson<UserInfoResult>(s);
                LongriverSDKUserPayment.instance.getUserInfoAsyncSuccess(r);
            });
        }

        public void getUserInfoAsyncFail(string s)
        {
            UnityMainThreadDispatcher.Instance().Enqueue(() => {
                var r = JsonUtility.FromJson<State>(s);
                LongriverSDKUserPayment.instance.getUserInfoAsyncFail(r);
            });
        }

        public void getBindTransferCodeSuccess(string s)
        {
            UnityMainThreadDispatcher.Instance().Enqueue(() => {
                var r = JsonUtility.FromJson<GetBindTransferCodeResult>(s);
                if (null != LongriverSDKUserPayment.instance.getBindTransferCodeSuccess) 
                {
                    LongriverSDKUserPayment.instance.getBindTransferCodeSuccess(r);
                }
            });
        }

        public void getBindTransferCodeFail(string s)
        {
            UnityMainThreadDispatcher.Instance().Enqueue(() => {
                var r = JsonUtility.FromJson<State>(s);
                if (null != LongriverSDKUserPayment.instance.getBindTransferCodeFail) 
                {
                    LongriverSDKUserPayment.instance.getBindTransferCodeFail(r);
                }
            });
        }

        public void generateTransferCodeSuccess(string s)
        {
            UnityMainThreadDispatcher.Instance().Enqueue(() => {
                var r = JsonUtility.FromJson<GenerateTransferCodeResult>(s);
                if (null != LongriverSDKUserPayment.instance.generateTransferCodeSuccess) 
                {
                    LongriverSDKUserPayment.instance.generateTransferCodeSuccess(r);
                }
            });
        }

        public void generateTransferCodeFail(string s)
        {
            UnityMainThreadDispatcher.Instance().Enqueue(() => {
                var r = JsonUtility.FromJson<State>(s);
                if (null != LongriverSDKUserPayment.instance.generateTransferCodeFail) 
                {
                    LongriverSDKUserPayment.instance.generateTransferCodeFail(r);
                }
            });
        }

        public void bindTransferCodeSuccess(string s)
        {
            UnityMainThreadDispatcher.Instance().Enqueue(() => {
                var r = JsonUtility.FromJson<BindTransferCodeResult>(s);
                if (null != LongriverSDKUserPayment.instance.bindTransferCodeSuccess) 
                {
                    LongriverSDKUserPayment.instance.bindTransferCodeSuccess(r);
                }
            });
        }

        public void bindTransferCodeFail(string s)
        {
            UnityMainThreadDispatcher.Instance().Enqueue(() => {
                var r = JsonUtility.FromJson<State>(s);
                if (null != LongriverSDKUserPayment.instance.bindTransferCodeFail) 
                {
                    LongriverSDKUserPayment.instance.bindTransferCodeFail(r);
                }
            });
        }

        public void bindTransferCodeAndReLoginSuccess(string s)
        {
            UnityMainThreadDispatcher.Instance().Enqueue(() => {
                var r = JsonUtility.FromJson<LoginResult>(s);
                LongriverSDKUserPayment.instance.bindTransferCodeAndReLoginSuccess?.Invoke(r);
            });
        }

        public void bindTransferCodeAndReLoginFail(string s)
        {
            UnityMainThreadDispatcher.Instance().Enqueue(() => {
                var r = JsonUtility.FromJson<State>(s);
                LongriverSDKUserPayment.instance.bindTransferCodeAndReLoginFail?.Invoke(r);
            });
        }

        public void getShopItemsAsyncSuccess(string s)
        {
            UnityMainThreadDispatcher.Instance().Enqueue(() => {
                var r = JsonUtility.FromJson<ShopItemResult>(s);
                var c = LongriverSDKUserPayment.instance.getShopItemsAsyncSuccess;
                if (c != null)
                {
                    c(r);
                }
            });
        }

        public void getShopItemsAsyncFail(string s)
        {
            UnityMainThreadDispatcher.Instance().Enqueue(() => {
                var r = JsonUtility.FromJson<State>(s);
                LongriverSDKUserPayment.instance.getShopItemsAsyncFail(r);
            });
        }

        public void startPaymentSuccess(string s)
        {
            UnityMainThreadDispatcher.Instance().Enqueue(() => {
                var r = JsonUtility.FromJson<StartPaymentResult>(s);
                LongriverSDKUserPayment.instance.startPaymentSuccess(r);
            });
        }

        public void startPaymentFail(string s)
        {
            UnityMainThreadDispatcher.Instance().Enqueue(() => {
                var r = JsonUtility.FromJson<State>(s);
                LongriverSDKUserPayment.instance.startPaymentFail(r);
            });
        }
        public void oneStepPaySuccess(string s)
        {
            UnityMainThreadDispatcher.Instance().Enqueue(() => {
                var r = JsonUtility.FromJson<StartPaymentResult>(s);
                LongriverSDKUserPayment.instance.oneStepPaySuccess(r);
            });
        }

        public void oneStepPayFail(string s)
        {
            UnityMainThreadDispatcher.Instance().Enqueue(() => {
                var r = JsonUtility.FromJson<State>(s);
                LongriverSDKUserPayment.instance.oneStepPayFail(r);
            });
        }

        public void onTransactionStatus(string s)
        {
            UnityMainThreadDispatcher.Instance().Enqueue(() => {
                var r = JsonUtility.FromJson<State>(s);
                if (null != LongriverSDKUserPayment.instance 
                && null != LongriverSDKUserPayment.instance.onTransactionStatus) {
                    LongriverSDKUserPayment.instance.onTransactionStatus(r);
                }
            });
        }

        public void getPurchaseItems(string s)
        {
            UnityMainThreadDispatcher.Instance().Enqueue(() => {
                var r = JsonUtility.FromJson<PurchaseItems>(s);
                var c = LongriverSDKUserPayment.instance.listener;
                if (c != null)
                {
                    c.getPurchaseItems(r);
                }
            });
        }

        public void onRestoreApplePurchasesSuccess(string s)
        {
            UnityMainThreadDispatcher.Instance().Enqueue(() => {
                var c = LongriverSDKUserPayment.instance.onRestoreApplePurchasesSuccess;
                if (null != c)
                {
                    c.Invoke(s);
                }
            });
        }

        public void onRestoreApplePurchasesFail(string s)
        {
            UnityMainThreadDispatcher.Instance().Enqueue(() => {
                var c = LongriverSDKUserPayment.instance.onRestoreApplePurchasesFail;
                if (null != c)
                {
                    c.Invoke(s);
                }
            });
        }

        public void querySubscriptionAsyncSuccess(string s)
        {
            UnityMainThreadDispatcher.Instance().Enqueue(() => {
                var r = JsonUtility.FromJson<SubscriptionData>(s);
                LongriverSDKUserPayment.instance.querySubscriptionAsyncSuccess(r);
            });
        }

        public void querySubscriptionAsyncFail(string s)
        {
            UnityMainThreadDispatcher.Instance().Enqueue(() => {
                var r = JsonUtility.FromJson<State>(s);
                LongriverSDKUserPayment.instance.querySubscriptionAsyncFail(r);
            });
        }

        public void getSubscriptionItems(string s)
        {
            UnityMainThreadDispatcher.Instance().Enqueue(() => {
                var r = JsonUtility.FromJson<SubscriptionData>(s);
                var c = LongriverSDKUserPayment.instance.listener;
                if (c != null)
                {
                    c.getSubscriptionItems(r);
                }
            });
        }

        public void queryOneTimeItemAsyncSuccess(string s)
        {
            UnityMainThreadDispatcher.Instance().Enqueue(() => {
                var r = JsonUtility.FromJson<OneTimeItemList>(s);
                LongriverSDKUserPayment.instance.queryOneTimeItemAsyncSuccess(r);
            });
        }

        public void queryOneTimeItemAsyncFail(string s)
        {
            UnityMainThreadDispatcher.Instance().Enqueue(() => {
                var r = JsonUtility.FromJson<State>(s);
                LongriverSDKUserPayment.instance.queryOneTimeItemAsyncFail(r);
            });
        }

        public void getOneTimeItems(string s)
        {
            UnityMainThreadDispatcher.Instance().Enqueue(() => {
                var r = JsonUtility.FromJson<OneTimeItemList>(s);
                var c = LongriverSDKUserPayment.instance.listener;
                if (c != null)
                {
                    c.getOneTimeItems(r);
                }
            });
        }

        //onlineConfig
        // public void onlineConfigComplete(string s)
        // {
        //     //Debug.Log("debug " + s);
        //     var r = JsonUtility.FromJson<OnlineConfigResult>(s);
        //     LongriverSDK.instance.fetchComplete(r);
        // }

         public void onlineConfigComplete(string s)
        {
                Debug.Log("debug " + s);
                OnlineConfigResult result = JsonUtility.FromJson<OnlineConfigResult>(s);
                LongriverSDK.instance.fetchComplete(result);
        }

        //share
        public void shareFacebookAsyncSuccess(string s)
        {
            UnityMainThreadDispatcher.Instance().Enqueue(() => {
                LongriverSDK.instance.shareFacebookAsyncSuccess();
            });
        }

        public void shareFacebookAsyncFail(string s)
        {
            UnityMainThreadDispatcher.Instance().Enqueue(() => {
                var r = JsonUtility.FromJson<State>(s);
                LongriverSDK.instance.shareFacebookAsyncFail(r);
            });
        }
    }
}

