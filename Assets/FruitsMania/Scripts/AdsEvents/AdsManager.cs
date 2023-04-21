using System.Diagnostics;
using System;
using System.Collections;
using System.Collections.Generic;
using dotmob.Scripts.Core;
using HmsPlugin;
using UnityEngine;
using HuaweiMobileServices.Ads;
using HuaweiMobileServices.IAP;
using HuaweiMobileServices.Utils;
namespace dotmob.Scripts.AdsEvents
{
    /// <summary>
    /// Ads manager responsible for initialization and showing
    /// </summary>
    public class AdsManager : UnityEngine.MonoBehaviour
    {
        public static AdsManager THIS;
        //EDITOR: ads events
        public List<AdEvents> adsEvents = new List<AdEvents>();
        //is unity ads enabled
        public bool enableUnityAds;
        //is admob enabled
        public bool enableGoogleMobileAds;
        //is chartboost enabled
        public bool enableChartboostAds;
        //rewarded zone for Unity ads
        public string rewardedVideoZone;
        //admob stuff
        private AdManagerScriptable adsSettings;
        private int type;
        private Action saveGems;
        bool adsRewardCompleted;
        private void Awake()
        {
            if (THIS == null) THIS = this;
            else if (THIS != this)
            {
                Destroy(gameObject);
                return;
            }
            DontDestroyOnLoad(this);
            RequestBanner();
        }


        private void Start()
        {
            // HMSIAPManager.Instance.InitializeIAP();
            HMSAdsKitManager.Instance.OnRewardAdCompleted = OnRewarded;
            HMSAdsKitManager.Instance.OnInterstitialAdClosed = OnInterstitialAdClosed;

            HMSAdsKitManager.Instance.ConsentOnFail = OnConsentFail;
            HMSAdsKitManager.Instance.ConsentOnSuccess = OnConsentSuccess;
            HMSAdsKitManager.Instance.RequestConsentUpdate();

            // HMSAdsKitManager.Instance.OnSplashAdShowed += OnABC;
            // saveGems += OnBCD;

            #region SetNonPersonalizedAd , SetRequestLocation

            var builder = HwAds.RequestOptions.ToBuilder();

            builder
                .SetConsent("tcfString")
                .SetNonPersonalizedAd((int)NonPersonalizedAd.ALLOW_ALL)
                .Build();

            bool requestLocation = true;
            var requestOptions = builder.SetConsent("testConsent").SetRequestLocation(requestLocation).Build();

            UnityEngine.Debug.Log($"RequestOptions NonPersonalizedAds:  {requestOptions.NonPersonalizedAd}");
            UnityEngine.Debug.Log($"Consent: {requestOptions.Consent}");

            #endregion
        }

        // void OnABC (){
        //     UnityEngine.Debug.Log("Anc123:" + type);
        //     // StartCoroutine(checkRewardVideo(type));
        //      if (HMSAdsKitManager.Instance.IsRewardedAdLoaded) {
        //         if(type == 1) {
        //             InitScript.Instance.AddLife(1);


        //         }
        //         else if (type == 2) {
        //             InitScript.Instance.AddGems(10);

        //         }

        // }
        // }

        private void LateUpdate()
        {
            adsIsOver();
        }

        private void OnEnable()
        {
            HMSIAPManager.Instance.OnBuyProductSuccess += OnBuyProductSuccess;
            HMSIAPManager.Instance.OnInitializeIAPSuccess += OnInitializeIAPSuccess;
            HMSIAPManager.Instance.OnInitializeIAPFailure += OnInitializeIAPFailure;
            HMSIAPManager.Instance.OnBuyProductFailure += OnBuyProductFailure;
        }

        private void OnDisable()
        {
            HMSIAPManager.Instance.OnBuyProductSuccess -= OnBuyProductSuccess;
            HMSIAPManager.Instance.OnInitializeIAPSuccess -= OnInitializeIAPSuccess;
            HMSIAPManager.Instance.OnInitializeIAPFailure -= OnInitializeIAPFailure;
            HMSIAPManager.Instance.OnBuyProductFailure -= OnBuyProductFailure;
        }

        private void RestoreProducts()
        {

            HMSIAPManager.Instance.RestorePurchaseRecords((restoredProducts) =>
            {
                foreach (var item in restoredProducts.InAppPurchaseDataList)
                {
                    if ((IAPProductType)item.Kind == IAPProductType.Consumable)
                    {
                        UnityEngine.Debug.Log($"aaaaaaaaaaaaaa Consumable: ProductId {item.ProductId} , SubValid {item.SubValid} , PurchaseToken {item.PurchaseToken} , OrderID  {item.OrderID}");
                        // consumablePurchaseRecord.Add(item);
                    }
                }
            });

            HMSIAPManager.Instance.RestoreOwnedPurchases((restoredProducts) =>
            {
                foreach (var item in restoredProducts.InAppPurchaseDataList)
                {
                    if ((IAPProductType)item.Kind == IAPProductType.Subscription)
                    {
                        UnityEngine.Debug.Log($"aaaaaaaaaaaaaa Subscription: ProductId {item.ProductId} , ExpirationDate {item.ExpirationDate} , AutoRenewing {item.AutoRenewing} , PurchaseToken {item.PurchaseToken} , OrderID {item.OrderID}");
                        // activeSubscriptions.Add(item);
                    }

                    else if ((IAPProductType)item.Kind == IAPProductType.NonConsumable)
                    {
                        UnityEngine.Debug.Log($"aaaaaaaaaaaaaa NonConsumable: ProductId {item.ProductId} , DaysLasted {item.DaysLasted} , SubValid {item.SubValid} , PurchaseToken {item.PurchaseToken} ,OrderID {item.OrderID}");
                        // activeNonConsumables.Add(item);
                    }
                }
            });

        }

        private void OnBuyProductSuccess(PurchaseResultInfo obj)
        {
            UnityEngine.Debug.Log($"aaaaaaaaaaaaaa OnBuyProductSuccess");
            InitScript.Instance.AddGems(100);
            // if (obj.InAppPurchaseData.ProductId == "removeads")
            // {
            //     IAPLog?.Invoke("Ads Removed!");
            // }
            // else if (obj.InAppPurchaseData.ProductId == "coins100")
            // {
            //     IAPLog?.Invoke("coins100 Purchased!");
            // }
            // else if (obj.InAppPurchaseData.ProductId == "premium")
            // {
            //     IAPLog?.Invoke("premium subscribed!");
            // }
        }

        private void OnInitializeIAPFailure(HMSException obj)
        {
            UnityEngine.Debug.Log("aaaaaaaaaaaaaa IAP is not ready.");
        }

        private void OnInitializeIAPSuccess()
        {
            UnityEngine.Debug.Log("aaaaaaaaaaaaaa IAP is ready.");

            RestoreProducts();
        }

        private void OnBuyProductFailure(int code)
        {
            UnityEngine.Debug.Log("aaaaaaaaaaaaaa Purchase Fail.");
        }



        private void OnConsentSuccess(ConsentStatus consentStatus, bool isNeedConsent, IList<AdProvider> adProviders)
        {
            UnityEngine.Debug.Log($"[HMS] AdsDemoManager OnConsentSuccess consentStatus:{consentStatus} isNeedConsent:{isNeedConsent}");
            foreach (var AdProvider in adProviders)
            {
                UnityEngine.Debug.Log($"[HMS] AdsDemoManager OnConsentSuccess adproviders: Id:{AdProvider.Id} Name:{AdProvider.Name} PrivacyPolicyUrl:{AdProvider.PrivacyPolicyUrl} ServiceArea:{AdProvider.ServiceArea}");
            }
        }

        private void OnConsentFail(string desc)
        {
            UnityEngine.Debug.Log($"[HMS] AdsDemoManager OnConsentFail:{desc}");
        }

        public void RequestBanner()
        {
            UnityEngine.Debug.Log("[HMS] AdsDemoManager ShowBannerAd");
            HMSAdsKitManager.Instance.ShowBannerAd();
        }

        public void HideBannerAd()
        {
            UnityEngine.Debug.Log("[HMS] AdsDemoManager HideBannerAd");

            HMSAdsKitManager.Instance.HideBannerAd();
        }

        public void ShowRewardedAd()
        {
            UnityEngine.Debug.Log("[HMS] AdsDemoManager ShowRewardedAd");
            HMSAdsKitManager.Instance.ShowRewardedAd();
        }

        public void ShowInterstitialAd()
        {
            UnityEngine.Debug.Log("[HMS] AdsDemoManager ShowInterstitialAd");
            HMSAdsKitManager.Instance.ShowInterstitialAd();
        }

        public void ShowSplashImage()
        {
            UnityEngine.Debug.Log("[HMS] ShowSplashImage!");

            HMSAdsKitManager.Instance.LoadSplashAd("testq6zq98hecj", SplashAd.SplashAdOrientation.PORTRAIT);
        }

        public void ShowSplashVideo()
        {
            UnityEngine.Debug.Log("[HMS] ShowSplashVideo!");

            HMSAdsKitManager.Instance.LoadSplashAd("testd7c5cewoj6", SplashAd.SplashAdOrientation.PORTRAIT);
        }

        public void OnRewarded()
        {
            UnityEngine.Debug.Log("[HMS] AdsDemoManager rewarded!");
            // HMSAdsKitManager.Instance.OnRewardAdCompleted();
            adsRewardCompleted = true;
        }

        void adsIsOver()
        {
            if (adsRewardCompleted == true)
            {
                if (type == 1)
                {
                    InitScript.Instance.AddLife(1);
                }
                else
                {
                    InitScript.Instance.AddCount(type);
                }
            }

            adsRewardCompleted = false;

        }

        public void OnInterstitialAdClosed()
        {
            UnityEngine.Debug.Log("[HMS] AdsDemoManager interstitial ad closed");
        }

        // Start a Coroutine to wait for rewarded ad completion
        // IEnumerator WaitForRewardedAd()
        // {
        //     // ... Wait for rewarded ad to complete ...

        //     // Call PlayerPrefs.SetInt


        //     yield return null;
        //     InitScript.Instance.AddGems(10);
        // }

        // Call the Coroutine from another script or component
        public void CheckAdsEvents(GameState state)
        {
            foreach (var item in adsEvents)
            {
                if (item.gameEvent == state)
                {
                    item.calls++;
                    if (item.calls % item.everyLevel == 0)
                        ShowAdByType(item.adType);
                }
            }
        }

        public void ShowRewardBasedVideo(int value)
        {

            if (Application.internetReachability != NetworkReachability.NotReachable)
            {
                this.type = value;
                ShowRewardedAd();
                // ShowSplashVideo();

            }
        }

        public void buyIAPChecked(string productID)
        {
            HMSIAPManager.Instance.PurchaseProduct(productID);
        }






        void ShowAdByType(AdType adType)
        {
            switch (adType)
            {
                case AdType.AdmobInterstitial:
                    ShowInterstitialAd();
                    break;
                case AdType.ChartboostInterstitial:
                    ShowInterstitialAd();
                    break;
                case AdType.UnityAdsVideo:
                    ShowSplashVideo();
                    break;
                default:
                    break;
            }
        }
    }
}
