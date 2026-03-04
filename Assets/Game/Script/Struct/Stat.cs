using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;

[System.Serializable]
public class Stat
{
    public float att;
    public float hp;
    public float pdef;
    public float mdef;
    public float absorb;
    public float critRate;
    public float critDamage;
    public float attackRange;
    public float attackSpeed;
    public float moveSpeed;
    public float coolRdx;
    public float expRange;
    public float activeSlot;
    public float passiveSlot;
    public float projSize;
    public float incDur;
    public float reflDmg;
    public float projRefl;
    public float avoid;
    public float dmgReduction;
    public float projCount;
    public float extraGold;
    public float extraExp;
    public float extraHeal;
    public float extraSpasticity;
    public float reduceSpasticity;
    public float resistDeath;
    public float extraSilver;
    public float incDmg;
    public float hpRecovery;


    
    public float this[int index]{
        get{
            switch(index){
                case 0: return att;
                case 1: return hp;
                case 2: return pdef;
                case 3: return mdef;
                case 4: return absorb;
                case 5: return critRate;
                case 6: return critDamage;
                case 7: return attackRange;
                case 8: return attackSpeed;
                case 9: return moveSpeed;
                case 10: return coolRdx;
                case 11: return expRange;
                case 12: return activeSlot;
                case 13: return passiveSlot;
                case 14: return projSize;
                case 15: return incDur;
                case 16: return reflDmg;
                case 17: return projRefl;
                case 18: return avoid;
                case 19: return dmgReduction;
                case 20: return projCount;
                case 21: return extraGold;
                case 22: return extraExp;
                case 23: return extraHeal;
                case 24: return extraSpasticity;
                case 25: return reduceSpasticity;
                case 26: return resistDeath;
                case 27: return extraSilver;
                case 28: return incDmg;
                case 29: return hpRecovery;
                default: return 0;
            }
        }
        set{
            switch(index){
                case 0: att = value; break;
                case 1: hp = value; break;
                case 2: pdef = value; break;
                case 3: mdef = value; break;
                case 4: absorb = value; break;
                case 5: critRate = value; break;
                case 6: critDamage = value; break;
                case 7: attackRange = value; break;
                case 8: attackSpeed = value; break;
                case 9: moveSpeed = value; break;
                case 10: coolRdx = value; break;
                case 11: expRange = value; break;
                case 12: activeSlot = value; break;
                case 13: passiveSlot = value; break;
                case 14: projSize = value; break;
                case 15: incDur = value; break;
                case 16: reflDmg = value; break;
                case 17: projRefl = value; break;
                case 18: avoid = value; break;
                case 19: dmgReduction = value; break;
                case 20: projCount = value; break;
                case 21: extraGold = value; break;
                case 22: extraExp = value; break;
                case 23: extraHeal = value; break;
                case 24: extraSpasticity = value; break;
                case 25: reduceSpasticity = value; break;
                case 26: resistDeath = value; break;
                case 27: extraSilver = value; break;
                case 28: incDmg = value; break;
                case 29: hpRecovery = value; break;
            }
        }
    }

    public int Count(){
        return 30;
    }

    public static Stat operator +(Stat a, Stat b)
    {
        Stat stat = new Stat();
        for(int i=0;i<a.Count();i++)
            stat[i] = a[i] + b[i];
        return stat;
    }
    public static Stat operator +(Stat a, float b)
    {
        Stat stat = new Stat();
        for(int i=0;i<a.Count();i++)
            stat[i] = a[i] + b;
        return stat;
    }
    public static Stat operator -(Stat a, Stat b)
    {
        Stat stat = new Stat();
        for(int i=0;i<a.Count();i++)
            stat[i] = a[i] - b[i];
        return stat;
    }

    public static Stat operator *(Stat a, Stat b)
    {
        Stat stat = new Stat();
        for(int i=0;i<a.Count();i++)
            stat[i] = a[i] * b[i];
        return stat;
    }
    public static Stat operator *(Stat a, float b)
    {
        Stat stat = new Stat();
        for(int i=0;i<a.Count();i++)
            stat[i] = a[i] * b;
        return stat;
    }

    public int GetTotalStat()
    {
        int total = 0;
        for(int i=0;i<this.Count();i++)
            total += (int)this[i];
        return total;
    }
    public override string ToString(){
        string str = "";
        for(int i=0;i<this.Count();i++){
            if(this[i]!=0f) str += ((Type)i).ToString()+":"+this[i].ToString()+" ";
        }
        return str;
    }

    public enum Type{
        att = 0,
        hp,
        pdef,
        mdef,
        absorb,
        critRate,
        critDamage,
        attackRange,
        attackSpeed,
        moveSpeed,
        coolRdx,
        expRange,
        activeSlot,
        passiveSlot,
        projSize,
        incDur,
        reflDmg,
        projRefl,
        avoid,
        dmgReduction,
        projCount,
        extraGold,
        extraExp,
        extraHeal,
        extraSpasticity,
        reduceSpasticity,
        resistDeath,
        extraSilver,
        incDmg,
        hpRecovery
    }
}