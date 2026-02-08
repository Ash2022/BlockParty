using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.InteropServices;

namespace LongriverSDKNS
{
#if UNITY_IOS
    public class LongriverSDKBridgeIOS : LongriverSDKBridge
    {
        [DllImport("__Internal")]
        private static extern void bridgeInitWithConfig(string fileContent);
        [DllImport("__Internal")]
        private static extern void bridgeEnableSimpleMode();
        [DllImport("__Internal")]
        private static extern string bridgeGetSDKVersion();
        [DllImport("__Internal")]
        private static extern void bridgeSetUseVersion1(bool enable);
        [DllImport("__Internal")]
        private static extern void bridgeSetUseShopItemFromServer(bool enable);
        [DllImport("__Internal")]
        private static extern void bridgeSetUmpWithIp(bool enable);
        [DllImport("__Internal")]
        private static extern void bridgeLog(string paramJson);
        [DllImport("__Internal")]
        private static extern void bridgeLogThirdEventWithCustomEvent(string paramsJson);
        [DllImport("__Internal")]
        private static extern void bridgeUploadEventToAllPlatform(string eventName, string eventDatasJson);
        [DllImport("__Internal")]
        private static extern void bridgeUploadEventToPlus(string eventName, string dataJson);
        [DllImport("__Internal")]
        private static extern string bridgeGetStaticInfo();

        // ump
        [DllImport("__Internal")]
        private static extern string bridgeGetUMPParameters();
        [DllImport("__Internal")]
        private static extern bool bridgeIsPrivacyOptionsRequired();
        [DllImport("__Internal")]
        private static extern void bridgeShowPrivacyOptionsForm();
        [DllImport("__Internal")]
        private static extern void bridgeSetDebugGeography(int index);

        // ad
        [DllImport("__Internal")]
        private static extern void bridgeShowAppOpenWithTimeout(float seconds);
        [DllImport("__Internal")]
        private static extern bool bridgeHasInterstitial(string adEntry);
        [DllImport("__Internal")]
        private static extern void bridgeShowInterstitial(string adEntry);
        [DllImport("__Internal")]
        private static extern bool bridgeHasReward(string adEntry);
        [DllImport("__Internal")]
        private static extern void bridgeShowReward(string adEntry);
        [DllImport("__Internal")]
        private static extern void bridgeShowOrReShowBanner(int pos);
        [DllImport("__Internal")]
        private static extern void bridgeSetBannerBackgroundColor(float red, float green, float blue, float alpha);
        [DllImport("__Internal")]
        private static extern void bridgeHideBanner();
        [DllImport("__Internal")]
        private static extern void bridgeRemoveBanner();
        [DllImport("__Internal")]
        private static extern string bridgeGetLoadingStatusSummary();
        [DllImport("__Internal")]
        private static extern void bridgeSetWebAdListener();
        [DllImport("__Internal")]
        private static extern bool bridgeIsWedAdReady();
        [DllImport("__Internal")]
        private static extern void bridgeShowUrl();

        //userpayment
        [DllImport("__Internal")]
        private static extern bool bridgeIsLogin();
        [DllImport("__Internal")]
        private static extern string bridgeGetGameAccountId();
        [DllImport("__Internal")]
        private static extern string bridgeGetSessionToken();
        [DllImport("__Internal")]
        private static extern void bridgeLogout();
        [DllImport("__Internal")]
        private static extern void bridgeSignOut();
        [DllImport("__Internal")]
        private static extern void bridgeUploadRoleLogin(string serverId, string serverName, string roleId, string roleName);
        [DllImport("__Internal")]
        private static extern void bridgeAutoLoginAsync(bool platformLogin);
        [DllImport("__Internal")]
        private static extern void bridgeCheckLoginAsync();
        [DllImport("__Internal")]
        private static extern void bridgeLoginWithTypeAsync(string loginType);
        [DllImport("__Internal")]
        private static extern void bridgeBindWithTypeAsync(string loginType);
        [DllImport("__Internal")]
        private static extern void bridgeUnbindWithTypeAsync(string loginType);
        [DllImport("__Internal")]
        private static extern void bridgeGetUserInfoAsync();
        [DllImport("__Internal")]
        private static extern void bridgeGetBindTransferCode();
        [DllImport("__Internal")]
        private static extern void bridgeGenerateTransferCode();
        [DllImport("__Internal")]
        private static extern void bridgeBindTransferCode(string transferCode);
        [DllImport("__Internal")]
        private static extern void bridgeBindTransferCodeAndReLogin(string transferCode);
        [DllImport("__Internal")]
        private static extern int bridgeTryLaunchOrOpenAppStore(string appURLScheme, string appStoreURL, string dataJson);
        [DllImport("__Internal")]
        private static extern string bridgeGetShopItems();
        [DllImport("__Internal")]
        private static extern void bridgeGetShopItemsAsync();
        [DllImport("__Internal")]
        private static extern void bridgeStartPayment(string itemId, string cpOrderId);
        [DllImport("__Internal")]
        private static extern void bridgeStartPaymentWithEnvIdAndExtraInfo(string itemId, string cpOrderId, string envId, string extraInfo);
        // [DllImport("__Internal")]
        // private static extern void bridgeStartPaymentForSimpleGame(string itemId);
        [DllImport("__Internal")]
        private static extern void bridgeConsumeItem(string gameOrderId);
        [DllImport("__Internal")]
        private static extern void bridgeQuerySubscriptionAsync();
        [DllImport("__Internal")]
        private static extern void bridgeQueryOneTimeItemAsync();
        [DllImport("__Internal")]
        private static extern void bridgeRestorePurchases();
        [DllImport("__Internal")]
        private static extern void bridgeRestorePurchasesWithCallback();
        [DllImport("__Internal")]
        private static extern void bridgeRestorePurchasesWithSyncAndCallback(bool syncEnabled);
        [DllImport("__Internal")]
        private static extern void bridgeShareLinkByFacebook(string url);
        [DllImport("__Internal")]
        private static extern void bridgeSharePhotoByFacebook(byte[] bytes, int size);

        public void initWithConfig(string fileContent)
        {
            bridgeInitWithConfig(fileContent);
        }

        public void enableSimpleMode()
        {
            bridgeEnableSimpleMode();
        }

        public string GetSDKVersion()
        {
            return bridgeGetSDKVersion();
        }

        public void SetUseVersion1(bool enable)
        {
            bridgeSetUseVersion1(enable);
        }

        public void SetUseShopItemFromServer(bool enable)
        {
            bridgeSetUseShopItemFromServer(enable);
        }

        public void setUmpWithIp(bool enable) 
        {
            bridgeSetUmpWithIp(enable);
        }

        public void onResume()
        {
            
        }
        public void onPause()
        {
            
        }
        public void Log(string eventName, Dictionary<string, string> paramMap)
        {
            Dictionary<string, object> sendParams = new Dictionary<string, object>();
            sendParams.Add("eventName", eventName);
            sendParams.Add("params", paramMap);
            string paramJson = Json.Serialize(sendParams);
            bridgeLog(paramJson);
        }
        public void logThirdLevel(int level)
        {

        }

        public void logThirdEvent(string eventName, Dictionary<string, string> paramMap)
        {

        }

        public void logThirdEventWithCustomEvent(CustomEvent customEvent)
        {
            string paramsJson = customEvent.ToJson();
            bridgeLogThirdEventWithCustomEvent(paramsJson);
        }
        
        public void UploadEventToAllPlatform(string eventName, Dictionary<string, object> eventDatas)
        {
            string eventDatasJson = Json.Serialize(eventDatas);
            bridgeUploadEventToAllPlatform(eventName, eventDatasJson);
        }

        public void UploadEventToPlus(string eventName, Dictionary<string, object> data)
        {
            string dataJson = null == data ? "{}" : Json.Serialize(data);
            bridgeUploadEventToPlus(eventName, dataJson);
        }

        public StaticInfo GetStaticInfo()
        {
            string s = bridgeGetStaticInfo();
            return StaticInfo.fromJson(s);
        }

        public string GetUMPParameters()
        {
            return bridgeGetUMPParameters();
        }

        public bool IsPrivacyOptionsRequired()
        {
            return bridgeIsPrivacyOptionsRequired();
        }

        public void ShowPrivacyOptionsForm()
        {
            bridgeShowPrivacyOptionsForm();
        }

        public void SetDebugGeography(int index)
        {
            bridgeSetDebugGeography(index);
        }

        public void SetWebAdListener() 
        {
            bridgeSetWebAdListener();
        }
        
        public bool IsWebAdReady() 
        {
            return bridgeIsWedAdReady();
        }

        public void ShowUrl() 
        {
            bridgeShowUrl();
        }

        public void ShowAppOpenWithTimeout(float timeout)
        {
            bridgeShowAppOpenWithTimeout(timeout);
        }

        public bool HasInterstitial(string adEntry)
        {
            return bridgeHasInterstitial(adEntry);
        }

        public void ShowInterstitial(string adEntry)
        {
            bridgeShowInterstitial(adEntry);
        }

        public bool HasReward(string entry)
        {
            return bridgeHasReward(entry);
        }

        public void ShowReward(string adEntry)
        {
            bridgeShowReward(adEntry);
        }
        public void ShowOrReShowBanner(BannerPos pos)
        {
            bridgeShowOrReShowBanner((int)pos);
        }
        public void HideBanner()
        {
            bridgeHideBanner();
        }

        public void RemoveBanner()
        {
            bridgeRemoveBanner();
        }

        public void SetBannerBackgroundColor(float red, float green, float blue, float alpha)
        {
            bridgeSetBannerBackgroundColor(red, green, blue, alpha);
        }

        public Dictionary<string, string> GetLoadingStatusSummary()
        {
            string s = bridgeGetLoadingStatusSummary();
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
            return bridgeIsLogin();
        }
        public long getGameAccountId()
        {
            return long.Parse(bridgeGetGameAccountId());
        }
        public string getSessionToken()
        {
            return bridgeGetSessionToken();
        }

        public void Logout()
        {
            bridgeLogout();
        }

        public void SignOut()
        {
            bridgeSignOut();
        }

        public void UploadRoleLogin(string serverId, string serverName, string roleId, string roleName)
        {
            bridgeUploadRoleLogin(serverId, serverName, roleId, roleName);
        }

        public void autoLoginAsync(bool platformLogin)
        {
            bridgeAutoLoginAsync(platformLogin);
        }

        public void checkLoginAsync()
        {
            bridgeCheckLoginAsync();
        }

        public void loginWithTypeAsync(LOGIN_TYPE loginType)
        {
            bridgeLoginWithTypeAsync(loginType.ToString());
        }

        public void bindWithTypeAsync(LOGIN_TYPE loginType)
        {
            bridgeBindWithTypeAsync(loginType.ToString());
        }

        public void unbindWithTypeAsync(LOGIN_TYPE loginType)
        {
            bridgeUnbindWithTypeAsync(loginType.ToString());
        }

        public void getUserInfoAsync()
        {
            bridgeGetUserInfoAsync();
        }

        public void getBindTransferCode()
        {
            bridgeGetBindTransferCode();
        }

        public void generateTransferCode()
        {
            bridgeGenerateTransferCode();
        }

        public void bindTransferCode(string transferCode)
        {
            bridgeBindTransferCode(transferCode);
        }

        public void bindTransferCodeAndReLogin(string transferCode)
        {
            bridgeBindTransferCodeAndReLogin(transferCode);
        }

        public int tryLaunchOrOpenAppStore(string appURLScheme, string appStoreURL, Dictionary<string, object> dataDict) 
        { 
            string dataJson = Json.Serialize(dataDict);
            return bridgeTryLaunchOrOpenAppStore(appURLScheme, appStoreURL, dataJson);
        }

        public ShopItemResult getShopItems()
        {
            string s = bridgeGetShopItems();
            if (s != null && s.Length > 0)
            {
                return JsonUtility.FromJson<ShopItemResult>(s);
            }
            else return null;
        }

        public void getShopItemsAsync()
        {
            bridgeGetShopItemsAsync();
        }

        public void startPayment(string itemId, string cpOrderId)
        {
            bridgeStartPayment(itemId, cpOrderId);
        }

        public void startPayment(string itemId, string cpOrderId, Dictionary<string, string> extraInfo)
        {
            extraInfo = extraInfo ?? new Dictionary<string, string>();
            string jsonString = Json.Serialize(extraInfo);
            bridgeStartPaymentWithEnvIdAndExtraInfo(itemId, cpOrderId, "", jsonString);
        }

        public void oneStepPay(string itemId, string cpOrderId)
        {
            //
        }
        // public void startPaymentForSimpleGame(string itemId)
        // {
        //     bridgeStartPaymentForSimpleGame(itemId);
        // }

        public void consumeItem(long gameOrderId)
        {
            bridgeConsumeItem(gameOrderId.ToString());
        }

        public void querySubscriptionAsync()
        {
            bridgeQuerySubscriptionAsync();
        }
        public void queryOneTimeItemAsync()
        {
            bridgeQueryOneTimeItemAsync();
        }

        public void restorePurchases()
        {
            bridgeRestorePurchases();
        }

        public void restorePurchasesWithCallback()
        {
            bridgeRestorePurchasesWithCallback();
        }

        public void restorePurchasesWithSyncAndCallback(bool syncEnabled)
        {
            bridgeRestorePurchasesWithSyncAndCallback(syncEnabled);
        }

        public void shareFacebookAsync(string url, byte[] image)
        {
            if (!string.IsNullOrWhiteSpace(url))
            {
                bridgeShareLinkByFacebook(url);
            }
            else if (null != image && image.Length > 0)
            {
                string imageString = System.Convert.ToBase64String(image);
                byte[] imageBytes = System.Text.Encoding.UTF8.GetBytes(imageString);
                // Debug.LogFormat("shareFacebookAsync imageBytes: {0}; imageString: {1};", imageBytes.Length, imageString.Length);
                bridgeSharePhotoByFacebook(imageBytes, imageBytes.Length);
            }
        }
    }
#endif
}

