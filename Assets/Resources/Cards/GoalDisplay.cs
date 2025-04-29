using UnityEngine;
using TMPro;
using Unity.Netcode; 
public class GoalDisplay : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI goalText;
    [SerializeField] private GameObject cardObject; 
    void Start()
    {
        NetworkPlayer localPlayer = null;

        
        foreach (var player in FindObjectsByType<NetworkPlayer>(FindObjectsSortMode.None))
        {
            if (player.IsOwner)
            {
                localPlayer = player;
                break;
            }
        }

        if (localPlayer != null)
        {
            Goal goal = GoalManager.Instance.GetGoal(localPlayer.goalIndex);
            goalText.text = goal.description;
        }
        else
        {
            Debug.LogError("Player local não encontrado!");
        }
    }

    public void closeGoal() {
         cardObject.SetActive(false);
    }
}
