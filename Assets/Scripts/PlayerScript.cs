using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerScript : MonoBehaviour
{

    PlayerInput input;
    Rigidbody2D rb;
    Animator animator;
    [SerializeField] float movementForce = 500f;
    [SerializeField] float jumpForce = 500f;
    [SerializeField] bool IsGrounded = false;
    
    void Start()
    {

    }

    void Update()
    {
    }

}
