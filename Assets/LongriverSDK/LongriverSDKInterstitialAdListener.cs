using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LongriverSDKNS
{
    public interface LongriverSDKInterstitialAdListener
    {
        /***
        * 广告展示
        */
        void onInterstitialAdShow(string unitId, CallbackInfo callbackInfo);
        /***
         * 广告关闭
         */
        void onInterstitialAdClose(string unitId, CallbackInfo callbackInfo);
        /***
         * 广告点击
         */
        void onInterstitialAdClick(string unitId, CallbackInfo callbackInfo);

        void onInterstitialAdLoaded(string unitId, CallbackInfo callbackInfo);

        void onInterstitialAdLoadFailed(string unitId, int errorCode, string errorMsg);

        void onInterstitialAdPlayFail(string unitId, int errorCode, string errorMsg);
    }
}

