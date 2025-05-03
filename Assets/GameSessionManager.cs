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
        if (playerCounter >= 4)
        {
            Debug.LogError("Max players reached!");
            return -1; // Return invalid index instead of forcing a value
        }
        
        int index = playerCounter;
        playerCounter++;
        return index;
    }
}