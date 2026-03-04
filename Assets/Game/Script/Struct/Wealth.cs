using UnityEngine;

[System.Serializable]
public class Wealth
{
    
    public int exp = 0;
    public int gold = 0;
    public int dia = 0;
    public int meat = 0;
    public int rune = 0;
    public int box0 = 0;
    public int box1 = 0;
    public int box2 = 0;
    public int box3 = 0;
    public int box4 = 0;
    public int whet = 0;
    public int silver = 0;
    public int reward = 0;
    
    public Wealth()
    {
    }
    public Wealth (Type type, int val)
    {
        //Debug.Log("Type : " + type.ToString() + " val : " + val);
        this[type.ToString()] = val;
    }
    public int this[int index]{
        get{
            switch(index){
                case 0: return exp;
                case 1: return gold;
                case 2: return dia;
                case 3: return meat;
                case 4: return rune;
                case 5: return box0;
                case 6: return box1;
                case 7: return box2;
                case 8: return box3;
                case 9: return box4;
                case 10: return whet;
                case 11: return silver;
                case 12: return reward;
                default: return 0;
            }
        }
        set{
            switch(index){
                case 0: exp = value; break;
                case 1: gold = value; break;
                case 2: dia = value; break;
                case 3: meat = value; break;
                case 4: rune = value; break;
                case 5: box0 = value; break;
                case 6: box1 = value; break;
                case 7: box2 = value; break;
                case 8: box3 = value; break;
                case 9: box4 = value; break;
                case 10: whet = value; break;
                case 11: silver = value; break;
                case 12: reward = value; break;
                default: break;
            }
        }
    }
    public int this[string index]{
        get{
            switch(index){
                case "exp": return exp;
                case "gold": return gold;
                case "dia": return dia;
                case "meat": return meat;
                case "rune": return rune;
                case "box0": return box0;
                case "box1": return box1;
                case "box2": return box2;
                case "box3": return box3;
                case "box4": return box4;
                case "whet": return whet;
                case "silver": return silver;
                case "reward": return reward;
                default: return 0;
            }
        }
        set{
            switch(index){
                case "exp": exp = value; break;
                case "gold": gold = value; break;
                case "dia": dia = value; break;
                case "meat": meat = value; break;
                case "rune": rune = value; break;
                case "box0": box0 = value; break;
                case "box1": box1 = value; break;
                case "box2": box2 = value; break;
                case "box3": box3 = value; break;
                case "box4": box4 = value; break;
                case "whet": whet = value; break;
                case "silver": silver = value; break;
                case "reward": reward = value; break;
                default: break;
            }
        }
    }
    public int Count(){
        return 13;
    }
    public bool IsEmpty(){
        for(int i=0;i<Count();i++){
            if(this[i] > 0)
                return false;
        }
        return true;
    }

    public static Wealth operator +(Wealth a, Wealth b)
    {
        Wealth wealth = new Wealth();
        for(int i=0;i<a.Count();i++)
            wealth[i] = a[i] + b[i];
        return wealth;
    }

    public static Wealth operator *(Wealth a, float b){
        Wealth wealth = new Wealth();
        for(int i=0;i<a.Count();i++)
            wealth[i] = Mathf.FloorToInt(a[i] * b);
        return wealth;
    }
    
    public enum Type{
        exp = 0,
        gold,
        dia,
        meat,
        rune,
        box0,
        box1,
        box2,
        box3,
        box4,
        whet,
        silver,
        reward
    }
}
