using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class SpriteFlipper : MonoBehaviour
{
    SpriteRenderer spriteRenderer;
    private void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }
    void Update()
    {
        if (transform.parent != null)
        {
            float dot = Vector3.Dot(transform.parent.transform.forward, Vector3.right);
            spriteRenderer.flipX = dot < 0;
        }
    }
}
