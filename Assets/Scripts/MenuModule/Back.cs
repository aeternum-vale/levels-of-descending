using Plugins;
using UnityEngine;
using UnityEngine.EventSystems;

namespace MenuModule
{
    public class Back : MonoBehaviour,IPointerClickHandler {
        
        public void OnPointerClick(PointerEventData eventData)
        {
            Messenger.Broadcast(Events.MenuBackClicked);
        }
    }
}
