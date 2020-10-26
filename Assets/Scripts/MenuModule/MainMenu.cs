using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Plugins;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Utils;

namespace MenuModule
{
    public class MainMenu : MonoBehaviour
    {
        private const float HoverAlpha = 1f;
        private const float NormalAlpha = .6f;
        private const float DisabledAlpha = 0.07f;
        private const float MenuItemAlphaRate = 0.15f;
        private const float AlphaRate = 0.05f;

        private List<Button> _buttons = new List<Button>();
        private bool _infoMode;
        private Graphic _infoText;
        private bool _isLoading;

        private Graphic _loadingText;
        private bool _menuIsActive = true;

        private void Awake()
        {
            _buttons = transform.GetComponentsInChildren<Button>(true).ToList();
            _infoText = transform.Find("infoText").GetComponent<Graphic>();
            _loadingText = transform.Find("loadingText").GetComponent<Graphic>();

            ForEachButton(item =>
            {
                item.HoverAlpha = HoverAlpha;
                item.NormalAlpha = NormalAlpha;
                item.DisabledAlpha = DisabledAlpha;
                item.AnimateAlpha = (text, target) => AnimateAlpha(text, target, AlphaRate);
                item.AnimateHoverAlpha = (text, target) => AnimateAlpha(text, target, MenuItemAlphaRate);

                item.SetAlpha(item.isLink ? 0 : NormalAlpha, true);

                if (!item.isLink) return;

                item.Locked = true;
            });

            _infoText.gameObject.SetActive(false);
            _loadingText.gameObject.SetActive(false);

            Messenger<EButtonId>.AddListener(Events.ButtonClicked, OnButtonClicked);
            Messenger.AddListener(Events.MenuBackClicked, OnMenuBackClicked);
        }

        private void Start()
        {
            EnableAllMenuItems();
        }

        private void ForEachMenuItem(Action<Button> action)
        {
            _buttons.Where(item => !item.isLink).ToList().ForEach(action);
        }

        private void ForEachButton(Action<Button> action)
        {
            _buttons.ForEach(action);
        }

        private void ForEachLink(Action<Button> action)
        {
            _buttons.Where(item => item.isLink).ToList().ForEach(action);
        }

        private void OnButtonClicked(EButtonId id)
        {
            if (!_menuIsActive) return;
            if (_isLoading) return;

            _menuIsActive = false;

            switch (id)
            {
                case EButtonId.NEW_GAME:
                    StartCoroutine(SwitchToLoadingMode());
                    break;
                case EButtonId.INFO:
                    StartCoroutine(SwitchToInfoMode());
                    break;
                case EButtonId.EXIT:
                    Application.Quit();
                    break;

                case EButtonId.INSTA:
                    Application.OpenURL("https://instagram.com/a.dedyulya");
                    break;
                case EButtonId.MAIL:
                    Application.OpenURL("mailto:aeternum-vale@ya.ru");
                    break;
                case EButtonId.FB:
                    Application.OpenURL("https://facebook.com/adedyulya");
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(id), id, null);
            }

            _menuIsActive = true;
        }

        private IEnumerator SwitchToLoadingMode()
        {
            _isLoading = true;

            _loadingText.color = GameUtils.SetColorAlpha(_loadingText.color, 0f);
            _loadingText.gameObject.SetActive(true);

            HideAllMenuItems();
            yield return StartCoroutine(AnimateAlpha(_loadingText, NormalAlpha, AlphaRate));
            SceneManager.LoadSceneAsync("game");

            yield return null;
        }

        private IEnumerator SwitchToInfoMode()
        {
            _infoMode = true;

            _infoText.color = GameUtils.SetColorAlpha(_infoText.color, 0f);
            _infoText.gameObject.SetActive(true);

            DisableAllMenuItems();
            ForEachLink(link => link.Enable());

            yield return StartCoroutine(AnimateAlpha(_infoText, NormalAlpha, AlphaRate));
        }

        private IEnumerator SwitchFromInfoMode()
        {
            _infoMode = false;

            EnableAllMenuItems();
            ForEachLink(link => link.Hide());

            yield return StartCoroutine(AnimateAlpha(_infoText, 0f, AlphaRate));
            _infoText.gameObject.SetActive(false);
        }

        private void DisableAllMenuItems()
        {
            ForEachMenuItem(item => item.Disable());
        }

        private void EnableAllMenuItems()
        {
            ForEachMenuItem(item => item.Enable());
        }

        private void HideAllMenuItems()
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

        public void OnMenuBackClicked()
        {
            if (!_infoMode) return;
            if (_isLoading) return;

            StartCoroutine(SwitchFromInfoMode());
        }

        private void OnDestroy()
        {
            StopAllCoroutines();
            
            Messenger<EButtonId>.RemoveListener(Events.ButtonClicked, OnButtonClicked);
            Messenger.RemoveListener(Events.MenuBackClicked, OnMenuBackClicked);
        }
    }
}