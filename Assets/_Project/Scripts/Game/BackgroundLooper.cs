using UnityEngine;

namespace _Project.Scripts.Game
{
    public class BackgroundLooper : MonoBehaviour
{
    [Header("Background Parts")]
    [SerializeField] private Transform bg1;
    [SerializeField] private Transform bg2;

    [Header("Renderers")]
    [SerializeField] private SpriteRenderer bgRenderer1;
    [SerializeField] private SpriteRenderer bgRenderer2;

    [Header("Level Backgrounds")]
    [SerializeField] private Sprite[] levelBackgrounds = new Sprite[5];

    [Header("Settings")]
    [SerializeField] private float speed = 2f;

    private float width;

    public float CurrentSpeed => speed;

    private void Start()
    {
        if (bgRenderer1 == null)
            bgRenderer1 = bg1.GetComponent<SpriteRenderer>();

        if (bgRenderer2 == null)
            bgRenderer2 = bg2.GetComponent<SpriteRenderer>();

        width = bgRenderer1.bounds.size.x;

        bg1.position = Vector3.zero;
        bg2.position = new Vector3(width, 0f, 0f);

        SetLevelBackground(0);
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

    public void SetLevelBackground(int levelIndex)
    {
        if (levelIndex < 0 || levelIndex >= levelBackgrounds.Length)
        {
            Debug.LogWarning("Неверный номер уровня: " + levelIndex);
            return;
        }

        if (levelBackgrounds[levelIndex] == null)
        {
            Debug.LogWarning("Фон для уровня не назначен: " + levelIndex);
            return;
        }

        bgRenderer1.sprite = levelBackgrounds[levelIndex];
        bgRenderer2.sprite = levelBackgrounds[levelIndex];

        width = bgRenderer1.bounds.size.x;

        bg1.position = Vector3.zero;
        bg2.position = new Vector3(width, 0f, 0f);

        Debug.Log("Фон изменён на уровень: " + (levelIndex + 1));
    }

    public void IncreaseSpeed(float amount)
    {
        speed += amount;
        Debug.Log("Скорость фона увеличена: " + speed);
    }

    public void SetSpeed(float newSpeed)
    {
        speed = newSpeed;
    }
}
}
