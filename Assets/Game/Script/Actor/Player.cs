using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : Actor
{
    private Equip equip;
    public Equip Equip { 
        get { return equip; } 
        set { 
            equip = value; 
            SetEquip();
        }
    }

    
    [SerializeField]
    protected List<PassiveData> passive = new List<PassiveData>();
    public List<PassiveData> Passive { 
        get { return passive; } 
        set { 
            passive = value; 
        }
    }

    private WeaponType weaponType;
    public WeaponType WeaponType { 
        get { return weaponType; } 
    }

    private WeaponType shieldType;
    public WeaponType ShieldType { 
        get { return shieldType; } 
    }

    private void Awake() {
        base.Awake();
        isPlayer = true;
    }

    void Update()
    {
        base.Update();
    }

    void SetEquip(){
        //List<ActiveData> active = new List<ActiveData>();
        //active = new List<ActiveData>();
        for(int i=0;i<equip.Count();i++){
            string equipKey = Info.DefaultEquip[i];
            if(equip[i] != -1){
                EquipmentData data = Data.Player.inventory[equip[i]];
                if(data != null){
                    equipKey = data.infoKey;
                    //인챈트 & 스탯 적용
                }
            }
            //무기 타입 설정
            if(Info.Equipment[equipKey].type == EquipmentType.weapon){
                weaponType = Info.Equipment[equipKey].weaponType;
            }
            if(Info.Equipment[equipKey].type == EquipmentType.shield){
                shieldType = Info.Equipment[equipKey].weaponType;
            }
            
            //헬멧의 헤어 설정
            if(Info.Equipment[equipKey].type == EquipmentType.helmet){
                if(Info.Equipment[equipKey].hair)
                    actorRoot.Renderers["Hair"].gameObject.SetActive(true);
                else
                    actorRoot.Renderers["Hair"].gameObject.SetActive(false);
                
            }

            //무기 스킬 설정
            if(Info.Equipment[equipKey].active != null){
                foreach(ActiveData activeData in Info.Equipment[equipKey].active){
                    AddActive(activeData.infoKey,activeData.level);
                }
            }

            

            foreach(var equipSprite in Info.Equipment[equipKey].images){
                actorRoot.SetSpriteRenderer(equipSprite.key,equipSprite.sprite,Info.Equipment[equipKey].flipY);
            }
        }
    }


}