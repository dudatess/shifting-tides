using UnityEngine;
using Unity.Netcode;
using UnityEngine.SceneManagement;

public class NetworkPlayer : NetworkBehaviour
{
    [SerializeField] private GameObject characterPrefab; 
    private NetworkVariable<int> playerIndex = new NetworkVariable<int>();
    private NetworkVariable<int> characterIndex = new NetworkVariable<int>(
        default,
        NetworkVariableReadPermission.Everyone,
        NetworkVariableWritePermission.Server);

     private static int readyPlayerCount = 0;

    private static readonly int maxPlayers = 4;

    private NetworkVariable<bool> isReady = new NetworkVariable<bool>(
        false,
        NetworkVariableReadPermission.Everyone,
        NetworkVariableWritePermission.Server
    );
    private Character character;
    private SpriteRenderer spriteRenderer;

    // Player stats
    private int money = 0;
    private int influence = 50;
    private int people = 0;
    public int goalIndex;

    // Waiting room positions
    private readonly Vector3[] waitingRoomSlots = new Vector3[]
    {
        new Vector3(-3f, 0, 0),
        new Vector3(-1f, 0, 0),
        new Vector3(1f, 0, 0),
        new Vector3(3f, 0, 0)
    };

    // Game state tracking
    private NetworkVariable<bool> isGameReady = new NetworkVariable<bool>(
        false, 
        NetworkVariableReadPermission.Everyone,
        NetworkVariableWritePermission.Server);
    private bool goalAssigned = false;

    private void Update()
    {
        // Assign goal once when conditions are met
        if (!goalAssigned && IsOwner && GoalManager.Instance != null && GoalManager.Instance.AreGoalsLoaded())
        {
            AssignGoal();
            goalAssigned = true;
        }
    }

    // public override void OnNetworkSpawn()
    // {
    //     // Server initialization
    //     if (IsServer && IsOwner)
    //     {
    //         playerIndex.Value = NetworkManager.Singleton.ConnectedClients.Count - 1;
    //         AssignCharacter();
    //     }

    //     // Client initialization
    //     if (IsClient)
    //     {
    //         playerIndex.Value = NetworkManager.Singleton.ConnectedClients.Count - 1;
    //         Debug.Log("[NetworkPlayer] Player index: " + playerIndex.Value);
    //         SetPosition();
    //         CreateCharacterVisual();
    //         AssignGoal();
    //     }

    //     // Server-side player connection handling
    //     if (IsServer)
    //     {
    //         NetworkManager.Singleton.OnClientConnectedCallback += OnClientConnected;
    //     }

    //     // Event subscriptions
    //     isGameReady.OnValueChanged += OnGameReadyChanged;
    //     characterIndex.OnValueChanged += OnCharacterIndexChanged;
    // }

    // private void OnClientConnected(ulong clientId)
    // {
    //     // Start game when 4 players connect
    //     if (IsServer && NetworkManager.Singleton.ConnectedClients.Count == 4)
    //     {
    //         isGameReady.Value = true;
    //     }
    // }

    // public void OnGameReadyChanged(bool oldValue, bool newValue)
    // {
    //     // Show start button when game is ready
    //     if (newValue && IsOwner && IsServer)
    //     {
    //         var lobbyUI = Object.FindFirstObjectByType<LobbyUI>();
    //         if (lobbyUI != null)
    //         {
    //            // lobbyUI.ShowStartButton();
    //         }
    //     }
    // }

    private void AssignCharacter()
    {
        if (!IsServer) return;

        // Random character assignment
        int randomIndex = CharacterManager.Instance.GetRandomCharacterIndex();
        characterIndex.Value = randomIndex;
        character = CharacterManager.Instance.GetCharacter(randomIndex);

        if (character == null)
        {
            Debug.LogError("[NetworkPlayer] Character is null! Check CharacterManager.");
            return;
        }

        Debug.Log("[NetworkPlayer] Loaded character: " + character.characterName);
        
        // Instantiate character prefab
        GameObject characterObject = Instantiate(characterPrefab, waitingRoomSlots[playerIndex.Value], Quaternion.identity);
        NetworkObject netObj = characterObject.GetComponent<NetworkObject>();
        
        if (netObj == null)
        {
            Debug.LogError("[NetworkPlayer] Instantiated prefab missing NetworkObject!");
            return;
        }
        
        netObj.Spawn(true);
        
        // Set character sprite
        SpriteRenderer sr = characterObject.GetComponent<SpriteRenderer>();
        sr.sprite = character.neutral;
        Debug.Log("[NetworkPlayer] Sprite applied: " + sr.sprite?.name);
    }

    private void CreateCharacterVisual()
    {
        // Client-side character visualization
        character = CharacterManager.Instance.GetCharacter(characterIndex.Value);

        if (character == null)
        {
            Debug.LogError($"[NetworkPlayer] Failed to load character with index: {characterIndex.Value}");
            return;
        }

        GameObject characterObject = Instantiate(characterPrefab, waitingRoomSlots[playerIndex.Value], Quaternion.identity);
        SpriteRenderer sr = characterObject.GetComponent<SpriteRenderer>();
        sr.sprite = character.neutral;
        Debug.Log("[NetworkPlayer] Client applied sprite: " + sr.sprite?.name);
    }

    private void OnCharacterIndexChanged(int oldIndex, int newIndex)
    {
        Debug.Log("[NetworkPlayer] Character index updated! Creating client visual.");
        if (IsClient)
        {
            CreateCharacterVisual();
        }
    }

    private void SetPosition()
    {
        // Position player in waiting room slot
        if (playerIndex.Value < waitingRoomSlots.Length)
        {
            transform.position = waitingRoomSlots[playerIndex.Value];
        }
    }

    public void ChangeExpression(string mood)
    {
        if (!IsOwner) return;

        // Change character expression based on mood
        switch (mood)
        {
            case "smile":
                spriteRenderer.sprite = character.smiling;
                break;
            case "angry":
                spriteRenderer.sprite = character.angry;
                break;
            default:
                spriteRenderer.sprite = character.neutral;
                break;
        }
    }

    private void AssignGoal()
    {
        // Validate goal manager state
        if (GoalManager.Instance == null || GoalManager.Instance.goals == null || GoalManager.Instance.goals.Length == 0)
        {
            Debug.LogWarning("[NetworkPlayer] GoalManager not ready. Will retry.");
            return;
        }

        // Assign random goal
        int randomIndex = GoalManager.Instance.GetRandomGoalIndex();
        goalIndex = randomIndex;
        Goal g = GoalManager.Instance.GetGoal(randomIndex);
        Debug.Log($"[NetworkPlayer] Goal assigned: {g.title} - {g.description}");
    }
     public override void OnNetworkSpawn()
    {
        if (IsServer)
        {
            NetworkManager.Singleton.OnClientConnectedCallback += OnClientConnected;
        }

        isReady.OnValueChanged += OnReadyChanged;

        if (IsClient)
        {
            SetPosition();
            CreateCharacterVisual();
        }

        if (IsServer && IsOwner)
        {
            AssignCharacter();
        }
    }

    private void OnClientConnected(ulong clientId)
    {
        if (IsServer && NetworkManager.Singleton.ConnectedClients.Count == maxPlayers)
        {
            Debug.Log("[NetworkPlayer] Todos os jogadores conectados.");
        }
    }

    [ServerRpc]
    public void SetReadyServerRpc(bool value)
    {
        isReady.Value = value;
    }

    private void OnReadyChanged(bool oldVal, bool newVal)
    {
        if (!IsServer) return;

        if (newVal)
        {
            readyPlayerCount++;
            Debug.Log($"[NetworkPlayer] Jogador pronto! Total: {readyPlayerCount}/{maxPlayers}");
        }
        else
        {
            readyPlayerCount--;
            Debug.Log($"[NetworkPlayer] Jogador ficou não pronto. Total: {readyPlayerCount}/{maxPlayers}");
        }

        if (readyPlayerCount == maxPlayers)
        {
            Debug.Log("[NetworkPlayer] Todos os jogadores estão prontos. Iniciando jogo...");
            NetworkManager.Singleton.SceneManager.LoadScene("GameScene", LoadSceneMode.Single);
        }
    }
}