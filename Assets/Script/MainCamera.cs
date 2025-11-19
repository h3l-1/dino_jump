using UnityEngine;

public class MainCamera : MonoBehaviour
{
    public float cameraSize = 5f; // Camera view size
    
    private void Start()
    {
        Camera.main.orthographicSize = cameraSize;
    }
    
    // Or to change it dynamically
    public void SetCameraSize(float newSize)
    {
        Camera.main.orthographicSize = newSize;
    }
}