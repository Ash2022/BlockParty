using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LongriverSDKNS
{
    public interface LongriverSDKWebAdListener
    {
        void onWebAdLoaded();

        void onWebAdLoadFailed(string errorMsg);

        void onWebAdClose();

        void onWebAdPlayStart();

        void onWebAdPlayFailed(string errorMsg);

        void onWebAdReward(string msg);
    }
}
