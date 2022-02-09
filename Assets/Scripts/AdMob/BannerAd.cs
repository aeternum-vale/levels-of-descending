using System;
using GoogleMobileAds.Api;
using Plugins;
using UnityEngine;

public class BannerAd : MonoBehaviour
{
	private BannerView _bannerAd;

	[SerializeField] private string _bannerId;

	private float floorTouchCount = 0;

	private void Awake()
	{
		_bannerAd = new BannerView(_bannerId, AdSize.SmartBanner, AdPosition.Top);

		_bannerAd.OnAdLoaded += OnAdLoaded;
		Messenger.AddListener(Events.AfterFloorWasTouched, OnAfterFloorWasTouched);
	}

	private void LoadAndShowAd()
	{
		AdRequest adRequest = new AdRequest.Builder().Build();
		_bannerAd.LoadAd(adRequest);
	}

	private void OnAdLoaded(object sender, EventArgs args)
	{
		Debug.Log($"<color=lightblue>{GetType().Name}:</color> OnAdLoaded");
		_bannerAd.Show();
	}

	private void OnAfterFloorWasTouched()
	{
		floorTouchCount++;
		
		Debug.Log($"<color=lightblue>{GetType().Name}:</color> OnAfterFloorWasTouched floorTouchCount={floorTouchCount}");

		if (floorTouchCount > 1 && floorTouchCount % 2 == 0)
			LoadAndShowAd();
	}
}