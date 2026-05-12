using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D), typeof(CapsuleCollider2D))]
public class PlayerController : MonoBehaviour
{
    [SerializeField] private float jumpForce = 12f;
    [SerializeField] private int maxJumps = 3;
    
    [SerializeField] private Vector2 slideColliderSize = new Vector2(0.8f, 0.5f);
    [SerializeField] private Vector2 normalColliderSize = new Vector2(0.8f, 1.2f);
    
    [SerializeField] private float groundCheckRadius = 0.2f;
    [SerializeField] private Transform groundCheckPoint;
    [SerializeField] private LayerMask groundLayer;
    
    private Rigidbody2D rb;
    private CapsuleCollider2D playerCollider;
    private Vector2 originalColliderSize;
    
    private bool isGrounded;
    private int jumpsRemaining;
    private bool isSliding;
    private float pressTime;
    private bool isSpaceHeld;
    
    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        playerCollider = GetComponent<CapsuleCollider2D>();
        originalColliderSize = playerCollider.size;
    }
    
    private void Update()
    {
        isGrounded = Physics2D.OverlapCircle(groundCheckPoint.position, groundCheckRadius, groundLayer);
        
        if (isGrounded)
        {
            jumpsRemaining = maxJumps;
        }
        
        Keyboard keyboard = Keyboard.current;
        if (keyboard == null) return;
        
        if (keyboard.spaceKey.wasPressedThisFrame)
        {
            pressTime = Time.time;
            isSpaceHeld = true;
        }
        
        if (keyboard.spaceKey.wasReleasedThisFrame)
        {
            float holdTime = Time.time - pressTime;
            isSpaceHeld = false;
            
            if (holdTime < 0.2f && !isSliding)
            {
                if (isGrounded || jumpsRemaining > 0)
                {
                    if (!isGrounded)
                    {
                        jumpsRemaining--;
                    }
                    
                    rb.linearVelocity = new Vector2(rb.linearVelocity.x, 0f);
                    rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
                    Debug.Log("Jump!");
                }
            }
            
            if (isSliding)
            {
                isSliding = false;
                playerCollider.size = originalColliderSize;
                Debug.Log("Slide end");
            }
        }
        
        if (keyboard.spaceKey.isPressed && isGrounded && !isSliding && Time.time - pressTime > 0.2f)
        {
            isSliding = true;
            playerCollider.size = slideColliderSize;
            Debug.Log("Slide start");
        }
        
        if (isSliding && !isGrounded)
        {
            isSliding = false;
            playerCollider.size = originalColliderSize;
            Debug.Log("Slide end (in air)");
        }
        
        Debug.Log($"Grounded: {isGrounded}, JumpsLeft: {jumpsRemaining}, Sliding: {isSliding}");
    }
    
    private void OnDrawGizmos()
    {
        if (groundCheckPoint != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(groundCheckPoint.position, groundCheckRadius);
        }
    }
}