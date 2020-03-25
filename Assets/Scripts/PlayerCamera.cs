using UnityEngine;
using System.Collections;

public class PlayerCamera : MonoBehaviour
{

    [SerializeField] private Material mat;
    public bool IsInventoryModeOn { get; set; }
    private Camera cameraComponent;
    public float MouseSensitivity { get; } = 1;
    readonly float mouseVerticalMin = -80;
    readonly float mouseVerticalMax = 70;
    float rotationX;


    void Start()
    {
        cameraComponent = GetComponent<Camera>();
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        if (!IsInventoryModeOn)
        {
            rotationX -= Input.GetAxis("Mouse Y") * MouseSensitivity;
            rotationX = Mathf.Clamp(rotationX, mouseVerticalMin, mouseVerticalMax);
            transform.localEulerAngles = new Vector3(rotationX, 0, 0);
        }
    }

    void OnGUI()
    {
        if (!IsInventoryModeOn)
        {
            int size = 8;
            float posX = cameraComponent.pixelWidth / 2 - size / 2;
            float posY = cameraComponent.pixelHeight / 2 - size;
            GUI.Label(new Rect(posX, posY, 80, 80), "◦");
        }
    }

    void OnRenderImage(RenderTexture src, RenderTexture dest)
    {
        if (IsInventoryModeOn)
        {
            Graphics.Blit(src, dest, mat);
        }
        else
        {
            Graphics.Blit(src, dest);
        }
    }

    public Texture2D GetBackgroundTexture()
    {
        cameraComponent.targetTexture = new RenderTexture(Screen.width, Screen.height, 24, RenderTextureFormat.ARGB32, 10);
        cameraComponent.Render();

        Texture2D t2d = new Texture2D(Screen.width, Screen.height, TextureFormat.ARGB32, false);
        Graphics.CopyTexture(cameraComponent.targetTexture, t2d);
        return t2d;
    }
}