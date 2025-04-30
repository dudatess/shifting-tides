using UnityEngine;

[CreateAssetMenu(fileName = "Trade", menuName = "Scriptable Objects/Trade")]
public class Trade : ScriptableObject
{
    [System.Serializable]
    public class EffectData
    {
        public TargetEffect self;
        public TargetEffect others;
    }

    [System.Serializable]
    public class TargetEffect
    {
        public int? money;
        public int? people;
        public int? influence;
    }

    public int id;
    public string type;
    public string title;
    public string description;
    public EffectData effect;
}
