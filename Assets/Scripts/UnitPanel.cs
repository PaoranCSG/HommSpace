using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class UnitPanel : MonoBehaviour
{
    public Button UpgradeButton;
    public TMP_Text UpgradeButtonText;
    public TMP_Text titleText;
    public TMP_Text breadText;
    public Unit unit;
    public TMP_Text RecruitCostText;
    public TMP_Text sliderText;
    public TMP_Text sliderMinText;
    public TMP_Text sliderMaxText;
    public Slider unitRecruitSlider;
    public int unitsLeft;
    public int maxUnits;
}
