using UnityEngine;

public class CharacterManager : MonoBehaviour
{
    public static CharacterManager Instance;

    public Character[] characters; // Mudei para public para debug

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            LoadCharacters();
        }
        else 
        {
            Destroy(gameObject);
        }
    }

    public Character GetCharacter(int index)
    {
        if (characters == null || characters.Length == 0)
        {
            Debug.LogError("Nenhum personagem carregado!");
            return null;
        }

        if (index < 0 || index >= characters.Length)
        {
            Debug.LogError($"Índice de personagem inválido: {index}");
            return null;
        }

        return characters[index];
    }

    private void LoadCharacters()
    {
        characters = Resources.LoadAll<Character>("Characters");
        if (characters == null || characters.Length == 0)
        {
            Debug.LogError("Falha ao carregar personagens da pasta Resources/Characters");
        }
        else
        {
            Debug.Log($"Personagens carregados: {characters.Length}");
        }
    }

    public int GetRandomCharacterIndex()
    {
        if (characters == null || characters.Length == 0)
        {
            Debug.LogError("Nenhum personagem disponível!");
            return 0;
        }

        return Random.Range(0, characters.Length);
    }
}