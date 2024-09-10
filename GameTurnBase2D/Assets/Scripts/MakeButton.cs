using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MakeButton : MonoBehaviour
{
    [SerializeField] private bool physical;
    [SerializeField] private GameObject enemyButtonPrefab; // Prefab của nút chọn kẻ thù
    public Transform enemySelectionPanel; // Panel để chứa các nút chọn kẻ thù
    public GameController gameManager;

    public Transform attackTarget;

    private GameObject hero;


    private int skillIndex =-1;


    void Start()
    {
        string temp = gameObject.name;
        gameObject.GetComponent<Button>().onClick.AddListener(() => AttachCallback(temp));
        hero = GameObject.FindGameObjectWithTag("Hero");

    }
    private void Update()
    {
        
    }

    private void AttachCallback(string btn)
    {

        

        if (hero != null)
        {
            if (btn == "MeleeBtn")
            {
                skillIndex = 1; // 0 for melee
                ShowEnemySelectionUI();
            }
            else if (btn == "RangeBtn")
            {
                skillIndex = 2; // 1 for range
                ShowEnemySelectionUI();
            }
            else if (btn == "BuffBtn")
            {
                skillIndex = 3; // 2 for buff (hoặc giá trị nào đó cho buff)
            }
            else
            {
                Debug.LogWarning("Unknown button type: " + btn);
                return;
            }

        }
        else
        {
            Debug.LogError("Hero GameObject không được tìm thấy!");
        }
    }

    public void ShowEnemySelectionUI()
    {
        // Kiểm tra các biến trước khi sử dụng
        if (enemySelectionPanel == null)
        {
            Debug.LogError("enemySelectionPanel chưa được gán trong Inspector!");
            return;
        }

        if (enemyButtonPrefab == null)
        {
            Debug.LogError("enemyButtonPrefab chưa được gán trong Inspector!");
            return;
        }

        if (gameManager == null || gameManager.enemiesOnField == null || gameManager.enemiesOnField.Count == 0)
        {
            Debug.LogError("GameManager hoặc danh sách enemiesOnField không hợp lệ!");
            return;
        }

        // Xóa các nút cũ trước khi thêm mới
        foreach (Transform child in enemySelectionPanel)
        {
            Destroy(child.gameObject);
        }

        // Tạo nút cho mỗi kẻ thù trên sân
        foreach (GameObject enemy in gameManager.enemiesOnField)
        {
            if (enemy == null) continue;
            GameObject button = Instantiate(enemyButtonPrefab, enemySelectionPanel);
            Button buttonComponent = button.GetComponent<Button>();
            Text buttonText = button.GetComponentInChildren<Text>();

            // Chuyển đổi vị trí của kẻ thù từ world space sang screen space
            Vector3 screenPos = Camera.main.WorldToScreenPoint(enemy.transform.position);

            // Chuyển đổi từ screen space của main camera sang world space của UI camera
            Vector3 worldPosUICam = Camera.main.ScreenToWorldPoint(screenPos);
            Vector3 screenPosUICam = Camera.main.WorldToScreenPoint(worldPosUICam);

            // Đặt vị trí của nút tại vị trí tương ứng trên màn hình của UI camera
            RectTransform buttonRect = button.GetComponent<RectTransform>();

            // Chuyển đổi từ tọa độ màn hình sang tọa độ của canvas (nếu cần)
            Vector2 localPos;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                enemySelectionPanel as RectTransform,
                screenPos,
                Camera.main,
                out localPos
            );

            buttonRect.localPosition = localPos;
            //buttonText.text = enemy.name; // Hiển thị tên hoặc ID của kẻ thù trên nút
            buttonComponent.onClick.AddListener(() => OnEnemySelected(enemy)); // Thêm sự kiện click cho nút
        }
    }

    public void OnEnemySelected(GameObject selectedEnemy)
    {
        // Thực hiện tấn công vào kẻ thù được chọn
        AttackEnemy(selectedEnemy);
        HideEnemySelectionUI(); // Ẩn giao diện lựa chọn khi đã chọn mục tiêu
    }

    void AttackEnemy(GameObject enemy)
    {
        HeroBase heroBase = hero.GetComponent<HeroBase>();
        // zoom camera

        heroBase.UseSkill(skillIndex, enemy);

        // Giảm máu của kẻ thù hoặc thực hiện hành động tấn công
        Debug.Log("Tấn công kẻ thù: " + skillIndex);
       // ResetCamera();
    }

    void HideEnemySelectionUI()
    {
        // Ẩn giao diện chọn kẻ thù
        foreach (Transform child in enemySelectionPanel)
        {
            Destroy(child.gameObject);
        }
    }





}
