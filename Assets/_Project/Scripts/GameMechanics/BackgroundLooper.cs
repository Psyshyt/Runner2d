using UnityEngine;

public class BackgroundLooper : MonoBehaviour
{
    [Header("Background Parts")]
    [SerializeField] private Transform bg1;
    [SerializeField] private Transform bg2;

    [Header("Settings")]
    [SerializeField] private float speed = 2f;

    private float width;

    private void Start()
    {
        SpriteRenderer sr = bg1.GetComponent<SpriteRenderer>();

        width = sr.bounds.size.x;

        bg1.position = Vector3.zero;
        bg2.position = new Vector3(width, 0f, 0f);
    }

    private void Update()
    {
        Move(bg1);
        Move(bg2);

        Loop(bg1, bg2);
        Loop(bg2, bg1);
    }

    private void Move(Transform bg)
    {
        bg.position += Vector3.left * speed * Time.deltaTime;
    }

    private void Loop(Transform current, Transform other)
    {
        if (current.position.x <= -width)
        {
            current.position = new Vector3(
                other.position.x + width,
                current.position.y,
                current.position.z
            );
        }
    }
}
