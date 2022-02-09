using GoogleMobileAds.Api;
using UnityEngine;

public class AdInitialize : MonoBehaviour
{
	private void Awake()
	{
		MobileAds.Initialize(initStatus => { });
	}
}