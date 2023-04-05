using System.Diagnostics;
using System;
using System.Collections.Generic;
using dotmob.Scripts.Core;
using HmsPlugin;
using UnityEngine;
using HuaweiMobileServices.Ads;
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
            HMSAdsKitManager.Instance.OnRewarded = OnRewarded;
            HMSAdsKitManager.Instance.OnInterstitialAdClosed = OnInterstitialAdClosed;

            HMSAdsKitManager.Instance.ConsentOnFail = OnConsentFail;
            HMSAdsKitManager.Instance.ConsentOnSuccess = OnConsentSuccess;
            HMSAdsKitManager.Instance.RequestConsentUpdate();


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

        public void OnRewarded(Reward reward)
        {
            UnityEngine.Debug.Log("[HMS] AdsDemoManager rewarded!");
        }

        public void OnInterstitialAdClosed()
        {
            UnityEngine.Debug.Log("[HMS] AdsDemoManager interstitial ad closed");
        }

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

        public void ShowRewardBasedVideo() {
            ShowSplashVideo();
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