using UnityEngine;

public class CharacterManager : MonoBehaviour
{
    public static CharacterManager Instance;

    private Character[] characters;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            LoadCharacters();
        }
        else Destroy(gameObject);
    }

    public Character GetCharacter(int index)
{
    return (index >= 0 && index < characters.Length) ? characters[index] : null;
}

    private void LoadCharacters()
    {
        characters = Resources.LoadAll<Character>("Characters");
            Debug.Log("Loaded characters: " + characters.Length);
    }
    public int GetRandomCharacterIndex()
    {
        return Random.Range(0, characters.Length);
    }
}