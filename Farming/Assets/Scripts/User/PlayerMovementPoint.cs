using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class PlayerMovementPoint : MonoBehaviour
{
    public static PlayerMovementPoint Instance;

    [Header("Player Settings")]
    public Rigidbody2D rb;
    public float speed = 5f;
    public Animator animator;
    public bool canClickToMove = true;

    [Header("Pathfinding")]
    public Pathfinding pathfinder;

    [Header("Joystick Controller (Optional)")]
    public PlayerJoystickController joystickController;

    private bool isMoving = false;
    private bool pathfindingActive = false;

    private Coroutine moveCoroutine;
    private Camera mainCam;


    private void Awake()
    {
        Instance = this;
        mainCam = Camera.main;
        rb.freezeRotation = true;
        rb.gravityScale = 0;
    }

    private void Update()
    {


        if (!canClickToMove) return;
        if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject()) return;

        // Chỉ click-to-move khi joystick không active
        if (Input.GetMouseButtonDown(0) && (joystickController == null || !joystickController.joystickActive))
        {
            Vector2 clickPos = mainCam.ScreenToWorldPoint(Input.mousePosition);
            MoveTo(clickPos);
        }

        if (animator != null)
            animator.SetFloat("Speed", isMoving ? 1 : 0);
    }

    public void MoveTo(Vector2 target, System.Action onArrive = null)
    {
        if (joystickController != null && joystickController.joystickActive)
        {
            StopMovement(); // hủy Coroutine pathfinding cũ hoàn toàn
            return;
        }

        if (moveCoroutine != null)
            StopCoroutine(moveCoroutine);

        moveCoroutine = StartCoroutine(MoveToTarget(target, onArrive));
    }


    private IEnumerator MoveToTarget(Vector2 target, System.Action onArrive)
    {
        isMoving = true;
        pathfindingActive = true;

        List<Vector3> path = pathfinder.FindPath(transform.position, target);
        if (path == null || path.Count == 0)
        {
            isMoving = false;
            pathfindingActive = false;
            yield break;
        }

        path.Add(target);

        foreach (Vector3 point in path)
        {
            while (Vector2.Distance(rb.position, point) > 0.05f)
            {
                // Nếu joystick active, hủy pathfinding ngay
                if (joystickController != null && joystickController.joystickActive)
                {
                    isMoving = false;
                    pathfindingActive = false;
                    yield break; // thoát Coroutine
                }

                Vector2 dir = (point - (Vector3)rb.position).normalized;
                rb.MovePosition(rb.position + dir * speed * Time.fixedDeltaTime);

                if ((joystickController == null || !joystickController.joystickActive) && animator != null)
                {
                    animator.SetFloat("Horizontal", dir.x);
                    animator.SetFloat("Vertical", dir.y);
                    animator.SetFloat("Speed", 1f);
                }

                yield return new WaitForFixedUpdate();
            }
        }

        rb.velocity = Vector2.zero;
        isMoving = false;
        pathfindingActive = false;

        if (animator != null) animator.SetFloat("Speed", 0f);

        onArrive?.Invoke();
    }


    public void StopMovement()
    {
        if (moveCoroutine != null)
            StopCoroutine(moveCoroutine);

        moveCoroutine = null;
        isMoving = false;
        pathfindingActive = false;

        if (animator != null)
            animator.SetFloat("Speed", 0f);
    }


    // 🪓 Cuốc đất
    public void PlayHoeAnimation(FarmPlot plot)
    {
        if (plot == null) return;

        Vector2 interactPos = (Vector2)plot.transform.position + new Vector2(-0.5f, 0f);
        float dist = Vector2.Distance(rb.position, interactPos);

        System.Action startHoe = () =>
        {
            canClickToMove = false;
            StartCoroutine(HoeRoutine(plot));
        };

        if (dist > 0.2f)
        {
            MoveTo(interactPos, () =>
            {
                canClickToMove = false;
                startHoe();
            });
        }
        else startHoe();
    }

    private IEnumerator HoeRoutine(FarmPlot plot)
    {
        if (animator != null) animator.SetBool("isHoe", true);
        yield return new WaitForSeconds(1.2f);
        if (animator != null) animator.SetBool("isHoe", false);
        plot?.Plow();
        canClickToMove = true;
    }

    // 💧 Tưới nước
    public void PlayWaterAnimation(FarmPlot plot)
    {
        if (plot == null) return;

        Vector2 interactPos = (Vector2)plot.transform.position + new Vector2(-0.5f, 0f);
        float dist = Vector2.Distance(rb.position, interactPos);

        System.Action startWater = () =>
        {
            canClickToMove = false;
            StartCoroutine(WaterRoutine(plot));
        };

        if (dist > 0.2f)
        {
            MoveTo(interactPos, () =>
            {
                canClickToMove = false;
                startWater();
            });
        }
        else startWater();
    }

    private IEnumerator WaterRoutine(FarmPlot plot)
    {
        if (animator != null) animator.SetBool("isWater", true);
        yield return new WaitForSeconds(1.2f);
        if (animator != null) animator.SetBool("isWater", false);
        plot?.Water();
        canClickToMove = true;
    }



    private void OnDestroy()
    {
        if (Instance == this) Instance = null;
    }
}
