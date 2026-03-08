using UnityEngine;
using UnityEngine.UI;
using System;

public class ConfirmDialog : MonoBehaviour
{
    [Header("UI Components")]
    public GameObject panel;
    public Text messageText;
    public Button confirmButton;
    public Button cancelButton;

    private Action onConfirm;
    private bool isAlertOnly = false;

    private void Start()
    {
        if (panel != null)
            panel.SetActive(false);
    }

    // 🟢 Hiện popup xác nhận (Có / Hủy)
    public void Show(string message, Action onConfirmAction)
    {
        isAlertOnly = false;
        SetupButtons();

        messageText.text = message;
        onConfirm = onConfirmAction;
        panel.SetActive(true);
    }

    // 🟡 Hiện popup thông báo đơn giản (chỉ có OK)
    public void ShowAlert(string message)
    {
        isAlertOnly = true;
        SetupButtons();

        messageText.text = message;
        onConfirm = null;
        panel.SetActive(true);
    }

    private void SetupButtons()
    {
        confirmButton.onClick.RemoveAllListeners();
        cancelButton.onClick.RemoveAllListeners();

        if (isAlertOnly)
        {
            confirmButton.gameObject.SetActive(true);
            cancelButton.gameObject.SetActive(false);

            confirmButton.onClick.AddListener(Hide);
        }
        else
        {
            confirmButton.gameObject.SetActive(true);
            cancelButton.gameObject.SetActive(true);

            confirmButton.onClick.AddListener(() =>
            {
                onConfirm?.Invoke();
                Hide();
            });

            cancelButton.onClick.AddListener(Hide);
        }
    }

    public void Hide()
    {
        panel.SetActive(false);
    }
}
