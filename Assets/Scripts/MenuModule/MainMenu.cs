using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Plugins;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Utils;

namespace MenuModule
{
    public class MainMenu : MonoBehaviour, IPointerClickHandler
    {
        private const float HoverAlpha = 1f;
        private const float NormalAlpha = .55f;
        private const float DisabledAlpha = 0.03f;
        private const float MenuItemAlphaRate = 0.1f;
        private const float AlphaRate = 0.05f;

        private Text _loadingText;
        private Text _infoText;
        private bool _menuIsActive = true;
        private bool _infoMode = false;
        private bool _isLoading = false;

        private List<MenuItem> _menuItems = new List<MenuItem>();

        private void Awake()
        {
            _menuItems = transform.GetComponentsInChildren<MenuItem>().ToList();

            ForEachMenuItem(item =>
            {
                item.HoverAlpha = HoverAlpha;
                item.NormalAlpha = NormalAlpha;
                item.DisabledAlpha = DisabledAlpha;
                item.AnimateAlpha = (text, target) => AnimateAlpha(text, target, MenuItemAlphaRate);
            });

            _infoText = transform.Find("infoText").GetComponent<Text>();
            _loadingText = transform.Find("loadingText").GetComponent<Text>();
            
            Messenger<EMenuItemId>.AddListener(Events.MenuItemClicked, OnMenuItemClicked);
        }

        private void Start()
        {
            EnableAllItems();
        }

        private void ForEachMenuItem(Action<MenuItem> action)
        {
            _menuItems.ForEach(action);
        }

        private void OnMenuItemClicked(EMenuItemId id)
        {
            if (!_menuIsActive) return;
            if (_isLoading) return;
            if (_infoMode) return ;
            
            _menuIsActive = false;

            switch (id)
            {
                case EMenuItemId.NEW_GAME:
                    StartCoroutine(LoadGameScene());
                    break;
                case EMenuItemId.INFO:
                    StartCoroutine(SwitchToInfoMode(_infoText));
                    break;
                case EMenuItemId.EXIT:
                    Application.Quit();
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(id), id, null);
            }

            _menuIsActive = true;
        }

        private IEnumerator LoadGameScene()
        {
            _isLoading = true;
            HideAllItems();
            yield return StartCoroutine(SwitchToInfoMode(_loadingText));
            SceneManager.LoadSceneAsync("game");
        }

        private IEnumerator SwitchToInfoMode(Text infoTypeText)
        {
            _infoMode = true;
            
            infoTypeText.color = GameUtils.SetColorAlpha(infoTypeText.color, 0f);
            infoTypeText.gameObject.SetActive(true);

            DisableAllItems();
            yield return StartCoroutine(AnimateAlpha(infoTypeText, NormalAlpha, AlphaRate));
        }
        
        private IEnumerator SwitchFromInfoMode(Text infoTypeText)
        {
            _infoMode = false;

            EnableAllItems();
            yield return StartCoroutine(AnimateAlpha(infoTypeText, 0f, AlphaRate));
            infoTypeText.gameObject.SetActive(false);
        }

        private void DisableAllItems()
        {
            ForEachMenuItem(item => item.Disable());
        }
        
        private void EnableAllItems()
        {
            ForEachMenuItem(item => item.Enable());
        }
        
        private void HideAllItems()
        {
            ForEachMenuItem(item => item.Hide());
        }

        private static IEnumerator AnimateAlpha(Graphic gr, float targetValue, float alphaRate)
        {
            return GameUtils.AnimateValue(
                () => gr.color.a,
                a => gr.color = GameUtils.SetColorAlpha(gr.color, a),
                targetValue,
                alphaRate);
        }
        public void OnPointerClick(PointerEventData eventData)
        {
            if (!_infoMode) return;
            if (_isLoading) return;
            
            StartCoroutine(SwitchFromInfoMode(_infoText));
        }
    }
}