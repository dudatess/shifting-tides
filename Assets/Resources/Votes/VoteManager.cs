using UnityEngine;

public class VoteManager : MonoBehaviour
{
    public static int totalYes = 0;
    public static int totalNo = 0;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void voteYes(){
        totalYes++;
    }

    public void voteNo() {
        totalNo++;
    }
}
