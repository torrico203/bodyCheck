using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class PlayerData
{
    public string nickname;
    public int level = 1;
    public Wealth wealth = new Wealth();
    public Equip equip = new Equip();
    public Stat stat = new Stat();
    public List<EquipmentData> inventory = new List<EquipmentData>();
    public List<StageData> dungeon = new List<StageData>();

    public enum Type{
        nickname = 0,
        level,
        wealth,
        equip,
        stat,
        inventory,
        dungeon
    }
}
