using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace LongriverSDKNS
{
    public interface IPurchaseItemsListener
    {
        void getPurchaseItems(PurchaseItems purchaseItems);
        void getOneTimeItems(OneTimeItemList oneTimeItemList);
        void getSubscriptionItems(SubscriptionData subscriptionItems);
    }
}

