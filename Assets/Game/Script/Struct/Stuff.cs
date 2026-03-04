using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class Stuff
{
    public Wealth wealth;
    public EquipmentData equipment;

    
    public Stuff(Wealth wealth){
        this.wealth = wealth;
        equipment = null;
    }
    public Stuff(EquipmentData equipment){
        this.equipment = equipment;
        wealth = null;
    }
}
