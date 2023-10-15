using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

[System.Serializable]
public class LocationEvent 
{
    public EventType type;
    public bool isNeutral;
    public string title;
    public float amount;
    [TextArea(3,5)]
    public string description;
    public string negativeResponse;
    public string positiveResponse;
    public string neutralResponse;
    public bool eventCompleted = false;
    public Unit fighter;
    public Unit frigate;
    public void SolveEvent()
    {
        UIController.instance.popup.gameObject.SetActive(true);
        UIController.instance.popup.SetupPopup(isNeutral, title, description);
        if (type == EventType.victory)
        {
            
            UIController.instance.popup.neutralButton.GetComponentInChildren<TMP_Text>().text = neutralResponse;
            UIController.instance.popup.neutralButton.onClick.AddListener(() => Application.Quit());
        }
        else if (type == EventType.alloys)
        {
            GameController.instance.alloys += amount;
            UIController.instance.UpdateUI();
        }
        else if (type == EventType.credits)
        {
            GameController.instance.credits += amount;
            UIController.instance.UpdateUI();
        }
        else if (type == EventType.fighter)
        {
            GameController.instance.selectedHero.AddToFleet(fighter, (int)amount);
            UIController.instance.UpdateUI();
        }
        else if (type == EventType.frigate)
        {
            GameController.instance.selectedHero.AddToFleet(frigate, (int)amount);
            UIController.instance.UpdateUI();
        }
        eventCompleted = true;
    }
    
}
public enum EventType
{
    alloys,credits,fighter,frigate,hero,victory
}

