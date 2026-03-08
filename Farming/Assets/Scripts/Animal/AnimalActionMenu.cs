using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System;

public class AnimalActionMenu : MonoBehaviour, IPointerClickHandler
{
    public static AnimalActionMenu Instance;

    [Header("UI References")]
    public GameObject panel;
    public Text animalNameText;
    public Text growthInfoText;
    public Slider growthProgressBar;
    public Button feedButton;
    public Button sellButton;
    public Button cancelButton;

    [Header("Interaction")]
    public Transform playerTransform;
    public float interactRange = 2f;

    // NOTE: changed from Animal -> AnimalBase
    private AnimalBase currentAnimal;
    private AnimalBarn currentBarn;
    private AnimalData currentData;
    private Transform targetTransform;
    private Camera mainCam;

    private void Awake()
    {
        Instance = this;
        mainCam = Camera.main;
        if (panel != null) panel.SetActive(false);
    }

    private void Update()
    {
        if (panel == null) return;
        if (!panel.activeSelf || targetTransform == null) return;

        UpdatePanelPosition();

        if (currentAnimal != null && currentData != null)
            UpdateGrowthUI();

        // Nếu click ra ngoài UI thì đóng
        if (Input.GetMouseButtonDown(0) && !IsPointerOverUI(panel))
            Hide();
    }

    public void Show(AnimalBase animalBase, AnimalBarn barn)
    {
        if (animalBase == null || barn == null) return;

        // Kiểm tra khoảng cách tới người chơi (nếu có)
        if (playerTransform != null && Vector3.Distance(playerTransform.position, animalBase.transform.position) > interactRange)
            return;

        // Gán đúng kiểu
        currentAnimal = animalBase;
        currentBarn = barn;
        currentData = animalBase.TemplateData;
        targetTransform = animalBase.transform;

        if (currentData == null)
        {
            Debug.LogWarning("AnimalActionMenu: TemplateData null, không thể mở UI.");
            return;
        }

        panel.SetActive(true);
        UpdatePanelPosition();
        RefreshUI();
    }

    private void RefreshUI()
    {
        if (currentAnimal == null || currentData == null) return;

        // Cập nhật hunger / trạng thái từ dữ liệu instance
        currentAnimal.Data?.UpdateHunger(currentData);

        animalNameText.text = $"{currentData.type} #{currentAnimal.Data?.IdAnimal ?? 0}";

        bool canBeSold = currentAnimal.Data != null && currentAnimal.Data.CanBeSold(currentData);

        feedButton.gameObject.SetActive(true);
        sellButton.gameObject.SetActive(canBeSold);
        cancelButton.gameObject.SetActive(!canBeSold);

        // Remove old listeners
        feedButton.onClick.RemoveAllListeners();
        sellButton.onClick.RemoveAllListeners();
        cancelButton.onClick.RemoveAllListeners();

        // Add new listeners
        feedButton.onClick.AddListener(FeedAnimal);
        sellButton.onClick.AddListener(SellAnimal);
        cancelButton.onClick.AddListener(RemoveAnimal);
    }

    private void UpdatePanelPosition()
    {
        if (panel == null || targetTransform == null) return;

        // Offset dựa vào chiều cao sprite
        float spriteHeight = 1.5f; // điều chỉnh nếu cần
        panel.transform.position = targetTransform.position + Vector3.up * spriteHeight;

        // Nếu muốn panel xoay theo camera (luôn nhìn camera)
        panel.transform.rotation = Camera.main.transform.rotation;
    }


    private void UpdateGrowthUI()
    {
        if (currentAnimal == null || currentData == null || currentAnimal.Data == null) return;

        float age = currentAnimal.Data.AgeSeconds;
        float progress = Mathf.Clamp01(age / currentData.matureTime);
        if (growthProgressBar != null) growthProgressBar.value = progress;

        if (currentAnimal.Data.CanBeSold(currentData))
        {
            growthInfoText.text = $"✅ Trưởng thành — bán được {currentData.sellPrice} vàng";
        }
        else
        {
            float remain = Mathf.Max(0, currentData.matureTime - age);
            TimeSpan t = TimeSpan.FromSeconds(remain);
            growthInfoText.text = $"⏳ Trưởng thành sau: {t.Hours:D2}:{t.Minutes:D2}:{t.Seconds:D2}";
        }
    }

    private void FeedAnimal()
    {
        if (currentAnimal?.Data == null) return;

        currentAnimal.Data.Feed();
        currentAnimal.OnFed(); // gọi behavior trên scene (particle, anim)
        UserManager.Instance?.SaveUserData();
        Hide();
    }

    private void SellAnimal()
    {
        if (currentAnimal?.Data == null || currentBarn == null) { Hide(); return; }

        bool sold = AnimalBarnManager.Instance?.SellAnimal(currentBarn.Type, currentAnimal.Data.IdAnimal) ?? false;
        if (sold) UserManager.Instance?.SaveUserData();
        Hide();
    }

    private void RemoveAnimal()
    {
        if (currentAnimal?.Data == null || currentBarn == null) { Hide(); return; }

        currentBarn.RemoveAnimal(currentAnimal.Data.IdAnimal);
        AnimalManager.Instance?.RemoveAnimalFromScene(currentAnimal.Data.IdAnimal);
        UserManager.Instance?.SaveUserData();
        Hide();
    }

    private void Hide()
    {
        if (panel != null) panel.SetActive(false);
        currentAnimal = null;
        currentBarn = null;
        currentData = null;
        targetTransform = null;
    }

    private bool IsPointerOverUI(GameObject uiElement)
    {
        if (uiElement == null) return false;
        Vector2 mousePos = Input.mousePosition;
        var rect = uiElement.GetComponent<RectTransform>();
        return rect != null && RectTransformUtility.RectangleContainsScreenPoint(rect, mousePos, mainCam);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (panel == null) return;
        var rect = panel.GetComponent<RectTransform>();
        if (rect == null) return;

        if (!RectTransformUtility.RectangleContainsScreenPoint(rect, eventData.position, mainCam))
            Hide();
    }
}
