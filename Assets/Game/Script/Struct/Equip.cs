using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class Equip
{
    public int weapon;
    public int shield;
    public int helmet;
    public int cloth;
    public int shoes;
    public int back;
    public int acc1;
    public int acc2;

    public Equip()
    {
        weapon = -1;
        shield = -1;
        helmet = -1;
        cloth = -1;
        shoes = -1;
        back = -1;
        acc1 = -1;
        acc2 = -1;
    }

    public int this[int index]{
        get{
            switch(index){
                case 0: return weapon;
                case 1: return shield;
                case 2: return helmet;
                case 3: return cloth;
                case 4: return shoes;
                case 5: return back;
                case 6: return acc1;
                case 7: return acc2;
                default: return -1;
            }
        }
        set{
            switch(index){
                case 0: weapon = value; break;
                case 1: shield = value; break;
                case 2: helmet = value; break;
                case 3: cloth = value; break;
                case 4: shoes = value; break;
                case 5: back = value; break;
                case 6: acc1 = value; break;
                case 7: acc2 = value; break;
            }
        }
    }
    public int Count(){
        return 8;
    }
}

public enum EquipmentType
{
    weapon = 0,
    shield,
    helmet,
    cloth,
    shoes,
    back,
    acc1,
    acc2
}