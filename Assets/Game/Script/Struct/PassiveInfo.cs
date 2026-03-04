using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(fileName = "PassiveInfo", menuName = "Scriptable Object/PassiveInfo", order = int.MaxValue), System.Serializable]
public class PassiveInfo : ScriptableObject
{
    public int no;
    public Grade grade;
    public WeaponType weaponType;
    public Actor.Type user;
    public Sprite icon;
    public bool addition;
    public Stat stat;
    public int maxLevel;
    public ActiveInfo[] relatedActive;
    public int level;
}
