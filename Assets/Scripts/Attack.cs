using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Attack 
{
    public string attackName;
    public GameObject projectile;
    public float attackRange;
    public float attackSpeed;
    public float attackDamage;
    public float moveSpeed;
    public bool useBarrel = false;
    public Targeting targeting;
    
}
public enum AttackType
{
    missile,laser
}


