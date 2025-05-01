using UnityEngine;
using Unity.Netcode;

public class GameSessionManager : NetworkBehaviour
{
    public static GameSessionManager Instance;
    private int playerCounter = 0;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public int GetNextPlayerIndex()
    {
        if (playerCounter >= 4) // Max players
        {
            Debug.LogError("Número máximo de jogadores alcançado!");
            return 3; // Retorna o último slot
        }
        
        return playerCounter++;
    }
}