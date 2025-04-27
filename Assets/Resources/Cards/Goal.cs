using UnityEngine;
[System.Serializable]
public class Goal : ScriptableObject {
    public class Resources {
        public int money;
        public int influence;
        public int people;
    }
    public int id;
    public string type;

    public string title;

    public string description;

    public Resources resources;

    public bool used = false;
}