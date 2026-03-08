using UnityEngine;

public class BuyPlotOnClick : MonoBehaviour
{
    public FarmPlotManager farmManager;

    private void OnMouseDown()
    {
        if (farmManager != null)
            farmManager.TryBuyNewPlot();
    }
}
