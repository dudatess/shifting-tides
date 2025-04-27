using UnityEngine;
using TMPro;
using Unity.Netcode; 
public class TradeDisplay : MonoBehaviour
{
    [SerializeField] private GameObject tradeCardObject; // O GameObject da carta de trade
    [SerializeField] private TextMeshProUGUI tradeTitleText;
    [SerializeField] private TextMeshProUGUI hiddenTitleText;

    private Trade currentTrade;
    private Hidden currentHidden;

    public void ShowNewTrade()
    {
        int tradeIndex = TradeManager.Instance.GetRandomTradeIndex();
        int hiddenIndex = TradeManager.Instance.GetRandomHiddenIndex();

        // Pega o Trade
        currentTrade = TradeManager.Instance.trades[tradeIndex];
        
        // Atualiza o título do trade
        tradeTitleText.text = currentTrade.title;

        // Verifica se tem hidden
        if (hiddenIndex != -1)
        {
            currentHidden = TradeManager.Instance.hiddens[hiddenIndex];
            hiddenTitleText.text = currentHidden.title; // Mostra o título da carta escondida
            hiddenTitleText.gameObject.SetActive(true);
        }
        else
        {
            currentHidden = null;
            hiddenTitleText.text = "";
            hiddenTitleText.gameObject.SetActive(false); // Esconde o campo se não tiver hidden
        }

        // Mostra o card de trade na tela
        tradeCardObject.SetActive(true);
    }

    public void HideTrade()
    {
        tradeCardObject.SetActive(false);
    }
}
