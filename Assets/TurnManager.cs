using UnityEngine;
using Unity.Netcode;
using System.Collections.Generic;
using System.Collections;

public class TurnManager : NetworkBehaviour
{
    public static TurnManager Instance;

    private NetworkList<ulong> playerIds; // IDs dos jogadores conectados
    private int currentTurnIndex = 0;

    private Dictionary<ulong, bool> playerVotes = new Dictionary<ulong, bool>();

    [SerializeField] private TradeDisplay tradeCardUI; // UI para mostrar a carta Trade e Hidden
    [SerializeField] private VoteUI voteUI; // UI para mostrar os botões de voto

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            playerIds = new NetworkList<ulong>();
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public override void OnNetworkSpawn()
    {
        if (IsServer)
        {
            foreach (var client in NetworkManager.Singleton.ConnectedClientsList)
            {
                playerIds.Add(client.ClientId);
            }

            Debug.Log("TurnManager detectou " + playerIds.Count + " jogadores.");

            if (playerIds.Count == 4)
            {
                StartCoroutine(StartTurns());
            }
        }
    }

    private IEnumerator StartTurns()
    {
        yield return new WaitForSeconds(1f); // Pequena espera para garantir que todos carregaram

        while (true)
        {
            StartTurn();
            yield return new WaitUntil(() => playerVotes.Count == 3); // Só 3 votam, o jogador do turno não vota
            yield return new WaitForSeconds(1f); // Pequeno intervalo entre turnos
            NextTurn();
        }
    }

    private void StartTurn()
    {
        ulong currentPlayerId = playerIds[currentTurnIndex];
        Debug.Log("Vez do jogador: " + currentPlayerId);

        playerVotes.Clear();

        int tradeIndex = TradeManager.Instance.GetRandomTradeIndex();
        int hiddenIndex = TradeManager.Instance.GetRandomHiddenIndex();

        foreach (var id in playerIds)
        {
            if (id == currentPlayerId)
            {
                // Jogador do turno vê as cartas
                ShowCardsClientRpc(tradeIndex, hiddenIndex, id);
                HideVoteButtonsClientRpc(id);
            }
            else
            {
                // Outros jogadores só veem os botões
                HideCardsClientRpc(id);
                ShowVoteButtonsClientRpc(id);
            }
        }
    }

    private void NextTurn()
    {
        currentTurnIndex = (currentTurnIndex + 1) % playerIds.Count;
    }

    [ClientRpc]
    private void ShowCardsClientRpc(int tradeIndex, int hiddenIndex, ulong targetClientId)
    {
        if (NetworkManager.Singleton.LocalClientId != targetClientId) return;

        var trade = TradeManager.Instance.GetTrade(tradeIndex);
        var hidden = hiddenIndex != -1 ? TradeManager.Instance.GetHidden(hiddenIndex) : null;

        tradeCardUI.Show(trade.title, hidden?.description);
    }

    [ClientRpc]
    private void HideCardsClientRpc(ulong targetClientId)
    {
        if (NetworkManager.Singleton.LocalClientId != targetClientId) return;

        tradeCardUI.Hide();
    }

    [ClientRpc]
    private void ShowVoteButtonsClientRpc(ulong targetClientId)
    {
        if (NetworkManager.Singleton.LocalClientId != targetClientId) return;

        voteUI.Show();
    }

    [ClientRpc]
    private void HideVoteButtonsClientRpc(ulong targetClientId)
    {
        if (NetworkManager.Singleton.LocalClientId != targetClientId) return;

        voteUI.Hide();
    }

    // Chamado pelo VoteUI
    public void SubmitVote(bool vote)
    {
        if (!IsClient) return;

        SubmitVoteServerRpc(vote);
    }

    [ServerRpc(RequireOwnership = false)]
    private void SubmitVoteServerRpc(bool vote, ServerRpcParams rpcParams = default)
    {
        if (!playerVotes.ContainsKey(rpcParams.Receive.SenderClientId))
        {
            playerVotes.Add(rpcParams.Receive.SenderClientId, vote);
            Debug.Log("Recebido voto de " + rpcParams.Receive.SenderClientId + ": " + (vote ? "Sim" : "Não"));
        }
    }
}
