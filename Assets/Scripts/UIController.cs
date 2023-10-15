using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
public class UIController : MonoBehaviour
{
    public static UIController instance;
    public List<Button> planetButtons = new List<Button>();
    public TMP_Text alloyText;
    public TMP_Text creditText;
    public TMP_Text turnText;
    public TMP_Text movesLeftText;
    public Button endTurnButton;

    public PopUp popup;
    // Planet Select
    public GameObject planetButtonHolder;
    public GameObject planetButtonPrefab;
    // Game Over Screen
    public GameObject gameOverScreen;
    public Button gameOverRestartButton;
    #region planetScreen
    public Image planetBackground;
    public Button buildingsTab;
    public Button unitsTab;
    
    // Buildings
    public TMP_Text planetNameText;
    public List<Button> buildingButtons = new List<Button>();
    public GameObject buildingHolder;
    public List<BuildingPanel> currentPlanetBuildings = new List<BuildingPanel>();
    public GameObject buildingPanelPrefab;
    public Button closePlanetViewButton;

    // ShipBuilding
    public GameObject unitConstructionHolder;
    public GameObject unitConstructionPrefab;
    public List<UnitPanel> currentPlanetUnitConstruction = new List<UnitPanel>();
    // Planet Fleet
    public GameObject planetFleetPanelHolder;
    public GameObject planetFleetPanelPrefab;


    #endregion
    // EnemyFleet
    public GameObject enemyFleetDisplayPrefab;
    public GameObject enemyFleetSpriteRendererPrefab;
    public GameObject enemyFleetTextPrefab;
    // Hero Fleet
    public GameObject heroFleetHolder;
    public GameObject heroFleetBackground;
    // Hero Select
    public GameObject heroSelectPrefab;
    public GameObject heroSelectHolder;
    public List<HeroSelect> heroSelects = new List<HeroSelect>();
    #region PauseMenu
    public Button resumeButton;
    public Button restartButton;
    public Button quitGameButton;
    public Image pauseMenuBackground;

    public List<GameObject> howToPlayList = new List<GameObject>();
    public Button nextTutorialButton;
    #endregion
    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Destroy(this);
        }
        nextTutorialButton.gameObject.SetActive(true);
        nextTutorialButton.onClick.AddListener(() => NextTutorial());
        howToPlayList[tutorialIndex].gameObject.SetActive(true);

    }
    private int tutorialIndex = 0;
    public void NextTutorial()
    {
        howToPlayList[tutorialIndex].gameObject.SetActive(false);
        
        tutorialIndex++;
        if(tutorialIndex >= howToPlayList.Count)
        {
            nextTutorialButton.gameObject.SetActive(false);
            return;
        }
        howToPlayList[tutorialIndex].gameObject.SetActive(true);
    }
    private void Start()
    {
        SetupHeroes();
       

        GameObject planetSelect = Instantiate(planetButtonPrefab, planetButtonHolder.transform);
        planetButtons.Add(planetSelect.GetComponent<Button>());
        planetSelect.GetComponentInChildren<TMP_Text>().text = GameController.instance.playerPlanets[0].GetComponent<Location>().locationName;
        endTurnButton.onClick.AddListener(() => GameController.instance.EndTurn());
        for (int i = 0; i < planetButtons.Count; i++)
        {
            int x = i;
            planetButtons[i].onClick.AddListener(() => OpenPlanet(x));
        }
        closePlanetViewButton.onClick.AddListener(() => ClosePlanet());
        resumeButton.onClick.AddListener(() => TogglePauseMenu());
        quitGameButton.onClick.AddListener(() => Application.Quit());
        buildingsTab.onClick.AddListener(() => OpenPlanet(GameController.instance.playerPlanets.IndexOf(GameController.instance.selectedPlanet)));
        unitsTab.onClick.AddListener(() => OpenUnitRecruitment());
        gameOverRestartButton.onClick.AddListener(() => RestartGame());
        restartButton.onClick.AddListener(() => RestartGame());
    }
    public void RestartGame()
    {
        SceneManager.LoadScene(0);
    }
    public void SetupHeroes()
    {
        GameObject heroSelect = Instantiate(heroSelectPrefab, heroSelectHolder.transform);
        HeroSelect select = heroSelect.GetComponent<HeroSelect>();
        select.button.onClick.AddListener(() => GameController.instance.heroes[0].SelectHero());
        select.nameText.text = NameController.instance.characterNames[Random.Range(0, NameController.instance.characterNames.Count)];
        select.iconImage.sprite = NameController.instance.characterSprites[Random.Range(0, NameController.instance.characterSprites.Count)];
    }
    public void SetupBuildButtons()
    {
        for (int i = 0; i < buildingButtons.Count; i++)
        {
            
        }
    }
    public void TogglePauseMenu()
    {
        if (pauseMenuBackground.gameObject.activeInHierarchy)
        {
            pauseMenuBackground.gameObject.SetActive(false);
        }
        else
        {
            pauseMenuBackground.gameObject.SetActive(true);
        }
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            TogglePauseMenu();
        }
    }
    public void UpgradeBuilding(Planet planet, int i,int planetIndex)
    {
        BuildingPanel panel = currentPlanetBuildings[i];
        Building building = panel.building;
        if (GameController.instance.alloys >= building.buildingUpgrades[panel.buildingLevel+1].alloyCost && GameController.instance.credits >= building.buildingUpgrades[panel.buildingLevel+1].creditCost) 
        {
            PlanetBuilding planetBuilding = FindPlanetBuilding(planet,building);
            planetBuilding.buildingLevel++;
            GameController.instance.alloys -= building.buildingUpgrades[panel.buildingLevel + 1].alloyCost;
            GameController.instance.credits -= building.buildingUpgrades[panel.buildingLevel + 1].creditCost;
            OpenPlanet(planetIndex);
            UpdateUI();

            List<float> prodAmount = new List<float>();
            List<int> currentXRoundsAmount = new List<int>();
            foreach(Production prod in planetBuilding.currentProductions)
            {
               
                prodAmount.Add(prod.amount);
                currentXRoundsAmount.Add(prod.everyXRoundsCounter);

            }
            planetBuilding.currentProductions.Clear();
            int counter = 0;
            foreach (Production prod in planetBuilding.building.buildingUpgrades[planetBuilding.buildingLevel].productions)
            {
                planetBuilding.currentProductions.Add(prod);

                Debug.Log(counter);
                if (prod.type == ProductionType.units && prodAmount.Count>counter)
                {
                    planetBuilding.currentProductions[counter].amount = prodAmount[counter];
                    planetBuilding.currentProductions[counter].everyXRoundsCounter = currentXRoundsAmount[counter];
                }
              
             
                counter++;
            }
            
           
        }
        else
        {
            Debug.Log("Cannot Afford Building");
        }
    }
    public PlanetBuilding FindPlanetBuilding(Planet p,Building b)
    {
        PlanetBuilding planetBuilding = null;
        for(int i = 0; i < p.buildings.Count; i++)
        {
            if(p.buildings[i].building == b)
            {
                planetBuilding = p.buildings[i];
                break;
            }
        }
        return planetBuilding;

    }
    public void ConstructBuilding(Planet p, int i,int planetIndex)
    {
        Debug.Log(i);
        BuildingPanel panel = currentPlanetBuildings[i];
        Building building = panel.building;
        Debug.Log(panel.building.buildingName+ " has been Built");
        if (GameController.instance.alloys >= building.buildingUpgrades[panel.buildingLevel+1].alloyCost && GameController.instance.credits >= building.buildingUpgrades[panel.buildingLevel+1].creditCost)
        {
            PlanetBuilding planetBuilding = new PlanetBuilding();
            planetBuilding.buildingLevel =0;
            planetBuilding.building = building;
            p.buildings.Add(planetBuilding);
            Debug.Log(planetBuilding);
            GameController.instance.alloys -= building.buildingUpgrades[panel.buildingLevel + 1].alloyCost;
            GameController.instance.credits -= building.buildingUpgrades[panel.buildingLevel + 1].creditCost;
            OpenPlanet(planetIndex);
            UpdateUI();
        }
        else
        {
            Debug.Log("Cannot Afford Building");
        }

    }
    public void ClosePlanet()
    {
        planetBackground.gameObject.SetActive(false);
        isUnitsTabOpen = false;
        isBuildingsTabOpen = false;
    }
    public void OpenPlanet(int i)
    {
        isUnitsTabOpen = false;
        isBuildingsTabOpen = true;
        GameController.instance.selectedPlanet = GameController.instance.playerPlanets[i];
        ClearPlanetBuildings();
        ClearUnitProduction();
        Planet planet = GameController.instance.playerPlanets[i];
        GameController.instance.selectedPlanet = planet;
        planetNameText.text = planet.GetComponent<Location>().locationName;
        planetBackground.gameObject.SetActive(true);
        int j = 0;
        foreach (PlanetBuilding planetBuilding in planet.buildings)
        {
            int x = j;
            BuildingPanel panel = Instantiate(buildingPanelPrefab, buildingHolder.transform).GetComponent<BuildingPanel>();
            panel.buildingLevel = planetBuilding.buildingLevel;
            panel.titleText.text = planetBuilding.building.buildingName+" " +planetBuilding.buildingLevel;
            panel.breadText.text = planetBuilding.building.buildingUpgrades[panel.buildingLevel].description;
            
           
            panel.building = planetBuilding.building;
            currentPlanetBuildings.Add(panel);
            Debug.Log(j);
            
            if(panel.buildingLevel >= panel.building.buildingUpgrades.Count-1)
            {
                panel.UpgradeButtonText.text = "Max Level";
                panel.upgradeCostText.text = "";
            }
            else
            {
                panel.UpgradeButton.onClick.AddListener(() => UpgradeBuilding(planet, x, i));
                panel.UpgradeButtonText.text = "Upgrade";
                panel.upgradeCostText.text = "Alloys: " + planetBuilding.building.buildingUpgrades[panel.buildingLevel+1].alloyCost + " Credits: " + planetBuilding.building.buildingUpgrades[panel.buildingLevel+1].creditCost;

            }
            j++;
            if (planetBuilding.currentProductions.Count ==0) 
            {
                foreach (Production prod in planetBuilding.building.buildingUpgrades[planetBuilding.buildingLevel].productions)
                {
                    Production prodCopy = new Production();
                    prodCopy.amount = prod.amount;
                    prodCopy.everyXRounds = prod.everyXRounds;
                    prodCopy.everyXRoundsCounter = prod.everyXRoundsCounter;
                    prodCopy.maxAmount = prod.maxAmount;
                    prodCopy.resource = prod.resource;
                    prodCopy.type = prod.type;
                    prodCopy.unit = prod.unit;
                    planetBuilding.currentProductions.Add(prodCopy);
                }
            }
            
          
           
            


        }
        foreach (Building building in GameSettings.instance.availableBuildings)
        {
            if (!DoesBuildingExist(planet, building))
            {
                
                int x = j;
                BuildingPanel panel = Instantiate(buildingPanelPrefab, buildingHolder.transform).GetComponent<BuildingPanel>();
                panel.titleText.text = building.buildingName;
                panel.breadText.text = building.description;
                panel.UpgradeButtonText.text = "Build";
                panel.UpgradeButton.onClick.AddListener(() => ConstructBuilding(planet, x, i));
                panel.building = building;
                panel.upgradeCostText.text = "Alloys: " + building.buildingUpgrades[0].alloyCost + " Credits: " + building.buildingUpgrades[0].creditCost;
                panel.buildingLevel = -1;
                currentPlanetBuildings.Add(panel);
                Debug.Log(j);
                j++;
            }
            else
            {
                Debug.Log("Building of type: " + building.buildingName +" has been built");
            }
          
        }
        OpenFleetPanel(false);
        
    }
    public void OpenFleetPanel(bool isHero)
    {

        ClearPlanetFleetPanel(isHero);
        Fleet fleet;
        GameObject holder;
        if (isHero)
        {
            fleet = GameController.instance.selectedHero;
            holder = heroFleetHolder;
        }
        else
        {
            fleet = GameController.instance.selectedPlanet.fleet; 
            holder = planetFleetPanelHolder;
        }
        foreach(UnitGroup unit in fleet.units)
        {
            PlanetFleetPanel  panel= Instantiate(planetFleetPanelPrefab, holder.transform).GetComponent<PlanetFleetPanel>();
            panel.amountText.text = unit.stackCount.ToString();
            panel.nameText.text = unit.unit.unitName;
            panel.icon.sprite = unit.unit.prefab.GetComponent<SpriteRenderer>().sprite;
            if (isHero)
            {
                panel.upButton.onClick.AddListener(() => MoveUnit(isHero,unit));
                panel.downButton.gameObject.SetActive(false);
            }
            else
            {
                panel.downButton.onClick.AddListener(() => MoveUnit(isHero,unit));
                panel.upButton.gameObject.SetActive(false);
            }
        }
    }
    public void MoveUnit(bool moveUp,UnitGroup unit)
    {
        if(GameController.instance.selectedHero == null || GameController.instance.selectedPlanet == null)
        {
            Debug.Log("No hero or planet has been selected");
            return;
        }
        if (!moveUp)
        {
            GameController.instance.selectedHero.AddToFleet(unit.unit, unit.stackCount);
            GameController.instance.selectedPlanet.fleet.RemoveFromFleet(unit);
        }
        else
        {
            GameController.instance.selectedPlanet.fleet.AddToFleet(unit.unit, unit.stackCount);
            GameController.instance.selectedHero.RemoveFromFleet(unit);
        }
        OpenFleetPanel(moveUp);
        OpenFleetPanel(!moveUp);
    }
    public void ClearPlanetFleetPanel(bool isHero)
    {
        GameObject holder;
        if (isHero)
        {
            holder = heroFleetHolder;
        }
        else
        {
            holder = planetFleetPanelHolder;
        }
        int childCount = holder.transform.childCount;
        
        for(int i = 0; i < childCount; i++)
        {
            GameObject destroyObject = holder.transform.GetChild(0).gameObject;
            destroyObject.transform.SetParent(null);
            Destroy(destroyObject);
            /*Destroy(holder.transform.GetChild(0).GetComponent<PlanetFleetPanel>());
            Destroy(holder.transform.GetChild(0).GetComponent<Image>());
            Destroy(holder.transform.GetChild(0).GetComponent<CanvasRenderer>());
            Debug.Log(holder.transform.GetChild(0).gameObject);
            Destroy(holder.transform.GetChild(0).gameObject);*/
            
        }
    }
    public bool isUnitsTabOpen = false;
    public bool isBuildingsTabOpen = false;
    public void OpenUnitRecruitment()
    {
        isUnitsTabOpen = true;
        isBuildingsTabOpen = false;
        ClearUnitProduction();
        ClearPlanetBuildings();
        foreach(PlanetBuilding planetBuilding in GameController.instance.selectedPlanet.buildings)
        {
            foreach(Production prod in planetBuilding.currentProductions)
            {
                if(prod.type == ProductionType.units)
                {
                    UnitPanel panel = Instantiate(unitConstructionPrefab, unitConstructionHolder.transform).GetComponent<UnitPanel>();
                    panel.titleText.text = prod.unit.unitName;
                    panel.breadText.text = prod.unit.description;
                    panel.UpgradeButtonText.text = "Construct";
                    panel.UpgradeButton.onClick.AddListener(() => BuildUnit(prod.unit,panel.unitRecruitSlider,prod));
                    panel.RecruitCostText.text = "Alloys: " + prod.unit.alloyCost + " Credits: " + prod.unit.creditCost;
                    panel.unit = prod.unit;
                    panel.unitRecruitSlider.minValue = 1;
                    panel.unitRecruitSlider.maxValue = prod.amount;
                    panel.sliderMaxText.text = prod.amount.ToString();
                    panel.sliderMinText.text = "1";
                    panel.unitRecruitSlider.onValueChanged.AddListener( delegate { SetSliderValueText(panel.unitRecruitSlider, panel.sliderText); });
                    panel.sliderText.text = "1";

                }
            }
        }
    }
    public void SetSliderValueText(Slider slider,TMP_Text text)
    {
        text.text = slider.value.ToString();
    }
    public void BuildUnit(Unit unit,Slider slider,Production prod)
    {
        int sliderValue = (int)slider.value;
        if(GameController.instance.alloys >= unit.alloyCost*sliderValue && GameController.instance.credits >= unit.creditCost * sliderValue)
        {

            GameController.instance.alloys -= unit.alloyCost * sliderValue;
            GameController.instance.credits -= unit.creditCost * sliderValue;

            GameController.instance.selectedPlanet.fleet.AddToFleet(unit, sliderValue);
            prod.amount -= sliderValue;
            slider.maxValue -= sliderValue;
            if(slider.maxValue == 0)
            {
                slider.minValue = 0;
            }
            slider.value = slider.maxValue;
        }
        else
        {
            Debug.Log("Not Enough Resources");
        }
        OpenFleetPanel(false);
        UpdateUI();
        OpenUnitRecruitment();
    }
    private void ClearUnitProduction()
    {
        int childCount = unitConstructionHolder.transform.childCount;
        for(int i = 0; i < childCount; i++)
        {
            DestroyImmediate(unitConstructionHolder.transform.GetChild(0).gameObject);
        }
    }
    private bool DoesBuildingExist(Planet planet,Building b)
    {
        Debug.Log(b.buildingName);
        for(int i = 0;i<planet.buildings.Count;i++)
        {
            Debug.Log(planet.buildings[i].building.buildingName);
            if(planet.buildings[i].building == b)
            {
                Debug.Log("Is TRUE");
                return true;
            }
        }
        return false;
    }
    private void ClearPlanetBuildings()
    {
       
        int childCount = buildingHolder.transform.childCount;
        Debug.Log("ChildCount = " + childCount);
        for (int i  = 0; i < childCount; i++)
        {
            DestroyImmediate(buildingHolder.transform.GetChild(0).gameObject);
        }
        currentPlanetBuildings.Clear();
    }
    public void UpdateHeroButtons()
    {
        foreach(Hero h in GameController.instance.heroes)
        {
            // add new Hero Button;
        }
    }
    public List<Button> HeroButtons = new List<Button>();
    public void UpdateUI()
    {
        alloyText.text = "Alloys: " + GameController.instance.alloys;
        creditText.text = "Credits: " + GameController.instance.credits;
        turnText.text = "Turn: " + GameController.instance.turn;
        movesLeftText.text = "Moves Left: " + GameController.instance.selectedHero.movesLeft;
        OpenFleetPanel(true);
    }

}
