using UnityEngine;
using UnityEngine.UI;
using Unity.Netcode;
using UnityEngine.SceneManagement;

public class LobbyUI : MonoBehaviour
{
    [SerializeField] private Button startButton;
    [SerializeField] private NetworkPlayer networkPlayer; // Referência ao NetworkPlayer

   private void Start()
{
    startButton.gameObject.SetActive(false);
    startButton.onClick.AddListener(StartGame);
    
    // Busca o NetworkPlayer do jogador LOCAL quando ele spawna
    NetworkManager.Singleton.OnClientConnectedCallback += OnClientConnected;
}

private void OnClientConnected(ulong clientId)
{
    // Só executa para o jogador local
    if (NetworkManager.Singleton.LocalClientId == clientId)
    {
        networkPlayer = NetworkManager.Singleton.SpawnManager
                       .GetLocalPlayerObject()
                       .GetComponent<NetworkPlayer>();

        if (networkPlayer == null)
        {
            Debug.LogError("NetworkPlayer não encontrado no LocalPlayer!");
            return;
        }

        Debug.Log($"NetworkPlayer encontrado: {networkPlayer.gameObject.name}");

        // Verifica se é o Host e mostra o botão quando 4 jogadores estiverem prontos
        if (NetworkManager.Singleton.IsServer)
        {
            networkPlayer.OnGameReadyChanged(false, true); // Força a verificação
        }
    }
}
    

    public void ShowStartButton()
    {
        Debug.Log("[LobbyUI] ShowStartButton chamado");
        
        if (startButton != null)
        {
            startButton.gameObject.SetActive(true);
            Debug.Log("[LobbyUI] Botão ativado com sucesso");
        }
        else
        {
            Debug.LogError("[LobbyUI] Tentativa de ativar botão nulo");
        }
    }

    private void StartGame()
    {
        Debug.Log("[LobbyUI] Tentativa de iniciar jogo");
        
        if (NetworkManager.Singleton.IsServer)
        {
            Debug.Log("[LobbyUI] Host confirmado, carregando cena...");
            NetworkManager.Singleton.SceneManager.LoadScene("GameScene", LoadSceneMode.Single);
        }
        else
        {
            Debug.LogWarning("[LobbyUI] StartGame chamado por cliente não-host");
        }
    }
}