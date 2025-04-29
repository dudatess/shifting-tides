using UnityEngine;

[System.Serializable]
public class Goal : ScriptableObject
{
    [System.Serializable]
    public class Resources
    {
        public int? money;
        public int? influence;
        public int? people;
    }

    [System.Serializable]
    public class Requirements
    {
        public Resources resources;
        public int? rounds;
        public string target;
    }

    public int id;
    public string type;
    public string title;
    public string description;
    public Requirements requirements;
    public bool used = false;
}
