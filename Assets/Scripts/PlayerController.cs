using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    private float horizontal;
    public float speed = 8f;
    public float jumpingPower = 16f;
    private bool isFacingRight = true;
    public bool hasDoubleJump = false;
    public bool isFlippingGravity = false;
    public bool gameHasEnded = false;
    public TextMeshProUGUI skillLastTimeText;
    public TextMeshProUGUI skillCDText;

    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private Transform groundCheck;
    [SerializeField] private LayerMask groundLayer;

    void Update()
    {

        if (gameHasEnded)
        {
            return;
        }

        horizontal = Input.GetAxisRaw("Horizontal");

        if (Input.GetButtonDown("Jump") && IsGrounded())
        {
            rb.velocity = new Vector2(rb.velocity.x, jumpingPower);
        }

        if (Input.GetButtonUp("Jump") && rb.velocity.y > 0f)
        {
            rb.velocity = new Vector2(rb.velocity.x, rb.velocity.y * 0.5f);
        }

        // double jump
        if (Input.GetButtonDown("Jump") && !IsGrounded() && !hasDoubleJump)
        {
            rb.velocity = new Vector2(rb.velocity.x, jumpingPower);
            hasDoubleJump = true;
        }

        if (IsGrounded())
        {
            hasDoubleJump = false;
        }

        Flip();


        // if press E, flip the gravity for 5 seconds
        if (Input.GetKeyDown(KeyCode.E) && !isFlippingGravity)
        {
            Physics2D.gravity = new Vector2(Physics2D.gravity.x, Physics2D.gravity.y * -1);
            StartCoroutine(FlipGravity());
            isFlippingGravity = true;
        }
    }

    private void FixedUpdate()
    {
        rb.velocity = new Vector2(horizontal * speed, rb.velocity.y);
    }

    private bool IsGrounded()
    {
        return Physics2D.OverlapCircle(groundCheck.position, 0.2f, groundLayer);
    }

    private void Flip()
    {
        if (isFacingRight && horizontal < 0f || !isFacingRight && horizontal > 0f)
        {
            isFacingRight = !isFacingRight;
            Vector3 localScale = transform.localScale;
            localScale.x *= -1f;
            transform.localScale = localScale;
        }
    }

    IEnumerator FlipGravity()
    {
        // flip gravity for 3 seconds; update skillLastTimeText to be the remaining time
        float timeLeft = 3;
        while (timeLeft > 0)
        {
            skillLastTimeText.text = "Skill Last Time: " + timeLeft.ToString("F1") + "s";
            timeLeft -= Time.deltaTime;
            yield return null;
        }

        // reset gravity
        Physics2D.gravity = new Vector2(Physics2D.gravity.x, Physics2D.gravity.y * -1);
        StartCoroutine(FlipGravityCD());

    }

    IEnumerator FlipGravityCD()
    {
        // flip gravity CD for 5 seconds; update skillCDText to be the remaining time
        float timeLeft = 5;
        while (timeLeft > 0)
        {
            skillCDText.text = "Skill CD: " + timeLeft.ToString("F1") + "s";
            timeLeft -= Time.deltaTime;
            yield return null;
        }

        // reset CD
        isFlippingGravity = false;
        skillCDText.text = "Skill CD: Ready";
    }
}