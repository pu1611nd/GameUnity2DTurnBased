using System.Collections;
using System.Collections.Generic;
using System.Xml;
using PolyAndCode.UI;
using UnityEngine;
using UnityEngine.UI;
using static UnityEditor.ShaderData;

public class CellItemData : MonoBehaviour, ICell
{
    //UI
    public Text nameLabel;
    public Text desLabel;
    //Model
    private InvenItem _contactInfo;
    private int  _cellIndex;
    //This is called from the SetCell method in DataSource

    public void ConfigureCell(InvenItem invenItem, int cellIndex)
        {

            _cellIndex = cellIndex;
            _contactInfo = invenItem;
            nameLabel.text = invenItem.name;
            desLabel.text = invenItem.descripsiton;
           
        }
}
