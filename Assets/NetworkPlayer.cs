using UnityEngine;
using Unity.Netcode;
using UnityEngine.SceneManagement;

public class NetworkPlayer : NetworkBehaviour
{
    [SerializeField] private GameObject characterPrefab; // Prefab visual do personagem
    private NetworkVariable<int> playerIndex = new NetworkVariable<int>();
    private NetworkVariable<int> characterIndex = new NetworkVariable<int>(
        default,
        NetworkVariableReadPermission.Everyone,
        NetworkVariableWritePermission.Server);

    private Character character;
    private SpriteRenderer spriteRenderer;

    private readonly Vector3[] waitingRoomSlots = new Vector3[]
    {
        new Vector3(-3f, 0, 0),
        new Vector3(-1f, 0, 0),
        new Vector3(1f, 0, 0),
        new Vector3(3f, 0, 0)
    };

    public override void OnNetworkSpawn()
    {
        if (IsServer && IsOwner)
        {
            playerIndex.Value = NetworkManager.Singleton.ConnectedClients.Count - 1;
            AssignCharacter();
        }

        if (IsClient)
        {
            SetPosition();
            CreateCharacterVisual();
        }

        if (IsServer && NetworkManager.Singleton.ConnectedClients.Count == 4)
        {
            Debug.Log("All players ready! Starting game...");
            StartGame();
        }

        characterIndex.OnValueChanged += OnCharacterIndexChanged;
    }

    private void AssignCharacter()
    {
        if (!IsServer) return;

        Debug.Log("AssignCharacter começou");

        int randomIndex = CharacterManager.Instance.GetRandomCharacterIndex();
        characterIndex.Value = randomIndex;
        character = CharacterManager.Instance.GetCharacter(randomIndex);

        if (character == null)
        {
            Debug.LogError("Character é null! Verifique o CharacterManager.");
            return;
        }

        Debug.Log("Loaded character: " + character.characterName);
        Debug.Log("Assigned neutral sprite: " + character.neutral?.name);

        GameObject characterObject = Instantiate(characterPrefab, waitingRoomSlots[playerIndex.Value], Quaternion.identity);

        NetworkObject netObj = characterObject.GetComponent<NetworkObject>();
        if (netObj == null)
        {
            Debug.LogError("Prefab instanciado não tem NetworkObject!");
            return;
        }
        netObj.Spawn(true);

        SpriteRenderer sr = characterObject.GetComponent<SpriteRenderer>();
        sr.sprite = character.neutral;

        Debug.Log("Sprite aplicado com sucesso: " + sr.sprite?.name);
    }

    private void CreateCharacterVisual()
    {
        character = CharacterManager.Instance.GetCharacter(characterIndex.Value);

        if (character == null)
        {
            Debug.LogError("Cliente não conseguiu carregar o personagem com índice: " + characterIndex.Value);
            return;
        }

        GameObject characterObject = Instantiate(characterPrefab, waitingRoomSlots[playerIndex.Value], Quaternion.identity);
        SpriteRenderer sr = characterObject.GetComponent<SpriteRenderer>();
        sr.sprite = character.neutral;

        Debug.Log("Cliente aplicou sprite: " + sr.sprite?.name);
    }

    private void OnCharacterIndexChanged(int oldIndex, int newIndex)
    {
        Debug.Log("Character index updated! Criando personagem visual no cliente.");
        if (IsClient)
        {
            CreateCharacterVisual();
        }
    }

    private void SetPosition()
    {
        if (playerIndex.Value < waitingRoomSlots.Length)
        {
            transform.position = waitingRoomSlots[playerIndex.Value];
        }
    }

    public void ChangeExpression(string mood)
    {
        if (!IsOwner) return;

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

    private void StartGame()
    {
        if (IsServer)
        {
            NetworkManager.Singleton.SceneManager.LoadScene("GameScene", LoadSceneMode.Single);
        }
    }
}
