using UnityEngine;

[RequireComponent(typeof(MeshRenderer))]
public class Parallax : MonoBehaviour
{
    [Header("Parallax Settings")]
    [Range(0f, 1f)]
    public float parallaxSpeed = 0.5f;     // This layer's scroll speed
    
    [Header("Speed Settings")]
    public float baseScrollMultiplier = 0.15f; // Overall background scroll speed
    
    private MeshRenderer meshRenderer;
    private float textureOffsetX = 0f;
    private Material materialInstance;

    private void Awake()
    {
        meshRenderer = GetComponent<MeshRenderer>();
        
        if (meshRenderer != null)
        {
            materialInstance = meshRenderer.material;
            meshRenderer.material = materialInstance;
        }
    }

    private void Update()
    {
        if (!meshRenderer || materialInstance == null || GameManager.Instance == null)
            return;

        float parallaxMultiplier = GameManager.Instance.CurrentParallaxMultiplier;
        float scrollSpeed = GameManager.Instance.gameSpeed * parallaxSpeed * baseScrollMultiplier * parallaxMultiplier;
        
        textureOffsetX += scrollSpeed * Time.deltaTime;

        if (textureOffsetX > 1f)
        {
            textureOffsetX -= 1f;
        }

        materialInstance.mainTextureOffset = new Vector2(textureOffsetX, 0f);
    }

    public void ResetParallax()
    {
        textureOffsetX = 0f;
        if (materialInstance != null)
        {
            materialInstance.mainTextureOffset = Vector2.zero;
        }
    }

    private void OnDestroy()
    {
        if (materialInstance != null && Application.isPlaying)
        {
            Destroy(materialInstance);
        }
    }
}