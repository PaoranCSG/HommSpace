using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Fleet:MonoBehaviour
{
    public List<UnitGroup> units = new List<UnitGroup>();
    public bool isPlayerHero;
    
    public void AddToFleet(Unit unit,int amount)
    {
        foreach(UnitGroup group in units)
        {
            if(group.unit == unit)
            {
                group.stackCount += amount;
                return;
            }
        }
        UnitGroup unitGroup = new UnitGroup(unit,amount);
        units.Add(unitGroup);

    }
    public void RemoveFromFleet(UnitGroup unitGroup)
    {
        units.Remove(unitGroup);
    }
}

