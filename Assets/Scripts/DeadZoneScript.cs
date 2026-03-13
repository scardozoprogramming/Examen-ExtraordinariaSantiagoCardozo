using UnityEngine;

public class DeadZoneScript : MonoBehaviour
{

    [SerializeField] GameObject playerSpawnPoint;
    [SerializeField] GameObject life1;
    [SerializeField] GameObject life2;
    [SerializeField] GameObject life3;
    [SerializeField] GameManagerScript gms;

    private GameObject[] lifeIcons;

    void Start()
    {
        lifeIcons = new GameObject[] { life1, life2, life3 };
    }

    void Update()
    {

    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other == null) return;
        if (!other.CompareTag("Player")) return;
        
        if (gms != null)
            gms.LoseLife();

        if (lifeIcons != null)
        {
            for (int i = lifeIcons.Length - 1; i >= 0; i--)
            {
                if (lifeIcons[i] != null && lifeIcons[i].activeSelf)
                {
                    lifeIcons[i].SetActive(false);
                    break;
                }
            }
        }

        if (playerSpawnPoint != null)
        {
            var player = other.gameObject;
            player.transform.position = playerSpawnPoint.transform.position;

            var rb = player.GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                rb.linearVelocity = Vector2.zero;
                rb.angularVelocity = 0f;
            }
        }
    }
}
