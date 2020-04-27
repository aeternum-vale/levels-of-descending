using UnityEngine;

public static class CameraUtils
{
    public static Texture2D GetCameraTexture(Camera cameraComponent, int width, int height)
    {
        cameraComponent.targetTexture = new RenderTexture(width, height, 24, RenderTextureFormat.ARGB32, 10);
        cameraComponent.Render();

        Texture2D t2d = new Texture2D(width, height, TextureFormat.ARGB32, false);
        Graphics.CopyTexture(cameraComponent.targetTexture, t2d);

        return t2d;
    }
}
