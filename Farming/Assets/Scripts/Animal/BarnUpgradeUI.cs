using UnityEngine;

public class BarnUpgradeUI : MonoBehaviour
{
    public string barnType;   // "Chicken" / "Cow" / "Pig"
    public int upgradeCost = 100; // bạn có thể đổi tùy loại

    public ConfirmDialog dialog;

    // Khi click vào GameObject (chuồng)
    private void OnMouseDown()
    {
        ShowUpgradeConfirm();
    }

    private void ShowUpgradeConfirm()
    {
        var barn = AnimalBarnManager.Instance.GetBarnByType(barnType);
        if (barn == null)
        {
            dialog.ShowAlert("Không tìm thấy chuồng!");
            return;
        }

        string msg = $"Bạn có muốn nâng cấp chuồng {barnType}?\n" +
                     $"Cấp hiện tại: {barn.Level}\n" +
                     $"Phí nâng cấp: {upgradeCost} vàng";

        dialog.Show(msg, () =>
        {
            TryUpgradeBarn();
        });
    }

    private void TryUpgradeBarn()
    {
        var user = UserManager.Instance.userInGame;

        if (user.Gold < upgradeCost)
        {
            dialog.ShowAlert("Không đủ vàng để nâng cấp!");
            return;
        }

        bool ok = AnimalBarnManager.Instance.UpgradeBarn(barnType);
        if (ok)
        {
            user.Gold -= upgradeCost;
            UserManager.Instance.SaveUserData();

            dialog.ShowAlert($"Nâng cấp chuồng {barnType} thành công!");
        }
        else
        {
            dialog.ShowAlert("Chuồng đã đạt cấp tối đa!");
        }
    }
}
