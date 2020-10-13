using System;
using System.Collections;
using Plugins;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Utils;

namespace MenuModule
{
    public enum EButtonId
    {
        NEW_GAME,
        INFO,
        EXIT,
        
        INSTA,
        MAIL
    }

    public class Button : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
    {
        private Graphic _graphic;
        [SerializeField] private EButtonId buttonIdId;
        [SerializeField] public bool isLink = false;
        public bool Locked { get; set; } = false;
        public float HoverAlpha { get; set; }
        public float NormalAlpha { get; set; }
        public float DisabledAlpha { get; set; }
        public Func<Graphic, float, IEnumerator> AnimateAlpha { get; set; }
        public Func<Graphic, float, IEnumerator> AnimateHoverAlpha { get; set; }

        public void OnPointerClick(PointerEventData eventData)
        {
            if (Locked) return;
            
            Messenger<EButtonId>.Broadcast(Events.ButtonClicked, buttonIdId);
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            if (Locked) return;
            
            SetHoverAlpha(HoverAlpha);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            if (Locked) return;

            SetHoverAlpha(NormalAlpha);
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
        
        private void Awake()
        {
            _graphic = transform.GetComponent<Graphic>();
        }

        public void SetAlpha(float targetValue, bool immediately = false)
        {
            if (immediately)
            {
                _graphic.color = GameUtils.SetColorAlpha(_graphic.color, targetValue);
                return;
            }

            StopAllCoroutines();
            StartCoroutine(AnimateAlpha(_graphic, targetValue));
        }
        
        public void SetHoverAlpha(float targetValue)
        {
            StopAllCoroutines();
            StartCoroutine(AnimateHoverAlpha(_graphic, targetValue));
        }

        private void OnDestroy()
        {
            StopAllCoroutines();
        }
    }
}