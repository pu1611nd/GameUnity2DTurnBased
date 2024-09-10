using System.Collections;
using UnityEngine;

public class Samurai : HeroBase
{
    [SerializeField] private Ghost ghost;
    [SerializeField] private AttackScript attackScript;
    [SerializeField] private GameObject skill1VFX;
    [SerializeField] private GameObject skill2VFX;
    [SerializeField] private GameObject skill3VFX;

    private Skill skill1;
    private Skill skill2;

    private GameObject targetEnemy;

    void Start()
    {

        // Kiểm tra nếu AttackScript không được gắn
        if (attackScript == null)
        {
            attackScript = gameObject.AddComponent<AttackScript>();
        }

        // Khởi tạo và thiết lập các kỹ năng
        skill1 = new Skill();
        skill1.SetSkillName("Skill 1");
        skill1.SetMagicAttack(false);
        skill1.SetMagicCost(0f);
        skill1.SetSkillType(SkillType.Attack);
        skill1.SetMinAttackMultiplier(1.5f);
        skill1.SetMaxAttackMultiplier(2.0f);
        skill1.SetVFX(skill1VFX); // skill1VFX được khởi tạo ở bên ngoài

        skill2 = new Skill();
        skill2.SetSkillName("Skill 2");
        skill2.SetMagicAttack(false);
        skill2.SetMagicCost(10f);
        skill2.SetSkillType(SkillType.Attack);
        skill2.SetMinAttackMultiplier(1.5f);
        skill2.SetMaxAttackMultiplier(2.0f);
        skill2.SetVFX(skill1VFX);


        AddAnimationEvents("Melee", OnSkill1Impact);
        AddAnimationEvents("Range", OnSkill2Impact);
        AddAnimationEvents("Skill3Animation", OnSkill3Impact);

        //SetUpCamera();
    }

    public override void UseSkill(int skillIndex, GameObject target)
    {
        targetEnemy = target;
        SetUpCamera();
        SetCamera(targetEnemy.transform.position + new Vector3(-2f, 0), 3f);
        switch (skillIndex)
        {
            case 1:
                StartCoroutine(ExecuteSkill1());
                
                break;
            case 2:
                StartCoroutine(ExecuteSkill2());
                break;
            case 3:
                StartCoroutine(ExecuteSkill3());
                break;
            default:
                Debug.LogWarning("Invalid skill index");
                break;
        }
    }

    private IEnumerator ExecuteSkill1()
    {
        Vector3 originalPosition = transform.position;
        yield return StartCoroutine(MoveToTargetAndAttack(targetEnemy));
        animator.Play("Melee");
        yield return new WaitForSeconds(0.5f); // Đợi để skill impact xảy ra
        ResetCamera();
        ghost.makeGhost = true;
        yield return StartCoroutine(JumpBackToInitialPosition(originalPosition));
        ghost.makeGhost = false;
    }

    private IEnumerator ExecuteSkill2()
    {
        Vector3 originalPosition = transform.position;
        yield return StartCoroutine(MoveToTargetAndAttack(targetEnemy));
        animator.Play("Range");
        yield return new WaitForSeconds(0.5f); // Đợi để skill impact xảy ra
        ResetCamera();
        ghost.makeGhost = true;
        yield return StartCoroutine(JumpBackToInitialPosition(originalPosition));
        ghost.makeGhost = false;
    }

    private IEnumerator ExecuteSkill3()
    {
        animator.Play("Skill3Animation");
        yield return StartCoroutine(MoveToTargetAndAttack(targetEnemy));
    }

    // Hàm được gọi bởi Animation Events
    private void OnSkill1Impact()
    {
        PlayVFX(targetEnemy.transform.position, skill1VFX);
        attackScript.SetSkillParameters(skill1);
        attackScript.Attack(targetEnemy);
    }

    private void OnSkill2Impact()
    {
        PlayVFX(targetEnemy.transform.position, skill2VFX);
        attackScript.SetSkillParameters(skill2);
        attackScript.Attack(targetEnemy);
    }

    private void OnSkill3Impact()
    {
        PlayVFX(targetEnemy.transform.position, skill3VFX);
        // GetComponent<AttackScript>().Attack(targetEnemy);
    }

    private void PlayVFX(Vector3 position, GameObject vfx)
    {
        if (vfx != null)
        {
            GameObject instantiatedVFX = Instantiate(vfx, position, Quaternion.identity);
            Destroy(instantiatedVFX, 2f); // Giả sử VFX cần khoảng 2 giây để phát
        }
    }
}
