using UnityEngine;
using UnityEngine.UI;
using Unity.Netcode;
using TMPro;

public class FactionInputModal : MonoBehaviour
{
    [Header("UI Components")]
    [SerializeField] public GameObject modalPanel;
    [SerializeField] public TMP_InputField factionInput;
    [SerializeField] public Button hostButton;
    [SerializeField] public Button clientButton;
    [SerializeField] public Button acceptButton;
    [SerializeField] public TextMeshProUGUI statusText;

    private int playerIndex = 0;
    private bool isHost = false;
    [SerializeField] private MainMenu mainMenu;
    private void Start()
    {
        // Configuração inicial
        modalPanel.SetActive(true);
        
        // Busca o NetworkPlayer para obter o índice
        NetworkPlayer[] players = FindObjectsOfType<NetworkPlayer>();
        playerIndex = players.Length;
        
        // Define o texto padrão
        factionInput.text = $"Faction {playerIndex + 1}";
        
        // Configura listeners de botão
        hostButton.onClick.AddListener(OnHostClicked);
        clientButton.onClick.AddListener(OnClientClicked);
        acceptButton.onClick.AddListener(OnAcceptClicked);
        
        hostButton.gameObject.SetActive(true);
        clientButton.gameObject.SetActive(true);
        acceptButton.gameObject.SetActive(true);
        factionInput.interactable = true;
    }

    
    public void OnHostClicked()
    {
        isHost = true;
        CardUtils.openCard(modalPanel);
    //    NetworkManager.Singleton.StartHost();
    //     OnConnectionStarted();
    }

    public void OnClientClicked()
    {
        isHost = false;
        CardUtils.openCard(modalPanel);
        // NetworkManager.Singleton.StartClient();
        // OnConnectionStarted();
    }

    // private void OnConnectionStarted()
    // {
    //     statusText.text = isHost ? "Hosting game..." : "Connecting...";
    //     ShowConnectionButtons(false);
        
    //     // Verifica conexão periodicamente
    //     InvokeRepeating(nameof(CheckConnection), 0.5f, 0.5f);
    // }

    // private void CheckConnection()
    // {
    //     if (NetworkManager.Singleton.IsClient || NetworkManager.Singleton.IsHost)
    //     {
    //         statusText.text = "Connected! Enter faction name:";
    //         CancelInvoke(nameof(CheckConnection));
    //     }
    // }

    private void OnAcceptClicked()
    {
        if (string.IsNullOrWhiteSpace(factionInput.text))
        {
            statusText.text = "Please enter a faction name!";
            return;
        }

        // Aqui você pode salvar o nome da facção para uso posterior
        PlayerPrefs.SetString($"Player{playerIndex}_Faction", factionInput.text);
        
        if(isHost){
            mainMenu.HostGame();
        }
        else {
            mainMenu.JoinGame();
        }
        // Carrega a cena do lobby
       //  NetworkManager.Singleton.SceneManager.LoadScene("Lobby", LoadSceneMode.Single);
    }

    // Atualiza o texto de status quando o input é alterado
    public void OnFactionInputChanged(string newText)
    {
        if (newText.Length > 20)
        {
            factionInput.text = newText.Substring(0, 20);
            statusText.text = "Max 20 characters!";
        }
        else
        {
            statusText.text = "";
        }
    }
}