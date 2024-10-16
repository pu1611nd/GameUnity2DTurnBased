using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;

public class InvenItem 
{
    public string name { get; set; }
    public string descripsiton { get; set; }

    public InvenItem()
    {

    }
    public InvenItem(string name , string descripsiton)
    {
        this.name = name;
        this.descripsiton = descripsiton;
    }

    public override string ToString()
    {
        return JsonConvert.SerializeObject(this);
    }
}
