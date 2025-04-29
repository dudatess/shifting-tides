using UnityEngine;
using TMPro;

public class TradeDisplay : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI tradeText; // A caixa de texto para exibir o título

    private Trade currentTrade;
    private Hidden currentHidden;

    public void Show(string tradeTitle, string hiddenDescription)
    {
        int tradeIndex = TradeManager.Instance.GetRandomTradeIndex();
        int hiddenIndex = TradeManager.Instance.GetRandomHiddenIndex();

        // Pega o Trade
        currentTrade = TradeManager.Instance.trades[tradeIndex];

        // Verifica se tem hidden
        if (hiddenIndex != -1)
        {
            // Pega a carta hidden
            currentHidden = TradeManager.Instance.hiddens[hiddenIndex];
            
            // Exibe a concatenação do título do trade com o título da carta hidden
            tradeText.text = currentTrade.title + " - " + currentHidden.title;
        }
        else
        {
            // Apenas exibe o título do trade se não houver carta hidden
            tradeText.text = currentTrade.title;
        }

        // Exibe a caixa de texto com a concatenação
        tradeText.gameObject.SetActive(true);
    }

    public void Hide()
    {
        tradeText.gameObject.SetActive(false); // Esconde a caixa de texto
    }
}
