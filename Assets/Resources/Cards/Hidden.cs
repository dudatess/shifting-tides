using UnityEngine;

[CreateAssetMenu(fileName = "Hidden", menuName = "Scriptable Objects/Hidden")]
public class Hidden : ScriptableObject
{
    [System.Serializable]
    public class TargetEffect
    {
        public int people;
        public int influence;
        public int money;
    }

    [System.Serializable]
    public class EffectData
    {
        public TargetEffect others;
        public TargetEffect self;
    }

    [System.Serializable]
    public class Votes
    {
        public string type;  // "yes" ou "no"
        public int count;
    }

    public int id;
    public string type;
    public string title;
    public string description;

    public EffectData effect;
    public Votes votes;
}
