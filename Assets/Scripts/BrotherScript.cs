using UnityEngine;

public class BrotherScript : MonoBehaviour
{
    [SerializeField] GameManagerScript gms;
    void Start()
    {
        
    }

    void Update()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other == null) return;
        if (!other.CompareTag("Player")) return;

        if (gms != null)
        {
            gms.WinGame();
        }
    }
}
