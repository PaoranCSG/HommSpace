using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Building",menuName = "",order =0)]
public class Building : ScriptableObject
{
    public int level = 0;
    public int buildTime;
    public List<BuildingUpgrade> buildingUpgrades = new List<BuildingUpgrade>();
    public string buildingName;
    [TextArea(3,5)]
    public string description;
    

}
public enum ProductionType
{
    resource,units,heroes,upgrades
}
[System.Serializable]
public class Production
{
    public ProductionType type;
    public Resource resource;
    public Unit unit;
    public float amount;
    public int maxAmount;
    public int everyXRoundsCounter;
    public int everyXRounds;
    
}
public enum Resource
{
    alloys,credits
}
[System.Serializable]
public class BuildingUpgrade
{
    public float alloyCost;
    public float creditCost;
    public int buildTime;
    [TextArea(3,5)]
    public string description;
    public List<Production> productions = new List<Production>();

}
