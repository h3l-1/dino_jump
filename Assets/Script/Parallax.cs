using UnityEngine;

[RequireComponent(typeof(MeshRenderer))]
public class Parallax : MonoBehaviour
{
    [Range(0f, 1f)]
    public float parallaxSpeed = 0.5f; // smaller = slower (farther layer)

    private MeshRenderer meshRenderer;
    private float textureOffsetX = 0f;

    private void Awake()
    {
        meshRenderer = GetComponent<MeshRenderer>();
    }

    private void Update()
    {
        if (!meshRenderer || !meshRenderer.material)
            return;

        // Move texture offset over time to create parallax effect
        textureOffsetX += GameManager.Instance.gameSpeed * parallaxSpeed * Time.deltaTime;

        meshRenderer.material.mainTextureOffset = new Vector2(textureOffsetX, 0f);
    }
}
