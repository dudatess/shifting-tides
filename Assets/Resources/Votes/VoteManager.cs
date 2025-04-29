using UnityEngine;
using UnityEngine.UI;

public class VoteManager : MonoBehaviour
{
    public static VoteManager Instance;

    [SerializeField] private Button yesButton;
    [SerializeField] private Button noButton;
    [SerializeField] private Color lockedColor = new Color(0.7f, 0.7f, 0.7f); // cinza claro
    [SerializeField] private Color normalColor = Color.white;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }
        public void Show()
    {
        yesButton.gameObject.SetActive(true);
        noButton.gameObject.SetActive(true);
    }

    public void Hide()
    {
        yesButton.gameObject.SetActive(false);
        noButton.gameObject.SetActive(false);
    }

    public void SetVotingEnabled(bool enabled)
    {
        yesButton.interactable = enabled;
        noButton.interactable = enabled;

        Color targetColor = enabled ? normalColor : lockedColor;

        var yesColors = yesButton.colors;
        yesColors.normalColor = targetColor;
        yesButton.colors = yesColors;

        var noColors = noButton.colors;
        noColors.normalColor = targetColor;
        noButton.colors = noColors;
    }

    public void VoteYes()
    {
        if (yesButton.interactable)
        {
            TurnManager.Instance.SubmitVote(true);
            Hide();
        }
    }

    public void VoteNo()
    {
        if (noButton.interactable)
        {
            TurnManager.Instance.SubmitVote(false);
            Hide();
        }
    }
}
