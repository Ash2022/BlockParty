using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace LongriverSDKNS
{
    [Serializable]
    public class CallbackInfo
    {
        [UnityEngine.SerializeField]
        private double revenue;
        [UnityEngine.SerializeField]
        private string network;
        [UnityEngine.SerializeField]
        private string placementId;
        [UnityEngine.SerializeField]
        private string adsourceId;

        public CallbackInfo()
        {

        }

        public Dictionary<string, string> GetAdditionInfo()
        {
            return new Dictionary<string, string>();
        }

        public double GetRevenue()
        {
            return revenue;
        }

        public string GetAdSourceId()
        {
            return adsourceId;
        }

        public string GetNetworkFirmId()
        {
            return network;
        }

        public string GetNetworkPlacement()
        {
            return placementId;
        }
    }
}

