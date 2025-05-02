using UnityEngine;
using Unity.Netcode;
using UnityEngine.SceneManagement;
using System.Collections; // Necessário para usar IEnumerator
public class NetworkPlayer : NetworkBehaviour
{
    [SerializeField] private float characterScale = 3f; // Adicione esta linha com as outras variáveis
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

    public int factionName;
    // Waiting room positions
    // Substitua o waitingRoomSlots por estas posições mais à esquerda
    private readonly Vector3[] waitingRoomSlots = new Vector3[]
    {
        new Vector3(-8f, 0, 0),  // Mais à esquerda
        new Vector3(-6f, 0, 0),
        new Vector3(-3f, 0, 0),
        new Vector3(2f, 0, 0)    // Mais à direita
    };
    private bool goalAssigned = false;

    public override void OnNetworkSpawn()
    {
        Debug.Log("[NetworkPlayer] OnNetworkSpawn chamado");

        if (IsServer)
        {
            Debug.Log("[NetworkPlayer] Sou o servidor");

            if (GameSessionManager.Instance == null)
            {
                Debug.LogError("[NetworkPlayer] GameSessionManager.Instance está NULL");
                return;
            }

            if (characterPrefab == null)
            {
                Debug.LogError("[NetworkPlayer] characterPrefab NÃO foi atribuído no Inspetor.");
                return;
            }

            playerIndex.Value = GameSessionManager.Instance.GetNextPlayerIndex();

            // Verifica se o índice é válido
            if (playerIndex.Value >= 0 && playerIndex.Value < waitingRoomSlots.Length)
            {
                AssignCharacter();
            }
            else
            {
                Debug.LogError($"[NetworkPlayer] Índice de jogador inválido: {playerIndex.Value}");
            }
        }

        if (IsClient)
        {
            characterIndex.OnValueChanged += OnCharacterIndexChanged;
        }

        isReady.OnValueChanged += OnReadyChanged;
    }

    private void AssignCharacter()
    {
        if (!IsServer) return;

        // Verifica se CharacterManager está disponível
        if (CharacterManager.Instance == null)
        {
            Debug.LogError("[NetworkPlayer] CharacterManager.Instance está NULL");
            return;
        }

        // Se já temos um personagem, não criar outro
        if (spriteRenderer != null && spriteRenderer.gameObject != null)
        {
            Debug.Log("[NetworkPlayer] Personagem já existe, apenas atualizando");
            return;
        }

        int randomIndex = CharacterManager.Instance.GetRandomCharacterIndex();
        characterIndex.Value = randomIndex;
        character = CharacterManager.Instance.GetCharacter(randomIndex);

        if (character == null)
        {
            Debug.LogError("[NetworkPlayer] Character é null! Verifique CharacterManager.");
            return;
        }

        Debug.Log("[NetworkPlayer] Personagem carregado: " + character.characterName);

        GameObject characterObject = Instantiate(characterPrefab, waitingRoomSlots[playerIndex.Value], Quaternion.identity);
        NetworkObject netObj = characterObject.GetComponent<NetworkObject>();

        if (netObj == null)
        {
            Debug.LogError("[NetworkPlayer] Prefab instanciado não tem NetworkObject!");
            Destroy(characterObject);
            return;
        }

        // Verifica se o objeto já foi spawnado antes de spawnar novamente
        if (!netObj.IsSpawned)
        {
            netObj.Spawn(true);
        }

        spriteRenderer = characterObject.GetComponent<SpriteRenderer>();
        if (spriteRenderer == null)
        {
            Debug.LogError("[NetworkPlayer] Prefab instanciado não tem SpriteRenderer!");
            return;
        }

        spriteRenderer.sprite = character.neutral;
        characterObject.transform.localScale = new Vector3(characterScale, characterScale, 1f);
        Debug.Log("[NetworkPlayer] Sprite aplicado: " + spriteRenderer.sprite?.name);
    }

    private IEnumerator RevealCharacter(GameObject characterObject, int index)
    {
        // Posição inicial (fora da tela à esquerda)
        Vector3 startPos = new Vector3(-10f, waitingRoomSlots[index].y, 0f);
        Vector3 endPos = waitingRoomSlots[index];
        float duration = 0.5f + (index * 0.3f); // Cada personagem tem um delay diferente

        characterObject.transform.position = startPos;

        float elapsed = 0f;
        while (elapsed < duration)
        {
            characterObject.transform.position = Vector3.Lerp(startPos, endPos, elapsed / duration);
            elapsed += Time.deltaTime;
            yield return null;
        }

        characterObject.transform.position = endPos;
    }
    private void CreateCharacterVisual()
    {
        character = CharacterManager.Instance.GetCharacter(characterIndex.Value);
        if (character == null) return;

        GameObject characterObject = Instantiate(characterPrefab, waitingRoomSlots[playerIndex.Value], Quaternion.identity);
        SpriteRenderer sr = characterObject.GetComponent<SpriteRenderer>();
        sr.sprite = character.neutral;

        // Aplica a mesma escala
        characterObject.transform.localScale = new Vector3(characterScale, characterScale, 1f);
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
    //  public override void OnNetworkSpawn()
    // {
    //     if (IsServer)
    //     {
    //         NetworkManager.Singleton.OnClientConnectedCallback += OnClientConnected;
    //     }

    //     isReady.OnValueChanged += OnReadyChanged;

    //     if (IsClient)
    //     {
    //         SetPosition();
    //         CreateCharacterVisual();
    //     }

    //     if (IsServer && IsOwner)
    //     {
    //         AssignCharacter();
    //     }
    // }

    //    public override void OnNetworkSpawn()
    // {
    //     Debug.Log("[NetworkPlayer] OnNetworkSpawn chamado");

    //     if (IsServer)
    //     {
    //         Debug.Log("[NetworkPlayer] Sou o servidor");

    //         if (GameSessionManager.Instance == null)
    //         {
    //             Debug.LogError("[NetworkPlayer] GameSessionManager.Instance está NULL");
    //         }

    //         if (characterPrefab == null)
    //         {
    //             Debug.LogError("[NetworkPlayer] characterPrefab NÃO foi atribuído no Inspetor.");
    //         }

    //         playerIndex.Value = GameSessionManager.Instance?.GetNextPlayerIndex() ?? 0;
    //         AssignCharacter();
    //     }

    //     if (IsClient)
    //     {
    //         characterIndex.OnValueChanged += OnCharacterIndexChanged;
    //         playerIndex.OnValueChanged += OnPlayerIndexChanged;
    //     }

    //     isReady.OnValueChanged += OnReadyChanged;
    // }


    private void OnPlayerIndexChanged(int oldIndex, int newIndex)
    {
        SetPosition();
        CreateCharacterVisual();
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