using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class Deal
{
    public int physics;
    public int magic;
    public int absolute;

    public float hp;
    public float mhp;
    public float lhp;


    public int Total(){
        return physics + magic + absolute;
    }
    
    public static Deal operator +(Deal a, Deal b)
    {
        Deal deal = new Deal();
        deal.physics = a.physics + b.physics;
        deal.magic = a.magic + b.magic;
        deal.absolute = a.absolute + b.absolute;
        deal.hp = a.hp + b.hp;
        deal.mhp = a.mhp + b.mhp;
        deal.lhp = a.lhp + b.lhp;
        return deal;
    }
    public static Deal operator *(Deal a, float b)
    {
        Deal deal = new Deal();
        deal.physics = Mathf.RoundToInt(a.physics * b);
        deal.magic = Mathf.RoundToInt(a.magic * b);
        deal.absolute = Mathf.RoundToInt(a.absolute * b);
        deal.hp = (a.hp * b);
        deal.mhp = (a.mhp * b);
        deal.lhp = (a.lhp * b);
        return deal;
    }
}

[System.Serializable]
public class DealStat{
    public Stat physics;
    public Stat magic;
    public Stat absolute;


    public static DealStat operator +(DealStat a, DealStat b)
    {
        DealStat dealStat = new DealStat();
        dealStat.physics = a.physics + b.physics;
        dealStat.magic = a.magic + b.magic;
        dealStat.absolute = a.absolute + b.absolute;
        return dealStat;
    }
    public static DealStat operator *(DealStat a, float b)
    {
        DealStat dealStat = new DealStat();
        dealStat.physics = a.physics * b;
        dealStat.magic = a.magic * b;
        dealStat.absolute = a.absolute * b;
        return dealStat;
    }
}