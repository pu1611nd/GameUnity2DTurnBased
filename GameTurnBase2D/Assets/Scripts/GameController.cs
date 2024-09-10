using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameController : MonoBehaviour
{
    private List<FighterStats> fighterStats;
    private GameObject battleMenu;
    public Text battleText;
    public List<GameObject> enemiesOnField;
    public List<GameObject> heroesOnField;

    private void Awake()
    {
        battleMenu = GameObject.Find("ActionMenu");
        if (battleMenu == null)
        {
            Debug.LogError("BattleMenu không được tìm thấy trong Scene!");
        }
    }

    private void Start()
    {
        fighterStats = new List<FighterStats>();
        enemiesOnField = new List<GameObject>();
        heroesOnField = new List<GameObject>();

        // Tìm và thêm hero vào danh sách
        GameObject[] heros = GameObject.FindGameObjectsWithTag("Hero");
        if (heros.Length > 0)
        {
            heroesOnField.AddRange(heros);
            foreach (GameObject hero in heros)
            {
                FighterStats heroStats = hero.GetComponent<FighterStats>();
                if (heroStats != null)
                {
                    heroStats.CalculateNextTurn(0);
                    fighterStats.Add(heroStats);
                }
            }
            Debug.LogWarning(heros.Length);
        }
        else
        {
            Debug.LogWarning("Không anh hung nào được tìm thấy trên sân!");
        }

        // Tìm và thêm các kẻ thù vào danh sách
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        if (enemies.Length > 0)
        {
            enemiesOnField.AddRange(enemies);
            foreach (GameObject enemy in enemies)
            {
                FighterStats enemyStats = enemy.GetComponent<FighterStats>();
                if (enemyStats != null)
                {
                    enemyStats.CalculateNextTurn(0);
                    fighterStats.Add(enemyStats);
                }
            }
            Debug.LogWarning(enemies.Length);
        }
        else
        {
            Debug.LogWarning("Không có kẻ thù nào được tìm thấy trên sân!");
        }

        // Sắp xếp theo thứ tự lượt đi
        fighterStats.Sort();
        battleMenu.SetActive(false);
        StartCoroutine(NextTurn());
    }

    public IEnumerator NextTurn()
    {
        battleText.gameObject.SetActive(false);

        if (fighterStats.Count == 0)
        {
            Debug.LogWarning("Không có chiến binh nào trong danh sách!");
            yield break;
        }

        FighterStats currentFighterStats = fighterStats[0];
        fighterStats.RemoveAt(0);

        if (!currentFighterStats.IsDead())
        {
            GameObject currentUnit = currentFighterStats.gameObject;
            currentFighterStats.CalculateNextTurn(currentFighterStats.nextActTurn);
            fighterStats.Add(currentFighterStats);
            fighterStats.Sort();

            if (currentUnit.CompareTag("Hero"))
            {
                battleMenu.SetActive(true);
                yield break;
            }
            else
            {
                battleMenu.SetActive(false);
                int attackType = Random.Range(0, 2); // Chọn ngẫu nhiên giữa 0 (cận chiến) và 1 (tầm xa)
                yield return StartCoroutine(currentUnit.GetComponent<FighterAction>().SelectAttack(attackType, currentUnit));
            }
        }
        else
        {
            Debug.Log(currentFighterStats.name + " đã chết, bỏ qua lượt.");
            // Loại bỏ kẻ thù đã chết khỏi danh sách enemiesOnField
            RemoveEnemy(currentFighterStats.gameObject);
            // Loại bỏ kẻ thù đã chết khỏi danh sách fighterStats
            fighterStats.Remove(currentFighterStats);
            StartCoroutine(NextTurn());
        }
    }

    public void RemoveEnemy(GameObject enemy)
    {
        if (enemiesOnField.Contains(enemy))
        {
            enemiesOnField.Remove(enemy);
            Destroy(enemy); // Hoặc chỉ loại bỏ nếu đối tượng vẫn cần tồn tại
        }
    }

}
