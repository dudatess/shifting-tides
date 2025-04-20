using UnityEngine;
using Unity.Netcode;
using UnityEngine.UI;
using UnityEngine.SceneManagement; 

public class NetworkPlayer : NetworkBehaviour
{
    [SerializeField] private GameObject characterPrefab; // Prefab to be instantiated
    private NetworkVariable<int> playerIndex = new NetworkVariable<int>(); // Synced across network
    private Character character; // Player's character data
    private SpriteRenderer spriteRenderer; // Character's sprite renderer

    //Static positions for 4 players in waiting area ?
    private readonly Vector3[] waitingRoomSlots = new Vector3[]
    {
        new Vector3(-3f, 0, 0), // Player 1 slot
        new Vector3(-1f, 0, 0), // Player 2 slot
        new Vector3(1f, 0, 0),  // Player 3 slot
        new Vector3(3f, 0, 0)    // Player 4 slot
    };

    public override void OnNetworkSpawn()
    {
        if (IsOwner)
        {
            // Server assigns unique index to each player
            if (IsServer)
            {
                playerIndex.Value = NetworkManager.Singleton.ConnectedClients.Count - 1;
            }
            
            AssignCharacter();
            SetPosition();

            //Auto-start game when 4 players connect (server only)
            if (IsServer && NetworkManager.Singleton.ConnectedClients.Count == 4)
            {
                Debug.Log("All players ready! Starting game...");
                StartGame();
            }
        }
    }

    // Assigns random character to player
    private void AssignCharacter()
    {
        int randomIndex = CharacterManager.Instance.GetRandomCharacterIndex();
        character = CharacterManager.Instance.GetCharacter(randomIndex);
        
        // Instantiates character sprite
        GameObject characterObject = Instantiate(characterPrefab, transform.position, Quaternion.identity);
        spriteRenderer = characterObject.GetComponent<SpriteRenderer>();
        
        // Applies neutral sprite initially
        spriteRenderer.sprite = character.neutral;
    }

    // Positions players side-by-side in waiting area
    private void SetPosition()
    {
        //Uses predefined slots instead of dynamic calculation
        if (playerIndex.Value < waitingRoomSlots.Length)
        {
            transform.position = waitingRoomSlots[playerIndex.Value];
        }
    }

    // Changes character expression (smile/angry/neutral)
    public void ChangeExpression(string mood)
    {
        if (!IsOwner) return; // Only owner can change expressions

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

    // Transitions to game 
    private void StartGame()
    {
        if (IsServer)
        {
            //Load new scene 
            NetworkManager.Singleton.SceneManager.LoadScene("GameScene", LoadSceneMode.Single);
            
        }
    }
}