using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace LongriverSDKNS
{
    [Serializable]
    public class CustomEvent
    {
        [UnityEngine.SerializeField]
        public string eventName = "";
        [UnityEngine.SerializeField]
        public string eventTag = "";
        [UnityEngine.SerializeField]
        public Dictionary<string, object> partnerParameter;
        [UnityEngine.SerializeField]
        public Dictionary<string, object> callbackParameter;
        [UnityEngine.SerializeField]
        public Dictionary<string, object> revenue;
        [UnityEngine.SerializeField]
        public string callbackId;
        [UnityEngine.SerializeField]
        public string orderId;
        [UnityEngine.SerializeField]
        public string productId;
        [UnityEngine.SerializeField]
        public string purchaseToken;
        [UnityEngine.SerializeField]
        public Dictionary<string, object> logs;
        
        public CustomEvent(){}

        public Dictionary<string, object> ToDictionary()
        {
            Dictionary<string, object> customEventDict = new Dictionary<string, object>();
            if (!string.IsNullOrWhiteSpace(eventName)) 
            {
                customEventDict.Add("eventName", eventName);
            }
            if (!string.IsNullOrWhiteSpace(eventTag)) 
            {
                customEventDict.Add("eventTag", eventTag);
            }
            if (!string.IsNullOrWhiteSpace(callbackId))
            {
                customEventDict.Add("callbackId", callbackId);
            }
            if (!string.IsNullOrWhiteSpace(orderId))
            {
                customEventDict.Add("orderId", orderId);
            }
            if (!string.IsNullOrWhiteSpace(productId))
            {
                customEventDict.Add("productId", productId);
            }
            if (!string.IsNullOrWhiteSpace(purchaseToken)) 
            {
                customEventDict.Add("purchaseToken", purchaseToken);
            }
            if (null != partnerParameter)
            {
                customEventDict.Add("partnerParameter", partnerParameter);
            }
            if (null != callbackParameter)
            {
                customEventDict.Add("callbackParameter", callbackParameter);
            }
            if (null != revenue)
            {
                customEventDict.Add("revenue", revenue);
            }
            if (null != logs)
            {
                if ("add_to_cart".Equals(this.eventTag)
                || "init_checkout".Equals(this.eventTag)
                || "purchase".Equals(this.eventTag)
                || "subscription".Equals(this.eventTag))
                {
                    Dictionary<string, object> newDict = new Dictionary<string, object>();
                    foreach (var item in logs)
                    {
                        if ("price".Equals(item.Key))
                        {
                            if (item.Value is string)
                            {
                                double value;
                                if (double.TryParse(item.Value as string, out value))
                                {
                                    newDict.Add(item.Key, value);
                                }
                                else
                                {
                                    Debug.LogError($"Unable to parse the string to a double. {this.eventTag}:{item.Value}");
                                }
                                continue;
                            }
                        }
                        newDict.Add(item.Key, item.Value);
                    }
                    customEventDict.Add("logs", newDict);
                } 
                else
                {
                    customEventDict.Add("logs", logs);
                }
            }
            return customEventDict;
        }

        public string ToJson() {
            return Json.Serialize(this.ToDictionary());
        }
    }
}

