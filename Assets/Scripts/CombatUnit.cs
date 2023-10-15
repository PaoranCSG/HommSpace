using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CombatUnit : MonoBehaviour
{
    public bool isEnemyShip;
    public float maxHealth;
    public float health;
    public Unit unit;
    public float turnSpeed;
    public float moveSpeed;
   
    public Targeting targeting;
    public TargetDistance targetDistance;
    public RotationTarget rotationTarget;
    public List<Attack> attackList = new List<Attack>();
    public List<float> attackCounters = new List<float>();
    public List<Transform> attackPositions = new List<Transform>();
    public GameObject target;
    public float wantedDistanceFromOpponent;
    public List<Barrel> barrels = new List<Barrel>();
    public List<SpawnUnit> spawnUnits = new List<SpawnUnit>();
    public List<Transform> spawnPositions = new List<Transform>();
    
    
    private float retargetTimer = 0;
    private int nonBarrelAttacks = 0;
    private void Awake()
    {
       
        
    }
    public void DesignateEnemyShip()
    {
        gameObject.tag = "EnemyShip";
        gameObject.layer = LayerMask.NameToLayer("EnemyShip");
        foreach(Barrel barrel in barrels)
        {
            barrel.isEnemyBarrel = true;
        }
    }
    private void Start()
    {
        if (isEnemyShip)
        {
            GetComponent<SpriteRenderer>().color = GameController.instance.enemyColor;
            DesignateEnemyShip();
        }
        else
        {
            GetComponent<SpriteRenderer>().color = GameController.instance.playerColor;
        }

        SetupShip();
        health = maxHealth;
        foreach (Attack a in attackList)
        {
            attackCounters.Add(0);
            if (!a.useBarrel)
            {
                nonBarrelAttacks++;
            }
            wantedDistanceFromOpponent = a.attackRange;
        }
        if (barrels.Count > 0)
        {
            for (int i = 0; i < attackList.Count; i++)
            {
                if (i > 0)
                {
                    barrels[i - nonBarrelAttacks].maxRange = attackList[i].attackRange;
                }


            }
        }
      

    }
    public List<CombatUnit> opposingUnits = new List<CombatUnit>();
    public float strafeDirection = 1;

    private float returnDistanceTarget;
    private void Update()
    {
        if (returnToField)
        {
            target = GameController.instance.goTowardsPoint;
            if (Vector2.Distance(transform.position,target.transform.position) < returnDistanceTarget)
            {
                returnToField = false;
                target = null;
            }
            else
            {
                Vector2 current = transform.up;


                Vector2 to = target.transform.position - transform.position;
                transform.up = Vector3.RotateTowards(current, to, turnSpeed * Mathf.Deg2Rad * Time.deltaTime, 1); ;
                transform.position += moveSpeed * Time.deltaTime * transform.up;
            }
            
        }
        else
        {
            if (target == null)
            {
                if (targeting == Targeting.closest)
                {
                    float distance = float.MaxValue;
                    foreach (CombatUnit unit in opposingUnits)
                    {
                        if (distance > Vector2.Distance(transform.position, unit.transform.position))
                        {
                            target = unit.gameObject;
                            distance = Vector2.Distance(transform.position, unit.transform.position);
                        }
                    }
                }
                else if (targeting == Targeting.strongest)
                {
                    float health = float.MinValue;
                    foreach (CombatUnit unit in opposingUnits)
                    {
                        if (unit.health > health)
                        {
                            health = unit.health;
                            target = unit.gameObject;

                        }
                    }



                }
                if (Random.value > 0.5f)
                {
                    strafeDirection = -0.5f;
                }
                else
                {
                    strafeDirection = 0.5f;
                }
            }
            else
            {

                if (wantedDistanceFromOpponent <= Vector2.Distance(transform.position, target.transform.position))
                {
                    retargetTimer += Time.deltaTime;
                    if (retargetTimer > 0.25f)
                    {
                        retargetTimer = 0;
                        target = null;
                        Debug.Log("Retargeting");
                    }
                    else
                    {
                        Vector2 current = transform.up;


                        Vector2 to = target.transform.position - transform.position;
                        transform.up = Vector3.RotateTowards(current, to, turnSpeed * Mathf.Deg2Rad * Time.deltaTime, 1); ;
                        transform.position += moveSpeed * Time.deltaTime * transform.up;
                        //transform.right = target.transform.position - transform.position;
                    }

                }
                else
                {



                    Vector3 desiredFacingDirection = transform.right;
                    if (rotationTarget == RotationTarget.front)
                    {
                        desiredFacingDirection = transform.up;
                    }
                    else if (rotationTarget == RotationTarget.right)
                    {
                        desiredFacingDirection = transform.right;
                    }
                    Vector2 current = transform.up;
                    Vector2 to = target.transform.position - transform.position;
                    transform.up = Vector3.RotateTowards(current, to, turnSpeed * Mathf.Deg2Rad * Time.deltaTime, 1);
                    transform.position = transform.position + desiredFacingDirection * Time.deltaTime * moveSpeed * strafeDirection;


                    //transform.right = target.transform.position - transform.position;
                }

            }
        }
        SpawnUnits();
       
        for(int i = 0; i < attackList.Count; i++)
        {
            if(attackCounters[i]>= attackList[i].attackSpeed && target != null && Vector2.Distance(transform.position,target.transform.position)<= attackList[i].attackRange)
            {
                
                if (attackList[i].useBarrel)
                {
                    Debug.Log(unit.unitName +" has a barrel and index is: "+ i);
                    
                    
                    Fire(i, attackList[i].useBarrel);
                    attackCounters[i] = 0;
                    
                   
                }
                else
                {
                    Fire(i, attackList[i].useBarrel);
                    attackCounters[i] = 0;
                }
               
            }
            else
            {
                attackCounters[i] += Time.deltaTime;
            }
            
        }
       
    }
    public void SpawnUnits()
    {
        if (spawnUnits.Count != 0)
        {
            int index = 0;
            foreach (SpawnUnit spawn in spawnUnits)
            {
                spawn.spawnCounter += Time.deltaTime;
                if (spawn.spawnCounter > spawn.spawnSpeed && spawn.unitsSpawnedCounter < spawn.maxUnitAmount)
                {
                    GameObject spawnedObject = Instantiate(spawn.spawnObject, GameController.instance.combatHolder);
                    spawnedObject.transform.position = spawnPositions[index].position;
                    spawn.spawnCounter = 0;
                    spawn.unitsSpawnedCounter++;
                    if (isEnemyShip)
                    {
                        GameController.instance.enemyUnits.Add(spawnedObject.GetComponent<CombatUnit>());
                        spawnedObject.GetComponent<CombatUnit>().isEnemyShip = true;
                        spawnedObject.GetComponent<CombatUnit>().opposingUnits = GameController.instance.playerUnits;
                    }
                    else
                    {
                        GameController.instance.playerUnits.Add(spawnedObject.GetComponent<CombatUnit>());
                        spawnedObject.GetComponent<CombatUnit>().opposingUnits = GameController.instance.enemyUnits;
                    }
                }
                index++;
            }
        }
    }
    public void SetupShip()
    {
        maxHealth = unit.maxHealth;
        turnSpeed = unit.turnSpeed;
        moveSpeed = unit.speed;
        foreach(Attack a in unit.attacks)
        {
            Attack attack = new Attack();
            attack.attackDamage = a.attackDamage;
            attack.attackName = a.attackName;
            attack.attackRange = a.attackRange;
            attack.attackSpeed = a.attackSpeed;
            attack.moveSpeed = a.moveSpeed;
            attack.useBarrel = a.useBarrel;
            attack.projectile = a.projectile;
            attackList.Add(attack);
        }
        foreach(SpawnUnit spawn in unit.spawnedUnits)
        {
            SpawnUnit newSpawn = new SpawnUnit();
            newSpawn.spawnObject = spawn.spawnObject;
            newSpawn.spawnSpeed = spawn.spawnSpeed;
            newSpawn.spawnCounter = 0;
            newSpawn.unit = spawn.unit;
            newSpawn.unitsSpawnedCounter = 0;
            newSpawn.maxUnitAmount = spawn.maxUnitAmount;
            spawnUnits.Add(newSpawn);
        }

    }
    public void Fire(int i,bool isUsingBarrel)
    {
        Projectile proj = Instantiate(attackList[i].projectile, attackPositions[i].position, Quaternion.identity).GetComponent<Projectile>();
        proj.transform.SetParent(GameController.instance.projectileHolder.transform);
        proj.transform.rotation = Quaternion.Euler(transform.right);
        if (isUsingBarrel)
        {
            proj.transform.Rotate(new Vector3(0, 0, barrels[i-nonBarrelAttacks].transform.rotation.eulerAngles.z+270));
            proj.moveDirection = barrels[i-nonBarrelAttacks].transform.up;
            proj.transform.position = barrels[i-nonBarrelAttacks].firePosition.position;
        }
        else
        {
            proj.transform.Rotate(new Vector3(0, 0, transform.rotation.eulerAngles.z+270));
            proj.moveDirection = transform.up;
        }
        if(proj.type == ProjectileType.homing)
        {
            proj.target = target;
        }
        
        
        
        proj.SetupProjectile(attackList[i]);
        if (isEnemyShip)
        {
            proj.isEnemyProjectile = true;
            proj.gameObject.layer = LayerMask.NameToLayer("EnemyBullet");
            proj.tag = "EnemyBullet";
        }
    }
    public void TakeDamage(float damage)
    {
        health -= damage;
        if (health < 0)
        {
            Death();
        }
    }
    public void Death()
    {
        if (isEnemyShip)
        {
            GameController.instance.enemyUnits.Remove(this);
            if(GameController.instance.enemyUnits.Count == 0 || CheckForFightEnd())
            {
                GameController.instance.EndCombat(true);
            }
           
        }
        else
        {
            GameController.instance.playerUnits.Remove(this);
            if (GameController.instance.playerUnits.Count == 0 || CheckForFightEnd())
            {
                GameController.instance.EndCombat(false);
            }
        }
        Destroy(gameObject);
    }
    public bool CheckForFightEnd()
    {
        if (isEnemyShip)
        {
            foreach(CombatUnit unit in GameController.instance.enemyUnits)
            {
                if (!unit.unit.isTempUnit)
                {
                    return false;
                }
            }
            return true;
        }
        else
        {
            foreach (CombatUnit unit in GameController.instance.playerUnits)
            {
                if (!unit.unit.isTempUnit)
                {
                    return false;
                }
            }
            return true;
        }
    }
    private void OnDisable()
    {
        if (isEnemyShip)
        {
            GameController.instance.enemyUnits.Remove(this);
        }
        else
        {
            GameController.instance.playerUnits.Remove(this);
        }
    }
    private bool returnToField;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("CombatWall"))
        {
            returnToField = true;
            returnDistanceTarget = Vector2.Distance(transform.position, GameController.instance.goTowardsPoint.transform.position) - 1.5f;
        }
    }
}
public enum Targeting
{
    closest,strongest
}
public enum TargetDistance
{
    weaponsRange,ramming,flyby
}
public enum RotationTarget
{
    front,right
}
