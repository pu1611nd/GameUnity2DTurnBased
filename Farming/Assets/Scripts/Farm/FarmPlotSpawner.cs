using System.Collections.Generic;
using UnityEngine;

public class FarmPlotManager : MonoBehaviour
{
    [Header("Prefab & Parent")]
    public GameObject plotPrefab;
    public Transform plotParent;

    [Header("Grid Settings")]
    public int columns = 5;
    public float spacing = 0.8f;

    [Header("Limits")]
    public int maxPlots = 48;

    [Header("Farm Data")]
    public Farm userFarm;

    [Header("UI")]
    public ConfirmDialog confirmDialog;

    private readonly List<GameObject> spawnedPlotObjects = new();
    private const int landPrice = 100;

    private void Start()
    {
        userFarm = UserManager.Instance.userInGame.UserFarm ?? new Farm();

        // Chờ crop data load xong mới spawn
        FarmManager.Instance.LoadCropData(() =>
        {
            if (userFarm.Plots != null && userFarm.Plots.Count > 0)
                SpawnFromExistingFarm();
            else
                GeneratePlotsFromCount(6);
        });
    }


    public void GeneratePlotsFromCount(int plotCount)
    {
        plotCount = Mathf.Clamp(plotCount, 0, maxPlots);
        ClearSpawnedPlots();

        userFarm.Plots = new List<PlotData>(plotCount);

        for (int i = 0; i < plotCount; i++)
        {
            PlotData pd = new PlotData(i);
            userFarm.Plots.Add(pd);
            Vector3 pos = plotParent.position + new Vector3((i % columns) * spacing, -(i / columns) * spacing, 0f);
            SpawnPlot(pd, pos, i);
        }
    }

    public void SpawnFromExistingFarm()
    {
        if (userFarm == null || userFarm.Plots == null) return;
        ClearSpawnedPlots();

        for (int i = 0; i < userFarm.Plots.Count; i++)
        {
            PlotData pd = userFarm.Plots[i];
            Vector3 pos = plotParent.position + new Vector3((i % columns) * spacing, -(i / columns) * spacing, 0f);
            SpawnPlot(pd, pos, i);
        }
    }

    private void SpawnPlot(PlotData pd, Vector3 pos, int index)
    {
        GameObject go = Instantiate(plotPrefab, pos, Quaternion.identity, plotParent);
        go.name = $"Plot_{index}";
        var farmPlot = go.GetComponent<FarmPlot>();
        if (farmPlot != null)
            farmPlot.LoadFromData(pd);

        spawnedPlotObjects.Add(go);
    }

    public void ClearSpawnedPlots()
    {
        foreach (var go in spawnedPlotObjects)
            if (go != null) Destroy(go);
        spawnedPlotObjects.Clear();
    }

    public void TryBuyNewPlot()
    {
        if (userFarm.Plots.Count >= maxPlots)
        {
            confirmDialog.ShowAlert("⚠️ Đã đạt giới hạn tối đa ô đất!");
            return;
        }

        var user = UserManager.Instance.userInGame;
        confirmDialog.Show(
            $"Bạn có muốn mua thêm 1 ô đất với giá <color=yellow>{landPrice}</color> vàng không?",
            () =>
            {
                if (user.Gold < landPrice)
                {
                    confirmDialog.ShowAlert("💸 Bạn không đủ vàng để mua ô đất!");
                    return;
                }

                user.Gold -= landPrice;
                AddPlotAndSpawn();
                UserManager.Instance.SaveUserData();
                confirmDialog.ShowAlert($"✅ Mua thành công! Còn lại {user.Gold} vàng.");
            });
    }

    public void AddPlotAndSpawn()
    {
        int nextIndex = userFarm.Plots.Count;
        if (nextIndex >= maxPlots) return;

        PlotData pd = new PlotData(nextIndex);
        userFarm.Plots.Add(pd);
        Vector3 pos = plotParent.position + new Vector3((nextIndex % columns) * spacing, -(nextIndex / columns) * spacing, 0f);
        SpawnPlot(pd, pos, nextIndex);
    }
}
