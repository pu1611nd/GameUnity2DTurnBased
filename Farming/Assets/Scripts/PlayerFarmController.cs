using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class PlayerFarmController : MonoBehaviour
{

    public Tilemap tm_Ground;
    public Tilemap tm_Grass;

    public TileBase tb_Ground;
    public TileBase tb_Grass;


    private RecyclableManager RecyclableManager;

    public TileMapManager tileMapManager;

    // Start is called before the first frame update
    void Start()
    {
        RecyclableManager = GameObject.Find("InventoryManager").GetComponent<RecyclableManager>();
    }

    // Update is called once per frame
    void Update()
    {
        HandleFarmAction();
    }

    public void HandleFarmAction()
    {
        if (Input.GetKeyDown(KeyCode.C))
        {
            Vector3Int cellPos = tm_Ground.WorldToCell(transform.position);
            TileBase crrTileBse = tm_Grass.GetTile(cellPos);
            if(crrTileBse == tb_Grass)
            {
                tm_Grass.SetTile(cellPos, null);
                tileMapManager.SetStateForTilemapDeltal(cellPos.x, cellPos.y, TilemapState.Ground);
            }
        }
        if (Input.GetKeyDown(KeyCode.V))
        {
            Vector3Int cellPos = tm_Ground.WorldToCell(transform.position);
            TileBase crrTileBse = tm_Grass.GetTile(cellPos);
            if (crrTileBse == null)
            {
                //tm_tree.SetTile(cellPos, tb_tree);
                tileMapManager.SetStateForTilemapDeltal(cellPos.x, cellPos.y, TilemapState.Tree);
            }
        }
        if (Input.GetKeyDown(KeyCode.M))
        {
            Vector3Int cellPos = tm_Ground.WorldToCell(transform.position);
            //TileBase crrTileBse = tm_tree.GetTile(cellPos);
            //if (crrTileBse != null)
            //{

            //    tm_tree.SetTile(cellPos, null);
            //    tm_Grass.SetTile(cellPos, tb_Grass);
            //    //lay item vao tui do
            //    InvenItem itemFlower = new InvenItem();
            //    itemFlower.name = "Hoa 10h";
            //    itemFlower.descripsiton = "Hoa tuoi";
            //RecyclableManager.AddInventoryItem(itemFlower);
            //}

        }
    }

    
}
