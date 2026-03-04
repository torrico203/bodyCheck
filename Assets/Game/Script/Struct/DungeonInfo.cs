using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(fileName = "DungeonInfo", menuName = "Scriptable Object/DungeonInfo", order = int.MaxValue), System.Serializable]
public class DungeonInfo : ScriptableObject
{
    public int no;
    public Sprite image;
    public GameObject[] maps;
    public GameObject[] monster;
    public MonsterRate[] middleBoss;
    public int level = 1;
    public DungeonRoomInfo[] rooms;
    public DungeonInfo nextStage;
}

[System.Serializable]
public class DungeonRoomInfo{
    public Stat rate = new Stat();
    public MonsterRate[] monsters;
    public int count;

}
// [System.Serializable]
// public class MonsterRate{
//     public string name;
//     public float rate;
// }