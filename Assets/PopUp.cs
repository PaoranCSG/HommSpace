using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PopUp : MonoBehaviour
{
    public TMP_Text titleText;
    public TMP_Text descriptionText;
    public Button negativeButton;
    public Button positiveButton;
    public Button neutralButton;

    private void Awake()
    {
        negativeButton.onClick.AddListener(() => DoNegativeAction());
        positiveButton.onClick.AddListener(() => DoPositiveAction());
        neutralButton.onClick.AddListener(() => DoNeutralAction());
    }
    public void SetupPopup(bool isNeutral,string titleString,string descriptionString)
    {
        titleText.text = titleString;
        descriptionText.text = descriptionString;
        negativeButton.onClick.RemoveAllListeners();
        positiveButton.onClick.RemoveAllListeners();
        neutralButton.onClick.RemoveAllListeners();
        if (isNeutral)
        {
            negativeButton.gameObject.SetActive(false);
            positiveButton.gameObject.SetActive(false);
            neutralButton.gameObject.SetActive(true);
        }
        else
        {
            negativeButton.gameObject.SetActive(true);
            positiveButton.gameObject.SetActive(true);
            neutralButton.gameObject.SetActive(false);
        }
        
    }
    public void DoNegativeAction()
    {
        
    }
    public void DoPositiveAction()
    {

    }
    public void DoNeutralAction()
    {

    }
}
