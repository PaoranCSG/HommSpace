using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Unit", menuName = "", order = 0)]
public class Unit : ScriptableObject
{

    public string unitName;
    [TextArea(3,5)]
    public string description;
    public float alloyCost;
    public float creditCost;
    public float speed;
    public float turnSpeed;
    public float maxHealth;
    public List<Attack> attacks = new List<Attack>();
    public GameObject prefab;
    public bool isTempUnit;
    public List<SpawnUnit> spawnedUnits = new List<SpawnUnit>();

    
}
[System.Serializable]
public class SpawnUnit
{
    public float spawnSpeed;
    public GameObject spawnObject;
    public float spawnCounter;
    public Unit unit;
    public int maxUnitAmount;
    public int unitsSpawnedCounter;

}
