using UnityEngine;

public class MainCamera : MonoBehaviour
{
    [Header("Camera Settings")]
    public float cameraSize = 5f;
    public float edgeBuffer = 2f;
    
    public static float LeftEdge { get; private set; }
    public static float RightEdge { get; private set; }

    private Camera mainCamera;

    private void Awake()
    {
        mainCamera = Camera.main;
        UpdateCameraEdges();
    }

    private void Update()
    {
        UpdateCameraEdges();
    }

    private void UpdateCameraEdges()
    {
        if (mainCamera == null) return;

        float cameraHeight = mainCamera.orthographicSize;
        float cameraWidth = cameraHeight * mainCamera.aspect;

        Vector3 cameraPosition = transform.position;
        
        LeftEdge = cameraPosition.x - cameraWidth - edgeBuffer;
        RightEdge = cameraPosition.x + cameraWidth + edgeBuffer;
    }

    public static bool IsObjectOffscreenLeft(Vector3 position)
    {
        return position.x < LeftEdge;
    }
}