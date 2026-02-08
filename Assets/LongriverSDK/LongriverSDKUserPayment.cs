using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEditor;

namespace LongriverSDKNS
{
    public class LongriverSDKUserPayment
    {
        static public LongriverSDKUserPayment instance = new LongriverSDKUserPayment(LongriverSDKBridgeFactory.instance);

        private LongriverSDKBridge bridge;
        public Action<SignOutResult> signOutSuccess = null;
        public Action<State> signOutFailed = null;
        public Action<AutoLoginResult> autoLoginAsyncSuccess = null;
        public Action<State> autoLoginAsyncFail = null;

        public Action<CheckLoginResult> checkLoginAsyncSuccess = null;
        public Action<State> checkLoginAsyncFail = null;

        public Action<LoginResult> loginWithTypeAsyncSuccess = null;
        public Action<State> loginWithTypeAsyncFail = null;

        public Action<UserInfoResult> bindWithTypeAsyncSuccess = null;
        public Action<State> bindWithTypeAsyncFail = null;

        public Action<UserInfoResult> unbindWithTypeAsyncSuccess = null;
        public Action<State> unbindWithTypeAsyncFail = null;

        public Action<UserInfoResult> getUserInfoAsyncSuccess = null;
        public Action<State> getUserInfoAsyncFail = null;

        public Action<GetBindTransferCodeResult> getBindTransferCodeSuccess = null;
        public Action<State> getBindTransferCodeFail = null;

        public Action<GenerateTransferCodeResult> generateTransferCodeSuccess = null;
        public Action<State> generateTransferCodeFail = null;

        public Action<BindTransferCodeResult> bindTransferCodeSuccess = null;
        public Action<State> bindTransferCodeFail = null;
        public Action<LoginResult> bindTransferCodeAndReLoginSuccess = null;
        public Action<State> bindTransferCodeAndReLoginFail = null;

        public Action<ShopItemResult> getShopItemsAsyncSuccess = null;
        public Action<State> getShopItemsAsyncFail = null;

        public Action<StartPaymentResult> startPaymentSuccess = null;
        public Action<State> startPaymentFail = null;

        public Action<StartPaymentResult> oneStepPaySuccess = null;
        public Action<State> oneStepPayFail = null;

        public Action<State> onTransactionStatus = null;

        // public Action<StartPaymentResult> startPaymentForSimpleGameSuccess = null;
        // public Action<State> startPaymentForSimpleGameFail = null;

        public IPurchaseItemsListener listener = null;

        public Action<string> onRestoreApplePurchasesSuccess = null;
        public Action<string> onRestoreApplePurchasesFail = null;

        public Action<SubscriptionData> querySubscriptionAsyncSuccess = null;
        public Action<State> querySubscriptionAsyncFail = null;

        public Action<OneTimeItemList> queryOneTimeItemAsyncSuccess = null;
        public Action<State> queryOneTimeItemAsyncFail = null;

        public LongriverSDKUserPayment(LongriverSDKBridge b) {
            this.bridge = b;
        }

        public void SetUseVersion1(bool enable)
        {
            bridge.SetUseVersion1(enable);
        }

        public void SetUseShopItemFromServer(bool enable)
        {
            bridge.SetUseShopItemFromServer(enable);
        }

        public bool isLogin()
        {
            return bridge.isLogin();
        }

        public long getGameAccountId()
        {
            return bridge.getGameAccountId();
        }

        public string getSessionToken()
        {
            return bridge.getSessionToken();
        }

        public void Logout()
        {
            bridge.Logout();
        }
        
        [Obsolete("该方法已废弃，请使用DeleteAccount方法替代，用法一致", false)]
        public void SignOut(Action<SignOutResult> success, Action<State> failed)
        {
            signOutSuccess = success;
            signOutFailed = failed;
            bridge.SignOut();
        }
        
        public void DeleteAccount(Action<SignOutResult> success, Action<State> failed)
        {
            signOutSuccess = success;
            signOutFailed = failed;
            bridge.SignOut();
        }

        public void autoLoginAsync(bool needGooglePlay, Action<AutoLoginResult> success, Action<State> fail)
        {
            autoLoginAsyncSuccess = success;
            autoLoginAsyncFail = fail;
            bridge.autoLoginAsync(needGooglePlay);
        }

        public void checkLoginAsync(Action<CheckLoginResult> success, Action<State> fail)
        {
            checkLoginAsyncSuccess = success;
            checkLoginAsyncFail = fail;
            bridge.checkLoginAsync();
        }

        public void loginWithTypeAsync(LOGIN_TYPE loginType, Action<LoginResult> success, Action<State> fail)
        {
            loginWithTypeAsyncSuccess = success;
            loginWithTypeAsyncFail = fail;
            bridge.loginWithTypeAsync(loginType);
        }

        public void bindWithTypeAsync(LOGIN_TYPE loginType, Action<UserInfoResult> success, Action<State> fail)
        {
            bindWithTypeAsyncSuccess = success;
            bindWithTypeAsyncFail = fail;
            bridge.bindWithTypeAsync(loginType);
        }

        public void unbindWithTypeAsync(LOGIN_TYPE loginType, Action<UserInfoResult> success, Action<State> fail)
        {
            unbindWithTypeAsyncSuccess = success;
            unbindWithTypeAsyncFail = fail;
            bridge.unbindWithTypeAsync(loginType);
        }

        public void getUserInfoAsync(Action<UserInfoResult> success, Action<State>fail)
        {
            getUserInfoAsyncSuccess = success;
            getUserInfoAsyncFail = fail;
            bridge.getUserInfoAsync();
        }

        public void GetBindTransferCode(Action<GetBindTransferCodeResult> success, Action<State> fail)
        {
            getBindTransferCodeSuccess = success;
            getBindTransferCodeFail = fail;
            bridge.getBindTransferCode();
        }

        public void GenerateTransferCode(Action<GenerateTransferCodeResult> success, Action<State> fail)
        {
            generateTransferCodeSuccess = success;
            generateTransferCodeFail = fail;
            bridge.generateTransferCode();
        }

        public void BindTransferCode(string transferCode, Action<BindTransferCodeResult> success, Action<State> fail)
        {
            bindTransferCodeSuccess = success;
            bindTransferCodeFail = fail;
            bridge.bindTransferCode(transferCode);
        }

        public void BindTransferCodeAndReLogin(string transferCode, Action<LoginResult> success, Action<State> fail)
        {
            this.bindTransferCodeAndReLoginSuccess = success;
            this.bindTransferCodeAndReLoginFail = fail;
            bridge.bindTransferCodeAndReLogin(transferCode);
        }

        public ShopItemResult getShopItems()
        {
            return bridge.getShopItems();
        }

        public void getShopItemsAsync(Action<ShopItemResult> success, Action<State> fail)
        {
            getShopItemsAsyncSuccess = success;
            getShopItemsAsyncFail = fail;
            bridge.getShopItemsAsync();
        }

        //
        public void startPaymentWithEnvIdAndExtraInfo(string itemId, string cpOrderId, string envId, Dictionary<string, string> extraInfo,
                          Action<StartPaymentResult> success, Action<State> fail)
        {
            startPaymentSuccess = success;
            startPaymentFail = fail;
            cpOrderId = string.IsNullOrEmpty(envId) ? cpOrderId : envId + "###" + cpOrderId;
            extraInfo = extraInfo ?? new Dictionary<string, string>();
            bridge.startPayment(itemId, cpOrderId, extraInfo);
        }

        public void startPaymentWithExtraInfo(string itemId, string cpOrderId, Dictionary<string, string> extraInfo,
                          Action<StartPaymentResult> success, Action<State> fail)
        {
            this.startPaymentWithEnvIdAndExtraInfo(itemId, cpOrderId, null, extraInfo, success, fail);
        }

        public void startPaymentWithEnvId(string itemId, string cpOrderId, string envId,
                          Action<StartPaymentResult> success, Action<State> fail)
        {
            this.startPaymentWithEnvIdAndExtraInfo(itemId, cpOrderId, envId, null, success, fail);
        }

        public void startPayment(string itemId, string cpOrderId,
                          Action<StartPaymentResult> success, Action<State>fail)
        {
            this.startPaymentWithEnvIdAndExtraInfo(itemId, cpOrderId, null, null, success, fail);
        }
        // public void startPaymentForSimpleGame(string itemId, Action<StartPaymentResult> success, Action<State> fail)
        // {
        //     startPaymentForSimpleGameSuccess = success;
        //     startPaymentForSimpleGameFail = fail;
        //     bridge.startPaymentForSimpleGame(itemId);
        // }
        public void oneStepPayWithEnvId(string itemId, string cpOrderId, string envId,
                         Action<StartPaymentResult> success, Action<State> fail)
        {
           oneStepPay(itemId, envId + "###" + cpOrderId, success, fail);
        }
        public void oneStepPay(string itemId, string cpOrderId,
                         Action<StartPaymentResult> success, Action<State> fail)
        {
            oneStepPaySuccess = success;
            oneStepPayFail = fail;
            bridge.oneStepPay(itemId, cpOrderId);
        }

        public void setTransactionStatusListener(System.Action<State> listener) {
            this.onTransactionStatus = listener;
        }

        public void setIPurchaseItemsListener(IPurchaseItemsListener listener)
        {
            this.listener = listener;
        }

        public void consumeItem(long gameOrderId)
        {
            bridge.consumeItem(gameOrderId);
        }

        public void querySubscriptionAsync(Action<SubscriptionData> success, Action<State> fail)
        {
            querySubscriptionAsyncSuccess = success;
            querySubscriptionAsyncFail = fail;
            bridge.querySubscriptionAsync();
        }

        public void queryOneTimeItemAsync(Action<OneTimeItemList> success, Action<State> fail)
        {
            queryOneTimeItemAsyncSuccess = success;
            queryOneTimeItemAsyncFail = fail;
            bridge.queryOneTimeItemAsync();
        }

        public void restorePurchases()
        {
            bridge.restorePurchases();
        }

        public void restorePurchasesWithCallback(Action<string> success, Action<string> fail)
        {
            this.onRestoreApplePurchasesSuccess = success;
            this.onRestoreApplePurchasesFail = fail;
            bridge.restorePurchasesWithCallback();
        }

        public void restorePurchasesWithSyncAndCallback(bool syncEnabled, Action<string> success, Action<string> fail)
        {
            this.onRestoreApplePurchasesSuccess = success;
            this.onRestoreApplePurchasesFail = fail;
            bridge.restorePurchasesWithSyncAndCallback(syncEnabled);
        }
    }
}

