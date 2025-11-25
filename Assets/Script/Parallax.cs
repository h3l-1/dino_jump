using UnityEngine;

[RequireComponent(typeof(MeshRenderer))]
public class Parallax : MonoBehaviour
{
    // THIS SCRIPT FIXES THE "Ground is faster than objects" BUG.
    
    private MeshRenderer meshRenderer;
    private Material material;
    
    // You must measure your Quad width in Unity Units!
    // If your Quad scale is X = 20, put 20 here.
    public float textureWidthInWorldUnits = 20f; 
    
    private float currentOffset = 1f;

    void Awake()
    {
        meshRenderer = GetComponent<MeshRenderer>();
        material = meshRenderer.material;
    }

    void Update()
    {
        if (GameManager.Instance.isGameOver) return;

        // THE MATH FIX:
        // To sync 100% with objects moving at 'gameSpeed', we divide by width.
        float speed = GameManager.Instance.gameSpeed;
        
        // Calculate how much of the texture (0 to 1) we pass per second
        float offsetDelta = (speed / textureWidthInWorldUnits) * Time.deltaTime;
        
        currentOffset += offsetDelta;
        
        // Keep it between 0 and 1 to avoid floating point errors over long runs
        if(currentOffset > 1) currentOffset -= 1;

        material.mainTextureOffset = new Vector2(currentOffset, 0);
    }
}