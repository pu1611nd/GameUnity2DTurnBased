using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.EventSystems.EventTrigger;

public enum SkillType
{
    Attack,
    Buff
}
[System.Serializable]
public class Skill
{
    [SerializeField] private string skillName;
    [SerializeField] public SkillType skillType;

    // Magic-specific Properties
    [Header("Magic Properties")]
    [SerializeField] private bool magicAttack;
    [SerializeField] private float magicCost;

    // Attack Modifiers
    [Header("Attack Modifiers")]
    [SerializeField] private float minAttackMultiplier;
    [SerializeField] private float maxAttackMultiplier;

    // Defense Modifiers
    [Header("Defense Modifiers")]
    [SerializeField] private float minDefenseMultiplier;
    [SerializeField] private float maxDefenseMultiplier;

    // Movement and Range
    [Header("Movement and Range")]
    [SerializeField] private float moveSpeed = 5.0f;
    [SerializeField] private float attackRange = 1.5f;

    // Visual and Sound Effects
    [Header("Visual and Sound Effects")]
    [SerializeField] private GameObject vfx;
    [SerializeField] private AudioClip sfx;

    // Getters
    public string GetSkillName() => skillName;
    public SkillType GetSkillType() => skillType;
    public bool IsMagicAttack() => magicAttack;
    public float GetMagicCost() => magicCost;
    public float GetMinAttackMultiplier() => minAttackMultiplier;
    public float GetMaxAttackMultiplier() => maxAttackMultiplier;
    public float GetMinDefenseMultiplier() => minDefenseMultiplier;
    public float GetMaxDefenseMultiplier() => maxDefenseMultiplier;
    public float GetMoveSpeed() => moveSpeed;
    public float GetAttackRange() => attackRange;
    public GameObject GetVFX() => vfx;
    public AudioClip GetSFX() => sfx;

    // Setters
    public void SetSkillName(string name) => skillName = name;
    public void SetSkillType(SkillType type) => skillType = type;
    public void SetMagicAttack(bool isMagic) => magicAttack = isMagic;
    public void SetMagicCost(float cost) => magicCost = cost;
    public void SetMinAttackMultiplier(float multiplier) => minAttackMultiplier = multiplier;
    public void SetMaxAttackMultiplier(float multiplier) => maxAttackMultiplier = multiplier;
    public void SetMinDefenseMultiplier(float multiplier) => minDefenseMultiplier = multiplier;
    public void SetMaxDefenseMultiplier(float multiplier) => maxDefenseMultiplier = multiplier;
    public void SetMoveSpeed(float speed) => moveSpeed = speed;
    public void SetAttackRange(float range) => attackRange = range;
    public void SetVFX(GameObject effect) => vfx = effect;
    public void SetSFX(AudioClip sound) => sfx = sound;
}



public abstract class HeroBase : MonoBehaviour
{


    [SerializeField] protected Animator animator;
    [SerializeField] public float moveSpeed;
    [SerializeField] protected List<Skill> skills;
    [SerializeField] private float jumpBackHeight = 1.5f;
    [SerializeField] private float jumpBackDuration = 0.5f;

    public CameraController cameraFollow;

    public void SetUpCamera()
    {
        cameraFollow.Setup(() => Vector3.zero, () => 5f, true, true);
        cameraFollow.SetCameraMoveSpeed(3f);
        cameraFollow.SetCameraZoomSpeed(3f);
        ResetCamera();

    }

    // skill
    public abstract void UseSkill(int skillIndex, GameObject target);

    // Di chuyển và tấn công
    public virtual IEnumerator MoveToTargetAndAttack(GameObject target)
    {
        Vector3 originalPosition = transform.position;

        // Di chuyển đến mục tiêu
        animator.SetBool("IsMoving", true);
        yield return StartCoroutine(MoveToPosition(target.transform.position, 1f, () =>
        {
            animator.SetBool("IsMoving", false);
        }));
        yield return new WaitForSeconds(.5f);
    }

    private IEnumerator MoveToPosition(Vector3 targetPosition, float distanceFromTarget, System.Action onReachTarget)
    {
        Vector3 directionToTarget = (targetPosition - transform.position).normalized;
        Vector3 newPosition = targetPosition - directionToTarget * distanceFromTarget;

        while (Vector3.Distance(transform.position, newPosition) > 0.5f)
        {
            transform.position = Vector3.MoveTowards(transform.position, newPosition, Time.deltaTime * moveSpeed);
            yield return null;
        }

        transform.position = newPosition;
        onReachTarget?.Invoke();
        
    }

    public virtual IEnumerator JumpBackToInitialPosition(Vector3 originalPosition)
    {
        Vector3 midPoint = (transform.position + originalPosition) / 2;
        animator.SetBool("JumpBack", true);
        yield return StartCoroutine(JumpToPosition(midPoint, jumpBackHeight, jumpBackDuration));
        yield return StartCoroutine(JumpToPosition(originalPosition, jumpBackHeight, jumpBackDuration));
        animator.SetBool("JumpBack", false);
    }

    private IEnumerator JumpToPosition(Vector3 targetPosition, float jumpHeight, float duration)
    {
        Vector3 startPosition = transform.position;
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            float t = elapsedTime / duration;
            transform.position = Vector3.Lerp(startPosition, targetPosition, t) + Vector3.up * Mathf.Sin(t * Mathf.PI) * jumpHeight;
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        transform.position = targetPosition;
    }

    protected void AddAnimationEvents(string animationClipName, System.Action eventAction)
    {
        AnimationClip clip = FindAnimationClip(animationClipName);
        if (clip != null)
        {
            AnimationEvent animationEvent = new AnimationEvent
            {
                time = clip.length * 0.5f, // Thời điểm mong muốn trong animation
                functionName = eventAction.Method.Name,
            };
            clip.AddEvent(animationEvent);
        }
        else
        {
            Debug.LogWarning($"Animation clip '{animationClipName}' not found.");
        }
    }

    private AnimationClip FindAnimationClip(string name)
    {
        AnimationClip[] clips = animator.runtimeAnimatorController.animationClips;
        foreach (AnimationClip clip in clips)
        {
            if (clip.name == name)
                return clip;
        }
        return null;
    }

    public void ResetCamera()
    {
        cameraFollow.SetCameraFollowPosition(Vector3.zero);
        cameraFollow.SetCameraZoom(5f);
    }

    public void SetCamera(Vector3 position, float zoom)
    {
        cameraFollow.SetCameraFollowPosition(position);
        cameraFollow.SetCameraZoom(zoom);
    }

    public SkillType GetSkillType(int skillIndex) => skills[skillIndex].skillType;

    public void EndTurn()
    {
        GameObject.Find("GameControllerObject").GetComponent<GameController>().NextTurn();
    }

    public Animator GetAnimator() => animator;

    public Vector3 GetInitialPosition() => GetComponent<FighterStats>().GetInitialPosition();
}
