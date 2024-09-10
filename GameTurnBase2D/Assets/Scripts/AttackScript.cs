using System.Collections;
using UnityEngine;

public class AttackScript : MonoBehaviour
{
    public GameObject owner;

    private bool magicAttack;
    private float magicCost;

    private float minAttackMultiplier;
    private float maxAttackMultiplier;

    private float minDefenseMultiplier;
    private float maxDefenseMultiplier;

    private FighterStats attackerStats;
    private FighterStats targetStats;
    private float damage = 0.0f;

    // Hàm để set các thông số của skill
    public void SetSkillParameters(Skill skill)
    {
        magicAttack = skill.IsMagicAttack();
        magicCost = skill.GetMagicCost();

        minAttackMultiplier = skill.GetMinAttackMultiplier();
        maxAttackMultiplier = skill.GetMaxAttackMultiplier();
        minDefenseMultiplier = skill.GetMinDefenseMultiplier();
        maxDefenseMultiplier = skill.GetMaxDefenseMultiplier(); 
    }

    public void Attack(GameObject victim)
    {
        attackerStats = owner.GetComponent<FighterStats>();
        targetStats = victim.GetComponent<FighterStats>();

        if (attackerStats.magic >= magicCost)
        {
            StartCoroutine(AttackAA());
        }
        else
        {
            Debug.LogWarning("Not enough magic to cast this skill.");
            Invoke("SkipTurnContinueGame", 2);
        }
    }

    private IEnumerator AttackAA()
    {
        // Thực hiện tấn công
        float multiplier = Random.Range(minAttackMultiplier, maxAttackMultiplier);
        damage = multiplier * attackerStats.attack;

        if (magicAttack)
        {
            damage = multiplier * attackerStats.magic;
        }

        float defenseMultiplier = Random.Range(minDefenseMultiplier, maxDefenseMultiplier);
        damage = Mathf.Max(0, damage - (defenseMultiplier * targetStats.defense));

        // Thực hiện tấn công vào mục tiêu
        targetStats.ReceiveDamage(Mathf.CeilToInt(damage));

        // Cập nhật năng lượng ma thuật của người tấn công
        attackerStats.UpdateMagicFill(magicCost);

        // Kết thúc lượt
        SkipTurnContinueGame();
        yield break;
    }

    private void SkipTurnContinueGame()
    {
        GameObject.Find("GameControllerObject").GetComponent<GameController>().NextTurn();
    }
}
