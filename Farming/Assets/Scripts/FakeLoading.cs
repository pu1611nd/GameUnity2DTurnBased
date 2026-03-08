using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LoadingUI : MonoBehaviour
{
    public Image progressFill;
    public TMP_Text progressText;

    private void Start()
    {
        if (LoadDataManager.Instance != null)
            LoadDataManager.Instance.OnProgressChanged += UpdateProgress;
    }

    private void OnDestroy()
    {
        if (LoadDataManager.Instance != null)
            LoadDataManager.Instance.OnProgressChanged -= UpdateProgress;
    }

    private void UpdateProgress(float value)
    {
        if (progressFill != null)
            progressFill.fillAmount = value;
        if (progressText != null)
            progressText.text = $"{Mathf.RoundToInt(value * 100)}%";
    }
}
