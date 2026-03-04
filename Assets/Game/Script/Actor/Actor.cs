using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

// 액터(플레이어, 몬스터 등)의 기본 동작과 상태를 관리하는 클래스
public class Actor : MonoBehaviour
{
    
    public int level = 1;
    
    [SerializeField]
    protected Stat stat; // 기본 스탯
    public Stat Stat{
        get => stat;
        set {
            stat = value;
            NStat = this.MStat;
            actorRoot.SetAttackSpeed(NStat.attackSpeed);
        }
    }
    [SerializeField]
    protected Stat vStat; // 가변 스탯(버프 등)
    public Stat VStat{
        get => vStat;
        set {
            vStat = value;
            NStat += ((MStat)-nStat);
            actorRoot.SetAttackSpeed(NStat.attackSpeed);
        }
    }
    public Stat MStat{  //원래의 최대치
        get => stat+vStat; // 기본+가변 스탯 합산
    }
    public Stat NMStat{ //컨디션 적용 후 최대치
        get => stat+vStat+cStat; // 모든 스탯 합산
    }

    [SerializeField]
    private Stat nStat; 
    public Stat NStat{  // 현재 최대치
        get => nStat+cStat;
        set {
            hp += (int)(value.hp-nStat.hp);
            nStat = value;
            actorRoot.SetAttackSpeed(NStat.attackSpeed);
        } 
    }

    [SerializeField]
    private Stat cStat; // 조건부 스탯(특수 효과 등)
    public Stat CStat{
        get => cStat;
        set {
            cStat = value;
            
            actorRoot.SetAttackSpeed(NStat.attackSpeed);
        }
    }

    public int hp = 0;
    protected int life = 0;
    
    [SerializeField]
    protected ActorCenter center; // 중심점(피격, 이펙트 등)
    public ActorCenter Center { get => center; }

    [SerializeField]
    protected int exp; // 경험치
    public int Exp { get => exp; set => exp = value; }

    [SerializeField]
    protected List<ActiveData> active; // 보유 액티브 스킬
    public List<ActiveData> Active { get => active; set => active = value; }
    private int usingActiveIdx = -1; // 현재 사용중인 액티브 인덱스
    private float executeActiveTime = 0f; // 액티브 실행 시간

    private int dmgTxtFloor = 0; // 데미지 텍스트 층
    private float dmgTxtTime = 0f; // 데미지 텍스트 시간
    
    protected bool inDeath = false; // 사망 여부
    public bool InDeath { get => inDeath; }
    

    protected bool isPlayer = false; // 플레이어 여부
    public bool IsPlayer { get => isPlayer; set => isPlayer = value; }

    [SerializeField]
    protected Actor target; // 타겟 액터
    public Actor Target { get => target; set => target = value; }

    protected Vector2 movedir = Vector2.zero; // 이동 방향
    public Vector2 MoveDir { get => movedir; set => movedir = value; }
    protected Vector2 _moveDir = Vector2.zero; // 이동 방향

    protected Vector3 force = Vector3.zero; // 외부 힘(넉백 등)
    public Vector3 Force { get => force; set => force = value; }

    [SerializeField]
    protected ActorRoot actorRoot; // 애니메이션, 방향 등 루트 오브젝트
    public ActorRoot ActorRoot { get => actorRoot; }

    [SerializeField]
    protected Light2D light; // 2D 라이트(플레이어, 몬스터 조명)
    public Light2D Light { get => light; set => light = value; }

    [SerializeField]
    protected bool isFixed = false; // 고정 여부

    protected List<Condition> conditions = new List<Condition>(); // 상태이상, 버프 등
    public List<Condition> Conditions { get => conditions; set => conditions = value; }

    protected bool stun = false; // 기절 상태
    public bool Stun { get => stun; set => stun = value; }
    protected bool inCast = false; // 시전 중 여부
    public bool InCast { get => inCast; set => inCast = value; }

    [SerializeField]
    protected float spasticity = 0f;

    private float motionDelay = 0f; // 모션 딜레이(공격 등)

    //private List<Projectile> projectiles = new List<Projectile>();

    // 초기화
    protected void Awake()
    {
        vStat = new Stat();
        NStat = this.MStat;
        cStat = new Stat();
        hp = Mathf.FloorToInt(NStat.hp);
        life = 0;
    }

    // 매 프레임마다 액터 상태 갱신
    protected void Update()
    {
        float now = Time.time;

        // 사망, 기절, 시전 중이 아니면 이동 및 타겟 처리
        if(this.hp>0&&!this.inDeath&&!this.stun&&!this.inCast&&!this.isFixed&&this.spasticity<=0f){
            
            Vector3 move = (movedir * Time.deltaTime * NStat.moveSpeed);
            transform.localPosition += move;

            if(movedir != Vector2.zero){
                _moveDir = movedir;
            }
            
            float dir = move.x;
            
            if(move != Vector3.zero)
                PlayAnimation("Move");
            else
                PlayAnimation("Idle");
            
            // 타겟이 있으면 방향 및 상태 체크
            
            if(target != null)
            {
                if(!target.gameObject.activeSelf) {
                    target = null;
                    return;
                }
                if(target.hp <= 0)
                {
                    target = null;
                    return;
                }
                dir = target.transform.position.x - transform.position.x;
            }
            actorRoot.SetDirection(dir);
            
            // 액티브 스킬 사용 대기
            if(usingActiveIdx != -1)
            {
                if(Time.time > executeActiveTime)
                {
                    ExecuteActive();
                }
            }
        }

        //경직 관련
        if(this.spasticity>0f){
            spasticity -= Time.deltaTime;
        }

        // 외부 힘 적용(넉백 등)
        transform.localPosition += force;
        force = force/2f;
    }

    // 피격 이펙트 재생
    public void HitEffect(string name, bool random = false){
        if(random){
            Vector3 pos = new Vector3(0, Random.Range(0f, center.radius), 0f);
            pos = Quaternion.Euler(0f, 0f, Random.Range(0f,360f)) * pos;
            pos+=center.transform.position;
            Effect.Play(name, pos);
        }
        else{
            Effect.Play(name, center.transform.position);
        }
    }

    // 힐(회복) 처리
    public void Heal(Deal deal,bool forced = false){
        int heal = deal.Total();
        if(deal.hp>0f) heal += Mathf.RoundToInt(hp * deal.hp);
        if(deal.mhp>0f) heal += Mathf.RoundToInt(NStat.hp * deal.mhp);
        if(deal.lhp>0f) heal += Mathf.RoundToInt((NStat.hp - hp) * deal.lhp);
        Heal(heal,forced);
    }
    public void Heal(int heal,bool forced = false){
        if(this.inDeath) return;
        if(this.hp<1 && !forced) return;

        heal += Mathf.FloorToInt(heal*NStat.extraHeal);

        if(hp + heal > NStat.hp) heal = ((int)NStat.hp) - ((int)hp);
        
        if(heal < 1) return;

        HitEffect("Healing_V5", true);
        
        hp += heal;
        if(hp > NStat.hp) hp = Mathf.FloorToInt(NStat.hp);

        float now = Time.time;
        if(now - dmgTxtTime > 0.6f){
            dmgTxtFloor = 0;
            dmgTxtTime = now;
        }
        
        DamageText dmgtxt = Pool.GetObject<DamageText>("DamageText");
        dmgtxt.transform.position = transform.position;
        dmgtxt.text = heal.ToString();
        dmgtxt.floor = dmgTxtFloor++;
        dmgtxt.color = Color.green;
        dmgtxt.scale = 0.4f;
        dmgtxt.startY = 0.8f;
        dmgtxt.InitObject();
    }

    public void HPUP(int hpup, bool heal = true){
        Stat stat = new Stat();
        stat.hp = hpup;
        VStat += stat;

        float now = Time.time;
        if(now - dmgTxtTime > 0.6f){
            dmgTxtFloor = 0;
            dmgTxtTime = now;
        }
        
        HitEffect("Heart_Buff_V2", true);
        
        DamageText dmgtxt = Pool.GetObject<DamageText>("DamageText");
        dmgtxt.transform.position = transform.position;
        dmgtxt.text = "<sprite name=hp> +"+hpup.ToString();
        dmgtxt.floor = dmgTxtFloor++;
        dmgtxt.color = Color.green;
        dmgtxt.scale = 0.4f;
        dmgtxt.startY = 0.8f;
        dmgtxt.InitObject();

        if(heal) hp = Mathf.FloorToInt(NStat.hp);
    }

    // 피격 처리(데미지, 크리티컬, 흡혈 등)
    public void Hit(Deal deal, Actor attacker = null, float spasticity=0f, bool absolute = false,ConditionData condition = null){
        if(this.inDeath) return;
        if(this.hp<1) return;
        
        bool isCrit = false;
        if(attacker != null){
            float critRate = Random.Range(0f, 1f);
            if(critRate < attacker.NStat.critRate){
                deal = deal*attacker.NStat.critDamage;
                isCrit = true;
            }
        }

        

        if(isCrit){
            HitEffect("CriticalHit", true);
        }
        else{
            HitEffect("Impact_Hit_Lv2", true);
        }

        int pdmg = deal.physics - Mathf.FloorToInt(NStat.pdef);
        if(pdmg < 0) pdmg = 0;
        int mdmg = deal.magic - Mathf.FloorToInt(NStat.mdef);
        if(mdmg < 0) mdmg = 0;
        int dmg = pdmg + mdmg;
        dmg = Mathf.FloorToInt(dmg * (1f-NStat.dmgReduction));
        dmg += deal.absolute;
        if(deal.hp>0f) dmg += Mathf.RoundToInt(hp * deal.hp);
        if(deal.mhp>0f) dmg += Mathf.RoundToInt(NStat.hp * deal.mhp);
        if(deal.lhp>0f) dmg += Mathf.RoundToInt((NStat.hp - hp) * deal.lhp);
        
        if(dmg < 1) dmg = 1;

        if(attacker != null){
            if(attacker.NStat.absorb > 0f){
                int absorb = Mathf.FloorToInt(dmg * attacker.NStat.absorb);
                if(absorb < 1) absorb = 1;
                attacker.Heal(absorb);
            }
            if(this.NStat.reflDmg > 0f){
                Deal reflectionDeal = new Deal();
                reflectionDeal.absolute = Mathf.FloorToInt(dmg*this.NStat.reflDmg);
                attacker.Hit(reflectionDeal);
            }
        }

        float now = Time.time;
        if(now - dmgTxtTime > 0.6f){
            dmgTxtFloor = 0;
            dmgTxtTime = now;
        }

        if(!absolute){
            if(this.NStat.avoid > 0f){
                float avoid = Random.Range(0f, 1f);
                if(avoid < this.NStat.avoid){
                    UI.Game.Manager.DamageText(Util.LocalStr("UI","Avoid"), transform.position, dmgTxtFloor++);
                    return;
                }
            }
        }

        if(condition != null) this.AddCondition(condition,deal,attacker);
        
        
        bool forcedAnimation = false;
        //경직에 대한
        spasticity -= spasticity*NStat.reduceSpasticity;
        if(spasticity>0f){
            CancelActive();
            this.spasticity = spasticity;
            forcedAnimation = true;
        }
        
        hp -= dmg;
        actorRoot.Hit();
        PlayAnimation("Damaged",forcedAnimation);

        UI.Game.Manager.DamageText(dmg.ToString(), transform.position, dmgTxtFloor++, isPlayer, isCrit);

        if(hp < 1) Death();
    }

    // 사망 처리
    protected void Death(){
        
        if(life>=this.NStat.resistDeath){    
            inDeath = true;
            //ClearProjectile();
            PlayAnimation("Death");
            OnDeath();
        }
        else{
            life++;
            Effect.Play("Revival", this.transform.position);
            if(isPlayer)
            {
                Time.timeScale = 0.6f;
                MainCamera.CurtainIn(1.33f,()=>{
                    Time.timeScale = 1f;
                    ExecuteRevival();
                    MainCamera.CurtainOut(0.6f);
                });
            }
            else{
                StartCoroutine(Revival());
            }
        }
    }
    
    IEnumerator Revival(){
        yield return new WaitForSeconds(1.33f);
        ExecuteRevival();
    }
    void ExecuteRevival(){
        
        Heal(Mathf.RoundToInt(this.MStat.hp),true);

        Projectile p = Pool.GetObject<Projectile>("Levelup");
        p.owner = this;
        p.fromTransform = this.transform;
        p.target = null;
        p.transform.position = this.transform.position;
        p.gameObject.tag = this.gameObject.tag;
        p.condition = new ConditionData();
        p.deal = new Deal();
        p.deal.physics = 1;
        p.dest = Vector3.zero;
        p.delay = 0f;
        p.InitObject();

        DamageText dmgtxt = Pool.GetObject<DamageText>("DamageText");
        dmgtxt.transform.position = transform.position;
        dmgtxt.text = Util.LocalStr("Stat","resistDeath");
        dmgtxt.floor = 0;
        dmgtxt.color = Color.red;
        dmgtxt.scale = 0.5f;
        dmgtxt.startY = 0f;
        dmgtxt.InitObject();
    }
    protected virtual void OnDeath(){}

    Deal GetBehaviourDeal(string infoKey, int activeLevel, int idx){
        Deal deal = new Deal();
        DealStat sDeal = Info.Active[infoKey].behaviours[idx].deal + (Info.Active[infoKey].behaviours[idx].lDeal * (activeLevel-1));
        deal.physics = Mathf.FloorToInt((NStat * sDeal.physics).GetTotalStat() / 100 * (1f+NStat.incDmg));
        deal.magic = Mathf.FloorToInt((NStat * sDeal.magic).GetTotalStat() / 100 * (1f+NStat.incDmg));
        deal.absolute = Mathf.FloorToInt((NStat * sDeal.absolute).GetTotalStat() / 100 * (1f+NStat.incDmg));
        return deal;
    }

    protected void CancelActive(){
        if(usingActiveIdx != -1){
            this.active[usingActiveIdx].useTime = 0f;
            usingActiveIdx = -1;
            executeActiveTime = 0f;
        }
    }
    // 액티브 스킬 실행
    protected void ExecuteActive(){
        string infoKey = this.active[usingActiveIdx].infoKey;
        int activeLevel = this.active[usingActiveIdx].level;
        
        // 타겟, 사정거리 등 조건 체크
        bool cancel = false;
        if(Info.Active[infoKey].target){
            if(target == null) cancel = true;
            else if(target.InDeath || target.hp<1) cancel = true;
            else{
                float range = Info.Active[infoKey].range;
                if(range == 0)
                    range = NStat.attackRange;
                if(Vector3.Distance(target.transform.position, transform.position) > range){
                    cancel = true;
                }
            }
        }

        if(cancel){
            CancelActive();
            return;
        }
        

        if(Info.Active[infoKey].effect != ""){
            Effect.Play(Info.Active[infoKey].effect, this.transform.position);
        }

        for(int x=0;x<Info.Active[infoKey].behaviours.Length;x++){
            ActiveInfo.Behaviour behaviour = Info.Active[infoKey].behaviours[x];

            ActiveInfo.Behaviour.Variable variable = behaviour.variable + (behaviour.lVariable * (activeLevel-1));
           
            variable.Floor();
            
            string projectile = behaviour.projectile;
            Projectile pInfo = Pool.GetObjectInfo<Projectile>(projectile);
            // 발사체가 있는 경우
            if(projectile != ""){
                Deal deal = GetBehaviourDeal(infoKey, activeLevel,x);
                
                Vector3 targetPos = _moveDir;
                if(Info.Active[infoKey].target){
                    targetPos = target != null ? (target.transform.position - this.transform.position) : _moveDir;
                }
                else{
                    targetPos = _moveDir*Info.Active[infoKey].range;
                }

                if(targetPos == Vector3.zero){
                    targetPos = Vector3.up;
                }

                
                // 여러 발사체, 각도, 체인 등 처리
                for(int j=0;j<variable.count;j++){
                    
                    int projCount = Mathf.FloorToInt(variable.projectileCount);
                    if(!behaviour.moving)
                        projCount = Mathf.FloorToInt(variable.projectileCount + NStat.projCount);
                    
                    if(projCount>1){
                        if(variable.projectileAngle == 0){
                            variable.projectileAngle = 15f;
                        }
                    }

                    float angle = -(variable.projectileAngle/2f);
                    float angleTerm = 0f;
                    if(variable.projectileAngle>=360) angleTerm = (variable.projectileAngle/projCount);
                    else angleTerm = (variable.projectileAngle/(projCount-1));

                    
                    for(int i = 0; i < projCount; i++){

                        if(pInfo.type == Projectile.Type.Random){
                            do{
                                targetPos = new Vector3(Random.Range(-Info.Active[infoKey].range,Info.Active[infoKey].range),Random.Range(-Info.Active[infoKey].range,Info.Active[infoKey].range),0f);
                            }while(targetPos.magnitude > Info.Active[infoKey].range);
                        }

                        if(Info.Active[infoKey].casting>0f){
                            // 시전형 발사체
                            PreProjectile p = Pool.GetObject<PreProjectile>("PreProjectile");
                            p.owner = this;
                            if(pInfo.FromCenter)
                                p.fromTransform = this.center.transform;
                            else
                                p.fromTransform = this.transform;
                            if(Info.Active[infoKey].target)
                                p.target = target;
                            else
                                p.target = null;
                            p.transform.position = this.transform.position;
                            p.gameObject.tag = this.gameObject.tag;
                            p.condition = new ConditionData();
                            p.condition.infoKey = behaviour.condition.infoKey;
                            p.condition.level = behaviour.condition.level;
                            p.condition.duration = variable.conditionDuration;
                            p.deal = deal;
                            p.dest = (Quaternion.Euler(0,0,angle) * targetPos);
                            p.range = Info.Active[infoKey].range;
                            p.spasticity = variable.spasticity;
                            p.delay = variable.term*j;
                            p.chain = Mathf.FloorToInt(variable.chain);
                            p.maxChain = p.chain;
                            p.chainCount = Mathf.FloorToInt(variable.chainCount);
                            p.chainRange = variable.chainRange;
                            p.angle = angle + ((variable.projectileAngle/projCount)/projCount);

                            p.duration = Info.Active[infoKey].casting;
                            p.ProjectileKey = projectile;
                            p.motion = Info.Active[infoKey].afterMotion;

                            //Debug.Log("startFromOwner : "+behaviour.startFromOwner);
                            p.StartFromOwner = behaviour.startFromOwner;

                            if(behaviour.moving){//이동기인경우
                                if(angle==0f) p.isRide = true;//각도가 0인경우만
                                else p.isRide = false;
                            }
                            else{
                                p.isRide = false;
                            }
                            p.InitObject();
                            
                            
                        }
                        else{
                            // 일반 발사체
                            Projectile p = Pool.GetObject<Projectile>(projectile);
                            p.owner = this;
                            if(p.FromCenter)
                                p.fromTransform = this.center.transform;
                            else
                                p.fromTransform = this.transform;
                            
                            if(Info.Active[infoKey].target)
                                p.target = target;
                            else
                                p.target = null;
                            p.transform.position = this.transform.position;
                            p.gameObject.tag = this.gameObject.tag;
                            p.condition = new ConditionData();
                            p.condition.infoKey = behaviour.condition.infoKey;
                            p.condition.level = behaviour.condition.level;
                            p.condition.duration = variable.conditionDuration;
                            p.deal = deal;
                            p.dest = (Quaternion.Euler(0,0,angle) * targetPos);
                            p.range = Info.Active[infoKey].range;
                            p.spasticity = variable.spasticity;
                            p.delay = variable.term*j;
                            p.chain = Mathf.FloorToInt(variable.chain);
                            p.maxChain = p.chain;
                            p.chainCount = Mathf.FloorToInt(variable.chainCount);
                            p.chainRange = variable.chainRange;
                            p.angle = angle;

                            p.StartFromOwner = behaviour.startFromOwner;
                            
                            if(behaviour.moving){//이동기인경우
                                if(angle==0f) p.isRide = true;//각도가 0인경우만
                            }
                            p.InitObject();
                        }
                        
                        angle += angleTerm;
                    }
                }
            }
            else{
                // 자기 자신에게 조건 부여(버프 등)
                if(behaviour.type == BehaviourType.SelfCondition){
                    Deal deal = GetBehaviourDeal(infoKey, activeLevel, x);
                    ConditionData condition = new ConditionData();
                    condition.infoKey = behaviour.condition.infoKey;
                    condition.level = this.active[usingActiveIdx].level;
                    condition.duration = variable.conditionDuration;
                    this.AddCondition(condition,deal,this);
                }
                else if(behaviour.type == BehaviourType.SelfHit){
                    Deal deal = GetBehaviourDeal(infoKey, activeLevel, x);
                    Hit(deal,this);
                }
                else if(behaviour.type == BehaviourType.SelfHeal){
                    Deal deal = GetBehaviourDeal(infoKey, activeLevel, x);
                    Heal(deal.physics+deal.magic+deal.absolute);
                }
            }
        }


        // 하드캐스팅인 경우
        if(Info.Active[infoKey].hardCasting){
            ConditionData condition = new ConditionData();
            condition.infoKey = "Casting";
            condition.level = 1;
            condition.duration = Info.Active[infoKey].casting;
            this.AddCondition(condition);
        }

        
        CheckCondition();

        

        

        usingActiveIdx = -1;
        executeActiveTime = 0f;
    }
    
    // 액티브 스킬 사용 시 조건 체크 및 대기/쿨타임 처리
    public ActiveErr UseActive(int idx, bool forced = false){
        if(this.inDeath) return ActiveErr.etc;
        if(this.hp<1) return ActiveErr.etc;
        if(this.stun) return ActiveErr.etc;
        if(this.spasticity>0f) return ActiveErr.etc;
        if(this.inCast) return ActiveErr.etc;
        if(idx >= active.Count) return ActiveErr.etc;

        if(isPlayer){
            if(Info.Active[active[idx].infoKey].moving){
                if(UI.Game.Joystick.Direction != Vector2.zero)
                {   //강제 이동중에 이동기 사용 불가
                    return ActiveErr.etc;
                }
            }
        }
        
        

        // 쿨타임 체크
        float now = Time.time;
        float coolTime = 1f/NStat.attackSpeed;
        if(Info.Active[active[idx].infoKey].type != ActiveType.DefaultAttack)
            coolTime = Info.Active[active[idx].infoKey].coolTime * (1f - NStat.coolRdx);
        if(now - this.active[idx].useTime - this.active[idx].delay > coolTime){

            if(Info.Active[active[idx].infoKey].target){
                if(target == null) return ActiveErr.Target;

                float range = Info.Active[active[idx].infoKey].range;
                if(range == 0)
                    range = NStat.attackRange;
                if(Vector3.Distance(target.transform.position, transform.position) > range){
                    return ActiveErr.Range;
                }
            }
            
            if(usingActiveIdx != -1) {
                if(!forced) return ActiveErr.Wait;
                this.active[usingActiveIdx].useTime = -999f;
            }
            if(Time.time <= motionDelay) {
                if(!forced) return ActiveErr.Wait;
            }

            string motion = Info.Active[active[idx].infoKey].motion;
            this.active[idx].useTime = now + 0.016f;//1프레임 뒤에 사용
            usingActiveIdx = idx;
            this.active[idx].delay = 0f;
            
            if(motion == "") {//모션 없는 경우 즉각 사용
                motionDelay = now+0.016f;
                executeActiveTime = now+0.016f;
            }
            else{
                PlayAnimation(motion);
                motionDelay = now + actorRoot.AttackTime + 0.016f;
                executeActiveTime = now + actorRoot.AttackTime/2f;
            }
            return ActiveErr.None;
        }
        else{
            return ActiveErr.CoolTime;
        }
    }

    // 애니메이션 재생
    public void PlayAnimation(string name,bool forced = false){
        actorRoot.PlayAnimation(name,forced);
    }


    void CheckCondition(){
        for(int i=0;i<conditions.Count;i++){
            if(conditions[i].ActiveCount > 0){
                conditions[i].ACount++;
                if(conditions[i].ACount > conditions[i].ActiveCount){
                    conditions[i].ReturnObject();
                    conditions.RemoveAt(i);
                    i--;
                }
            }
        }
    }
    // 상태이상, 버프 추가
    public void AddCondition(ConditionData condition,Deal deal = null,Actor attacker = null){
        if(condition == null) return;
        if(condition.level == 0) return;
        for(int i=0;i<conditions.Count;i++){
            if(conditions[i].InfoKey == condition.infoKey){
                conditions[i].StartTime = Time.time;
                return;
            }
        }
        Condition c = Pool.GetObject<Condition>(condition.infoKey);
        c.transform.SetParent(this.transform);
        c.transform.localPosition = Vector3.zero;
        c.Owner = this;
        c.InfoKey = condition.infoKey;
        c.level = condition.level;
        c.Duration = condition.duration * (1f+NStat.incDur);
        //c.Deal = new Deal();
        if(deal!=null){
            c.Deal.physics = deal.physics;
            c.Deal.magic = deal.magic;
            c.Deal.absolute = deal.absolute;
        }
        c.Attacker = attacker;
        c.InitObject();
        conditions.Add(c);
    }

    // 상태이상, 버프 제거
    public void DelCondition(string infoKey){
        for(int i=0;i<conditions.Count;i++){
            if(conditions[i].InfoKey == infoKey){
                conditions[i].ReturnObject();
                conditions.RemoveAt(i);
                return;
            }
        }
    }

    // 액티브 스킬 추가
    public void AddActive(string infoKey, int level = 1){
        if(this.active == null) this.active = new List<ActiveData>();
        for(int i=0;i<active.Count;i++){
            if(active[i].infoKey == infoKey){
                active[i].level += level;
                return;
            }
        }
        ActiveData data = new ActiveData();
        data.infoKey = infoKey;
        data.level = level;
        data.useTime = -999f;
        this.active.Add(data);
    }

    // 액티브 스킬 제거
    public void RemoveActive(string infoKey){
        if(this.active == null) return;
        for(int i=0;i<active.Count;i++){
            if(active[i].infoKey == infoKey){
                active.RemoveAt(i);
                return;
            }
        }
    }

    // 액터 타입(플레이어/몬스터)
    public enum Type
    {
        Player = 0,
        Monster
    }
}

// 액티브 스킬 사용 결과 에러 코드
public enum ActiveErr
{
    None,
    CoolTime,
    Range,
    Target,
    Wait,
    etc
}