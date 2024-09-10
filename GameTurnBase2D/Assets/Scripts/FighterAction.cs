using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FighterAction : MonoBehaviour
{
    private GameObject enemy;
    private GameObject hero;

    [SerializeField] private GameObject meleePrefab;
    [SerializeField] private GameObject rangerPrefab;
    [SerializeField] private Sprite faceIcon;
    [SerializeField] private float moveSpeed = 15.0f;
    [SerializeField] private float attackRange = 1.5f;

    [SerializeField] private Ghost ghost;

    void Awake()
    {
        hero = GameObject.FindGameObjectWithTag("Hero");
        enemy = GameObject.FindGameObjectWithTag("Enemy");
    }

    public IEnumerator SelectAttack(int attackType, GameObject target)
    {
        GameObject victim = (tag == "Hero") ? enemy : hero;

        if (attackType == 0) // 0 for melee
        {
            yield return StartCoroutine(PerformMeleeAttack(target));
        }
        else if (attackType == 1) // 1 for range
        {
            yield return StartCoroutine(PerformRangeAttack(target));
        }
        else
        {
            Debug.Log("Invalid attack type");
        }
    }

    private IEnumerator PerformMeleeAttack(GameObject target)
    {
        Animator animator = GetComponent<Animator>();

        // Start the movement animation (if available)
        animator.SetBool("IsMoving", true);

        // Move towards the target
        while (Vector3.Distance(transform.position, target.transform.position) > attackRange)
        {
            transform.position = Vector3.MoveTowards(transform.position, target.transform.position, moveSpeed * Time.deltaTime);
            yield return null;
        }

        // Stop the movement animation
        animator.SetBool("IsMoving", false);

        // Perform the melee attack
        meleePrefab.GetComponent<AttackScript>().Attack(target);

        // Wait for the attack animation to finish
        yield return new WaitForSeconds(1.0f);

        // Perform the jump-back animation
        animator.SetBool("JumpBack", true);

        ghost.makeGhost = true;

        // Execute the two-step jump-back
        yield return StartCoroutine(JumpBackToInitialPosition());

        // End the jump-back animation
        animator.SetBool("JumpBack", false);
        ghost.makeGhost = false;
    }

    private IEnumerator JumpBackToInitialPosition()
    {
        Vector3 originalPosition = transform.position;
        Vector3 midPoint = (transform.position + GetComponent<FighterStats>().initialPosition) / 2;
        Vector3 finalPosition = GetComponent<FighterStats>().initialPosition;

        float jumpHeight = 1.5f; // Height of the jump
        float jumpDuration = 0.5f; // Duration of each jump

        // First jump-back step
        yield return StartCoroutine(JumpToPosition(midPoint, jumpHeight, jumpDuration));

        // Second jump-back step
        yield return StartCoroutine(JumpToPosition(finalPosition, jumpHeight, jumpDuration));

        // Ensure the character is precisely at the initial position
        transform.position = finalPosition;

        GameObject.Find("GameControllerObject").GetComponent<GameController>().NextTurn();
    }

    private IEnumerator JumpToPosition(Vector3 targetPosition, float jumpHeight, float duration)
    {
        Vector3 startPosition = transform.position;
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            float t = elapsedTime / duration;
            // Move along a parabolic curve to create a jump effect
            transform.position = Vector3.Lerp(startPosition, targetPosition, t) + Vector3.up * Mathf.Sin(t * Mathf.PI) * jumpHeight;

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // Ensure the character is exactly at the target position
        transform.position = targetPosition;
    }

    private IEnumerator PerformRangeAttack(GameObject target)
    {
        // Perform the range attack
        rangerPrefab.GetComponent<AttackScript>().Attack(target);

        // Wait for animation to finish
        yield return new WaitForSeconds(1.0f);
        // End turn
        GameObject.Find("GameControllerObject").GetComponent<GameController>().NextTurn();
    }
}
