using System.Collections;
using System.Collections.Generic;
using PolyAndCode.UI;
using Unity.VisualScripting;
using UnityEditorInternal.VersionControl;
using UnityEngine;

public class RecyclableManager : MonoBehaviour, IRecyclableScrollRectDataSource
{
    [SerializeField]
    RecyclableScrollRect _recyclableScrollRect;
    [SerializeField]
    private int _dataLenght;

    public GameObject inventoryGameObject;
    //Dummy data List
    private List<InvenItem> _inventItem = new List<InvenItem>();
    //Recyclable scroll rect's data source must be assigned in Awake. ​

    private void Awake()
    {

        _recyclableScrollRect.DataSource = this;
    }

     public int GetItemCount()
    {
        return _inventItem.Count;
    }
    public void  SetCell​(ICell cell,int index)
    {
        //Casting to the implemented Cell
        var item = cell as CellItemData;
        item.ConfigureCell(_inventItem[index], index);
    }

    private void Start()
    {
        List<InvenItem> listItem = new List<InvenItem>();
        for(int i = 0; i<50; i++)
        {
            InvenItem invenItem = new InvenItem();
            invenItem.name = "Name_" + i.ToString();
            invenItem.descripsiton = "Descripsiton_" + i.ToString();
            listItem.Add(invenItem);
        }

        setListItem(listItem);
        _recyclableScrollRect.ReloadData();

    }

    public void setListItem(List<InvenItem> invenItems)
    {
        _inventItem = invenItems;
    }


    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.L))
        {

        }
        if (Input.GetKeyDown(KeyCode.B))
        {
            //inventoryGameObject.SetActive(!inventoryGameObject.activeSelf);
            Vector3 currPosInven = inventoryGameObject.GetComponent<RectTransform>().anchoredPosition;
            inventoryGameObject.GetComponent<RectTransform>().anchoredPosition = currPosInven.y == 1000 ? Vector3.zero : new Vector3(0, 1000, 0);
            
        }
    }

    public void AddInventoryItem( InvenItem item)
    {
        _inventItem.Add(item);
        _recyclableScrollRect.ReloadData();
    }
}