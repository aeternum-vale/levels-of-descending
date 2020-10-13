using System;
using System.Collections;
using Plugins;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Utils;

namespace MenuModule
{
    public enum EMenuItemId
    {
        NEW_GAME,
        INFO,
        EXIT
    }

    public class MenuItem : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
    {
        private Text _text;
        [SerializeField] private EMenuItemId menuItemIdId;
        public bool Locked { get; set; } = false;
        public float HoverAlpha { get; set; }
        public float NormalAlpha { get; set; }
        public float DisabledAlpha { get; set; }
        public Func<Text, float, IEnumerator> AnimateAlpha { get; set; }

        public void OnPointerClick(PointerEventData eventData)
        {
            if (Locked) return;
            
            Messenger<EMenuItemId>.Broadcast(Events.MenuItemClicked, menuItemIdId);
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            if (Locked) return;
            
            SetAlpha(HoverAlpha);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            if (Locked) return;

            SetAlpha(NormalAlpha);
        }

        public void Disable()
        {
            Locked = true;
            SetAlpha(DisabledAlpha);
        }

        public void Enable()
        {
            Locked = false;
            SetAlpha(NormalAlpha);
        }

        public void Hide()
        {
            Locked = true;
            SetAlpha(0f);
        }
        
        private void Start()
        {
            _text = transform.GetComponent<Text>();
        }

        public void SetAlpha(float targetValue)
        {
            StopAllCoroutines();
            StartCoroutine(AnimateAlpha(_text, targetValue));
        }
    }
}