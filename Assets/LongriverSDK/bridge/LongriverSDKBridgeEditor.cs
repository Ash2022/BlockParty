using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
//using Newtonsoft.Json;
using System.Security.Cryptography;
using System.Globalization;
using System.Text;

namespace LongriverSDKNS
{
#if UNITY_EDITOR
    public class LongriverSDKBridgeEditor : LongriverSDKBridge
	{
        private StaticInfo info = null;
        private string gameId = "";
        private long gameAccountId = -1;
        private ShopItemResult result = new ShopItemResult();
        private System.Random r = new System.Random();
        private PurchaseItems purchaseItems = new PurchaseItems();
        public void enableSimpleMode() {}
        public string GetSDKVersion()
        {
            return LongriverSDK.SDK_VERSION;
        }
        public void SetUseVersion1(bool enable) {}

        public void SetUseShopItemFromServer(bool enable) {}

        public void setUmpWithIp(bool enable) {}
        public void initWithConfig(string fileContent)
        {
            string j = fileContent.Trim().Replace("\n", "");
            Debug.Log("simple sdk start with " + j);
            string findConfigString = null;
            if (j.StartsWith("{") && j.EndsWith("}")) 
            {
                findConfigString = j;
            } 
            else 
            {
                findConfigString = AesDecryptorBase64(fileContent, "aGVsbG93b3JsZGtl");
            }

            Dictionary<string, object> dict = (Dictionary<string, object>)Json.Deserialize(findConfigString);
            Dictionary<string, object> infoDict = new Dictionary<string, object>();
            gameId = (string)dict["gameName"];
            infoDict.Add("gameName", (string)dict["gameName"]);
            infoDict.Add("pn", Application.identifier);
            infoDict.Add("appVersion", "editor");
            infoDict.Add("deviceid", SystemInfo.deviceUniqueIdentifier);
            infoDict.Add("platform", "editor");
            infoDict.Add("idfa", "idfa1");
            infoDict.Add("uid", "uid");
            infoDict.Add("sessionId", "sessionId");

            infoDict.Add("idfv", "");
            infoDict.Add("android_id", "");

            infoDict.Add("band", "band");
            infoDict.Add("model", "model");
            infoDict.Add("deviceName", "mywindows");
            infoDict.Add("systemVersion", "win");
            infoDict.Add("network", "none");
            infoDict.Add("isVpn", 0);
            infoDict.Add("isProxy", 0);

            info = StaticInfo.fromJson(Json.Serialize(infoDict));

            //callback
            var r = new InitSuccessResult();
            r.isSuccess = true;
            r.msg = "";
            LongriverSDK.instance.initSuccess(r);

            //set attr info
            LongriverSDK.instance.StartCoroutine(LongriverSDKBase.ICoroutine(3, setAttrInfo));
            
            //set shop item
            LongriverSDK.instance.StartCoroutine(LongriverSDKBase.ICoroutine(5, SetShopItem));
        }
        public int tryLaunchOrOpenAppStore(string packageName, string className, Dictionary<string, object> dataDict) { return -1; }
        private RijndaelManaged GetRijndaelManaged(string key)
        {
            byte[] keyArray = Encoding.UTF8.GetBytes(key);
            RijndaelManaged rDel = new RijndaelManaged();
            rDel.Key = keyArray;
            rDel.Mode = CipherMode.ECB;
            rDel.Padding = PaddingMode.PKCS7;
            return rDel;
        }

        private string AesDecryptorBase64(string DecryptStr, string Key)
        {
            try
            {
                byte[] toEncryptArray = Convert.FromBase64String(DecryptStr);
                ICryptoTransform cTransform = GetRijndaelManaged(Key).CreateDecryptor();
                byte[] resultArray = cTransform.TransformFinalBlock(toEncryptArray, 0, toEncryptArray.Length);

                return Encoding.UTF8.GetString(resultArray);//  UTF8Encoding.UTF8.GetString(resultArray);
            }
            catch (Exception ex)
            {
                Debug.Log("aes decryptor error");
                Debug.LogException(ex);
                return null;
            }
        }
    
        private void setAttrInfo()
        {
            AttributionInfo ainfo = new AttributionInfo();
            ainfo.network = LongriverSDK.instance.editorChannel.ToString();
            ainfo.campaign = "";
            ainfo.adgroup = "";
            ainfo.creative = "";
            LongriverSDKCallback.instance.setAttributionInfo(ainfo.ToJson());
        }
        private void SetShopItem()
        {
            result = new ShopItemResult();
            result.items = new List<ShopItem>();
            int index = 99;
            foreach (var one in LongriverSDK.instance.editorTestMockShopItemIdsList)
            {
                var s = new ShopItem();
                s.itemType = "consume";
                s.itemId = one;
                s.price = index;
                s.currency = "USD";
                s.formattedPrice = "$ " + (index / 100.0);
                s.usdPrice = index / 100.0;
                s.doublePrice = index / 100.0;
                s.stringPrice = (index / 100.0).ToString();
                result.items.Add(s);
                index += 100;
            }
            //add unconsume item
            {
                var s = new ShopItem();
                s.itemType = "unconsume";
                s.itemId = "unconsume_productid";
                s.price = index;
                s.currency = "USD";
                s.formattedPrice = "$ " + (index / 100.0);
                s.usdPrice = index / 100.0;
                s.doublePrice = index / 100.0;
                s.stringPrice = (index / 100.0).ToString();
                result.items.Add(s);
            }

            //add subscription
            foreach (var one in LongriverSDK.instance.editorTestMockSubscriptionItemIdsList)
            {
                var s = new ShopItem();
                s.itemType = "subscription";
                s.itemId = one;
                s.price = index;
                s.currency = "USD";
                s.formattedPrice = "$ " + (index / 100.0);
                s.usdPrice = index / 100.0;
                s.doublePrice = index / 100.0;
                s.stringPrice = (index / 100.0).ToString();
                result.items.Add(s);
                index += 100;
            }

            //
            LongriverSDKCallback.instance.getShopItemsAsyncSuccess(JsonUtility.ToJson(result));

        }
        public void onPause()
        {
            Debug.Log("simple sdk onPause ");
        }
        public void onResume()
        {
            Debug.Log("simple sdk onResume");
        }
        public StaticInfo GetStaticInfo()
        {
            return info;
        }
        public void Log(string eventName, Dictionary<string, string> paramMap)
        {
            Debug.Log("read to send log " + eventName + " " + Json.Serialize(paramMap));
        }
        public void logThirdLevel(int level)
        {

        }

        public void logThirdEvent(string eventName, Dictionary<string, string> paramMap)
        {

        }

        public void logThirdEventWithCustomEvent(CustomEvent customEvent)
        {

        }

        public void UploadEventToAllPlatform(string eventName, Dictionary<string, object> eventDatas)
        {

        }

        public void UploadEventToPlus(string eventName, Dictionary<string, object> data)
        {
            
        }

        public string GetUMPParameters()
        {
            return "{}";
        }

        public bool IsPrivacyOptionsRequired()
        {
            return false;
        }

        public void ShowPrivacyOptionsForm()
        {
            
        }

        public void SetDebugGeography(int index)
        {
            
        }

        public void ShowAppOpenWithTimeout(float timeout)
        {
            Debug.Log("ShowAppOpen");
            if (EditorAdMock.HasInit())
            {
                LongriverSDKCallback.instance.onAppOpenAdShow("{\"unitId\":\"test\"}");
                EditorAdMock.ShowInterAd(AppOpenFinish);
                LongriverSDKDemo.onFront = false;
            }
            else
            {
                LongriverSDKCallback.instance.onAppOpenAdShow("{\"unitId\":\"test\"}");
                LongriverSDKCallback.instance.onAppOpenAdClose("{\"unitId\":\"test\"}");
            }
        }
        public void AppOpenFinish(bool isReward)
        {
            LongriverSDKDemo.onFront = true;
            LongriverSDKCallback.instance.onAppOpenAdClose("{\"unitId\":\"test\"}");
        }

        public void SetWebAdListener(){}
        
        public bool IsWebAdReady() {return false;}

        public void ShowUrl() {}

        public bool HasInterstitial(string adEntry)
        {
            return true;
        }

        public void ShowInterstitial(string adEntry)
        {
            Debug.Log("ShowInter");
            if (EditorAdMock.HasInit())
            {
                LongriverSDKCallback.instance.onInterstitialAdShow("{\"unitId\":\"test\"}");
                EditorAdMock.ShowInterAd(InterFinish);
                LongriverSDKDemo.onFront = false;
            }
            else
            {
                LongriverSDKCallback.instance.onInterstitialAdShow("{\"unitId\":\"test\"}");
                LongriverSDKCallback.instance.onInterstitialAdClose("{\"unitId\":\"test\"}");
            }
        }
        public void InterFinish(bool isReward)
        {
            LongriverSDKDemo.onFront = true;
            LongriverSDKCallback.instance.onInterstitialAdClose("{\"unitId\":\"test\"}");
        }
        public bool HasReward(string adEntry)
        {
            return true;
        }

        public void ShowReward(string adEntry)
        {
            Debug.Log("ShowReward");
            if (EditorAdMock.HasInit())
            {
                LongriverSDKCallback.instance.onRewardedVideoAdPlayStart("{\"unitId\":\"test\"}");
                EditorAdMock.ShowRewardAd(RewardFinish);
                LongriverSDKDemo.onFront = false;
            }
            else
            {
                LongriverSDKCallback.instance.onRewardedVideoAdPlayStart("{\"unitId\":\"test\"}");
                LongriverSDKCallback.instance.onReward("{\"unitId\":\"test\"}");
                LongriverSDKCallback.instance.onRewardedVideoAdPlayClosed("{\"unitId\":\"test\"}");
            }
        }

        public void RewardFinish(bool isReward)
        {
            LongriverSDKDemo.onFront = true;
            if(isReward){
                LongriverSDKCallback.instance.onReward("{\"unitId\":\"test\"}");
            }
            LongriverSDKCallback.instance.onRewardedVideoAdPlayClosed("{\"unitId\":\"test\"}");
        }


        public void ShowOrReShowBanner(BannerPos pos)
        {
            Debug.Log("show banner");
            LongriverSDKCallback.instance.onAdImpress("{\"unitId\":\"test\"}");
            LongriverSDKCallback.instance.onAdClick("{\"unitId\":\"test\"}");
        }

        public void HideBanner()
        {
            Debug.Log("hide banner");
        }

        public void RemoveBanner()
        {
            Debug.Log("remove banner");
        }

        public void SetBannerBackgroundColor(float red, float green, float blue, float alpha)
        {
            Debug.Log("set banner background color");
        }

        public Dictionary<string, string> GetLoadingStatusSummary()
        {
            var r = new Dictionary<string, string>();
            r.Add("reward", "succss");
            r.Add("inter", "succss");
            r.Add("banner", "succss");
            return r;
        }
        public bool isLogin()
        {
            return gameAccountId > 0;
        }

        public long getGameAccountId()
        {
            return gameAccountId;
        }

        public string getSessionToken()
        {
            return "sampleToken";
        }

        public void Logout()
        {
            gameAccountId = -1;
        }

        public void SignOut()
        {
            gameAccountId = -1;
            var s = new SignOutResult(200, "success");
            LongriverSDKCallback.instance.onSignOutSuccess(JsonUtility.ToJson(s));
        }

        public void UploadRoleLogin(string serverId, string serverName, string roleId, string roleName)
        {

        }

        public void autoLoginAsync(bool platformLogin)
        {
            if (LongriverSDK.instance.editorTestMockAutoLoginReturnSucc)
            {
                //测试有缓存的情况下登录
                gameAccountId = 22345678;
                var t = new AutoLoginResult(gameId, gameAccountId, gameId, gameAccountId, "editorMock", true);
                LongriverSDKCallback.instance.autoLoginSuccess(JsonUtility.ToJson(t));
            }
            else
            {
                LongriverSDK.instance.StartCoroutine(LongriverSDKBase.ICoroutine(1, delegate
                {
                    var s = new State(-1, "FAIL IN EDITOR MOCK NET FAIL");
                    LongriverSDKCallback.instance.autoLoginFail(JsonUtility.ToJson(s));

                }));
            }
        }

        public void checkLoginAsync()
        {
            if (LongriverSDK.instance.editorTestMockCheckLoginReturnSucc)
            {
                //测试有缓存的情况下登录
                gameAccountId = 12345678;
                var t = new CheckLoginResult(gameId, gameAccountId, gameId, gameAccountId, "editorMock");
                LongriverSDKCallback.instance.checkLoginSuccess(JsonUtility.ToJson(t));
            }
            else
            {
                
                //return fail after network request
                LongriverSDK.instance.StartCoroutine(LongriverSDKBase.ICoroutine(1, delegate
                {
                    var s = new State(-1, "FAIL IN EDITOR MOCK NET FAIL");
                    LongriverSDKCallback.instance.checkLoginFail(JsonUtility.ToJson(s));

                }));
            }

        }

        public void loginWithTypeAsync(LOGIN_TYPE loginType)
        {
            gameAccountId = 321654987;
            switch (loginType)
            {
                case LOGIN_TYPE.DEVICE:
                    {
                        if (LongriverSDK.instance.editorTestMockCheckLoginDeviceReturnSucc)
                        {
                            LongriverSDK.instance.StartCoroutine(LongriverSDKBase.ICoroutine(1, delegate
                            {
                                var t = new LoginResult();
                                t.gameAccountId = gameAccountId;
                                t.saveGameId = gameId;
                                t.saveId = gameAccountId;
                                t.isNew = true;
                                t.loginType = loginType.ToString();
                                LongriverSDKCallback.instance.loginWithTypeAsyncSuccess(JsonUtility.ToJson(t));
                            }));

                            LongriverSDKCallback.instance.getSubscriptionItems(JsonUtility.ToJson(GetSubscriptionData()));

                        }
                        else
                        {
                            LongriverSDK.instance.StartCoroutine(LongriverSDKBase.ICoroutine(1, delegate
                            {
                                var s = new State(-1, "FAIL IN EDITOR");
                                LongriverSDKCallback.instance.loginWithTypeAsyncFail(JsonUtility.ToJson(s));
                            }));
                        }
                    };
                    break;
                default:
                    {
                        var s = new State(-1, "unsupport IN EDITOR");
                        LongriverSDKCallback.instance.loginWithTypeAsyncFail(JsonUtility.ToJson(s));
                    };
                    break;
            }
        }

        public void bindWithTypeAsync(LOGIN_TYPE loginType)
        {
            if (gameAccountId != -1)
            {
                LongriverSDK.instance.StartCoroutine(LongriverSDKBase.ICoroutine(1, delegate
                {
                    var r = new UserInfoResult();
                    r.gameId = gameId;
                    r.gameAccountId = gameAccountId;
                    r.loginInfo = new List<PlatformAccountInfo>();
                    var info = new PlatformAccountInfo();
                    r.loginInfo.Add(info);
                    info.platform = "DEVICE";
                    info.hasLinked = true;
                    info.uniqeId = SystemInfo.deviceUniqueIdentifier;
                    info.iconUrl = "";
                    info.nickName = "";
                    LongriverSDKCallback.instance.bindWithTypeAsyncSuccess(JsonUtility.ToJson(r));
                }));
            }
            else
            {
                var s = new State(-1, "not login");
                LongriverSDKCallback.instance.bindWithTypeAsyncFail(JsonUtility.ToJson(s));
            }
        }

        public void unbindWithTypeAsync(LOGIN_TYPE loginType)
        {
            if (gameAccountId != -1)
            {
                LongriverSDK.instance.StartCoroutine(LongriverSDKBase.ICoroutine(1, delegate
                {
                    var r = new UserInfoResult();
                    r.gameId = gameId;
                    r.gameAccountId = gameAccountId;
                    r.loginInfo = new List<PlatformAccountInfo>();
                    var info = new PlatformAccountInfo();
                    r.loginInfo.Add(info);
                    info.platform = "DEVICE";
                    info.hasLinked = true;
                    info.uniqeId = SystemInfo.deviceUniqueIdentifier;
                    info.iconUrl = "";
                    info.nickName = "";
                    LongriverSDKCallback.instance.unbindWithTypeAsyncSuccess(JsonUtility.ToJson(r));
                }));
            }
            else
            {
                var s = new State(-1, "not login");
                LongriverSDKCallback.instance.unbindWithTypeAsyncFail(JsonUtility.ToJson(s));
            }
        }

        public void getUserInfoAsync()
        {
            if(gameAccountId != -1)
            {
                LongriverSDK.instance.StartCoroutine(LongriverSDKBase.ICoroutine(1, delegate
                {
                    var r = new UserInfoResult();
                    r.gameId = gameId;
                    r.gameAccountId = gameAccountId;
                    r.saveGameId = gameId;
                    r.saveId = gameAccountId;
                    r.loginInfo = new List<PlatformAccountInfo>();
                    var info = new PlatformAccountInfo();
                    r.loginInfo.Add(info);
                    info.platform = "DEVICE";
                    info.hasLinked = true;
                    info.uniqeId = SystemInfo.deviceUniqueIdentifier;
                    info.iconUrl = "";
                    info.nickName = "";
                    LongriverSDKCallback.instance.getUserInfoAsyncSuccess(JsonUtility.ToJson(r));
                }));
            }
            else
            {
                var s  = new State(-1, "not login");
                LongriverSDKCallback.instance.getUserInfoAsyncFail(JsonUtility.ToJson(s));
            }
        }

        public void getBindTransferCode()
        {
            if (gameAccountId != -1)
            {
                var r = new GetBindTransferCodeResult(gameId, gameAccountId);
                LongriverSDKCallback.instance.getBindTransferCodeSuccess(JsonUtility.ToJson(r));
            } 
            else 
            {
                var s  = new State(-1, "not login");
                LongriverSDKCallback.instance.getBindTransferCodeFail(JsonUtility.ToJson(s));
            }
        }

        public void generateTransferCode()
        {
            if (gameAccountId != -1)
            {
                var r = new GenerateTransferCodeResult("this is just test data");
                LongriverSDKCallback.instance.generateTransferCodeSuccess(JsonUtility.ToJson(r));
            } 
            else 
            {
                var s  = new State(-1, "not login");
                LongriverSDKCallback.instance.generateTransferCodeFail(JsonUtility.ToJson(s));
            }
        }

        public void bindTransferCode(string transferCode)
        {
            if (gameAccountId != -1)
            {
                var r = new BindTransferCodeResult(gameId, gameAccountId);
                LongriverSDKCallback.instance.bindTransferCodeSuccess(JsonUtility.ToJson(r));
            } 
            else 
            {
                var s  = new State(-1, "not login");
                LongriverSDKCallback.instance.bindTransferCodeFail(JsonUtility.ToJson(s));
            }
        }

        public void bindTransferCodeAndReLogin(string transferCode)
        {
            if (gameAccountId != -1)
            {
                var r = new BindTransferCodeResult(gameId, gameAccountId);
                LongriverSDKCallback.instance.bindTransferCodeAndReLoginSuccess(JsonUtility.ToJson(r));
            } 
            else 
            {
                var s  = new State(-1, "not login");
                LongriverSDKCallback.instance.bindTransferCodeAndReLoginFail(JsonUtility.ToJson(s));
            }
        }

        public ShopItemResult getShopItems()
        {
            return result;
        }

        public void getShopItemsAsync()
        {
            if(result != null)
            {
                LongriverSDKCallback.instance.getShopItemsAsyncSuccess(JsonUtility.ToJson(result));
            }
            else
            {

            }
        }

        public void startPayment(string itemId, string cpOrderId)
        {
            startPayment(itemId, cpOrderId, null);
        }

        public void startPayment(string itemId, string cpOrderId, Dictionary<string, string> extraInfo)
        {
            if (gameAccountId != -1)
            {

                if (LongriverSDK.instance.editorTestMockPaySucc)
                {
                    long ts = (long)LongriverSDKBase.NowTimestamp() * 1000;
                    long gameOrderId = ts + 987654321 ;
                    LongriverSDK.instance.StartCoroutine(LongriverSDKBase.ICoroutine(3, delegate
                    {
                        var r = new StartPaymentResult();
                        r.gameOrderId = gameOrderId;
                        r.transactionId = System.Guid.NewGuid().ToString();;
                        r.payload = System.Guid.NewGuid().ToString();;
                        LongriverSDKCallback.instance.startPaymentSuccess(JsonUtility.ToJson(r));
                    }));

                    LongriverSDK.instance.StartCoroutine(LongriverSDKBase.ICoroutine(3.5, delegate
                    {
                        //uncomsume item callback
                        var one = new UnconsumeItem();
                        one.gameOrderId = gameOrderId;
                        one.createTime = ts;
                        one.itemId = itemId;
                        one.purchaseTime = ts;
                        one.status = 1;
                        purchaseItems.unconsumeItems.Add(one);

                        LongriverSDKCallback.instance.getPurchaseItems(JsonUtility.ToJson(purchaseItems));
                    }));
                }
                else
                {
                    LongriverSDK.instance.StartCoroutine(LongriverSDKBase.ICoroutine(3, delegate
                    {
                        var s = new State(-1, "payment not success");
                        LongriverSDKCallback.instance.startPaymentFail(JsonUtility.ToJson(s));
                    }));
                }
            }
            else
            {
                var s = new State(-1, "user not login");
                LongriverSDKCallback.instance.startPaymentFail(JsonUtility.ToJson(s));
            }
        }

        public void oneStepPay(string itemId, string cpOrderId)
        {
            startPayment(itemId, cpOrderId);
        }
        // public void startPaymentForSimpleGame(string itemId)
        // {
        //     if (isLogin())
        //     {
        //         startPayment(itemId, "");
        //     }
        //     else
        //     {
        //         gameAccountId = 22345678;
        //         startPayment(itemId, "");
        //     }
        // }

        public void consumeItem(long gameOrderId)
        {
            UnconsumeItem pickOne = null;
            foreach(var one in purchaseItems.unconsumeItems)
            {
                if(one.gameOrderId == gameOrderId)
                {
                    pickOne = one;
                }
            }
            if(pickOne!= null)
            {
                purchaseItems.unconsumeItems.Remove(pickOne);
            }
        }

        private SubscriptionData GetSubscriptionData()
        {
            SubscriptionData subscriptionData = new SubscriptionData();
            subscriptionData.subscriptionItems = new List<SubscriptionItem>();
            var list = LongriverSDK.instance.editorTestMockSubscriptionItemIdsList;
            if (list != null && list.Count > 0)
            {
                var itemId = list[0];
                var subscriptionItem = new SubscriptionItem();
                subscriptionItem.itemId = itemId;
                subscriptionItem.productId = itemId;
                subscriptionData.subscriptionItems.Add(subscriptionItem);
            }
            return subscriptionData;
        }

        public void querySubscriptionAsync()
        {
            LongriverSDKCallback.instance.querySubscriptionAsyncSuccess(JsonUtility.ToJson(GetSubscriptionData()));
        }

        private OneTimeItemList GetOneTimeItemListData()
        {
            OneTimeItemList oneTimeItemList = new OneTimeItemList();
            oneTimeItemList.oneTimeItems = new List<OneTimeItem>();
            { 
                var one = new OneTimeItem();
                one.itemId = "unconsume_productid";
                one.productId = "unconsume_productid";
                one.purchaseTime = LongriverSDKBase.NowTimestamp() * 1000;
                oneTimeItemList.oneTimeItems.Add(one);
            }
            return oneTimeItemList;
        }

        public void queryOneTimeItemAsync()
        {
            LongriverSDKCallback.instance.queryOneTimeItemAsyncSuccess(JsonUtility.ToJson(GetOneTimeItemListData()));
        }

        public void restorePurchases()
        {
            Debug.Log("sdk is restore the purchases");
        }

        public void restorePurchasesWithCallback()
        {
            Debug.Log("sdk is restore the purchases");
            LongriverSDKCallback.instance.onRestoreApplePurchasesSuccess("success");
        }

        public void restorePurchasesWithSyncAndCallback(bool syncEnabled)
        {
            Debug.Log("sdk is restore the purchases");
            LongriverSDKCallback.instance.onRestoreApplePurchasesSuccess("success");
        }

        public void shareFacebookAsync(string url, byte[] image)
        {
            Debug.Log("share facebook");
            LongriverSDKCallback.instance.shareFacebookAsyncSuccess(JsonUtility.ToJson(GetOneTimeItemListData()));
        }
    }
#endif
}


