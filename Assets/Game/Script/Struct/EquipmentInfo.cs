using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(fileName = "EquipmentInfo", menuName = "Scriptable Object/EquipmentInfo", order = int.MaxValue), System.Serializable]
public class EquipmentInfo : ScriptableObject
{
    public EquipmentType type;
    public WeaponType weaponType;   //무기인 경우에만
    public EquipSprite[] images;
    public Stat stat = new Stat();
    public Stat lStat = new Stat(); //레벨업 시 증가하는 스탯
    public ActiveData[] active; //장비가 스킬을 가지고 있는 경우

    public bool hair = true; //머리카락 사용여부 (헬멧만 사용)

    public bool flipY = false; //뒤집을건지

    public bool inUse = true; //인게임 사용 여부

    [System.Serializable]
    public class EquipSprite{
        public string key;
        public Sprite sprite;
    }

    
}
public enum WeaponType{
    None = 0,   //무기가 아님
    Sword,
    Bow,
    Axe,
    Dagger,
    Wand,
    Shield,
    Staff,
    Spear,
    Claw,
    Gun,
    Crossbow,
    Hammer,
    Quiver,
    MagicOrb,
    Fist,
    FlipDagger
}
