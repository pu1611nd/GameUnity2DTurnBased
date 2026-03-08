using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UserUIController : MonoBehaviour
{
    [Header("Sliders")]
    public Slider hpSlider;
    public Slider staminaSlider;

    [Header("Slider Texts")]
    public TextMeshProUGUI hpText;
    public TextMeshProUGUI staminaText;

    [Header("Currency Texts")]
    public TextMeshProUGUI goldText;
    public TextMeshProUGUI diamondText;

    private User currentUser;

    // Lưu giá trị cũ để tránh update liên tục mỗi frame
    private int lastHP = -1;
    private int lastMaxHP = -1;
    private int lastStamina = -1;
    private int lastMaxStamina = -1;
    private int lastGold = -1;
    private int lastDiamond = -1;

    private void Start()
    {
        if (UserManager.Instance != null)
        {
            currentUser = UserManager.Instance.userInGame;
            RefreshUI(true); // Lần đầu update tất cả
        }
    }

    private void Update()
    {
        if (currentUser != null)
        {
            RefreshUI();
        }
    }

    private void RefreshUI(bool force = false)
    {
        // HP
        if (currentUser.HP != lastHP || currentUser.MaxHP != lastMaxHP || force)
        {
            lastHP = currentUser.HP;
            lastMaxHP = currentUser.MaxHP;

            if (hpSlider != null)
            {
                hpSlider.maxValue = currentUser.MaxHP;
                hpSlider.value = currentUser.HP;
            }

            if (hpText != null)
                hpText.text = $"{currentUser.HP}/{currentUser.MaxHP}";
        }

        // Stamina
        if (currentUser.Stamina != lastStamina || currentUser.MaxStamina != lastMaxStamina || force)
        {
            lastStamina = currentUser.Stamina;
            lastMaxStamina = currentUser.MaxStamina;

            if (staminaSlider != null)
            {
                staminaSlider.maxValue = currentUser.MaxStamina;
                staminaSlider.value = currentUser.Stamina;
            }

            if (staminaText != null)
                staminaText.text = $"{currentUser.Stamina}/{currentUser.MaxStamina}";
        }

        // Gold
        if (currentUser.Gold != lastGold || force)
        {
            lastGold = currentUser.Gold;
            if (goldText != null)
                goldText.text = currentUser.Gold.ToString("N0");
        }

        // Diamond
        if (currentUser.Diamond != lastDiamond || force)
        {
            lastDiamond = currentUser.Diamond;
            if (diamondText != null)
                diamondText.text = currentUser.Diamond.ToString("N0");
        }
    }
}
