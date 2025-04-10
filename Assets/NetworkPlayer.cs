using UnityEngine;
using Unity.Netcode; // Ou Mirror se for o Mirror
using UnityEngine.UI;

public class NetworkPlayer : NetworkBehaviour
{
    [SerializeField] private GameObject characterPrefab; // Prefab que será instanciado
    private static int playerCount = 0; // Para contar quantos jogadores já conectaram
    private Character character; // Personagem do jogador
    private SpriteRenderer spriteRenderer; // Sprite do personagem

    void Start()
    {
        if (IsOwner) // Só o jogador local (quem se conectou) vai fazer isso
        {
            AssignCharacter(); // Atribui um personagem aleatório
            SetPosition(); // Posiciona o personagem na tela
        }
    }

    // Função que atribui um personagem aleatório para o jogador
    private void AssignCharacter()
    {
        int randomIndex = CharacterManager.Instance.GetRandomCharacterIndex();
        character = CharacterManager.Instance.GetCharacter(randomIndex);
        
        // Instancia o personagem (sprite) no jogo
        GameObject characterObject = Instantiate(characterPrefab, transform.position, Quaternion.identity);
        spriteRenderer = characterObject.GetComponent<SpriteRenderer>();
        
        // Atribui o sprite neutro inicialmente
        spriteRenderer.sprite = character.neutral;
    }

    // Posiciona os jogadores na tela, da esquerda para a direita
    private void SetPosition()
    {
        // A posição X será determinada pelo número de jogadores conectados
        float posX = playerCount * 2.0f; // 2 unidades de distância entre jogadores (ajuste conforme necessário)
        transform.position = new Vector3(posX, 0, 0); // Y = 0, Z = 0 para 2D

        playerCount++; // Incrementa o contador de jogadores
    }

    // Se o jogador mudar a expressão (por exemplo, sorrir ou ficar bravo)
    public void ChangeExpression(string mood)
    {
        if (mood == "smile")
        {
            spriteRenderer.sprite = character.smiling;
        }
        else if (mood == "angry")
        {
            spriteRenderer.sprite = character.angry;
        }
        else
        {
            spriteRenderer.sprite = character.neutral;
        }
    }
}
