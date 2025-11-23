using UnityEngine;

[RequireComponent(typeof(MeshRenderer))]
public class Ground : MonoBehaviour
{
    private MeshRenderer meshRenderer;

    private void Awake()
    {
        meshRenderer = GetComponent<MeshRenderer>();
    }

    private void Update()
    {
        if (GameManager.Instance.isGameOver || GameManager.Instance.isPaused) return;
        
        float speed = GameManager.Instance.gameSpeed / 10f; // Slower for background
        meshRenderer.material.mainTextureOffset += speed * Time.deltaTime * Vector2.right;
    }
}