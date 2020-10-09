using Plugins;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

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
        private readonly float _hoverAlpha = 1f;

        private float _normalAlpha;

        private Text _text;
        [SerializeField] private EMenuItemId menuItemIdId;
        public Font NormalFont { get; set; }
        public Font HoverFont { get; set; }

        public void OnPointerClick(PointerEventData eventData)
        {
            Messenger<EMenuItemId>.Broadcast(Events.MenuItemClicked, menuItemIdId);
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            //_text.font = HoverFont;

            Color c = _text.color;
            _text.color = new Color(c.r, c.g, c.b, _hoverAlpha);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            _text.font = NormalFont;

            Color c = _text.color;
            _text.color = new Color(c.r, c.g, c.b, _normalAlpha);
        }

        private void Start()
        {
            _text = transform.GetComponent<Text>();

            _text.font = NormalFont;
            _normalAlpha = _text.color.a;
        }
    }
}