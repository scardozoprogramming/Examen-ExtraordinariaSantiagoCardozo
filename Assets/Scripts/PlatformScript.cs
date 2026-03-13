using UnityEngine;

public class PlatformScript : MonoBehaviour
{
    [Header("Movimiento")]
    [SerializeField] private float minSpeed = 1f;
    [SerializeField] private float maxSpeed = 2f;
    [SerializeField] private Vector2 moveDirection = Vector2.left;
    [Tooltip("Coordenada X en Viewport bajo la cual el objeto se destruye (ej. -0.1)")]
    [SerializeField] private float destroyViewportX = -0.1f;

    private float speed;
    private Rigidbody2D rb;
    private Camera mainCamera;

    void Start()
    {
        speed = Random.Range(minSpeed, maxSpeed);
        rb = GetComponent<Rigidbody2D>();
        mainCamera = Camera.main;
    }

    void FixedUpdate()
    {
        Vector2 displacement = moveDirection.normalized * speed * Time.fixedDeltaTime;

        if (rb != null && rb.bodyType == RigidbodyType2D.Kinematic)
        {
            rb.MovePosition(rb.position + displacement);
        }
        else
        {
            transform.Translate(displacement, Space.World);
        }
    }

    void Update()
    {
        if (mainCamera == null)
            mainCamera = Camera.main;

        if (mainCamera == null)
            return;

        Vector3 vp = mainCamera.WorldToViewportPoint(transform.position);

        if (vp.x < destroyViewportX)
        {
            Destroy(gameObject);
        }
    }
}
