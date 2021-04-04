using UnityEngine.UI;
using UnityEngine;
using MD.CraftingSystem;
public class CraftItemUI : MonoBehaviour
{
    [SerializeField]
    private Image image;

    private CraftItemName itemName = CraftItemName.SpeedPotion1;

    public void SetItem(CraftItemName name) 
    {
        this.itemName = name;
        Initialize();
    }
    public CraftItemName Name() => itemName;   
    private void Initialize()
    {
        // this.image = image;
    }
}
