using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LongriverSDKNS
{
    public enum BannerPos
    {
        TOP,
        BOTTOM,
        SAFE_TOP,
        SAFE_BOTTOM
    }
    public class LongriverSDKAd
    {
        static public BridgeAdapterInterface instance = new BridgeAdapterInterface(LongriverSDKBridgeFactory.instance);
    }
    public class BridgeAdapterInterface: AdapterInterface
    {
        LongriverSDKBridge bridge = null;
        private System.Action<string> advertisingActiveListenter = null;
        LongriverSDKWebAdListener longriverSDKWebAdListener = null;
        LongriverSDKAppOpenAdListener LongriverSDKAppOpenAdListener = null;
        LongriverSDKBannerAdListener LongriverSDKBannerAdListener = null;
        LongriverSDKInterstitialAdListener LongriverSDKInterstitialAdListener = null;
        LongriverSDKRewardedVideoListener LongriverSDKRewardedVideoListener = null;
        public BridgeAdapterInterface(LongriverSDKBridge inputBridge)
        {
            this.bridge = inputBridge;
        }

        public void ShowAppOpenWithTimeout(float timeout)
        {
            if (null == this.bridge) return;
            this.bridge.ShowAppOpenWithTimeout(timeout);
        }

        public bool IsWebAdReady()
        {
            return bridge.IsWebAdReady();
        }

        public void ShowUrl()
        {
            bridge.ShowUrl();
        }

        public bool HasInterstitial()
        {
            return bridge.HasInterstitial("");
        }
        public bool HasInterstitial(string adEntry)
        {
            return bridge.HasInterstitial(adEntry);
        }

        public void ShowInterstitial(string adEntry)
        {
            bridge.ShowInterstitial(adEntry);
        }

        public bool HasReward()
        {
            return bridge.HasReward("");
        }

        public bool HasReward(string adEntry)
        {
            return bridge.HasReward(adEntry);
        }

        public void ShowReward(string adEntry)
        {
            bridge.ShowReward(adEntry);
        }

        public void SetBannerBackgroundColor(float red, float green, float blue, float alpha)
        {
            bridge.SetBannerBackgroundColor(red, green, blue, alpha);
        }
        
        public void ShowOrReShowBanner(BannerPos pos)
        {
            bridge.ShowOrReShowBanner(pos);
        }
        public void HideBanner()
        {
            bridge.HideBanner();
        }

        public void RemoveBanner()
        {
            bridge.RemoveBanner();
        }

        public Dictionary<string, string> GetLoadingStatusSummary()
        {
            return bridge.GetLoadingStatusSummary();
        }

        public void SetAdvertisingActiveListenter(System.Action<string> action)
        {
            this.advertisingActiveListenter = action;
        }

        internal System.Action<string> GetAdvertisingActiveListenter()
        {
            return this.advertisingActiveListenter;
        }

        public void SetlongriverSDKWebAdListener(LongriverSDKWebAdListener longriverSDKWebAdListener)
        {
            this.longriverSDKWebAdListener = longriverSDKWebAdListener;
            this.bridge.SetWebAdListener();
        }

        public LongriverSDKWebAdListener getLongriverSDKWebAdListener()
        {
            return this.longriverSDKWebAdListener;
        }

        public void SetLongriverSDKAppOpenAdListener(LongriverSDKAppOpenAdListener LongriverSDKAppOpenAdListener)
        {
            this.LongriverSDKAppOpenAdListener = LongriverSDKAppOpenAdListener;
        }

        public LongriverSDKAppOpenAdListener GetLongriverSDKAppOpenAdListener()
        {
            return this.LongriverSDKAppOpenAdListener;
        }

        public void SetLongriverSDKBannerAdListener(LongriverSDKBannerAdListener LongriverSDKBannerAdListener)
        {
            this.LongriverSDKBannerAdListener = LongriverSDKBannerAdListener;
        }

        public LongriverSDKBannerAdListener GetLongriverSDKBannerAdListener()
        {
            return this.LongriverSDKBannerAdListener;
        }

        public void SetLongriverSDKInterstitialAdListener(LongriverSDKInterstitialAdListener LongriverSDKInterstitialAdListener)
        {
            this.LongriverSDKInterstitialAdListener = LongriverSDKInterstitialAdListener;
        }
        public LongriverSDKInterstitialAdListener GetLongriverSDKInterstitialAdListener()
        {
            return this.LongriverSDKInterstitialAdListener;
        }

        public void SetLongriverSDKRewardedVideoListener(LongriverSDKRewardedVideoListener LongriverSDKRewardedVideoListener)
        {
            this.LongriverSDKRewardedVideoListener = LongriverSDKRewardedVideoListener;
        }
        public LongriverSDKRewardedVideoListener GetLongriverSDKRewardedVideoListener()
        {
            return this.LongriverSDKRewardedVideoListener;
        }


    }
}

