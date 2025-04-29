using UnityEngine;

public class VoteUI : MonoBehaviour
{
    public void Show()
    {
        gameObject.SetActive(true); // Ativa o próprio objeto (que é o painel)
    }

    public void Hide()
    {
        gameObject.SetActive(false); // Desativa o próprio objeto
    }
}
