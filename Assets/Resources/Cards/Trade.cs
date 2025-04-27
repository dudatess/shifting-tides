using UnityEngine;

[CreateAssetMenu(fileName = "Trade", menuName = "Scriptable Objects/Trade")]
public class Trade : ScriptableObject
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

    public int id;
    public string type;

    public string title;
    public string description;
    
    public EffectData effect;
    
}
