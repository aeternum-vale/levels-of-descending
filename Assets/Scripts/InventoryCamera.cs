using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryCamera : MonoBehaviour
{
    public RenderTexture BackgroundTexture { get; set; }

    void OnPreRender()
    {

        GL.PushMatrix();
  		GL.LoadPixelMatrix ();
        Graphics.DrawTexture(new Rect(0, Screen.height, Screen.width, -Screen.height), BackgroundTexture);
        GL.PopMatrix ();
    }

}
