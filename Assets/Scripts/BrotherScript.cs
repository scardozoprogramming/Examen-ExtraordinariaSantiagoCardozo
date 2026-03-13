using UnityEngine;

public class BrotherScript : MonoBehaviour
{
    [SerializeField] GameManagerScript gms;
    void Start()
    {
        
    }

    // Update is called once per frame
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
