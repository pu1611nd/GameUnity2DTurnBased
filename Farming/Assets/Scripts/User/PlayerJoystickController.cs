using UnityEngine;
using UnityEngine.Tilemaps;

public class PlayerJoystickController : MonoBehaviour
{
    public Joystick joystick;
    public Rigidbody2D rb;
    public Animator animator;
    public float speed = 5f;


    [HideInInspector]
    public bool joystickActive = false;

    private void FixedUpdate()
    {
        HandleJoystickMovement();
    }

    private void HandleJoystickMovement()
    {
        if (joystick == null || rb == null ) return;

        Vector2 moveDir = new Vector2(joystick.Horizontal, joystick.Vertical);
        float moveSpeedFactor = Mathf.Clamp01(moveDir.magnitude);

        if (moveSpeedFactor > 0f)
        {
            joystickActive = true;

            Vector2 targetPos = rb.position + moveDir.normalized * speed * moveSpeedFactor * Time.fixedDeltaTime;


            rb.MovePosition(targetPos);

            if (animator != null)
            {
                animator.SetFloat("Horizontal", moveDir.x);
                animator.SetFloat("Vertical", moveDir.y);
                animator.SetFloat("Speed", moveSpeedFactor);
            }
        }
        else
        {
            joystickActive = false;
            if (animator != null)
                animator.SetFloat("Speed", 0f);
        }
    }
}
