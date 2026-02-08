using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LongriverSDKNS
{
    public interface LongriverSDKAppOpenAdListener
    {
        /***
        * 广告超时
        */
        void onAppOpenAdTimeout(string unitId);
        /***
        * 广告展示
        */
        void onAppOpenAdShow(string unitId, CallbackInfo callbackInfo);
        /***
         * 广告关闭
         */
        void onAppOpenAdClose(string unitId, CallbackInfo callbackInfo);
        /***
         * 广告点击
         */
        void onAppOpenAdClick(string unitId, CallbackInfo callbackInfo);

        void onAppOpenAdLoaded(string unitId, CallbackInfo callbackInfo);

        void onAppOpenAdLoadFailed(string unitId, int errorCode, string errorMsg);
    }
}

