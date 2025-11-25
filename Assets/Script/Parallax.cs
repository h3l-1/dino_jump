using UnityEngine;

[RequireComponent(typeof(MeshRenderer))]
public class Parallax : MonoBehaviour
{
    
    private MeshRenderer meshRenderer;
    private Material material;
    
    // resolution size/ pixel size = scale
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

        // sync the bg with the game manager speed
        float speed = GameManager.Instance.gameSpeed;
        
        // calculate how much of the texture (0 to 1) we pass per second
        float offsetDelta = (speed / textureWidthInWorldUnits) * Time.deltaTime;
        
        currentOffset += offsetDelta;
        
        // should be kept between 0 and 1 to avoid floating point errors 
        if(currentOffset > 1) currentOffset -= 1;

        material.mainTextureOffset = new Vector2(currentOffset, 0);
    }
}