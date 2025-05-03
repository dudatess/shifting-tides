using UnityEngine;
using TMPro;
using Unity.Netcode; 
public class CardUtils : MonoBehaviour
{
    [SerializeField] private GameObject card; 

    public void Start() {
        card.SetActive(false);
    }
    public static void closeCard(GameObject cardParameter) {
         cardParameter.SetActive(false);
    }

    public static void openCard(GameObject cardParameter) {
        cardParameter.SetActive(true);
    }
    
}
