using System.Collections;
using System.Collections.Generic;
using Firebase;
using Firebase.Auth;
using Firebase.Database;
using Firebase.Extensions;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TileMapManager : MonoBehaviour
{

    public Tilemap tm_Ground;
    public Tilemap tm_Grass;

    public TileBase tb_tree;

    private FirebaseDataBaseManager dataBaseManager;

    private FirebaseUser user;

    private DatabaseReference reference;

    private Map map;

    private void Start()
    {
        map = new Map();
        user = FirebaseAuth.DefaultInstance.CurrentUser;
        dataBaseManager = GameObject.Find("DataBaseManager").GetComponent<FirebaseDataBaseManager>();
        //WirteAllTileMapToFirebase();

        FirebaseApp app = FirebaseApp.DefaultInstance;
        reference = FirebaseDatabase.DefaultInstance.RootReference;
        LoadMapForUser();
    }
    public void WirteAllTileMapToFirebase()
    {
        List<TilemapDeltal> tilemaps = new List<TilemapDeltal>();
        for(int x = tm_Ground.cellBounds.min.x; x <= tm_Ground.cellBounds.max.x; x++)
        {
            for (int y = tm_Ground.cellBounds.min.y; y <= tm_Ground.cellBounds.max.y; y++)
            {
                TilemapDeltal tm_deltel = new TilemapDeltal(x,y,TilemapState.Ground);
                tilemaps.Add(tm_deltel);
            }
        }

        map = new Map(tilemaps);
        dataBaseManager.WriteDatabase(user.UserId +"/Map",map.ToString());
    }

    public void LoadMapForUser()
    {
        reference.Child("Users").Child(user.UserId+"/Map").GetValueAsync().ContinueWithOnMainThread(task =>
        {
            if (task.IsCompleted)
            {
                DataSnapshot snapshot = task.Result;
                map = JsonConvert.DeserializeObject<Map>(snapshot.Value.ToString());
                MapToUi(map);
            }
            else
            {
                Debug.Log("doc that bai" + task.Exception);
            }
        });

    }

    public void TilemapDeltalToTileBase(TilemapDeltal tilemapDeltal)
    {
        Vector3Int cellPoss = new Vector3Int(tilemapDeltal.x, tilemapDeltal.y, 0);
        if(tilemapDeltal.tilemapState == TilemapState.Ground)
        {
            tm_Grass.SetTile(cellPoss, null);
            //tm_tree.SetTile(cellPoss, null);
        }else if(tilemapDeltal.tilemapState == TilemapState.Grass)
        {
            //tm_tree.SetTile(cellPoss, null);
        }else if (tilemapDeltal.tilemapState == TilemapState.Tree)
        {
            tm_Grass.SetTile(cellPoss, null);

            //tm_Tree.SetTile(cellPoss, tb_tree);
        }
    }

    public void MapToUi(Map map)
    {
        for(int i = 0; i < map.GetLenght(); i++)
        {
            TilemapDeltalToTileBase(map.listTilemapDeltals[i]);
        }
    }

    public void SetStateForTilemapDeltal(int x , int y , TilemapState state)
    {
        for (int i = 0; i < map.GetLenght(); i++)
        {
            if (map.listTilemapDeltals[i].x == x && map.listTilemapDeltals[i].y == y)
            {
                map.listTilemapDeltals[i].tilemapState = state;
                dataBaseManager.WriteDatabase(user.UserId + "/Map", map.ToString());
            }
        }
    }

}
