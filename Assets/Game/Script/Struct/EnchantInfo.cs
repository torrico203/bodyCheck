using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(fileName = "EnchantInfo", menuName = "Scriptable Object/EnchantInfo", order = int.MaxValue), System.Serializable]
public class EnchantInfo : ScriptableObject
{
    public string[] active;
    public ActiveData[] activeData; 

}
