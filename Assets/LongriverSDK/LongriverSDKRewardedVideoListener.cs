using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LongriverSDKNS
{
    public interface LongriverSDKRewardedVideoListener
    {
        /***
        * 视频播放开始
        */
        void onRewardedVideoAdPlayStart(string unitId, CallbackInfo callbackInfo);
        /***
         * 视频播放失败
         * @param code 错误码
         * @param message 错误信息
         */
        void onRewardedVideoAdPlayFail(string unitId, string code, string message);
        /**
         * 视频页面关闭
         * @param isReward 视频是否播放完成
         */
        void onRewardedVideoAdPlayClosed(string unitId, CallbackInfo callbackInfo);

        /***
         * 视频点击
         */
        void onRewardedVideoAdPlayClicked(string unitId, CallbackInfo callbackInfo);

        void onRewardedVideoAdLoaded(string unitId, CallbackInfo callbackInfo);

        void onRewardedVideoAdLoadFailed(string unitId, int errorCode, string errorMsg);

        /***
         * 发放奖励
         */
        void onReward(string unitId, CallbackInfo callbackInfo);
    }
}

