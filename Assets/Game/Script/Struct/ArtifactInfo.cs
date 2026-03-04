using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(fileName = "ArtifactInfo", menuName = "Scriptable Object/ArtifactInfo", order = int.MaxValue), System.Serializable]
public class ArtifactInfo : ScriptableObject
{
    public int no;
    public Sprite icon;
    public bool addition;
    public Stat stat;
    //public int price;
    public int priceMin;
    public int priceMax;
}