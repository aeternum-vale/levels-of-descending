using UnityEngine;

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
        return CameraUtils.GetCameraTexture(cameraComponent, Screen.width, Screen.height);
    }

    public void ActivateInventoryMode()
    {
        IsInventoryModeOn = true;
        gameObject.SetActive(false);
    }

    public void DeactivateInventoryMode()
    {
        IsInventoryModeOn = false;
        cameraComponent.targetTexture = null;
        gameObject.SetActive(true);
    }
}