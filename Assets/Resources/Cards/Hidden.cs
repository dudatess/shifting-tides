using UnityEngine;

[CreateAssetMenu(fileName = "Hidden", menuName = "Scriptable Objects/Hidden")]
public class Hidden : ScriptableObject
{
          public class EffectData
    {
        public TargetEffect others;
        public TargetEffect self;
    }
    public class TargetEffect
    {
        public int people;
        public int influence;

        public int money;
    }
    public class Votes {
        public string type;
        public int count;
    }
    public int id;
    public string type;
    public string title;
    public string description;
    
    public EffectData effect;

}
