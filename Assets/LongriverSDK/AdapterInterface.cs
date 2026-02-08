using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LongriverSDKNS
{
    public interface AdapterInterface
    {
        void SetLongriverSDKRewardedVideoListener(LongriverSDKRewardedVideoListener simpleSDKRewardedVideoListener);

        void SetLongriverSDKInterstitialAdListener(LongriverSDKInterstitialAdListener simpleSDKInterstitialAdListener);

        void SetLongriverSDKBannerAdListener(LongriverSDKBannerAdListener simpleSDKBannerAdListener);

        bool HasReward();

        void ShowReward(string adEntry);

        bool HasInterstitial();

        void ShowInterstitial(string adEntry);

        void ShowOrReShowBanner(BannerPos pos);

        void HideBanner();

        void RemoveBanner();

        Dictionary<string, string> GetLoadingStatusSummary();
    }
}

