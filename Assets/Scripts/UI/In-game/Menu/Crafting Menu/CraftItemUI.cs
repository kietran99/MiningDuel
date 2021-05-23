using UnityEngine.UI;
using UnityEngine;
using MD.CraftingSystem;
public class CraftItemUI : MonoBehaviour
{
    [SerializeField]
    private Image image;

    [SerializeField]
    private CraftingRecipe recipeSO;

    [SerializeField]
    private CraftItemName itemName;

    public void SetItem(CraftItemName name) 
    {
        Debug.Log("Setitem image " + name);
        this.itemName = name;
        Setup();
    }

    private void Awake()
    {
        itemName = CraftItemName.SpeedPotion1;
        Setup();
    }

    public CraftItemName Name() => itemName;   
    private void Setup()
    {
        image.sprite = recipeSO.GetImage(itemName);
    }
}
