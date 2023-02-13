using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    private float horizontal;
    private float currentHorizontal = 0;
    private float maxspeed = 11f;
    private float jumpPower = 270f;
    private bool isFacingRight = true;
    private float acceleration = 0.06f;

    private bool canDash = true;
    private bool isDashing;
    private float dashingPower = 600f;
    private float dashingTime = 0.075f;
    private float dashingCooldown = 0.8f;

    private float dJumpPower = 200f;
    private bool canDJump;

    [SerializeField] private Camera cam;
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private Transform groundCheck;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private TrailRenderer tr;
    [SerializeField] private AudioSource soundPlayer;

    // Update is called once per frame
    void Update()
    {
        if (horizontal != 0 && Mathf.Abs(currentHorizontal) < maxspeed)
        {
            currentHorizontal += acceleration * horizontal;
        }
        if ((horizontal * currentHorizontal) < 0 || horizontal == 0)
        {
            currentHorizontal /= 1.1f;
        }

        horizontal = Input.GetAxisRaw("Horizontal");

        Flip();

        if (isGrounded() == true)
        {
            canDJump = true;
        }

        if (Input.GetButtonDown("Jump"))
        {           
            if (isGrounded() == true)
            {
                rb.velocity = new Vector2(rb.velocity.x, jumpPower);
            }
            else if (canDJump == true)
            {
                rb.velocity = new Vector2(rb.velocity.x, dJumpPower);
                canDJump = false;
            }
        }

        if (Input.GetButtonUp("Jump") && rb.velocity.y > 0f)
        {
            rb.velocity = new Vector2(rb.velocity.x, rb.velocity.y * 0.5f);
        }

        if (Input.GetKeyDown(KeyCode.Mouse0) && canDash == true)
        {
            StartCoroutine(Dash());
        }
    }

    //Checks if the player is grounded
    private bool isGrounded()
    {
        return Physics2D.OverlapCircle(groundCheck.position, 0.75f, groundLayer);
    }

    //Allows user to affect horizontal velocity
    private void FixedUpdate()
    {
        if (isDashing == false)
        {
            rb.velocity = new Vector2(currentHorizontal * maxspeed, rb.velocity.y);
        }

        if (Input.GetKeyDown(KeyCode.Mouse0) && canDash == true)
        {
            StartCoroutine(Dash());
        }
    }

    //Checks velocity of the player to determine if they sprite should be flipped
    private void Flip()
    {
        if (isDashing == false)
        {
            if (isFacingRight && horizontal < 0f || !isFacingRight && horizontal > 0f)
            {
                isFacingRight = !isFacingRight;
                Vector3 localScale = transform.localScale;
                localScale.x *= -1f;
                transform.localScale = localScale;
            }
    }   }

    private IEnumerator Dash()
    {
        canDash = false;
        isDashing = true;
        soundPlayer.Play();

        Vector3 mousePos = cam.ScreenToWorldPoint(Input.mousePosition);
        mousePos.z = cam.nearClipPlane;
        Vector3 charPos = new Vector3(rb.position.x, rb.position.y, cam.nearClipPlane);
        Vector3 difference = mousePos - charPos;
        difference = difference.normalized;
        rb.velocity = new Vector2(0, 0);

        rb.velocity = new Vector2(difference.x * 700, difference.y * 700);
        tr.emitting = true;
        yield return new WaitForSeconds(dashingTime);
        isDashing = false;
        rb.velocity = new Vector2(rb.velocity.x, rb.velocity.y * 0.27f);
        tr.emitting = false;
        yield return new WaitForSeconds(dashingCooldown);
        canDash = true;
    }
}