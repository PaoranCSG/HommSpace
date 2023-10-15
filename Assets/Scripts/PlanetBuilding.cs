using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PlanetBuilding
{
    public int buildingLevel;
    public Building building;
    public int currentRound;
    public List<Production> currentProductions = new List<Production>();
    
}
