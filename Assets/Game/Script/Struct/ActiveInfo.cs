using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(fileName = "ActiveInfo", menuName = "Scriptable Object/ActiveInfo", order = int.MaxValue), System.Serializable]
public class ActiveInfo : ScriptableObject
{
    public int no;
    public ActiveType type;
    public Grade grade;
    public WeaponType weaponType;
    public Actor.Type user;
    public Sprite icon;
    public string effect;
    public float casting = 0f;
    public bool hardCasting = false; //하드캐스팅 여부
    public float range;
    public float coolTime;
    public string motion;
    public string afterMotion;
    public bool target;
    public bool moving;
    public bool mobility;
    public bool approach = true;    //사정거리 밖인 경우 접근 여부
    public int level;
    public bool forLevelup;
    public int maxLevel;
    public Behaviour[] behaviours;
    public ActiveInfo[] nextActive;
    public PassiveInfo[] relatedPassive;

    [System.Serializable]
    public class Behaviour
    {
        public BehaviourType type;
        public string projectile;
        public bool moving;
        public bool startFromOwner = true;
        public DealStat deal;
        public DealStat lDeal;
        public Variable variable;
        public Variable lVariable;
        public ConditionData condition;

        [System.Serializable]
        public class Variable{
            public float projectileCount;
            public float projectileAngle;
            public float count;
            public float term;
            public float chain;     //체인 횟수
            public float chainCount;    //한번에 체인되는 수
            public float chainRange;    //체인 범위
            public float conditionDuration;
            public float spasticity;

            public void Floor(){
                this.projectileCount = Mathf.Floor(this.projectileCount);
                this.projectileAngle = Mathf.Floor(this.projectileAngle);
                this.count = Mathf.Floor(this.count);
                //this.term = Mathf.Floor(this.term);
                this.chain = Mathf.Floor(this.chain);
                this.chainCount = Mathf.Floor(this.chainCount);
                //this.chainRange = Mathf.Floor(this.chainRange);
                //this.conditionDuration = Mathf.Floor(this.conditionDuration);
            }

            public static Variable operator +(Variable a, Variable b)
            {
                Variable v = new Variable();
                v.projectileCount = a.projectileCount + b.projectileCount;
                v.projectileAngle = a.projectileAngle + b.projectileAngle;
                v.count = a.count + b.count;
                v.term = a.term + b.term;
                v.chain = a.chain + b.chain;
                v.chainCount = a.chainCount + b.chainCount;
                v.chainRange = a.chainRange + b.chainRange;
                v.conditionDuration = a.conditionDuration + b.conditionDuration;
                v.spasticity = a.spasticity + b.spasticity;
                return v;
            }
            public static Variable operator *(Variable a, float b)
            {
                Variable v = new Variable();
                v.projectileCount = a.projectileCount * b;
                v.projectileAngle = a.projectileAngle * b;
                v.count = a.count * b;
                v.term = a.term * b;
                v.chain = a.chain * b;
                v.chainCount = a.chainCount * b;
                v.chainRange = a.chainRange * b;
                v.conditionDuration = a.conditionDuration * b;
                v.spasticity = a.spasticity * b;
                return v;
            }
        }
    }
}

public enum ActiveType
{
    DefaultAttack,
    Cooltime
}

public enum BehaviourType
{
    Projectile,
    SelfCondition,
    SelfHit,
    SelfHeal
}


public enum ActiveCategory
{
    Melee = 0,
    Magic

}