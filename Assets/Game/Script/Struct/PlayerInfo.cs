//BOMIN
using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "PlayerInfo", menuName = "Scriptable Object/PlayerInfo", order = int.MaxValue), System.Serializable]
public class PlayerInfo : ScriptableObject
{
    public PlayerData initial;
    public Stat lStat;
}