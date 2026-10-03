using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerController : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float jumpForce = 8f;
    [SerializeField] private Transform groundCheck;
    [SerializeField] private float groundCheckRadius = 0.1f;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private float fallGravityMultiplier = 2.5f;
    [SerializeField] private float lowJumpGravityMultiplier = 2f;
    [Header("Wall Kick")]
    [SerializeField, Min(0.1f), Tooltip("壁から離れる横方向の速度")]
    private float wallKickSpeed = 7f;
    [SerializeField, Min(0.1f), Tooltip("通常ジャンプに対する壁キックの高さの速度倍率")]
    private float wallKickJumpMultiplier = 1f;
    [SerializeField, Min(0f), Tooltip("壁キック直後に横移動入力を抑える秒数")]
    private float wallKickControlLock = 0.18f;

    private Rigidbody2D rb;
    private SpriteRenderer spriteRenderer;
    private float moveInput;
    private bool jumpRequested;
    private bool jumpHeld;
    private bool isGrounded;
    private readonly ContactPoint2D[] groundContacts = new ContactPoint2D[16];
    private float wallKickLockedUntil;
    private float nextWallKickTime;
    private float wallKickHorizontalSpeed;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void Update()
    {
        var keyboard = Keyboard.current;
        moveInput = 0f;
        jumpHeld = false;
        if (keyboard != null)
        {
            if (keyboard.leftArrowKey.isPressed || keyboard.aKey.isPressed) moveInput -= 1f;
            if (keyboard.rightArrowKey.isPressed || keyboard.dKey.isPressed) moveInput += 1f;
            if (keyboard.spaceKey.wasPressedThisFrame) jumpRequested = true;
            jumpHeld = keyboard.spaceKey.isPressed;
        }
        if (spriteRenderer != null && moveInput != 0f && Time.time >= wallKickLockedUntil)
            spriteRenderer.flipX = moveInput < 0f;
    }

    private void FixedUpdate()
    {
        // Only real upward-facing contacts count: overlap queries also see the
        // underside of a one-way platform while the player passes through it.
        var groundFilter = new ContactFilter2D();
        groundFilter.SetLayerMask(groundLayer);
        groundFilter.useTriggers = false;
        int contactCount = rb.GetContacts(groundFilter, groundContacts);
        isGrounded = false;
        int wallKickDirection = 0;

        for (int i = 0; i < contactCount; i++)
        {
            var contact = groundContacts[i];
            var support = contact.collider.attachedRigidbody == rb ? contact.otherCollider : contact.collider;
            // Only solid terrain side contacts qualify. One-way platforms,
            // ceilings and sloped floors must not create an extra air jump.
            if (Mathf.Abs(contact.normal.x) > 0.85f && Mathf.Abs(contact.normal.y) < 0.35f &&
                !support.usedByEffector)
            {
                int away = contact.normal.x > 0f ? 1 : -1;
                if (wallKickDirection == 0 || moveInput * away < 0f) wallKickDirection = away;
            }
            if (contact.normal.y <= 0.65f) continue;
            if (rb.linearVelocity.y <= 0.1f) isGrounded = true;
        }
        if (isGrounded) wallKickLockedUntil = 0f;
        float horizontalSpeed = Time.time < wallKickLockedUntil
            ? wallKickHorizontalSpeed : moveInput * moveSpeed;
        rb.linearVelocity = new Vector2(horizontalSpeed,
            isGrounded ? 0f : rb.linearVelocity.y);

        if (jumpRequested && isGrounded)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
            isGrounded = false;
        }
        else if (jumpRequested && wallKickDirection != 0 && Time.time >= nextWallKickTime)
        {
            wallKickHorizontalSpeed = wallKickDirection * wallKickSpeed;
            rb.linearVelocity = new Vector2(wallKickHorizontalSpeed, jumpForce * wallKickJumpMultiplier);
            wallKickLockedUntil = Time.time + wallKickControlLock;
            nextWallKickTime = Time.time + Mathf.Max(0.12f, wallKickControlLock);

            if (spriteRenderer != null) spriteRenderer.flipX = wallKickDirection < 0;
        }
        jumpRequested = false;

        if (!isGrounded && rb.linearVelocity.y < 0f)
        {
            rb.linearVelocity += Vector2.up * (Physics2D.gravity.y * (fallGravityMultiplier - 1f) * Time.fixedDeltaTime);
        }
        else if (!isGrounded && rb.linearVelocity.y > 0f && !jumpHeld)
        {
            rb.linearVelocity += Vector2.up * (Physics2D.gravity.y * (lowJumpGravityMultiplier - 1f) * Time.fixedDeltaTime);
        }

    }

    private void OnDrawGizmosSelected()
    {
        if (groundCheck == null) return;
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
    }
}
