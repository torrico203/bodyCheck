using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class DungeonIcon : MonoBehaviour
{
    private string infoKey;
    public string InfoKey{
        set{
            infoKey = value;
            Reload();
        }
        get => infoKey;
    }
    
    [SerializeField]
    private Transform mapParent;

    [SerializeField]
    private GameObject lockIcon;

    void Start()
    {
        GameObject map = Instantiate(Info.Dungeon[infoKey].maps[Info.Dungeon[infoKey].maps.Length-1],mapParent);
        map.GetComponent<DungeonRoom>().forUI = true;
    }

    void Reload(){
        for(int i=0;i<Data.Player.dungeon.Count;i++){
            if(Data.Player.dungeon[i].infoKey == infoKey)
            {
                lockIcon.SetActive(false);
                break;
            }
        }   
    }
    void OnEnable()
    {
        Reload();
    }



}