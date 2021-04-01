using UnityEngine;
using UnityEngine.UI;

public class PlayerNameInput : MonoBehaviour
{
    [Header("UI")]
    [SerializeField]
    private InputField nameInputField = null;

    [SerializeField]
    private Button continueButton = null;

    [SerializeField]
    private Text shownPlayerNameText = null;

    [SerializeField]
    private GameObject Mask = null;

    public static string DisplayName { get; private set; }

    public const string PLAYER_PREF_NAME_KEY = "PlayerName";

    public void SetActiveNameInput(bool status)
    {
        Mask.gameObject.SetActive(status);
        gameObject.SetActive(status);
        continueButton.gameObject.SetActive(status);
        if (status) 
        {
            nameInputField.text = shownPlayerNameText.text;
            continueButton.interactable = !string.IsNullOrEmpty(nameInputField.text);
        }
    }

    private void Start() 
    {
        string currentName = PlayerPrefs.GetString(PlayerNameInput.PLAYER_PREF_NAME_KEY);
        if(!string.IsNullOrEmpty(currentName))
        {
            shownPlayerNameText.text = currentName;
            SetActiveNameInput(false);
        } 
    }
    // private void SetUpInputField(string currentName) 
    // {
    //     nameInputField.text = currentName;
    // }
    public void OnChangeInput()
    {
        continueButton.interactable = !string.IsNullOrEmpty(nameInputField.text);
    }

    public void SavePlayerName()
    {
        DisplayName = nameInputField.text;
        PlayerPrefs.SetString(PLAYER_PREF_NAME_KEY, DisplayName);
        gameObject.SetActive(false);
        shownPlayerNameText.text = DisplayName;
        PlayerData player = new PlayerData(DisplayName);
        SavePlayerData.Save(player);
    }
}
