using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LongriverSDKNS
{
    public interface LongriverSDKBridge
    {
        void initWithConfig(string fileContent);
        int tryLaunchOrOpenAppStore(string packageName, string className, Dictionary<string, object> dataDict);
        void enableSimpleMode();
        string GetSDKVersion();
        void SetUseVersion1(bool enable);
        void SetUseShopItemFromServer(bool enable);
        void setUmpWithIp(bool enable);
        void onResume();
        void onPause();
        void Log(string eventName, Dictionary<string, string> paramMap);

        void logThirdLevel(int level);

        void logThirdEvent(string eventname, Dictionary<string, string> paramMap);

        void logThirdEventWithCustomEvent(CustomEvent customEvent);

        void UploadEventToAllPlatform(string eventName, Dictionary<string, object> eventDatas);

        void UploadEventToPlus(string eventName, Dictionary<string, object> data);

        StaticInfo GetStaticInfo();

        string GetUMPParameters();

        bool IsPrivacyOptionsRequired();

        void ShowPrivacyOptionsForm();

        void SetDebugGeography(int index);

        void SetWebAdListener();
        
        bool IsWebAdReady();

        void ShowUrl();

        void ShowAppOpenWithTimeout(float timeout);

        bool HasInterstitial(string adEntry);

        void ShowInterstitial(string adEntry);

        bool HasReward(string adEntry);

        void ShowReward(string adEntry);
        void ShowOrReShowBanner(BannerPos pos);
        void HideBanner();

        void RemoveBanner();

        void SetBannerBackgroundColor(float red, float green, float blue, float alpha);

        Dictionary<string, string> GetLoadingStatusSummary();

        //USERPAYMENT
        bool isLogin();
        
        long getGameAccountId();

        string getSessionToken();

        void Logout();

        void SignOut();

        void UploadRoleLogin(string serverId, string serverName, string roleId, string roleName);

        void autoLoginAsync(bool platformLogin);

        void checkLoginAsync();

        void loginWithTypeAsync(LOGIN_TYPE loginType);

        void bindWithTypeAsync(LOGIN_TYPE loginType);

        void unbindWithTypeAsync(LOGIN_TYPE loginType);

        void getUserInfoAsync();

        void getBindTransferCode();

        void generateTransferCode();

        void bindTransferCode(string transferCode);

        void bindTransferCodeAndReLogin(string transferCode);

        ShopItemResult getShopItems();

        void getShopItemsAsync();

        //
        void startPayment(string itemId, string cpOrderId);

        void startPayment(string itemId, string cpOrderId, Dictionary<string, string> extraInfo);

        void oneStepPay(string itemId, string cpOrderId);

        // void startPaymentForSimpleGame(string itemId);

        void consumeItem(long gameOrderId);

        void querySubscriptionAsync();

        void queryOneTimeItemAsync();

        void restorePurchases();

        void restorePurchasesWithCallback();

        void restorePurchasesWithSyncAndCallback(bool syncEnabled);

        //void printDatabase();

        void shareFacebookAsync(string url, byte[] image);
    }
}


