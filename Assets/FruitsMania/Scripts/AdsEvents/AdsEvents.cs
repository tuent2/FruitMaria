using System;
using dotmob.Scripts.Core;

namespace dotmob.Scripts.AdsEvents
{
	public enum AdType
	{
		AdmobInterstitial,
		UnityAdsVideo,
		ChartboostInterstitial,
		
	}
/// <summary>
/// Ad event
/// </summary>
	[Serializable]
	public class AdEvents
	{
		public GameState gameEvent;
		public AdType adType;
		public int everyLevel;
		//1.6
		public int calls;

	}
}