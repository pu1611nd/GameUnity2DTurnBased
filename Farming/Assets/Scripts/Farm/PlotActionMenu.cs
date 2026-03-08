using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

public class PlotActionMenu : MonoBehaviour
{
    [Header("Buttons")]
    public Button buttonPlow;
    public Button buttonWater;
    public Button buttonPlant;
    public Button buttonHarvest;

    [Header("References")]
    public SeedSelectPanel seedSelectPanel;
    public GameObject toolPanel;
    public Text growthTimerText;
    public Slider growthProgressSlider; // Thanh tiến độ

    private Camera mainCam;
    private FarmPlot currentPlot;

    private void Start()
    {
        mainCam = Camera.main;
    }

    private void Update()
    {
        if (currentPlot != null)
        {
            UpdateGrowthUI();

            // 👇 Kiểm tra click ra ngoài menu để đóng
            if (Input.GetMouseButtonDown(0))
            {
                if (EventSystem.current == null)
                {
                    CloseCurrentMenu();
                    return;
                }

                bool overToolPanel = IsPointerOverUIObject(toolPanel);
                bool overSeedPanel = seedSelectPanel != null && IsPointerOverUIObject(seedSelectPanel.gameObject);

                if (!overToolPanel && !overSeedPanel)
                    CloseCurrentMenu();
            }
        }
    }

    public void ShowAt(FarmPlot plot)
    {
        currentPlot = plot;

        if (PlayerMovementPoint.Instance != null)
            PlayerMovementPoint.Instance.canClickToMove = false;

        if (toolPanel != null)
            toolPanel.SetActive(true);

        UpdateButtons(plot.data.stage);

        buttonPlow.onClick.RemoveAllListeners();
        buttonWater.onClick.RemoveAllListeners();
        buttonPlant.onClick.RemoveAllListeners();
        buttonHarvest.onClick.RemoveAllListeners();

        buttonPlow.onClick.AddListener(() =>
        {
            PlayerMovementPoint.Instance?.PlayHoeAnimation(plot);
            CloseCurrentMenu();
        });

        buttonWater.onClick.AddListener(() =>
        {
            PlayerMovementPoint.Instance?.PlayWaterAnimation(plot);
            CloseCurrentMenu();
        });

        buttonPlant.onClick.AddListener(() =>
        {
            if (toolPanel != null)
                toolPanel.SetActive(false);

            if (seedSelectPanel != null)
            {
                seedSelectPanel.gameObject.SetActive(true);
                seedSelectPanel.Show(plot);
            }
        });

        buttonHarvest.onClick.AddListener(() =>
        {
            plot.Harvest();
            CloseCurrentMenu();
        });
    }

    private void UpdateButtons(SoilStage stage)
    {
        buttonPlow?.gameObject.SetActive(false);
        buttonWater?.gameObject.SetActive(false);
        buttonPlant?.gameObject.SetActive(false);
        buttonHarvest?.gameObject.SetActive(false);

        switch (stage)
        {
            case SoilStage.Raw: buttonPlow?.gameObject.SetActive(true); break;
            case SoilStage.Plowed: buttonPlant?.gameObject.SetActive(true); break;
            case SoilStage.Seeded:
            case SoilStage.Watered:
            case SoilStage.Growing: buttonWater?.gameObject.SetActive(true); break;
            case SoilStage.Mature: buttonHarvest?.gameObject.SetActive(true); break;
        }
    }

    private void UpdateGrowthUI()
    {
        if (currentPlot == null || currentPlot.data == null || !currentPlot.data.HasCrop())
        {
            if (growthTimerText != null) growthTimerText.enabled = false;
            if (growthProgressSlider != null) growthProgressSlider.gameObject.SetActive(false);
            return;
        }

        var crop = currentPlot.GetCurrentCrop();
        if (crop == null) return;

        float progress = Mathf.Clamp01(currentPlot.data.growthTimer / crop.TotalGrowthTime);
        if (growthProgressSlider != null)
        {
            growthProgressSlider.gameObject.SetActive(true);
            growthProgressSlider.value = progress;
        }

        if (growthTimerText != null)
        {
            growthTimerText.enabled = true;

            float remaining = Mathf.Max(crop.TotalGrowthTime - currentPlot.data.growthTimer, 0f);
            int minutes = Mathf.FloorToInt(remaining / 60f);
            int seconds = Mathf.FloorToInt(remaining % 60f);

            switch (currentPlot.data.stage)
            {
                case SoilStage.Seeded:
                case SoilStage.Watered:
                case SoilStage.Growing:
                    growthTimerText.text = $"⏳ {minutes:00}:{seconds:00}s đến thu hoạch";
                    break;
                case SoilStage.Mature:
                    growthTimerText.text = "🌾 Sẵn sàng thu hoạch!";
                    if (growthProgressSlider != null) growthProgressSlider.value = 1f;
                    break;
                default:
                    growthTimerText.enabled = false;
                    if (growthProgressSlider != null) growthProgressSlider.gameObject.SetActive(false);
                    break;
            }
        }
    }

    private void CloseCurrentMenu()
    {
        currentPlot?.CloseMenu();
        currentPlot = null;

        if (toolPanel != null) toolPanel.SetActive(false);
        if (seedSelectPanel != null) seedSelectPanel.gameObject.SetActive(false);

        if (PlayerMovementPoint.Instance != null)
            PlayerMovementPoint.Instance.canClickToMove = true;
    }

    public void Hide() => CloseCurrentMenu();

    private void OnDisable()
    {
        if (PlayerMovementPoint.Instance != null)
            PlayerMovementPoint.Instance.canClickToMove = true;
    }

    private void OnDestroy()
    {
        if (PlayerMovementPoint.Instance != null)
            PlayerMovementPoint.Instance.canClickToMove = true;
    }


    private bool IsPointerOverUIObject(GameObject uiObject)
    {
        if (uiObject == null || EventSystem.current == null) return false;

        PointerEventData eventDataCurrentPosition = new PointerEventData(EventSystem.current)
        {
            position = Input.mousePosition
        };

        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventDataCurrentPosition, results);

        foreach (var r in results)
        {
            if (r.gameObject == uiObject || r.gameObject.transform.IsChildOf(uiObject.transform))
                return true;
        }

        return false;
    }
}
