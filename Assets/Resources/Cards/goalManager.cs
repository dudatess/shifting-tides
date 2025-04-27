using UnityEngine;

public class GoalManager : MonoBehaviour
{
    public static GoalManager Instance;
    
    public Goal[] goals;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            //LoadGoal();
        }
        else Destroy(gameObject);
    }
    void Start()
    {
         TextAsset jsonFile = Resources.Load<TextAsset>("./Cards/goal-cards");

        if (jsonFile != null)
        {
            goals = JsonHelper.FromJson<Goal>(jsonFile.text);
            
            foreach (var goal in goals)
            {
                Debug.Log("Nome: " + goal.title);
            }
        }
        else
        {
            Debug.LogError("Arquivo JSON não encontrado!");
        }
    }

     public Goal GetGoal(int index)
{
    return (index >= 0 && index < goals.Length) ? goals[index] : null;
}

    // private void LoadGoal()
    // {
    //     goals = Resources.LoadAll<Goal>("Goals");
    //         Debug.Log("Loaded goals: " + goals.Length);
    // }
    public int GetRandomGoalIndex()
    {
        int goalAssigned = Random.Range(0, goals.Length);

        while (goals[goalAssigned].used) {
            goalAssigned = Random.Range(0, goals.Length);
        }
        goals[goalAssigned].used = true;
        return goalAssigned;
    }

}
