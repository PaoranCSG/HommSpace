using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Barrel : MonoBehaviour
{
    public float maxRotation;
    public float minRotation;
    public float defaultRotation;
    public Transform firePosition;
    public GameObject target;
    public float turnSpeed;
    public float turnSpeedMulti = 1;
    public float lastZRotation;
    public bool isEnemyBarrel;
    public bool isTargetInRange = false;
    public float maxRange;
    
    private void Start()
    {
        transform.Rotate(new Vector3(0, 0, defaultRotation));
        CheckForTarget();
    }
    private void Update()
    {
        if(target != null)
        {
            if(Vector2.Distance(transform.position,target.transform.position) > maxRange)
            {
                isTargetInRange = false;
            }
            Vector3 current = transform.up;
            Vector3 to = target.transform.position - transform.position;
            if (transform.localRotation.eulerAngles.z > maxRotation && lastZRotation < transform.rotation.eulerAngles.z)
            {
                transform.SetPositionAndRotation(transform.position, Quaternion.Euler(0, 0, maxRotation));
                turnSpeedMulti = 0;
                isTargetInRange = false;
            }
            else if (transform.localRotation.eulerAngles.z < minRotation && lastZRotation > transform.rotation.eulerAngles.z)
            {
                transform.SetPositionAndRotation(transform.position, Quaternion.Euler(0, 0, minRotation));
                turnSpeedMulti = 0;
                isTargetInRange = false;

            }
            else
            {

                turnSpeedMulti = 1;
            }
            transform.up = Vector3.RotateTowards(current, to, turnSpeed * turnSpeedMulti *Mathf.Rad2Deg* Time.deltaTime, 1);
        }
        else
        {
            isTargetInRange = false;
        }
      
        lastZRotation = transform.rotation.eulerAngles.z;
        if (!isTargetInRange)
        {
            CheckForTarget();
        }
       
    }
    public void CheckForTarget()
    {
        List<CombatUnit> targetUnits = new List<CombatUnit>();
        if (isEnemyBarrel)
        {
            targetUnits = GameController.instance.playerUnits;
        }
        else
        {
            targetUnits = GameController.instance.enemyUnits;
        }
        float distance = float.MaxValue;
        foreach (CombatUnit unit in targetUnits)
        {
            if (distance > Vector2.Distance(transform.position, unit.transform.position))
            {
                target = unit.gameObject;
                distance = Vector2.Distance(transform.position, unit.transform.position);
            }
            
        }
        //Debug.Log("Barrel target is: " + target);
        isTargetInRange = true;
        /*
        //Debug.Log("Checking for Target");
        for (int i = 0;i< targetUnits.Count; i++)
        {
            Vector2 barrelDirection = transform.up;
            Vector2 targeting = targetUnits[i].transform.position - transform.position;
            Vector2 barrelDirectionNorm = barrelDirection.normalized;
            Vector2 targetingNorm = targeting.normalized;
            
            float angle = Mathf.Acos(Vector2.Dot(barrelDirectionNorm, targetingNorm))*Mathf.Rad2Deg;
            Debug.Log(angle);
            Debug.Log("MaxRot: " + maxRotation);
            Debug.Log("MinRot: " + minRotation);
           
            target = targetUnits[i].gameObject;
            isTargetInRange = true;
            break;
           
        }*/
        
    }


}
