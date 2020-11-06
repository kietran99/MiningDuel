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

    public static string DisplayName { get; private set; }

    private const string PLAYER_PREF_NAME_KEY = "PlayerName";

    private void Start() => SetUpInputField();

    private void SetUpInputField()
    {
        if (!PlayerPrefs.HasKey(PLAYER_PREF_NAME_KEY)) { return; }

        string defaultName = PlayerPrefs.GetString(PLAYER_PREF_NAME_KEY);

        nameInputField.text = defaultName;

        SetPlayerName(defaultName);
    }

    public void SetPlayerName(string name)
    {
        continueButton.interactable = !string.IsNullOrEmpty(name);
    }

    public void SavePlayerName()
    {
        DisplayName = nameInputField.text;
        
        PlayerPrefs.SetString(PLAYER_PREF_NAME_KEY, DisplayName);
        gameObject.SetActive(false);
        shownPlayerNameText.text = DisplayName;
    }
}
