using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class UnitGroup 
{
    public Unit unit;
    public int stackCount;
    public UnitGroup(Unit unit,int stackCount)
    {
        this.unit = unit;
        this.stackCount = stackCount;
    }
}
