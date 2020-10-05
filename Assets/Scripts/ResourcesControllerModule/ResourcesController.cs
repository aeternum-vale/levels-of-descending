using UnityEngine;

namespace ResourcesControllerModule
{
    public class ResourcesController : MonoBehaviour
    {
        public Texture2D LostRabbitAdTexture { get; private set; }
        public Texture2D HandsTexture { get; private set; }
        public Texture2D[] StaticAdPicTextures { get; private set; }

        private void Start()
        {
            LostRabbitAdTexture = Resources.Load<Texture2D>("Textures/rabbit");
            HandsTexture = Resources.Load<Texture2D>("Textures/hands");
            StaticAdPicTextures = Resources.LoadAll<Texture2D>("AdStaticPics");
        }
    }
}