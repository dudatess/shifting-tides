using UnityEngine;

public class CharacterPreview : MonoBehaviour
{
    [SerializeField] private SpriteRenderer faceRenderer;

    private void Start()
    {
        int randomIndex = CharacterManager.Instance.GetRandomCharacterIndex();
        Character c = CharacterManager.Instance.GetCharacter(randomIndex);

        faceRenderer.sprite = c.neutral; // Aplica o sprite neutro
    }
}
