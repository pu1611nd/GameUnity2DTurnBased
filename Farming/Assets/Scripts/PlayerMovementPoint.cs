using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovementPoint : MonoBehaviour
{
    public Rigidbody2D rb;
    public float speed = 5f;
    public Animator animator;
    private bool isMoving = false;
    private Vector2 targetPosition;
    private Vector2 movement;

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            // Lấy vị trí chạm trên màn hình và chuyển sang tọa độ thế giới
            targetPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            isMoving = true;

            // Tính toán hướng di chuyển
            movement = (targetPosition - rb.position).normalized;

            // Thiết lập thông số animator
            animator.SetFloat("Horizontal", movement.x);
            animator.SetFloat("Vertical", movement.y);
        }

        // Cập nhật trạng thái di chuyển
        animator.SetFloat("Speed", isMoving ? 1 : 0);
    }

    void FixedUpdate()
    {
        if (isMoving)
        {
            // Di chuyển nhân vật
            rb.MovePosition(rb.position + movement * speed * Time.fixedDeltaTime);

            // Kiểm tra khoảng cách tới mục tiêu
            if (Vector2.Distance(rb.position, targetPosition) < 0.1f)
            {
                isMoving = false;
                movement = Vector2.zero;
            }
        }
    }
}
