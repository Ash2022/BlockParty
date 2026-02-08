using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using Newtonsoft.Json;
namespace LongriverSDKNS
{
#if UNITY_ANDROID
    public class LongriverSDKBridgeAndroid : LongriverSDKBridge
    {
        string className = "com.longriversdk.longriverunitybridge.LongriverUnityBridge";

        private string CallStaticWithString(string methodName, params object[] args)
        {
            AndroidJavaClass jc = new AndroidJavaClass(className);
            return jc.CallStatic<string>(methodName, args);
        }
        private bool CallStaticWithBool(string methodName, params object[] args)
        {
            AndroidJavaClass jc = new AndroidJavaClass(className);
            return jc.CallStatic<bool>(methodName, args);
        }
        private void CallStatic(string methodName, params object[] args)
        {
            AndroidJavaClass jc = new AndroidJavaClass(className);
            jc.CallStatic(methodName, args);
        }

        public void initWithConfig(string fileContent)
        {
            AndroidJavaClass jc = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
            AndroidJavaObject jo = jc.GetStatic<AndroidJavaObject>("currentActivity");
            CallStatic("setActivity", jo);

            CallStatic("init", fileContent);
        }
        public int tryLaunchOrOpenAppStore(string packageName, string className, Dictionary<string, object> dataDict)
        {
            string dataJson = Json.Serialize(dataDict);
            string s = CallStaticWithString("tryLaunchOrOpenAppStore", packageName, className, dataJson);
            int value;
            if (int.TryParse(s, System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out value)) 
            {
                return value;
            } 
            else 
            {
                return -1;
            }
        }

        public void enableSimpleMode() 
        {
            CallStatic("enableSimpleMode");
        }

        public string GetSDKVersion()
        {
            return CallStaticWithString("getSDKVersion");
        }

        public void SetUseVersion1(bool enable) {}

        public void SetUseShopItemFromServer(bool enable)
        {
            CallStatic("setUseShopItemFromServer", enable);
        }

        public void setUmpWithIp(bool enable)
        {
            CallStatic("setUmpWithIp", enable);
        }

        public void onResume()
        {
            CallStatic("onResume");
        }
        public void onPause()
        {
            CallStatic("onPause");
        }
        public void Log(string eventName, Dictionary<string, string> paramMap)
        {
            Dictionary<string, object> sendParams = new Dictionary<string, object>();
            sendParams.Add("eventName", eventName);
            sendParams.Add("params", paramMap);
            string paramJson = Json.Serialize(sendParams);
            CallStatic("log", paramJson);
        }
        public void logThirdLevel(int level)
        {
            CallStatic("logThirdLevel", level);
        }

        public void logThirdEvent(string eventName, Dictionary<string, string> paramMap)
        {
            Dictionary<string, object> sendParams = new Dictionary<string, object>();
            sendParams.Add("eventName", eventName);
            sendParams.Add("params", paramMap);
            string paramJson = Json.Serialize(sendParams);
            CallStatic("logThirdEvent", paramJson);
        }

        public void logThirdEventWithCustomEvent(CustomEvent customEvent)
        {   
            if (null == customEvent) {
                return;
            }
            CallStatic("logThirdEventWithCustomEvent", customEvent.ToJson());
        }

        public void UploadEventToAllPlatform(string eventName, Dictionary<string, object> eventDatas)
        {
            string eventDatasJson = Json.Serialize(eventDatas);
            CallStatic("uploadEventToAllPlatform", eventName, eventDatasJson);
        }

        public void UploadEventToPlus(string eventName, Dictionary<string, object> data)
        {
            // string dataJson = Json.Serialize(data);
            // CallStatic("uploadEventToPlus", eventName, dataJson);
        }

        public StaticInfo GetStaticInfo()
        {
            string s = CallStaticWithString("getStaticInfo");
            return StaticInfo.fromJson(s);
        }

        public string GetUMPParameters()
        {
            return CallStaticWithString("getUMPParameters");
        }

        public bool IsPrivacyOptionsRequired()
        {
            return CallStaticWithBool("isPrivacyOptionsRequired");
        }

        public void ShowPrivacyOptionsForm()
        {
            CallStatic("showPrivacyOptionsForm");
        }

        public void SetDebugGeography(int index)
        {
            CallStatic("setDebugGeography", index);
        }

        public void SetWebAdListener()
        {
            CallStatic("setWebAdListener");
        }
        
        public bool IsWebAdReady() 
        {
            return CallStaticWithBool("isWebAdReady");
        }

        public void ShowUrl() 
        {
            CallStatic("showUrl");
        }

        public void ShowAppOpenWithTimeout(float timeout)
        {
            CallStatic("showAppOpenWithTimeout", timeout);
        }

        public bool HasInterstitial(string adEntry)
        {
            return CallStaticWithBool("hasInterstitial", adEntry);
        }

        public void ShowInterstitial(string adEntry)
        {
            CallStatic("showInterstitial", adEntry);
        }

        public bool HasReward(string adEntry)
        {
            return CallStaticWithBool("hasReward", adEntry);
        }

        public void ShowReward(string adEntry)
        {
            CallStatic("showReward", adEntry);
        }
        public void ShowOrReShowBanner(BannerPos pos)
        {
            CallStatic("showOrReShowBanner", (int)pos);
        }
        public void HideBanner()
        {
            CallStatic("hideBanner");
        }

        public void RemoveBanner()
        {
            CallStatic("removeBanner");
        }

        public void SetBannerBackgroundColor(float red, float green, float blue, float alpha)
        {
            CallStatic("setBannerBackgroundColor", red, green, blue, alpha);
        }

        public Dictionary<string, string> GetLoadingStatusSummary()
        {
            string s = CallStaticWithString("getLoadingStatusSummary");
            Dictionary<string, object> m = (Dictionary<string, object>)Json.Deserialize(s);
            Dictionary<string, string> re = new Dictionary<string, string>();
            foreach (var entry in m)
            {
                re.Add(entry.Key, (string)entry.Value);
            }
            return re;
        }
        public bool isLogin()
        {
            return CallStaticWithBool("isLogin");
        }
        public long getGameAccountId()
        {
            string s = CallStaticWithString("getGameAccountId");
            return long.Parse(s);
        }
        public string getSessionToken()
        {
            return CallStaticWithString("getSessionToken");
        }

        public void Logout()
        {
            CallStatic("logout");
        }

        public void SignOut()
        {
            CallStatic("signOut");
        }

        public void UploadRoleLogin(string serverId, string serverName, string roleId, string roleName)
        {
            CallStatic("uploadRoleLogin", serverId, serverName, roleId, roleName);
        }

        public void autoLoginAsync(bool platformLogin)
        {
            CallStatic("autoLoginAsync", platformLogin);
        }
        public void checkLoginAsync()
        {
            CallStatic("checkLoginAsync");
        }

        public void loginWithTypeAsync(LOGIN_TYPE loginType)
        {
            CallStatic("loginWithTypeAsync", loginType.ToString());
        }

        public void bindWithTypeAsync(LOGIN_TYPE loginType)
        {
            CallStatic("bindWithTypeAsync", loginType.ToString());
        }

        public void unbindWithTypeAsync(LOGIN_TYPE loginType)
        {
            CallStatic("unbindWithTypeAsync", loginType.ToString());
        }

        public void getUserInfoAsync()
        {
            CallStatic("getUserInfoAsync");
        }

        public void getBindTransferCode()
        {
            CallStatic("getBindTransferCode");
        }

        public void generateTransferCode()
        {
            CallStatic("generateTransferCode");
        }

        public void bindTransferCode(string transferCode)
        {
            CallStatic("bindTransferCode", transferCode);
        }

        public void bindTransferCodeAndReLogin(string transferCode)
        {
            CallStatic("bindTransferCodeAndReLogin", transferCode);
        } 

        public ShopItemResult getShopItems()
        {
            string s = CallStaticWithString("getShopItems");
            if (s != null && s.Length > 0)
            {
                return JsonUtility.FromJson<ShopItemResult>(s);
            }
            else return null;
        }

        public void getShopItemsAsync()
        {
            CallStatic("getShopItemsAsync");
        }

        public void startPayment(string itemId, string cpOrderId)
        {
            CallStatic("startPayment", itemId, cpOrderId);
        }

        public void startPayment(string itemId, string cpOrderId, Dictionary<string, string> extraInfo)
        {
            extraInfo = extraInfo ?? new Dictionary<string, string>();
            string jsonString = Json.Serialize(extraInfo);
            CallStatic("startPayment", itemId, cpOrderId, jsonString);
        }

        public void oneStepPay(string itemId, string cpOrderId)
        {
            CallStatic("oneStepPay", itemId, cpOrderId);
        }

        // public void startPaymentForSimpleGame(string itemId)
        // {
        //     CallStatic("startPaymentForSimpleGame", itemId);
        // }

        public void consumeItem(long gameOrderId)
        {
            CallStatic("consumeItem", gameOrderId.ToString());
        }
        public void querySubscriptionAsync()
        {
            CallStatic("querySubscriptionAsync");
        }
        public void queryOneTimeItemAsync()
        {
            CallStatic("queryOneTimeItemAsync");
        }
        public void restorePurchases()
        {
            Debug.Log("android no need to restore");
        }
        public void restorePurchasesWithCallback()
        {
            Debug.Log("android no need to restore");
        }
        public void restorePurchasesWithSyncAndCallback(bool syncEnabled)
        {
            Debug.Log("android no need to restore");
        }
        public void shareFacebookAsync(string url, byte[] image)
        {
            //string imageBase64 = "";
            //if (image != null)
            //{
            //    imageBase64 = Convert.ToBase64String(image);
            //}
            CallStatic("shareFacebookAsync", url, image);
        }
    }
#endif
}


