using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DebugController : MonoBehaviour
{
	[SerializeField] private Button _r1Button;
	[SerializeField] private Button _r2Button;
	[SerializeField] private Button _r3Button;
	[SerializeField] private Button _togglePPButton;

	[SerializeField] private GameObject _ppVolume;

	private void Awake()
	{
		_r1Button.onClick.AddListener(() => ChangeResolution(800, 480));
		_r2Button.onClick.AddListener(() => ChangeResolution(1920, 1080));
		_r3Button.onClick.AddListener(() => ChangeResolution(2960, 1440));

		_togglePPButton.onClick.AddListener(() => _ppVolume.SetActive(!_ppVolume.activeSelf));
	}

	private void ChangeResolution(int width, int height)
	{
		Debug.Log($"<color=lightblue>{GetType().Name}:</color> ChangeResolution w={width} h={height}");
		Screen.SetResolution(width, height, FullScreenMode.ExclusiveFullScreen);
	}
}
