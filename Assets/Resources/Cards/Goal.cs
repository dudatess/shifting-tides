using UnityEngine;

[System.Serializable]
public class Goal
{
    [System.Serializable]
    public class Resources
    {
        public int money = 0;  
        public int influence = 0;
        public int people = 0;
    }

    // Required fields (must exist in JSON)
    public int id;
    public string type;
    public string title;
    public string description;

    // Optional fields (with default values)
    public Resources resources = new Resources();
    public int rounds = -1; 

    [System.NonSerialized] 
    public bool used = false; 
}