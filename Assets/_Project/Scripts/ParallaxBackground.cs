using UnityEngine;

public class ParallaxBackground : MonoBehaviour
{
    public Transform[] backgrounds;
    public float speed = 2f;

    private float backgroundWidth;

    private void Start()
    {
        SpriteRenderer sr = backgrounds[0].GetComponent<SpriteRenderer>();
        backgroundWidth = sr.bounds.size.x;
    }

    private void Update()
    {
        foreach (Transform bg in backgrounds)
        {
            bg.position += Vector3.left * speed * Time.deltaTime;

            if (bg.position.x <= -backgroundWidth)
            {
                bg.position += Vector3.right * backgroundWidth * backgrounds.Length;
            }
        }
    }
}