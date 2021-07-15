using UnityEngine.UI;
using UnityEngine;
using MD.CraftingSystem;
public class CraftItemUI : MonoBehaviour
{
    [SerializeField]
    private Image image = null;

    [SerializeField]
    private CraftingRecipe recipeSO = null;

    [SerializeField]
    private CraftItemName itemName = CraftItemName.SpeedBoost1;

    public void SetItem(CraftItemName name) 
    {
        Debug.Log("Setitem image " + name);
        this.itemName = name;
        Setup();
    }

    private void Awake()
    {
        itemName = CraftItemName.SpeedBoost1;
        Setup();
    }

    public CraftItemName Name() => itemName;   
    private void Setup()
    {
        image.sprite = recipeSO.GetImage(itemName);
    }
}
