using UnityEngine;
using UnityEngine.UI;
using Unity.Netcode;
using UnityEngine.SceneManagement;

public class LobbyUI : MonoBehaviour
{
    [SerializeField] private Button readyButton;

    private NetworkPlayer networkPlayer;

    private void Start()
    {
        readyButton.gameObject.SetActive(false);
        readyButton.onClick.AddListener(OnReadyClicked);
        NetworkManager.Singleton.OnClientConnectedCallback += OnClientConnected;
    }

    private void OnClientConnected(ulong clientId)
    {
        if (NetworkManager.Singleton.LocalClientId == clientId)
        {
            networkPlayer = NetworkManager.Singleton.SpawnManager
                .GetLocalPlayerObject()
                .GetComponent<NetworkPlayer>();

            if (networkPlayer == null)
            {
                Debug.LogError("NetworkPlayer não encontrado!");
                return;
            }

            Debug.Log("NetworkPlayer conectado: " + networkPlayer.gameObject.name);

            // Mostra o botão para o jogador local após encontrar o NetworkPlayer
            readyButton.gameObject.SetActive(true);
        }
    }

    private void OnReadyClicked()
    {
        if (networkPlayer != null)
        {
            networkPlayer.SetReadyServerRpc(true);
            readyButton.interactable = false; // Desativa para evitar múltiplos cliques
        }
    }
}
