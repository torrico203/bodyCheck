using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(fileName = "RewardInfo", menuName = "Scriptable Object/RewardInfo", order = int.MaxValue), System.Serializable]
public class RewardInfo : ScriptableObject
{
    public int no;
    public Grade grade;
    public Sprite icon;
    public Stuff[] stuff;
}
