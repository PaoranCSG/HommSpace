using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Splines;

public class Hero : Fleet
{
    public Location currentLocation;
    public int movesLeft;
    public int maxMoves;
    public SplineAnimate splineAnimate;
    public SplineContainer moveTarget;
    public int Level;
    public bool isEnemyFleet;
    public bool startCombatOnMoveFinish = false;
    public Image movesLeftImage;

    private bool isMoving;
    private float targetNumber;
    
    private void Start()
    {
        splineAnimate = GetComponent<SplineAnimate>();
        if (isEnemyFleet)
        {
            Vector2 offset = Vector2.zero;
            EnemyFleetDisplay enemyFleet = Instantiate(UIController.instance.enemyFleetDisplayPrefab, transform).GetComponent<EnemyFleetDisplay>();
            foreach(UnitGroup group in units)
            {
                SpriteRenderer sr = Instantiate(UIController.instance.enemyFleetSpriteRendererPrefab,enemyFleet.transform).GetComponent<SpriteRenderer>();
                sr.transform.position += (Vector3)offset;
                offset += new Vector2(0, 0);

                TMP_Text text = Instantiate(UIController.instance.enemyFleetTextPrefab, sr.transform).GetComponent<TMP_Text>();
                
                text.transform.position += new Vector3(0, 0.1f);
                Debug.Log(offset);
                offset += new Vector2(0.5f, 0);
                sr.sprite = group.unit.prefab.GetComponent<SpriteRenderer>().sprite;

                text.text = group.stackCount.ToString();
                enemyFleet.texts.Add(text);
                enemyFleet.spriteRenderers.Add(sr);

            }
        }
    }
    public void SelectHero()
    {
        GameController.instance.selectedHero.DeselectHero();
        UIController.instance.OpenFleetPanel(true);
        GameController.instance.selectedHero = this;
        transform.GetChild(0).gameObject.SetActive(true);
        UIController.instance.heroFleetBackground.SetActive(true);
        
    }
    public void DeselectHero()
    {
        transform.GetChild(0).gameObject.SetActive(false);
        UIController.instance.heroFleetBackground.SetActive(false);
    }
    private void OnMouseDown()
    {
        SelectHero();
    }
    private void OnMouseEnter()
    {
        if (isEnemyFleet)
        {
            ToggleEnemyFleetStrength(true);
        }
    }
    private void OnMouseExit()
    {
        if (isEnemyFleet)
        {
            ToggleEnemyFleetStrength(false);
        }
    }
    public void ToggleEnemyFleetStrength(bool enable)
    {
        if (enable)
        {
            
        }
        else
        {

        }
    }
    public Lane lastUsedLane;
    
    public void Move(Lane lane)
    {
        
        
        
        
        isMoving = true;
        moveTarget = lane.container;
        lastUsedLane = lane;
        Debug.Log("Is opposite Direction "+ lane.isOppositeDirection);
        if (!lane.isOppositeDirection)
        {
            
            transform.position = lane.container.Spline.EvaluatePosition(1);
            target = 0;
            timer = 1;
            isOpposite = true;
            if(lane.startLocation.fleet != null)
            {
                startCombatOnMoveFinish = true;
                Debug.Log(lane.startLocation.fleet.units.Count);
              
            }
            else
            {
                startCombatOnMoveFinish = false;
                lane.startLocation.fleet = this;
                lane.endLocation.fleet = null;
                currentLocation = lane.startLocation;
                
            }
            
            

        }
        else
        {
            transform.position = lane.container.Spline.EvaluatePosition(0);
            target = 1;
            timer = 0;
            isOpposite = false;
            
            if (lane.endLocation.fleet != null)
            {
                Debug.Log(lane.endLocation.fleet.units.Count);
                startCombatOnMoveFinish = true;
                
            }
            else
            {
                startCombatOnMoveFinish = false;
                lane.endLocation.fleet = this;
                lane.startLocation.fleet = null;
                currentLocation = lane.endLocation;
            }
            

        }
        
        
        movesLeft--;
        UIController.instance.UpdateUI();
        movesLeftImage.fillAmount = (float)movesLeft / (float)maxMoves;

    }
    float timer;
    float target;
    public  bool isOpposite;
    public void SetupCombat()
    {
        Time.timeScale = GameController.instance.fightSpeed;
        GameController.instance.inCombat = true;
        GameController.instance.combatUIHolder.SetActive(true);
        GameController.instance.overworldUIHolder.SetActive(false);
        GameController.instance.combatCamera.gameObject.SetActive(true);
        int unitCount= 0;
        foreach (UnitGroup unit in units)
        {
            unitCount += unit.stackCount;
        }
        int i = -unitCount/2;
       foreach(UnitGroup unit in units)
       {
            for(int j = 0; j < unit.stackCount; j++)
            {
                GameObject ship = Instantiate(unit.unit.prefab, GameController.instance.combatHolder);
                CombatUnit combatUnit = ship.GetComponent<CombatUnit>();
                ship.transform.position = GameController.instance.combatOffset + new Vector2(0, 0.2f * i);


                if (isEnemyFleet)
                {
                    GameController.instance.enemyUnits.Add(combatUnit);
                    combatUnit.isEnemyShip = true;
                    ship.transform.Rotate(new Vector3(0, 0, 90));
                    ship.transform.position += new Vector3(6, 0);
                }
                else
                {
                    GameController.instance.playerUnits.Add(combatUnit);
                    ship.transform.Rotate(new Vector3(0, 0, 270));
                    ship.transform.position -= new Vector3(6, 0);

                }
                i++;
            }
            
           
            
           
       }
    }
    private void Update()
    {
        if (isMoving)
        {
            if (isOpposite)
            {
                timer -= Time.deltaTime;
                transform.position = moveTarget.Spline.EvaluatePosition(timer);
                if (timer <= target)
                {
                    if (startCombatOnMoveFinish)
                    {
                        StartCombat();
                    }
                    else if (!currentLocation.locationEvent.eventCompleted)
                    {
                        currentLocation.locationEvent.SolveEvent();
                    }
                    
                    isMoving = false;
                    startCombatOnMoveFinish = false;
                    
                }
            }
            else
            {
                timer += Time.deltaTime;
                transform.position = moveTarget.Spline.EvaluatePosition(timer);
                if (timer >= target)
                {
                    if (startCombatOnMoveFinish)
                    {
                        StartCombat();
                    }
                    else if (!currentLocation.locationEvent.eventCompleted)
                    {
                        currentLocation.locationEvent.SolveEvent();
                    }
                    isMoving = false;
                    startCombatOnMoveFinish = false;
                   
                }
               
            }
        }
        
    }
    public void StartCombat()
    {
        GameController.instance.enemyUnits.Clear();
        GameController.instance.playerUnits.Clear();
        Camera.main.gameObject.SetActive(false);
        SetupCombat();
        if (isOpposite)
        {
            Hero enemyHero = (Hero)lastUsedLane.startLocation.fleet;
            enemyHero.SetupCombat();
            GameController.instance.lastFightEnemyHero = enemyHero;
            //currentLocation = lastUsedLane.startLocation;
            //lastUsedLane.startLocation.fleet = null;

        }
        else
        {
            Hero enemyHero = (Hero)lastUsedLane.endLocation.fleet;
            
            enemyHero.SetupCombat();
            GameController.instance.lastFightEnemyHero = enemyHero;
            //currentLocation = lastUsedLane.endLocation;
            //lastUsedLane.endLocation.fleet = null;
        }
        GameController.instance.SetupShipLists();
    }


}
