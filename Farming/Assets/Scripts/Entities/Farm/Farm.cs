using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Farm
{
    public List<PlotData> Plots;        // ô đất trồng
    public List<AnimalBarn> Barns;       // danh sách chuồng chăn nuôi

    public Farm()
    {
        Plots = new List<PlotData>();
        Barns = new List<AnimalBarn>();
    }

    /// <summary>
    /// Tạo 6 ô đất mặc định, tất cả mở.
    /// Đồng bộ với PlotData mới (CropId = PlotData.NoCrop)
    /// </summary>
    public void InitDefaultPlots(int columns = 3)
    {
        int initialPlots = 6;  // chỉ tạo 6 ô đất
        Plots = new List<PlotData>(initialPlots);

        for (int i = 0; i < initialPlots; i++)
        {
            PlotData pd = new PlotData(i);
            Plots.Add(pd);
        }

        Debug.Log($"[Farm] Initialized {initialPlots} plots (all unlocked).");
    }

    /// <summary>
    /// Khởi tạo các chuồng mặc định (chuồng gà, bò, lợn).
    /// </summary>
    public void InitDefaultBarns()
    {
        Barns = new List<AnimalBarn>
        {
            new AnimalBarn("Chicken", 1),
            new AnimalBarn("Cow", 1),
            new AnimalBarn("Pig", 1)
        };
        Debug.Log("[Farm] Initialized default barns: Chicken, Cow, Pig");
    }

    public PlotData GetPlotById(int id)
    {
        return Plots?.Find(p => p.plotID == id);
    }

    public int GetUnlockedPlotCount()
    {
        return Plots?.Count ?? 0;
    }


    public AnimalBarn GetBarnByType(string type)
    {
        return Barns?.Find(b => b.Type.Equals(type, StringComparison.OrdinalIgnoreCase));
    }
}
