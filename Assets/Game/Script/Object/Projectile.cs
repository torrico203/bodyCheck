using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.SceneManagement;


public class Projectile : PoolingObject
{
    //프로젝타일필수
    [HideInInspector]
    public Actor owner,target;
    [HideInInspector]
    public Transform fromTransform;
    [HideInInspector]
    public Deal deal = new Deal();
    [HideInInspector]
    public float delay = 0f,chainRange = 0f,angle = 0f,range = 0f,spasticity = 0f;
    [HideInInspector]
    public Vector3 dest;
    [HideInInspector]
    public bool isRide = false;
    [HideInInspector]
    public ConditionData condition = null;
    [HideInInspector]
    public int maxChain = 0, chain = 0,chainCount = 0;
    



    private Vector3 from = Vector3.zero;

    [SerializeField]
    public Type type = Type.Point;

    [SerializeField]
    private SubType subType = SubType.None;

    [SerializeField]
    private Stat summonRate = new Stat();
    


    [SerializeField]
    private SpriteRenderer[] spriteRenderer;
    [SerializeField]
    private string rendererKey = "";

    [SerializeField]
    private string projectileName = "";
    
    [SerializeField]
    private bool isDir = false, barrier = false, boomerang = false, invisible = false, positive = false, isDirX = false,immediateDisappear = false;

    [SerializeField]
    private bool hitable = true, fromCenter = false, selfChain = false, inevitable = false, remainable = false;

    public bool Inevitable{
        get => inevitable;
    }

    public bool FromCenter {
        get => fromCenter;
    }


    [SerializeField]
    private float extraDelay = 0f, term = 0f;
    private float termTime = 0f;

    [SerializeField]
    private string sound = "";

    [SerializeField]
    private float speed = 1f;
    public float Speed {
        get => speed;
    }

    [SerializeField]
    private int hitcount = 0;
    private int hit = 0;

    [SerializeField]
    private float duration = 0f,dealDuration = 0f;
    public float Duration {
        get => duration;
    }

    



    
    private float projSize = 1f,fireTime = 0f;

    private float dist = 0f;

    [SerializeField]
    private string effect = "";

    [SerializeField]
    private bool effectOnHit = false;
    
    private bool startFromOwner = true;
    public bool StartFromOwner
    {
        get => startFromOwner;
        set => startFromOwner = value;
    }

    [SerializeField]
    private float force = 0f, velocity = 0f;

    [SerializeField]
    private Vector3 chaseSpeed = Vector3.zero;

    [SerializeField]
    private float spin = 0f;

    [SerializeField]
    private Ease ease = Ease.Linear, boomerangEase = Ease.Linear;

    private float dirX = 0f;

    private List<Actor> hitList = new List<Actor>();

    private Tween tween = null;

    [SerializeField]
    private GameObject[] objs;


    [SerializeField]
    private Collider2D collider;
    public Collider2D Collider
    {
        get => collider;
    }

    private bool isFired = false;
    private bool isLive = false;

    

    void Awake()
    {

    }

    void Update()
    {
        if(hitList.Count > 0)
        {
            
            float now = Time.time;
            if(now - termTime > term)
            {
                for(int i=0;i<hitList.Count;i++){
                    if(hitList[i] != null)
                        Hit(hitList[i]);
                }
                // foreach(Actor actor in hitList){
                //     if(actor != null)
                //         Hit(actor);
                // }
                termTime = now;
            }
        }

        if(isRide){
            if(this.isLive){
                if(owner != null) {
                    owner.transform.position = this.transform.position;
                    if(invisible){
                        owner.gameObject.SetActive(false);
                    }
                }
            }
        }

        //룸이 없는 경우에는 유지 가능 발사체 제외하고 모두 없애기
        if(!remainable){
            if(UI.Game.Manager.Dungeon.InGate){
                Debug.Log("게이트군요??");
                ForcedReturn();
                return;
            }
        }
        

        if(!isFired)
        {
            if(delay > 0f)
            {
                delay -= Time.deltaTime;
                if(delay <= 0f)
                {
                    Fire();
                }
            }
            else
            {
                Fire();
            }
        }
        else{
            if(!this.isLive) return;

            if(type == Type.Laser){
                if(target != null && this.transform != null){
                    
                    if(this.fromTransform != null)
                        this.transform.position = this.fromTransform.position;
                    float dist = Vector3.Distance(this.target.Center.transform.position,transform.position);
                
                    Vector3 v = this.target.Center.transform.position - transform.position;
                    transform.localRotation = Quaternion.Euler(0f,0f,Vector2.SignedAngle(Vector2.up,v));
                    transform.localScale = new Vector3(1f, dist, 1f);
                    // for(int i=0;i<objs.Length;i++)
                    // {
                    //     objs[i].transform.localRotation = Quaternion.Euler(0f,0f,Vector2.SignedAngle(Vector2.up,v));
                    //     objs[i].transform.localScale = new Vector3(1f, dist, 1f);
                    // }

                    float now = Time.time;
                    if(now - termTime > term)
                    {
                        Hit(target);
                        termTime = now;
                    }
                    
                }
                
            }
            else if(type == Type.Around){
                this.transform.position = this.fromTransform.position;
                for(int i=0;i<objs.Length;i++)
                {
                    objs[i].transform.Rotate(new Vector3(0f,0f,Time.deltaTime*speed*360f));
                }
                //this.transform.Rotate(new Vector3(0f,0f,Time.deltaTime*speed*360f));
            }
            else if(type == Type.Chase){
                if(target != null){
                    this.transform.position += chaseSpeed * Time.deltaTime;
                    Vector3 dir = target.transform.position - this.transform.position;
                    dir.Normalize();
                    chaseSpeed += dir * (velocity * Random.Range(0.9f,1.1f)) * Time.deltaTime;
                    chaseSpeed = chaseSpeed.normalized * speed;
                    if(target.hp < 1f){
                        target = null;
                    }

                    if(owner.IsPlayer){
                        if(UI.Game.Manager.monsters.Count != 0){
                            if(target != null){
                                if(target.IsPlayer) target = null;
                            }
                        }
                    }
                    else{
                        target = UI.Game.Player;
                    }
                    
                    

                    for(int i=0;i<objs.Length;i++)
                    {
                        if(objs[i] != null)
                        {
                            objs[i].transform.localRotation = Quaternion.Euler(0f,0f,Mathf.Atan2(chaseSpeed.y,chaseSpeed.x)*Mathf.Rad2Deg-90f);
                        }
                    }
                }
                else{
                    //Actor nextTarget = GetNextTarget(chainRange);
                    target = GetNextTarget(this.transform);
                }
            }
            else if(type == Type.Sticker){
                if(this.owner != null){
                    if(this.fromCenter)
                        this.transform.position = this.owner.Center.transform.position;
                    else
                        this.transform.position = this.owner.transform.position;

                    if(!isDir)
                    {
                        for(int i=0;i<objs.Length;i++)
                        {
                            if(objs[i] != null)
                            {
                                objs[i].transform.localRotation = this.owner.ActorRoot.transform.localRotation;
                            }
                        }
                    }
                }
            }
            
            
            if(subType == SubType.Explosion){
                if(term > 0f){
                    float now = Time.time;
                    if(now - termTime > term)
                    {
                        Projectile p = MakeProjectile(projectileName); 
                        p.dest = Vector3.zero;
                                
                        p.InitObject();
                        
                        termTime = now;
                    }
                }
            }


            //스핀
            if(spin > 0f)
            {
                for(int i=0;i<objs.Length;i++)
                {
                    objs[i].transform.Rotate(new Vector3(0f,0f,Time.deltaTime*spin*360f));
                }
            }
            
        }
        
    }

    public void Fire()
    {
        if(this.owner.hp < 1f) this.owner = null;
        if(this.owner == null) {
            ForcedReturn();
            return;
        }
        if(sound != ""){
            Sound.PlaySFX(sound);
        }

        //발사한 시각
        fireTime = Time.time;

        //렌더러 복사하기
        if(rendererKey != "")
        {
            if(owner != null){
                if(owner.ActorRoot.Renderers.ContainsKey(rendererKey))
                {
                    for(int i=0;i<spriteRenderer.Length;i++)
                    {
                        if(spriteRenderer[i] != null)
                            spriteRenderer[i].sprite = owner.ActorRoot.Renderers[rendererKey].sprite;
                    }
                }
            }
        }

        //시작 위치 지정이 필요한 경우
        if(startFromOwner)
        {
            if(this.fromTransform != null) this.transform.position = this.fromTransform.position;
        }

    
        
        //켠다
        OnOff(true);
        from = Vector3.zero;

        if(subType == SubType.Flip){
            Vector3 newDest = dest - from;
            newDest = Quaternion.Euler(0f,0f,180f) * newDest;
            dest = from + newDest;
        }
        
        Vector3 dir = dest - from;
        if(type == Type.Line)
            dest = from + dir.normalized * speed * duration;

        
        
        if(subType == SubType.Reverse)
        {
            Vector3 temp = dest;
            dest = from;
            from = temp;

            this.transform.position = from+this.transform.position;
            dir = dest - from;
        }

        //Vector3 dir = dest - from;

        if(type == Type.Point)
        {
            if(this.target != null){
                dist = Vector3.Distance(this.transform.position, this.target.transform.position);
                tween = this.transform.DOMove(this.target.transform.position, dist/speed).SetEase(ease).OnComplete(()=>{
                    if(duration == 0f) GoReturn();
                });
            }
            
        }
        else if(type == Type.Line)
        {
            //dir = dir.normalized * speed * duration;
            float _duration = duration;
            if(boomerang)
                _duration = duration * 0.5f;
            Vector3 home = this.transform.position;
            tween = this.transform.DOMove(dest+this.transform.position, _duration).SetEase(ease).OnComplete(()=>{
                if(boomerang){
                    if(this.owner != null) home = this.owner.transform.position;
                    tween = this.transform.DOMove(home, _duration).SetEase(boomerangEase).OnComplete(()=>{
                        if(duration == 0f) GoReturn();
                    });
                }
                else{
                    if(duration == 0f) GoReturn();
                }
            });
        }
        else if(type == Type.Missile)
        {
            dist = Vector3.Distance(from, dest);

            Vector3 _dest = dir.normalized * -1f;
            _dest = Quaternion.Euler(0f,0f,Random.Range(-45f,45f)) * _dest;
            Vector3 destination = this.transform.position + dest;
            tween = this.transform.DOMove(this.transform.position+from+_dest, dist/speed * 0.3f).SetEase(Ease.OutCubic).OnComplete(()=>{
                tween = this.transform.DOMove(destination, dist/speed * 0.7f).SetEase(ease).OnComplete(()=>{
                    if(duration == 0f) GoReturn();
                });
            });
        }
        else if(type == Type.Be){
            this.transform.position = dest+this.transform.position;
        }
        else if(type == Type.Laser){
            if(target != null && this.fromTransform != null){
                this.transform.position = this.fromTransform.position;
                float dist = Vector3.Distance(this.target.Center.transform.position,transform.position);
            
                Vector3 v = this.target.Center.transform.position - transform.position;
                transform.localRotation = Quaternion.Euler(0f,0f,Vector2.SignedAngle(Vector2.up,v));
                transform.localScale = new Vector3(1f, dist, 1f);
            }
            
        }
        else if(type == Type.Around)
        {
            for(int i=0;i<objs.Length;i++)
            {
                if(objs[i] != null)
                {
                    objs[i].transform.localRotation = Quaternion.Euler(0f,0f,angle);
                }
            }
        }
        else if(type == Type.Chase){
            //chaseSpeed = Vector3.zero;
            chaseSpeed = dir.normalized * 10f;
        }
        else if(type == Type.Random){
            //dist = Vector3.Distance(Vector3.zero, dest);
            //Debug.Log("dist : " + dist);
            // Vector3 pos = Vector3.zero;
            // do{
            //     pos = new Vector3(Random.Range(-range,range),Random.Range(-range,range),0f);
            // }while(pos.magnitude > range);
            // dest = from + pos;
            this.transform.position = dest+this.transform.position;
        }
        else if(type == Type.Sticker){

        }
        

        if(isDir)
        {
            for(int i=0;i<objs.Length;i++)
            {
                if(objs[i] != null)
                {
                    objs[i].transform.localRotation = Quaternion.Euler(0f,0f,Mathf.Atan2(dir.y,dir.x)*Mathf.Rad2Deg-90f);
                }
            }
        }
        else
        {
            //this.transform.DORotate(new Vector3(0f,0f,0f), duration).SetEase(ease);
        }

        if(dir.x>=0f) dirX = 1f;
        else dirX = -1f;

        if(subType == SubType.Roller){
            if(isRide){
                if(owner != null){
                    owner.ActorRoot.Roll(duration*0.8f, dirX);
                }
            }
        }

        if(isDirX){
            for(int i=0;i<objs.Length;i++)
            {
                if(objs[i] != null){
                    if(dirX > 0){
                        objs[i].transform.localRotation = Quaternion.Euler(new Vector3(0f,180f,0f));
                    }else{
                        objs[i].transform.localRotation = Quaternion.Euler(new Vector3(0f,0f,0f));
                    }
                }
            }
        }

        if(duration > 0f){
            StartCoroutine(End());
        }
    }

    void OnOff(bool on)
    {
        if(on)
        {
            for(int i=0;i<objs.Length;i++)
            {
                if(objs[i] != null)
                {
                    objs[i].SetActive(true);
                }
            }
            isFired = true;
            collider.enabled = true;
        }
        else
        {
            for(int i=0;i<objs.Length;i++)
            {
                if(objs[i] != null)
                {
                    objs[i].SetActive(false);
                }
            }
            isFired = false;
            collider.enabled = false;
        }
    }

    void OnSceneUnloaded(Scene scene)
    {
        ForcedReturn();
    }

    protected override void OnInit(){
        if(maxChain == chain){  //첫번째 체인이고
            if(subType != SubType.Shooter){//슈터가 아니고
                if(hitable){    //타격 가능 발사체인 경우에만
                    if(term>0f)
                    {
                        // if(hitcount>0){
                        //     deal = deal*(1f/hitcount);
                        // }
                        // else if(duration>0f)
                        // {
                        //     deal = deal*(term/duration);
                        // }
                        // else{
                        //     deal = deal*(term/(dist/speed));
                        // }
                    }
                }
                
            }
            
        }


        SceneManager.sceneUnloaded += OnSceneUnloaded;
        
        this.delay += extraDelay;

        this.hit = 0;
        projSize = 1f;
        if(owner != null){
            projSize = 1f+owner.NStat.projSize;
           // owner.AddProjectile(this);
        }
        

        transform.localScale = Vector3.one*projSize;
        for(int i=0;i<objs.Length;i++)
            objs[i].transform.localScale = Vector3.one;
        
        
        this.isLive = true;
        OnOff(false);
        
    }
    protected override void OnReturn(){
        //if(owner != null) owner.DelProjectile(this);
        
        hitList.Clear();
        if(isRide && owner != null && invisible)
            owner.gameObject.SetActive(true);
        
        isRide = false;
        if(tween != null)
        {
            tween.Kill();
            tween = null;
        }
        OnOff(false);
        //if(isRide) owner.transform.SetParent(null);

        SceneManager.sceneUnloaded -= OnSceneUnloaded;
        
    }

    protected IEnumerator End(){
        yield return new WaitForSeconds(duration);
        GoReturn();
    }

    Actor GetNextTarget(Transform standard, float range = 999999f ){
        Actor nextTarget = this.owner;
        float dist = 999999f;
        for(int i=0;i<UI.Game.Manager.monsters.Count;i++){
            float _dist = Vector3.Distance(UI.Game.Manager.monsters[i].transform.position, standard.position);
            
            if(_dist < dist && _dist < range && (this.target != UI.Game.Manager.monsters[i] || selfChain) && UI.Game.Manager.monsters[i].hp > 0f)
            {
                dist = _dist;
                nextTarget = UI.Game.Manager.monsters[i];
            }
        }
        return nextTarget;
    }

    void GoReturn(){
        if(!this.isLive) return;
        this.isLive = false;

        if(tween != null)
        {
            tween.Kill();
            tween = null;
        }

        if(subType == SubType.Summon){
            if(owner != null){
                Stat rate = new Stat();
                Monster monster = UI.Game.Manager.MonsterPool.MonsterSpawn(projectileName, owner.level, this.transform.position, rate,false,false);
                monster.NStat = owner.NStat * summonRate;
            }
            ReturnObject();
        }

        if(subType == SubType.Explosion){
            Projectile p = MakeProjectile(projectileName); 
            p.dest = Vector3.zero;    
                    
            p.InitObject();
            
            ReturnObject();
        }
        else{
            if(immediateDisappear) ReturnObject();
            else{
                Sequence seq = DOTween.Sequence();
                for(int i=0;i<objs.Length;i++){
                    seq.Join(objs[i].transform.DOScale(Vector3.zero, 0.2f));
                }
                seq.OnComplete(()=>{
                    ReturnObject();
                });
            }
        }
        
    }

    public void ForcedReturn(){
        if(!this.isLive) return;
        this.isLive = false;

        if(tween != null)
        {
            tween.Kill();
            tween = null;
        }

        if(effect != "")
        {
            Effect.Play(effect, transform.position);
            //actor.HitEffect(effect, effectOnHit);
        }

        ReturnObject();
    }

    Projectile MakeProjectile(string name, Actor target = null){
        Projectile p = Pool.GetObject<Projectile>(name);
        p.owner = this.owner;
        if(p.FromCenter && this.target != null)
            p.fromTransform = this.target.Center.transform;
        else
            p.fromTransform = this.transform;
        p.target = target; 
        p.transform.position = this.transform.position;
        p.gameObject.tag = this.gameObject.tag;
        p.condition = this.condition;
        p.deal = this.deal;
        p.dest = this.dest;
        p.range = this.range;
        p.spasticity = this.spasticity;
        p.delay = this.delay;
        p.isRide = this.isRide;
        p.chain = this.chain;
        p.maxChain = this.maxChain;
        p.chainCount = this.chainCount;
        p.chainRange = this.chainRange;
        return p;
    }

    void Hit(Actor actor)
    {
        if(!this.isLive) return;
        if(actor.gameObject.tag != this.gameObject.tag && positive) return;
        if(actor.gameObject.tag == this.gameObject.tag && !positive) return;
        if(dealDuration!=0f && Time.time - fireTime > dealDuration) return;
        if(actor.hp>0) hit++;

        if(force != 0f){
            Vector3 dir = actor.transform.position - this.transform.position;
            dir.Normalize();
            actor.Force += dir * force;
        }

        if(subType == SubType.Shooter)
        {
            if(actor.hp > 0f)
            {
                Projectile p = MakeProjectile(projectileName, actor);
                if(objs.Length>0)
                    p.fromTransform = objs[0].transform;
                else
                    p.fromTransform = this.transform;
                p.dest = actor.transform.position - this.transform.position;
                
                
                p.InitObject();
            }
        }
        else{
            switch(type){
                case Type.Point:
                case Type.Line:
                case Type.Missile:
                    if(actor.NStat.projRefl > 0f){
                        float reflection = Random.Range(0f, 1f);
                        if(reflection < actor.NStat.projRefl){
                            Projectile p = MakeProjectile(gameObject.name, owner);
                            p.target = owner;
                            p.owner = actor;
                            p.dest = owner.transform.position-this.transform.position;
                            p.gameObject.tag = actor.gameObject.tag;
                            p.InitObject();
                        }
                        else{
                            if(deal.Total() != 0 || !positive) actor.Hit(deal,owner,this.spasticity,false,condition);
                            //if(condition != null) actor.AddCondition(condition,deal,owner);
                        }
                    }
                    else{
                        if(deal.Total() != 0 || !positive) actor.Hit(deal,owner,this.spasticity,false,condition);
                        //if(condition != null) actor.AddCondition(condition,deal,owner);
                    }

                    break;
                default:
                    if(deal.Total() != 0 || !positive) actor.Hit(deal,owner,this.spasticity,false,condition);
                    //if(condition != null) actor.AddCondition(condition,deal,owner);
                    break;
            }
           // actor.Hit(deal,owner);
           // if(condition != null) actor.AddCondition(condition);
        }

        
        
        
        if(effect != "")
        {
            //Effect.Play(effect, actor.transform.position);
            actor.HitEffect(effect, effectOnHit);
        }


        //체인이 있는 경우
        if(subType == SubType.Chain)
        {
            if(chain>0){
                Actor nextTarget = GetNextTarget(this.target.Center.transform, chainRange);


                if(nextTarget != null){

                    Projectile p = MakeProjectile(this.gameObject.name, nextTarget);
                    p.chain = this.chain - 1;
                    p.dest = nextTarget.Center.transform.position - this.transform.position;
                    
                    
                    p.InitObject();
                }
                
            }
            
            GoReturn();
        }
        else if(hitcount > 0 && hit >= hitcount)
        {
            if(isInit) GoReturn();
        }
    }


    // void OnCollisionEnter2D(Collision2D collision)
    // {
    //     Debug.Log("tag : "+collision.gameObject.tag.ToString());
    //     if(collision.gameObject.tag == "Block"){
    //         ForcedReturn();
    //         return;
    //     }
    // }

    void OnTriggerEnter2D(Collider2D other)
    {
        if(isRide){
            if(other.gameObject.tag == "Block"){
                GoReturn();
                return;
            }
        }
        

        if(type == Type.Laser) return;
        if(!this.hitable) return;
        if(!this.isLive) return;
        //if(!other.isTrigger) return;
        //if(other.gameObject.tag == this.gameObject.tag) return;
        
        if(other.gameObject.tag != this.gameObject.tag && positive) return;
        if(other.gameObject.tag == this.gameObject.tag && !positive) return;

        //if(other.gameObject != owner.gameObject)
        //{
            //Debug.Log("this : " + this.gameObject.name + " other : " + other.gameObject.name);

            Actor actor = other.gameObject.GetComponent<Actor>();
            if(actor != null){
                if(term > 0f){
                    hitList.Add(actor);
                }
                else{
                    Hit(actor);
                }
            }
            else{
                Projectile p = other.gameObject.GetComponent<Projectile>();
                if(p != null){
                    if(!p.Inevitable){
                        if(barrier){
                            p.ForcedReturn();
                        }
                    }
                    
                }
            }
            

        //}
    }
    void OnTriggerExit2D(Collider2D other)
    {
        if(type == Type.Laser) return;
        if(!this.hitable) return;
        if(!this.isLive) return;
        //if(!other.isTrigger) return;

        if(other.gameObject.tag != this.gameObject.tag && positive) return;
        if(other.gameObject.tag == this.gameObject.tag && !positive) return;

        //if(other.gameObject != owner.gameObject)
        //{
            Actor actor = other.gameObject.GetComponent<Actor>();
            if(actor != null){
                if(term > 0f){
                    hitList.Remove(actor);
                }
            }
        //}
    }

    public enum Type{
        Point = 0,
        Line,
        Missile,
        Be,
        Laser,
        Around,
        Chase,
        Self,
        Random,
        Sticker
    }

    public enum SubType{
        None = 0,
        Reverse,
        Flip,
        Chain,
        Shooter,
        Explosion,
        Barrier,
        Roller,
        Summon
    }
}
