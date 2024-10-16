using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;


 public enum TilemapState
{
    Ground,
    Grass,
    Tree
}
public class TilemapDeltal
{
    public int x { get; set; }

    public int y { get; set; }

    public TilemapState tilemapState { get; set; }


    public TilemapDeltal()
    {

    }

     public TilemapDeltal(int x , int y , TilemapState tilemapState)
    {
        this.x = x;
        this.y = y;
        this.tilemapState = tilemapState;
    }

    public override string ToString()
    {
        return JsonConvert.SerializeObject(this);
    }

}
