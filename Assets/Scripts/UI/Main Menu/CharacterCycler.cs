using Cycler;
using MD.Character;
using System;
using UnityEngine;
using UnityEngine.UI;

public class CharacterCycler : MonoBehaviour, ICycler<CharacterStats>
{
    public CharacterStats Current { get { return characters.current.value; } }

    [SerializeField]
    private Text charName = null;

    [SerializeField]
    private Image charIcon = null;

    public Action<CharacterStats> OnCycle { get; set; }

    private CircularLinkedList<CharacterStats> characters;

    public void LoadElements(CharacterStats[] elements)
    {
        characters = new CircularLinkedList<CharacterStats>(elements);
        charIcon.sprite = Current.CharacterSprite;
        OnCycle?.Invoke(Current);
    }

    public void GoNext()
    {
        characters.NextPos();
        ProcessAfterCycle();
    }

    public void GoPrev()
    {
        characters.PrevPos();
        ProcessAfterCycle();
    }

    private void ProcessAfterCycle()
    {
        charIcon.sprite = Current.CharacterSprite;
        charName.text = Current.CharacterName;
        OnCycle?.Invoke(Current);
    }
}
