using System.Linq;
using UnityEngine;

namespace MenuModule
{
    public class MainMenu : MonoBehaviour
    {
        [SerializeField] private Font hoverFont;
        [SerializeField] private Font normalFont;

        private void Awake()
        {
            transform.GetComponentsInChildren<MenuItem>().ToList().ForEach(item =>
            {
                item.NormalFont = normalFont;
                item.HoverFont = hoverFont;
            });
        }
    }
}