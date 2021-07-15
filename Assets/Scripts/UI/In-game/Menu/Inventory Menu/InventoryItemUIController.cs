using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using MD.Character;
public class InventoryItemUIController : MonoBehaviour
{

    [SerializeField]
    Text amountText = null;

    public void SetAmount (int amount)
    {
        if (amountText == null) return;
        string StrAmount = "";
        if (amount> 1)
            StrAmount = amount.ToString();
        amountText.text = StrAmount;
    }
}
