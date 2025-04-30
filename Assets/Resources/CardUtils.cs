using UnityEngine;
using TMPro;
using Unity.Netcode; 
public class CardUtils : MonoBehaviour
{
    [SerializeField] private GameObject card; 

    public void Start() {
        card.SetActive(false);
    }
    public void closeCard() {
         card.SetActive(false);
    }

    public void openCard() {
        card.SetActive(true);
    }
    
}
