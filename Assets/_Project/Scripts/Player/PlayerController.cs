using _Project.Scripts.ObjScripts;
using UnityEngine;
using UnityEngine.InputSystem;

namespace _Project.Scripts.Player
{
    [RequireComponent(typeof(Rigidbody2D), typeof(CapsuleCollider2D))]
    public class PlayerController : MonoBehaviour
    {
        private PlayerHealth playerHealth;

        [Header("Components")]
        [SerializeField] private Animator animator;

        [Header("Jump")]
        [SerializeField] private float jumpForce = 14f;
        [SerializeField] private int maxJumps = 3;

        [Header("Better Jump Gravity")]
        [SerializeField] private float upwardGravityScale = 3.5f;
        [SerializeField] private float fallGravityScale = 8f;
        [SerializeField] private float groundedGravityScale = 3f;
        [SerializeField] private float maxFallSpeed = 22f;

        [Header("Slide")]
        [SerializeField] private float slideHoldTime = 0.18f;
        [SerializeField] private Vector2 slideColliderSize = new Vector2(0.8f, 0.5f);

        [Header("Ground Check")]
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

        private static readonly int IsGroundedHash = Animator.StringToHash("IsGrounded");
        private static readonly int IsJumpingHash = Animator.StringToHash("IsJumping");
        private static readonly int IsFallingHash = Animator.StringToHash("IsFalling");
        private static readonly int IsSlidingHash = Animator.StringToHash("IsSliding");

        private void Awake()
        {
            playerHealth = GetComponent<PlayerHealth>();
            rb = GetComponent<Rigidbody2D>();
            playerCollider = GetComponent<CapsuleCollider2D>();

            originalColliderSize = playerCollider.size;
            rb.gravityScale = groundedGravityScale;

            if (animator == null)
            {
                animator = GetComponentInChildren<Animator>();
            }
        }

        private void Update()
        {
            CheckGround();
            HandleInput();
            StopSlideIfInAir();
            UpdateAnimator();
        }

        private void FixedUpdate()
        {
            ApplyBetterJumpGravity();
        }

        private void CheckGround()
        {
            if (groundCheckPoint == null)
                return;

            isGrounded = Physics2D.OverlapCircle(
                groundCheckPoint.position,
                groundCheckRadius,
                groundLayer
            );

            if (isGrounded)
            {
                jumpsRemaining = maxJumps;
            }
        }

        private void HandleInput()
        {
            Keyboard keyboard = Keyboard.current;

            if (keyboard == null)
                return;

            if (keyboard.spaceKey.wasPressedThisFrame)
            {
                pressTime = Time.time;
            }

            if (keyboard.spaceKey.isPressed && isGrounded && !isSliding)
            {
                float holdTime = Time.time - pressTime;

                if (holdTime >= slideHoldTime)
                {
                    StartSlide();
                }
            }

            if (keyboard.spaceKey.wasReleasedThisFrame)
            {
                float holdTime = Time.time - pressTime;

                if (isSliding)
                {
                    StopSlide();
                    return;
                }

                if (holdTime < slideHoldTime)
                {
                    TryJump();
                }
            }
        }

        private void TryJump()
        {
            if (!isGrounded && jumpsRemaining <= 0)
                return;

            if (!isGrounded)
            {
                jumpsRemaining--;
            }

            StopSlide();

            rb.linearVelocity = new Vector2(rb.linearVelocity.x, 0f);
            rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
        }

        private void ApplyBetterJumpGravity()
        {
            if (isGrounded && rb.linearVelocity.y <= 0.05f)
            {
                rb.gravityScale = groundedGravityScale;
                return;
            }

            if (rb.linearVelocity.y > 0.1f)
            {
                rb.gravityScale = upwardGravityScale;
            }
            else if (rb.linearVelocity.y < -0.1f)
            {
                rb.gravityScale = fallGravityScale;
            }

            if (rb.linearVelocity.y < -maxFallSpeed)
            {
                rb.linearVelocity = new Vector2(rb.linearVelocity.x, -maxFallSpeed);
            }
        }

        private void StartSlide()
        {
            if (isSliding)
                return;

            isSliding = true;
            playerCollider.size = slideColliderSize;
        }

        private void StopSlide()
        {
            if (!isSliding)
                return;

            isSliding = false;
            playerCollider.size = originalColliderSize;
        }

        private void StopSlideIfInAir()
        {
            if (isSliding && !isGrounded)
            {
                StopSlide();
            }
        }

        private void UpdateAnimator()
        {
            if (animator == null)
                return;

            bool isJumping = !isGrounded && rb.linearVelocity.y > 0.1f;
            bool isFalling = !isGrounded && rb.linearVelocity.y < -0.1f;

            animator.SetBool(IsGroundedHash, isGrounded);
            animator.SetBool(IsJumpingHash, isJumping);
            animator.SetBool(IsFallingHash, isFalling);
            animator.SetBool(IsSlidingHash, isSliding);
        }

        private void OnDrawGizmos()
        {
            if (groundCheckPoint != null)
            {
                Gizmos.color = Color.red;
                Gizmos.DrawWireSphere(groundCheckPoint.position, groundCheckRadius);
            }
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            BonusMb bonusMb = other.GetComponent<BonusMb>();

            if (bonusMb == null)
            {
                bonusMb = other.GetComponentInParent<BonusMb>();
            }

            if (bonusMb != null)
            {
                bonusMb.PickUpBonus();
                return;
            }

            EnemyMb enemyMb = other.GetComponent<EnemyMb>();

            if (enemyMb == null)
            {
                enemyMb = other.GetComponentInParent<EnemyMb>();
            }

            if (enemyMb != null)
            {
                enemyMb.HitPlayer(playerHealth);
            }
        }
    }
}