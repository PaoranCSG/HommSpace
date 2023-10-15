using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Splines;
using Unity.Mathematics;
using Interpolators = UnityEngine.Splines.Interpolators;


[System.Serializable]
public class Lane
{
    public Location startLocation;
    public Location endLocation;
    public SplineContainer container;
    public bool isOppositeDirection;
    

}

