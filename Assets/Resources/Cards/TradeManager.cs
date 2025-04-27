using UnityEngine;

public class TradeManager : MonoBehaviour
{
    public static TradeManager Instance;
    
    public Trade[] trades;
    public Hidden[] hiddens;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            // LoadTrades();
        }
        else Destroy(gameObject);
    }

     void Start()
    {
         TextAsset jsonFileTrades = Resources.Load<TextAsset>("./Cards/trade-cards");
        TextAsset jsonFileHidden = Resources.Load<TextAsset>("./Cards/hidden-cards");
        if (jsonFileTrades != null && jsonFileHidden != null)
        {0
            trades = JsonHelper.FromJson<Trade>(jsonFileTrades.text);
            hiddens = JsonHelper.FromJson<Hidden>(jsonFileHidden);
            foreach (var trade in trades)
            {
                Debug.Log("Nome: " + trade.title);
            }
        }
        else
        {
            Debug.LogError("Arquivo JSON não encontrado!");
        }
    }
     public Goal GetTrade(int index)
{
    return (index >= 0 && index < trades.Length) ? trades[index] : null;
}
    public Goal GetHidden(int index)
    {
        return (index >= 0 && index < hiddens.Length) ? hiddens[index] : null;
    }
    // private void LoadTrades()
    // {
    //     goals = Resources.LoadAll<Trades>("Trades");
    //         Debug.Log("Loaded goals: " + goals.Length);
    // }
    public int GetRandomTradeIndex()
    {
        return  Random.Range(0, trades.Length);
    }
    public int GetRandomHiddenIndex()
    {
        if (Random.Range(0, 2) == 0) // 50% chance
        {
            return Random.Range(0, hiddens.Length);
        }

        return -1; // Retorna -1 se não quiser escolher nada
    }

}
