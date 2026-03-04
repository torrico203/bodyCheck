using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using DG.Tweening;
using System;


public class ActorRoot : MonoBehaviour
{
    [SerializeField]
    private Animator _anim;

    [SerializeField]
    private Transform body;
    public Transform Body { get => body; }

    private float dirX = 0f;

    private Dictionary<string,Color> defaultColor = new Dictionary<string, Color>();

    private SortingGroup _sortingGroup;

    [System.Serializable]
    public class RendererDic : CustomDic.SerializableDictionary<string, SpriteRenderer>{}
    [SerializeField]
    private RendererDic _renderers;
    public RendererDic Renderers { get => _renderers; }

    private Sequence seq = null;

    private float _attackTime = 0.6f;
    public float AttackTime { get => _attackTime; }

    [System.Serializable]
    public class OrderDic : CustomDic.SerializableDictionary<string, int>{}
    [SerializeField]
    private OrderDic order;

    // 애니메이션 이름과 해시값을 저장할 딕셔너리
    private Dictionary<string, int> _nameToHashPair = new Dictionary<string, int>();

    private void InitAnimPair(){
        _nameToHashPair.Clear();
        AnimationClip[] _animationClips = _anim.runtimeAnimatorController.animationClips;
        foreach (var clip in _animationClips)
        {
            int hash = Animator.StringToHash(clip.name);
            _nameToHashPair.Add(clip.name, hash);
        }
    }
    private void Awake() {
        _sortingGroup = GetComponent<SortingGroup>();
        InitAnimPair();
        InitColor();
    }

    void Update()
    {

    }

    public void SetDirection(float x){
        if(x > 0){
            transform.localRotation = Quaternion.Euler(new Vector3(0f,180f,0f));
            dirX = 1f;
        }else if(x < 0){
            transform.localRotation = Quaternion.Euler(new Vector3(0f,0f,0f));
            dirX = -1f;
        }
        //dirX = x;
    }

    public void SetAttackSpeed(float speed){
        if(speed < 2f)
            speed = 2f;
        _attackTime = 1.0f / speed;
        _anim.SetFloat("ASPD", speed);
    }

    void InitColor(){
        foreach (var renderer in _renderers){
            if(defaultColor.ContainsKey(renderer.Key)){
                defaultColor[renderer.Key] = renderer.Value.color;
            }
            else{
                defaultColor.Add(renderer.Key, renderer.Value.color);
            }
        }
    }
    public Dictionary<string,Color> GetColor(){
        return defaultColor;
    }

    void KillSeq(){
        if(seq != null) {
            seq.Kill();
            seq = null;
        }
        body.localPosition = Vector3.zero;
        body.localRotation = Quaternion.Euler(0f,0f,0f);
    }
    public void SetColor(Color color){
        KillSeq();
        
        foreach (var renderer in _renderers){
            renderer.Value.color = color;
        }
        InitColor();
    }
    public void SetColor(string name,Color color){
        if(this._renderers.ContainsKey(name)){ 
            this._renderers[name].color = color;
        }
        InitColor();
    }
    public void Hit(){
        if(seq == null){
            seq = DOTween.Sequence();
            seq.Join(body.DOShakePosition(0.2f, 0.1f, 30, 90, false));
            foreach (var renderer in _renderers){
                //Color color = renderer.Value.color;
                Color color = defaultColor[renderer.Key];
                seq.Join(renderer.Value.DOColor(Color.red, 0.1f).OnComplete(()=>{renderer.Value.DOColor(color, 0.1f);}));
            }
            transform.localScale = Vector3.one * 0.6f;
            seq.Join(transform.DOScale(Vector3.one, 0.2f).SetEase(Ease.OutBack));
                
            seq.AppendCallback(KillSeq);
        }
    }
    public void Roll(float duration, float dir){
        KillSeq();
        seq = DOTween.Sequence();
        //Debug.Log("Roll to : " + (-1f*dir*360f));
        seq.Join(body.DORotate(new Vector3(0f,0f,(dirX)*dir*360f), duration,RotateMode.LocalAxisAdd).SetEase(Ease.OutQuart));
        seq.Join(body.DOLocalMoveY(0.5f, duration/6f).OnComplete(()=>{
            body.DOLocalMoveY(0f, duration/6f);
        }));
        seq.AppendCallback(KillSeq);
    }

    public void SetAlpha(float alpha, float time = 0f, Action onComplete = null,Ease ease = Ease.Linear){
        Sequence alphaSeq = DOTween.Sequence();
        foreach (var renderer in _renderers)
            alphaSeq.Join(renderer.Value.DOFade(alpha, time).SetEase(ease));
        
        alphaSeq.AppendCallback(()=>{
            onComplete?.Invoke();
        });
    }

    public void SetSortingOrder(int order){
        _sortingGroup.sortingOrder = order;
    }

    public void SetSpriteRenderer(string name, Sprite sprite, bool flipY = false){
        if(this._renderers.ContainsKey(name)){ 
            this._renderers[name].sprite = sprite;
            this._renderers[name].flipY = flipY;
        }
    }
    // 이름으로 애니메이션 실행
    public void PlayAnimation(string name,bool forced = false){
        //Debug.Log(name);
       // _anim.GetCurrentAnimatorStateInfo(0).IsName(name);

        string currentName = "";
        
        if(_anim.GetCurrentAnimatorClipInfo(0).Length > 0)
            currentName = _anim.GetCurrentAnimatorClipInfo(0)[0].clip.name;
        if(currentName == name){
            switch(name){
                case "Idle":
                case "Move":
                    return;
            }
        }

        if(!forced || currentName == "Death"){
            if(order.ContainsKey(name) && order.ContainsKey(currentName)){
                if(order[name] < order[currentName]) return;
            }
        }
        
        
        
        
        if(_nameToHashPair.ContainsKey(name)){
            //Debug.Log(currentName + " -> " + name);
            _anim.Play(_nameToHashPair[name], -1, float.NegativeInfinity);
            _anim.Update(0f);
        }
        else{
            foreach (var animationName in _nameToHashPair)
            {
                if(animationName.Key.ToLower().Contains(name.ToLower()) ){
                    _anim.Play(animationName.Value, -1, float.NegativeInfinity);
                    _anim.Update(0f);
                    break;
                }
            }
        }
        
    }
}