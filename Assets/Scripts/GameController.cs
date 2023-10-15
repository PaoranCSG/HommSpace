using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Splines;
using UnityEngine.SceneManagement;

public class GameController : MonoBehaviour
{
    public static GameController instance;
    public Transform combatHolder;
    public Camera combatCamera;
    public Camera mainCamera;
    public int turn;
    public int maxTurn;
    public float cameraMoveSpeed;
    public List<Hero> heroes = new List<Hero>();
    public List<Location> locations = new List<Location>();
    public List<Planet> playerPlanets = new List<Planet>();
    public Planet selectedPlanet;
    public Vector2 combatOffset = new Vector2(100,100);
    public GameObject projectileHolder;
    public bool inCombat;
    public Color playerColor;
    public Color enemyColor;
    public GameObject goTowardsPoint;
    public float fightSpeed = 0.2f;
    public Hero lastFightEnemyHero;
    public Location selectedLocation;
    
    public GameObject combatUIHolder;
    public GameObject overworldUIHolder;
    #region global buffs
    public float healthBoost;
    #endregion
    

    public List<CombatUnit> playerUnits = new List<CombatUnit>();
    public List<CombatUnit> enemyUnits = new List<CombatUnit>();
    public Hero selectedHero;
    #region resources
    public float alloys =5000;
    public float credits = 5000;
    #endregion

    public GameObject splineObject;
    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
        }
        else if(instance != this)
        {
            Destroy(this);
        }
       
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.M))
        {
            //FindFirstObjectByType<Hero>().Move();
        }
        if (Input.GetKeyDown(KeyCode.KeypadPlus))
        {
            Time.timeScale += 0.1f;
            if(Time.timeScale >= 1.5f)
            {
                Time.timeScale = 1.5f;
            }
        }
        else if (Input.GetKeyDown(KeyCode.KeypadMinus))
        {
            Time.timeScale -= 0.1f;
            if (Time.timeScale < 0.1f)
            {
                Time.timeScale = 0.1f;
            }
        }
       
        if (!inCombat)
        {
            //mainCamera.transform.position += new Vector3(Input.GetAxis("Horizontal") * cameraMoveSpeed * Time.deltaTime, Input.GetAxis("Vertical") * cameraMoveSpeed * Time.deltaTime, 0);


        }
       
    }
    private void Start()
    {
        selectedHero = heroes[0];
        LaneSetup();
        StartTurn();
        heroes[0].SelectHero();
    }
    public bool startIsVertical;
    public bool endIsVertical;
    public Direction startDirection;
    public Direction endDirection;
    public float laneDistance;
    public void LaneSetup() // Setting Up Lanes for movement
    {
        
        foreach(Location location in locations)
        {


            location.gameObject.name = location.locationName;
            foreach (Lane lane in location.connectedLanes)
            {

                GameObject tempLane = Instantiate(splineObject, new Vector3(0, 0, 2), Quaternion.identity);
                Vector3 startPos = FindClosestPosition(lane.startLocation, lane.endLocation.transform.position, true);
                Vector3 endPos = FindClosestPosition(lane.endLocation, startPos, false);
                Vector3 midPoint = (startPos + endPos) * 0.5f;


               

                Spline tempSpline = tempLane.GetComponent<SplineContainer>().Splines[0];
                float startMultiplier = 1;
                if ((int)startDirection % 2 != 0)
                {
                    startMultiplier = -1;
                }
                if (startIsVertical)
                {
                    tempSpline.Add(new BezierKnot(startPos, new Vector3(0, 0, 0), new Vector3(0, 0, 0), Quaternion.Euler(90, 0, 0)));
                    tempSpline.Add(new BezierKnot(startPos + new Vector3(0, 0.4f * startMultiplier, 0), new Vector3(0, 0, 0), new Vector3(0, 0, 0), Quaternion.Euler(90, 0, 0)));
                }
                else
                {
                    tempSpline.Add(new BezierKnot(startPos, new Vector3(0, 0, 0), new Vector3(0, 0, 0), Quaternion.Euler(90, 0, 0)));
                    tempSpline.Add(new BezierKnot(startPos + new Vector3(0.4f * startMultiplier, 0, 0), new Vector3(0, 0, 0), new Vector3(0, 0, 0), Quaternion.Euler(90, 0, 0)));
                }
                //tempSpline.Add(new BezierKnot(midPoint, new Vector3(laneDistance/2f,0,0), new Vector3(laneDistance / -2f, 0,0), Quaternion.Euler(90, 0, 0)));


                float endMultiplier = 1;
                if ((int)endDirection % 2 != 0)
                {
                    endMultiplier = -1;
                }
                if (endIsVertical)
                {
                    tempSpline.Add(new BezierKnot(endPos + new Vector3(0, 0.4f * endMultiplier, 0), new Vector3(0, 0, 0), new Vector3(0, 0, 0), Quaternion.Euler(90, 0, 0)));
                    tempSpline.Add(new BezierKnot(endPos, new Vector3(0, 0, 0), new Vector3(0, 0, 0), Quaternion.Euler(90, 0, 0)));


                }
                else
                {
                    tempSpline.Add(new BezierKnot(endPos + new Vector3(0.4f * endMultiplier, 0, 0), new Vector3(0, 0, 0), new Vector3(0, 0, 0), Quaternion.Euler(90, 0, 0)));
                    tempSpline.Add(new BezierKnot(endPos, new Vector3(0, 0, 0), new Vector3(0, 0, 0), Quaternion.Euler(90, 0, 0)));

                }
                lane.container = tempLane.GetComponent<SplineContainer>();
                tempLane.transform.SetParent(lane.startLocation.transform);
                lane.startLocation.connectedLocations.Add(lane.endLocation);
                lane.endLocation.connectedLocations.Add(lane.startLocation);
                lane.container.gameObject.name = lane.startLocation.locationName+"-"+lane.endLocation.locationName;
                lane.isOppositeDirection = false;
                
            }
        }
        foreach (Location location in locations)
        {



            foreach (Lane lane in location.connectedLanes)
            {
                if(lane.endLocation != location)
                {
                   
                    Lane newLane = new Lane();
                    newLane.startLocation = lane.startLocation;
                    newLane.endLocation = lane.endLocation;
                    newLane.isOppositeDirection = true;
                    newLane.container = lane.container; 
                    lane.endLocation.connectedLanes.Add(newLane);



                }
                
            }
        }
    }
    public Vector3 FindClosestPosition(Location loc, Vector3 endPosition,bool isStartPos)
    {
        float distance = float.MaxValue;
        Transform closestTransform = loc.laneStartPositions[0];
        int index = 0;
        for(int i= 0; i < loc.laneStartPositions.Count; i++)
        {
            float tempDistance = Vector2.Distance(loc.laneStartPositions[i].position, endPosition);
            if(tempDistance < distance)
            {
                distance = tempDistance;
                closestTransform = loc.laneStartPositions[i];
                index = i;
                
            }
        }
        laneDistance = distance;
        if (isStartPos)
        {
            startDirection = (Direction)index;
            if (index < 2)
            {
                startIsVertical = true;
            }
            else
            {
                startIsVertical = false;
            }
        }
        else
        {
            endDirection = (Direction)index;
            if (index < 2)
            {
                endIsVertical = true;
            }
            else
            {
                endIsVertical = false;
            }
        }
        
        return closestTransform.position;
    }
    public void SetSelectedPlanet(int index)
    {
        selectedPlanet = playerPlanets[index];
    }
    public enum Direction
    {
        north,south,east,west
    }
    public void StartTurn()
    {
        foreach(Hero hero in heroes)
        {
            hero.movesLeft = hero.maxMoves;
            hero.movesLeftImage.fillAmount = (float)hero.movesLeft / (float)hero.maxMoves;
        }
        UIController.instance.UpdateUI();
    }
    public void EndTurn()
    {
        foreach(Planet planet in playerPlanets)
        {
            foreach(PlanetBuilding building in planet.buildings)
            {
                foreach(Production prod in building.currentProductions)
                {
                    building.currentRound++;
                    prod.everyXRoundsCounter++;
                    if (prod.everyXRoundsCounter >= prod.everyXRounds)
                    {
                        building.currentRound = 0;
                        if (prod.type == ProductionType.resource)
                        {
                            if(prod.resource == Resource.alloys)
                            {
                                alloys += prod.amount;
                            }
                            else if (prod.resource == Resource.credits)
                            {
                                credits += prod.amount;
                            }
                        }
                        else if (prod.type == ProductionType.units)
                        {
                            
                            Debug.Log(prod.everyXRoundsCounter);
                            if (prod.everyXRoundsCounter >= prod.everyXRounds)
                            {
                                if (prod.amount < prod.maxAmount)
                                {
                                    prod.amount++;
                                }
                                
                               
                            }
                            Debug.Log(prod.everyXRoundsCounter);


                        }
                        else if (prod.type == ProductionType.upgrades)
                        {

                        }

                        prod.everyXRoundsCounter = 0;
                    }
                   
                   
                 
                }
            }
        }
        
        turn++;
        
        StartTurn();
    }
    public void SetupShipLists()
    {
        foreach(CombatUnit unit in enemyUnits)
        {
            unit.opposingUnits = playerUnits;
        }
        foreach(CombatUnit unit in playerUnits)
        {
            unit.opposingUnits = enemyUnits;
        }
    }
    public void EndCombat(bool playerWon)
    {
        if (!inCombat)
        {
            return;
        }
        Debug.Log("Player won = " + playerWon);
        Time.timeScale = 1;
        Lane combatLane = GameController.instance.selectedHero.lastUsedLane;
        bool isOpposite = GameController.instance.selectedHero.isOpposite;
        GameController.instance.inCombat = false;
        GameController.instance.combatUIHolder.SetActive(false);
        GameController.instance.overworldUIHolder.SetActive(true);
        if (playerWon)
        {
            
            selectedHero.units.Clear();
            foreach (CombatUnit unit in playerUnits)
            {
                if (!unit.unit.isTempUnit)
                {
                    selectedHero.AddToFleet(unit.unit, 1);
                    Debug.Log("unit " + unit.unit.unitName + "  has been added back to hero");
                }

                
            }
            
            
            lastFightEnemyHero.gameObject.SetActive(false);
            if (isOpposite)
            {

                
                combatLane.endLocation.fleet = null;
                combatLane.startLocation.fleet = selectedHero;
                selectedHero.currentLocation = combatLane.startLocation;
            }
            else
            {
                //combatLane.endLocation.fleet.gameObject.SetActive(false);
                combatLane.startLocation.fleet = null;
                combatLane.endLocation.fleet = selectedHero;
                selectedHero.currentLocation = combatLane.endLocation;
            }
            lastFightEnemyHero.gameObject.SetActive(false);
            if (!selectedHero.currentLocation.locationEvent.eventCompleted)
            {
                selectedHero.currentLocation.locationEvent.SolveEvent();
            }
            UIController.instance.UpdateUI();


        }
        else
        {
            UIController.instance.gameOverScreen.SetActive(true);
            if (isOpposite)
            {
                combatLane.endLocation.fleet = null;
                
            }
            else
            {
                combatLane.startLocation.fleet = null;
                
            }
            Debug.Log("You lost the fight and hero");
        }
        ClearCombat();
    }
    public void ClearCombat()
    {
        Destroy(combatHolder.gameObject);
        GameObject newCombatHolder = new GameObject("CombatHolder");
        combatHolder = newCombatHolder.transform;
        enemyUnits.Clear();
        playerUnits.Clear();
        combatCamera.gameObject.SetActive(false);
        mainCamera.gameObject.SetActive(true);
        Destroy(projectileHolder);
        projectileHolder = new GameObject("ProjectileHolder");

        
    }
}
