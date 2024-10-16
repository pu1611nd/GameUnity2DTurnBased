using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;

public class Map
{
    public List<TilemapDeltal> listTilemapDeltals { get; set; }

    public Map(List<TilemapDeltal> map)
    {
        this.listTilemapDeltals = map;
    }

    public Map()
    {
    }

    public override string ToString()
    {
        return JsonConvert.SerializeObject(this);
    }

    public int GetLenght()
    {
        return listTilemapDeltals.Count;
    }

}
