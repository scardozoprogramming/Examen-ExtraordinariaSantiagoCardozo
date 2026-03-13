using UnityEngine;

public class BackgroundPlatformScript : MonoBehaviour
{
    [Header("Movimiento Background")]
    [SerializeField] private float minSpeed = 0.2f;
    [SerializeField] private float maxSpeed = 0.5f;
    [SerializeField] private Vector2 moveDirection = Vector2.left;

    [Header("Reaparición")]
    [Tooltip("Coordenada X en Viewport bajo la cual el objeto se reubica a la derecha")]
    [SerializeField] private float destroyViewportX = -0.05f;
    [Tooltip("Coordenada X en Viewport a la que se reubica cuando desaparece")]
    [SerializeField] private float respawnViewportX = 1.05f;
    [Tooltip("Variación vertical aleatoria al reubicar")]
    [SerializeField] private float respawnYVariance = 0.2f;

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

        // Si sale por la izquierda, reubicar a la derecha (wrap)
        if (vp.x < destroyViewportX)
        {
            Vector3 newVp = vp;
            newVp.x = respawnViewportX;
            // variar la posición Y ligeramente para evitar repetición exacta
            newVp.y = Mathf.Clamp01(newVp.y + Random.Range(-respawnYVariance, respawnYVariance));

            Vector3 newWorld = mainCamera.ViewportToWorldPoint(newVp);
            newWorld.z = transform.position.z; // conservar Z de la escena

            transform.position = newWorld;

            // renovar velocidad para variedad
            speed = Random.Range(minSpeed, maxSpeed);
        }
    }
}
