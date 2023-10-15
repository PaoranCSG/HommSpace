using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Location: MonoBehaviour
{
    public string locationName;
    public string locationDescription;
    public List<Location> connectedLocations = new List<Location>();
    public List<Lane> connectedLanes = new List<Lane>();
    public Fleet fleet;
    public bool isEmpty;
    public List<Transform> laneStartPositions = new List<Transform>();
    public SpriteRenderer sr;
    public TMP_Text nameText;
    public GameObject selectedObject;
    public LocationEvent locationEvent;
    public bool isSelected;
    private void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
        nameText.text = locationName;
        
    }
    
    private void OnMouseDown()
    {
        Debug.Log("Clicked on " + locationName);
        
        if (UIController.instance.planetBackground.gameObject.activeInHierarchy)
        {
            Debug.Log("Planet view is open no moving");
            return;
        }
        if(GameController.instance.selectedHero == null)
        {
            // Show location info
        }
        if (GameController.instance.selectedHero.movesLeft <= 0)
            return;
        if(GameController.instance.selectedHero != null && connectedLocations.Contains(GameController.instance.selectedHero.currentLocation))
        {
            foreach(Lane l in connectedLanes)
            {
                if (l.startLocation == GameController.instance.selectedHero.currentLocation)
                {
                    GameController.instance.selectedHero.Move(l);
                    break;
                }
                else if(l.endLocation == GameController.instance.selectedHero.currentLocation)
                {
                    GameController.instance.selectedHero.Move(l);
                    break;
                }
                
            }
           
        }
    }

    public void checkIsEmpty()
    {
        if(fleet == null)
        {
            isEmpty = true;
        }
        else
        {
            isEmpty = false;
        }
    }
}
