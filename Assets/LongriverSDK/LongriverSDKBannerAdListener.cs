using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LongriverSDKNS
{
    public interface LongriverSDKBannerAdListener
    {
        /**
        * 广告展示
        */
        void onAdImpress(string unitId, CallbackInfo callbackInfo);
        /**
         * 广告点击
         */
        void onAdClick(string unitId, CallbackInfo callbackInfo);

        void onAdLoaded(string unitId, CallbackInfo callbackInfo);

        void onAdLoadFailed(string unitId, int errorCode, string errorMsg);
    }
}

