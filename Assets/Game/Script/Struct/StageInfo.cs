using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(fileName = "StageInfo", menuName = "Scriptable Object/StageInfo", order = int.MaxValue), System.Serializable]
public class StageInfo : ScriptableObject
{
    public int no;
    public Sprite image;
    public GameObject field;
    public float globalLightIntensity = 1f;
    public Color globalLightColor = Color.white;
    public GameObject[] monster;
    public int level = 1;
    public StagePoint[] stagePoint;
    public StageInfo nextStage;
}

[System.Serializable]
public class StagePoint{
    public float time;
    public Type type;
    public bool alert = false;
    public Stat rate = new Stat();
    public string[] monster;    //삭제 예정
    public MonsterRate[] monsters;
    public int count;

    public enum Type{
        Monster = 0,
        Boss,
        aggressive
    }
}
[System.Serializable]
public class MonsterRate{
    public string name;
    public float rate;
}