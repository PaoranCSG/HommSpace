using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Planet : MonoBehaviour
{
    public int level;
    public List<PlanetBuilding> buildings = new List<PlanetBuilding>();
    public Fleet fleet;

    private void Awake()
    {
        
    }

}
