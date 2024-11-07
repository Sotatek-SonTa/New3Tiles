using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public Button addOneMoreSlot;
    public Button reverseMove;
    public Button shuffleTiles;
    public Button NextLevel;
    public GameObject winUI;
    public void SetAddSlotButton(bool status)
    {
        addOneMoreSlot.gameObject.SetActive(status);
    }
    public void SetActiveUIWin(bool status)
    {
        winUI.gameObject.SetActive(status);
    }
}
