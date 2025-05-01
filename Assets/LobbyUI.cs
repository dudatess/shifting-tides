using UnityEngine;
using UnityEngine.UI;
using Unity.Netcode;
using UnityEngine.SceneManagement;

public class LobbyUI : MonoBehaviour
{
    [SerializeField] private Button readyButton;
    [SerializeField] private float checkInterval = 0.5f; // Intervalo para verificar o jogador
    
    private NetworkPlayer networkPlayer;
    private float checkTimer;

    private void Start()
    {
        readyButton.gameObject.SetActive(false);
        readyButton.onClick.AddListener(OnReadyClicked);
        
        // Duas formas de detectar o jogador:
        NetworkManager.Singleton.OnClientConnectedCallback += OnClientConnected;
        NetworkManager.Singleton.OnClientDisconnectCallback += OnClientDisconnected;
    }

    private void Update()
    {
        // Sistema de fallback caso o callback não funcione
        if (networkPlayer == null)
        {
            checkTimer += Time.deltaTime;
            if (checkTimer >= checkInterval)
            {
                checkTimer = 0;
                TryFindLocalPlayer();
            }
        }
    }

    private void OnClientConnected(ulong clientId)
    {
        if (NetworkManager.Singleton.LocalClientId == clientId)
        {
            TryFindLocalPlayer();
        }
    }

    private void OnClientDisconnected(ulong clientId)
    {
        if (NetworkManager.Singleton.LocalClientId == clientId)
        {
            readyButton.gameObject.SetActive(false);
            networkPlayer = null;
        }
    }

    private void TryFindLocalPlayer()
    {
        if (NetworkManager.Singleton == null || 
            NetworkManager.Singleton.SpawnManager == null) 
            return;

        // Tenta encontrar o jogador local
        NetworkObject localPlayerObj = NetworkManager.Singleton.SpawnManager.GetLocalPlayerObject();
        
        if (localPlayerObj != null)
        {
            networkPlayer = localPlayerObj.GetComponent<NetworkPlayer>();
            
            if (networkPlayer != null)
            {
                Debug.Log("NetworkPlayer encontrado: " + networkPlayer.gameObject.name);
                readyButton.gameObject.SetActive(true);
            }
            else
            {
                Debug.LogError("Objeto do jogador não tem componente NetworkPlayer!");
            }
        }
    }

    private void OnReadyClicked()
    {
        if (networkPlayer != null)
        {
            networkPlayer.SetReadyServerRpc(true);
            readyButton.interactable = false;
            readyButton.GetComponentInChildren<Text>().text = "Waiting...";
        }
    }

    private void OnDestroy()
    {
        // Limpe os callbacks quando o objeto for destruído
        if (NetworkManager.Singleton != null)
        {
            NetworkManager.Singleton.OnClientConnectedCallback -= OnClientConnected;
            NetworkManager.Singleton.OnClientDisconnectCallback -= OnClientDisconnected;
        }
    }
}