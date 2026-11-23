using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    public float moveSpeed = 3.5f;

    Rigidbody2D rb;
    Animator animator;
    Vector2 input;
    Vector2 lastDirection = Vector2.down;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        rb.gravityScale = 0;
        rb.freezeRotation = true;
        rb.interpolation = RigidbodyInterpolation2D.Interpolate;
        rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
    }

    void Update()
    {
        Keyboard kb = Keyboard.current;
        if (kb == null)
            return;

        input.x = 0;
        input.y = 0;

        if (kb.aKey.isPressed || kb.leftArrowKey.isPressed) input.x -= 1;
        if (kb.dKey.isPressed || kb.rightArrowKey.isPressed) input.x += 1;
        if (kb.sKey.isPressed || kb.downArrowKey.isPressed) input.y -= 1;
        if (kb.wKey.isPressed || kb.upArrowKey.isPressed) input.y += 1;

        // no diagonal walking, horizontal wins
        if (input.x != 0)
            input.y = 0;

        bool moving = input != Vector2.zero;

        if (moving)
            lastDirection = input;

        animator.SetBool("Moving", moving);
        animator.SetFloat("MoveX", lastDirection.x);
        animator.SetFloat("MoveY", lastDirection.y);
    }

    void FixedUpdate()
    {
        rb.linearVelocity = input * moveSpeed;
    }
}
