using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using DG.Tweening;
using Lofelt.NiceVibrations;
using UnityEngine.UI;

public class GameUI : MonoBehaviour
{


    [SerializeField]
    private string animName = "attack";

    [SerializeField]
    private VariableJoystick joystick;
    public VariableJoystick Joystick { get => joystick; }

    private Game manager;
    public Game Manager { get => manager; 
        set{
            manager = value; 
            SetUp();
        }
    }

    private Player player;
    public Player Player { get => player; }

    // [SerializeField]
    // private List<bool> slotStatus = new List<bool>();
    // public List<bool> SlotStatus { 
    //     get => slotStatus; 
    //     set => slotStatus = value;
    // }
    private List<ActiveSlotButton> slotButtons = new List<ActiveSlotButton>(), mobilityButtons = new List<ActiveSlotButton>();
    private int slotButtonCount = 0;
    private List<GameObject> emptySlots = new List<GameObject>();

    private bool approach = false;

    private Vector2 moveDir = Vector2.zero;
    public Vector2 MoveDir { get => joystick.Direction; }

    [SerializeField]
    private UIGauge hpGauge, expGauge;
    [SerializeField]
    private TextMeshProUGUI levelText;

    [SerializeField]
    private Transform activeSlotParent, passiveSlotParent, bossHpGaugeParent, mobilityParent;

    private List<PassiveSlot> passiveSlots = new List<PassiveSlot>();

    [SerializeField]
    private StageGauge stageGauge;

    [SerializeField]
    private GameObject bossHpGaugePrefab;

    [SerializeField]
    private StatRow[] statRows;

    private List<UIGauge> bossHpGauge = new List<UIGauge>();

    private bool gameOver = false;
    public bool GameOver { get => gameOver; }

    private int waitActive = -1, deleteActiveCnt = 0, mobilityActiveCnt = 0;

    private float gameStartTime = 0f;
    public float GameStartTime{
        get{
            return gameStartTime;
        }
    }

    private int firstOrderActive = 0;

    private bool inLevelup = false;

    [SerializeField]
    private List<BossIndicator> bossIndicators = new List<BossIndicator>();

    [SerializeField]
    private GridLayoutGroup mobilitySlotGroup;


    [SerializeField]
    private TextMeshProUGUI testText;
    
    


    private Sequence bossGaugeSeq = null;

    void Awake(){
        //slotStatus = new bool[manager.Player.Active.Length];
        foreach(Transform passiveSlot in passiveSlotParent){
            passiveSlots.Add(passiveSlot.GetComponent<PassiveSlot>());
        }
    }

    public void RepositionMobilitySlots(){
        int mr = PlayerPrefs.GetInt("MobilityRight",0);
        if(mr == 1) mobilitySlotGroup.childAlignment = TextAnchor.MiddleRight;
        else mobilitySlotGroup.childAlignment = TextAnchor.MiddleLeft;
    }

    void SetUp(){
        player = manager.Player;
        
        //slotStatus.Clear();
        foreach(ActiveSlotButton button in slotButtons){
            Destroy(button.gameObject);
        }
        slotButtons.Clear();
        slotButtonCount = 0;

        foreach(ActiveSlotButton button in mobilityButtons){
            Destroy(button.gameObject);
        }
        mobilityButtons.Clear();
        mobilityActiveCnt = 0;

        foreach(BossIndicator indicator in bossIndicators){
            Destroy(indicator.gameObject);
        }
        bossIndicators.Clear();

        foreach(Transform slot in activeSlotParent){
            Destroy(slot.gameObject);
        }
        emptySlots.Clear();

        RepositionMobilitySlots();

        //Debug.Log("dungeon : "+Data.DungeonName);
        if(manager.GameType == Game.Type.Field)
            stageGauge.InfoKey = "";//Data.StageName;
        else if(manager.GameType == Game.Type.Dungeon)
            stageGauge.InfoKey = Data.DungeonName;

        firstOrderActive = 0;
        waitActive = -1;
        deleteActiveCnt = 0;
        mobilityActiveCnt = 0;

        gameOver = false;
        inLevelup = false;

        bossGaugeSeq = null;
        bossHpGaugeParent.gameObject.SetActive(false);
        stageGauge.gameObject.SetActive(true);
        stageGauge.transform.localScale = Vector3.one;

        gameStartTime = Time.time;
    }

    void OnEnable()
    {
        Debug.Log("GameUI Enable");
    }

    void Update(){
        if(manager != null){
            int i=0;

            Vector2 joysticDirection = joystick.Direction;

            #if UNITY_EDITOR
            //PC에서 조작관련 코드
            if(Input.GetKeyDown(KeyCode.Z)) LevelUpCheat();
            if(Input.GetKeyDown(KeyCode.X)) NextPointCheat();
            if(Input.GetKeyDown(KeyCode.C)) NextBossCheat();
            // 방향키가 눌렸을 때의 조건
            if(Input.GetKey(KeyCode.LeftArrow))
                joysticDirection += Vector2.left;
            if(Input.GetKey(KeyCode.RightArrow))
                joysticDirection += Vector2.right;
            if(Input.GetKey(KeyCode.UpArrow))
                joysticDirection += Vector2.up;
            if(Input.GetKey(KeyCode.DownArrow))
                joysticDirection += Vector2.down;
            // 스페이스바가 눌렸을 때의 조건
            if(Input.GetKeyDown(KeyCode.Space))
            {
                if(mobilityButtons.Count>0)
                    mobilityButtons[0].GetComponent<Button>().OnPointerClick(null);
            }
            int keyActive = -1;
            if(Input.GetKeyDown(KeyCode.Q))
                keyActive = 4;
            if(Input.GetKeyDown(KeyCode.W))
                keyActive = 3;
            if(Input.GetKeyDown(KeyCode.E))
                keyActive = 2;
            if(Input.GetKeyDown(KeyCode.R))
                keyActive = 1;
            if(Input.GetKeyDown(KeyCode.T))
                keyActive = 0;
            if(keyActive!=-1){
                int _keyActive = 0;
                for(i=0;i<slotButtons.Count;i++){
                    if(slotButtons[i].gameObject.activeSelf){
                        if(keyActive == _keyActive){
                            slotButtons[i].GetComponent<Button>().OnPointerClick(null);
                            break;
                        }
                        _keyActive++;
                    }
                }
            }
            //PC에서 조작관련 코드(끝)
            #endif


            
            
            //보스 체력 게이지 및 인디케이터 생성
            for(i=bossHpGauge.Count;i<manager.Boss.Count;i++){
                Monster boss = manager.Boss[i];
                UIGauge gauge = Instantiate(bossHpGaugePrefab,bossHpGaugeParent).GetComponent<UIGauge>();
                bossHpGauge.Add(gauge);
                gauge.NameText.text = Util.LocalStr("Name",boss.gameObject.name);

                Assets.CreateAsset("Assets/Game/Prefab/UI/BossIndicator.prefab", (indicator) => {
                    BossIndicator bossIndicator = indicator.GetComponent<BossIndicator>();
                    bossIndicators.Add(bossIndicator);
                    bossIndicator.SetBoss(boss.transform);
                },this.transform);
            }
            //보스 체력 게이지 및 인디케이터 삭제
            for(i=manager.Boss.Count; i<bossHpGauge.Count; i++){
                Destroy(bossHpGauge[i].gameObject);
                bossHpGauge.RemoveAt(i);
                if(i<bossIndicators.Count){
                    Destroy(bossIndicators[i].gameObject);
                    bossIndicators.RemoveAt(i);
                }
                i--;
            }
            //보스 체력 게이지 업데이트
            for(i=0;i<bossHpGauge.Count;i++){
                bossHpGauge[i].Set(0l,Mathf.FloorToInt(manager.Boss[i].NStat.hp),Mathf.FloorToInt(manager.Boss[i].hp));
            }

            //플레이어 게이지 업데이트
            hpGauge.Set(0l,Mathf.FloorToInt(player.NStat.hp), Mathf.FloorToInt(player.hp));
            

            //여기서부턴 게임오버시 안함
            if(gameOver) return;


            //플레이어 스킬 사용하기
            bool aConfirm = false;
            if(waitActive != -1){
                ActiveErr err = manager.Player.UseActive(slotButtons[waitActive].SlotNo);
                if(err == ActiveErr.Range) 
                {
                    if(joysticDirection != Vector2.zero){
                        waitActive = -1;
                        ActiveOrderUpdate();
                    }
                    else{
                        //접근 가능한 스킬인 경우
                        if(Info.Active[player.Active[slotButtons[waitActive].SlotNo].infoKey].approach)
                            aConfirm = true;
                        waitActive = -1;
                        ActiveOrderUpdate();
                    }
                }
                else if(err == ActiveErr.None || err == ActiveErr.CoolTime) {
                    waitActive = -1;
                    ActiveOrderUpdate();
                }
                else if(err == ActiveErr.etc){
                    waitActive = -1;
                    ActiveOrderUpdate();
                }
            }
            else{
                if(slotButtons.Count > firstOrderActive) {
                    for(int x=0;x<slotButtons.Count;x++){
                        if(slotButtons[firstOrderActive].on){
                            ActiveErr err = manager.Player.UseActive(slotButtons[firstOrderActive].SlotNo);
                            if(err == ActiveErr.Range) {
                                if(joysticDirection != Vector2.zero){
                                    waitActive = -1;
                                    ActiveOrderUpdate();
                                }
                                else{
                                    //접근 가능한 스킬인 경우
                                    if(Info.Active[player.Active[slotButtons[firstOrderActive].SlotNo].infoKey].approach)
                                        aConfirm = true;
                                    ActiveOrderUpdate();
                                }
                            }
                            else if(err == ActiveErr.None || err == ActiveErr.CoolTime) {
                                ActiveOrderUpdate();
                            }
                            else if(err == ActiveErr.Wait){
                                waitActive = firstOrderActive;
                            }
                            else if(err == ActiveErr.etc){
                                ActiveOrderUpdate();
                            }
                        }
                        else{
                            ActiveOrderUpdate();
                        }
                    }
                }
            }
            
            approach = aConfirm;

        

            if(joysticDirection != Vector2.zero) moveDir = joysticDirection.normalized;
            else if(approach){
                moveDir = manager.Player.Target.transform.position - manager.Player.transform.position;
                moveDir.Normalize();
            }else{
                moveDir = joysticDirection;
            }

            manager.Player.MoveDir = moveDir;

            //빈 액티브 슬롯
            for(i = emptySlots.Count; i < GetRemainActiveCount();i++){
                Assets.CreateAsset("Assets/Game/Prefab/UI/EmptyActiveSlot.prefab", (slot) => {
                    //slot.transform.SetParent(activeSlotParent);
                    //slot.transform.localScale = Vector3.one;
                    emptySlots.Add(slot);
                    slot.transform.SetSiblingIndex(99);
                },activeSlotParent);
            }
            for(i = GetRemainActiveCount(); i < emptySlots.Count; i++){
                Destroy(emptySlots[i]);
                emptySlots.RemoveAt(i--);
            }


            //액티브 슬롯 버튼 생성
            //mobilityActiveCnt = 0;
            for(i = slotButtonCount+mobilityActiveCnt; i < player.Active.Count; i++){
                if(Info.Active[player.Active[i].infoKey].mobility) {
                    mobilityActiveCnt++;
                    int index = i;
                    Assets.CreateAsset<ActiveSlotButton>("Assets/Game/Prefab/UI/MobilitySlotButton.prefab", (slotButton) => {
                        //slotButton.transform.SetParent(activeSlotParent);
                        //slotButton.transform.localScale = Vector3.one;
                        slotButton.SlotNo = index;
                        slotButton.on = false;
                        mobilityButtons.Add(slotButton);
                        slotButton.transform.SetSiblingIndex(mobilityActiveCnt);
                    },mobilityParent);
                    
                }
                else{
                    //slotStatus.Add(true);
                    slotButtonCount++;
                    int index = i;
                    Assets.CreateAsset<ActiveSlotButton>("Assets/Game/Prefab/UI/ActiveSlotButton.prefab", (slotButton) => {
                        //slotButton.transform.SetParent(activeSlotParent);
                        //slotButton.transform.localScale = Vector3.one;
                        slotButton.SlotNo = index;
                        slotButtons.Add(slotButton);
                        slotButton.transform.SetSiblingIndex(slotButtons.Count-1);
                    },activeSlotParent);
                }

                
            }
            //액티브 슬롯 없어지는 가능성에 대해

            
            //패시브 슬롯 컨트롤
            for(i=0;i<passiveSlots.Count;i++){
                if(i<player.Passive.Count){
                    passiveSlots[i].Data = player.Passive[i];
                }
                else{
                    passiveSlots[i].Data = null;
                }
            }
            
            

            //타임 게이지와 보스 게이지 전환
            if(stageGauge.gameObject.activeSelf){
                if(manager.Boss.Count > 0){
                    if(bossGaugeSeq == null){
                        bossHpGaugeParent.gameObject.SetActive(true);
                        bossHpGaugeParent.localScale = Vector3.zero;
                        bossGaugeSeq = DOTween.Sequence();
                        bossGaugeSeq.Append(stageGauge.transform.DOScale(Vector3.zero,0.3f).SetEase(Ease.OutBack));
                        bossGaugeSeq.Append(bossHpGaugeParent.DOScale(Vector3.one,0.3f).SetEase(Ease.InBack));
                        bossGaugeSeq.OnComplete(()=>{
                            stageGauge.gameObject.SetActive(false);
                            bossGaugeSeq = null;
                        });
                    }
                }
            }
            else{
                if(manager.Boss.Count == 0){
                    if(bossGaugeSeq == null){
                        stageGauge.gameObject.SetActive(true);
                        stageGauge.transform.localScale = Vector3.zero;
                        bossGaugeSeq = DOTween.Sequence();
                        bossGaugeSeq.Append(bossHpGaugeParent.DOScale(Vector3.zero,0.3f).SetEase(Ease.OutBack));
                        bossGaugeSeq.Append(stageGauge.transform.DOScale(Vector3.one,0.3f).SetEase(Ease.InBack));
                        bossGaugeSeq.OnComplete(()=>{
                            bossHpGaugeParent.gameObject.SetActive(false);
                            bossGaugeSeq = null;
                            Debug.Log("Boss Out!");
                            Sound.PlayBGM(Data.DungeonName);
                        });
                    }
                }
            }



            

            //경험치 게이지
            int level = Util.GetLevel(player.Exp);
            int maxExp = Util.GetExp(level+1);
            int minExp = Util.GetExp(level);
            expGauge.Set(0,minExp-maxExp,minExp-player.Exp);

            levelText.text = "LEVEL "+level.ToString();

            if(!inLevelup){
                if(manager.Level < level){
                    //레벨업
                    manager.Level += 1;
                    inLevelup = true;
                    //manager.Player.hp = manager.Player.NStat.hp;

                    Projectile p = Pool.GetObject<Projectile>("Levelup");
                    p.owner = manager.Player;
                    p.fromTransform = manager.Player.transform;
                    p.target = null;
                    p.transform.position = manager.Player.transform.position;
                    p.gameObject.tag = manager.Player.gameObject.tag;
                    p.condition = new ConditionData();
                    p.deal = new Deal();
                    p.deal.physics = 1;
                    p.dest = Vector3.zero;
                    p.delay = 0f;

                    p.InitObject();
                    

                    Haptic.Play(HapticPatterns.PresetType.MediumImpact);
                    
                    StartCoroutine(ShowLevelupPopup());
                }
            }


            for(int x=0;x<statRows.Length;x++){
                if(statRows[x] != null){
                    statRows[x].Stat = player.NStat;
                }
            }



            //테스트용
            //testText.text = "Monster : "+manager.monsterCount.ToString()+"\n";
            testText.text = "Kill : "+manager.KillCount.ToString()+"\n";
            //testText.text += "Aggressive : "+manager.Aggressive.Count.ToString()+"\n";
            //testText.text += "Boss : "+manager.Boss.Count.ToString()+"\n";
            testText.text += "Gold : "+manager.TotalCoin.ToString()+"\n";
            testText.text += "Exp : "+player.Exp.ToString()+"\n";
            testText.text += "Time : "+Mathf.FloorToInt(Time.time - gameStartTime).ToString()+"s";
        }
    }

    public int GetRemainActiveCount(){
        int cnt = Mathf.FloorToInt(player.NStat.activeSlot + deleteActiveCnt + mobilityActiveCnt - player.Active.Count);
        if(cnt < 0) cnt = 0;
        return cnt;
    }
    public int GetRemainPassiveCount(){
        int cnt = Mathf.FloorToInt(player.NStat.passiveSlot - player.Passive.Count);
        if(cnt < 0) cnt = 0;
        return cnt;
    }

    IEnumerator ShowLevelupPopup(){
        yield return new WaitForSeconds(0.5f);
        if(!gameOver){
            LevelupPopup popup = UI.OpenPopup<LevelupPopup>("InGameLevelup");
            popup.IsSkillBox = false;
            popup.callback = ()=>{
                inLevelup = false;
            };
        }else{
            yield break;
        }
        //UI.OpenPopupSimple("InGameLevelup");
    }

    void ActiveOrderUpdate(){
        firstOrderActive ++;
        if(firstOrderActive >= slotButtons.Count)
            firstOrderActive = 0;
    }
    public void JoystickOnOff(bool on){
        joystick.gameObject.SetActive(on);
    }

    public void DelActive(string infoKey){
        //slotButtons[waitActive].SlotNo
        for(int i=0;i<slotButtons.Count;i++){
            if(player.Active[slotButtons[i].SlotNo].infoKey == infoKey){
                slotButtons[i].on = false;
                slotButtons[i].gameObject.SetActive(false);
                deleteActiveCnt++;
                break;
            }
        }
        // for(int i=0;i<player.Active.Count;i++){
        //     if(player.Active[i].infoKey == infoKey){
        //         //slotStatus[i] = false;
        //         slotButtons[i].on = false;
        //         slotButtons[i].gameObject.SetActive(false);
        //         deleteActiveCnt++;
        //         break;
        //     }
        // }
    }

    // public void Anim(){
        
    //     slotStatus[0] = !slotStatus[0];
    // }



    public void SetStageClear(){
        gameOver = true;
        GameOverPopup popup = UI.OpenPopup<GameOverPopup>("GameOver");

        int now = 0;
        for(int i=0;i<Data.Player.dungeon.Count;i++){
            if(Data.DungeonName == Data.Player.dungeon[i].infoKey) now = i;
        }
        float rewardRate = (float)(now + 1)/Data.Player.dungeon.Count;


        //Wealth wealth = new Wealth();
        //int exp = Mathf.FloorToInt(Time.time - gameStartTime);
        int exp = (manager.exp-1)*100;
        exp = Mathf.FloorToInt(exp * rewardRate);
        int[] boxes = new int[5];
        for(int i=0;i<boxes.Length;i++){
            float rate = Random.Range(0f,1f);
            switch(i){
                case 0:
                    boxes[i] = Random.Range(3,8);
                    break;
                case 1:
                    boxes[i] = Random.Range(0,1);
                    break;
                case 2:
                    if(rate<0.1f) boxes[i] = 1;
                    break;
                case 3:
                    if(rate<0.01f) boxes[i] = 1;
                    break;
                case 4:
                    if(rate<0.001f) boxes[i] = 1;
                    break;
            }
            
        }

        
        

        //int gold = manager.TotalCoin;
        int gold = exp/2;
        popup.rewards = new List<Stuff>();
        if(exp>0)
            popup.rewards.Add(new Stuff(new Wealth(Wealth.Type.exp,exp)));
        if(gold>0)
            popup.rewards.Add(new Stuff(new Wealth(Wealth.Type.gold,gold)));

        for(int i=0;i<boxes.Length;i++){
            if(boxes[i] >= 1){
                popup.rewards.Add(new Stuff(new Wealth((Wealth.Type)(5+i),boxes[i])));
            }
        }

        if(Data.Player.wealth.reward == 0){
            EquipmentData weapon = new EquipmentData();
            weapon.infoKey = "Bow1";
            weapon.grade = Grade.Common;
            weapon.level = 1;
            weapon.quality = 0.5f;
            popup.rewards.Add(new Stuff(weapon));
            EquipmentData shield = new EquipmentData();
            shield.infoKey = "Quiver1";
            shield.grade = Grade.Common;
            shield.level = 1;
            shield.quality = 0.5f;
            popup.rewards.Add(new Stuff(shield));
            Data.AddWealth(Wealth.Type.reward,1);
        }
        else if(Data.Player.wealth.reward == 1){
            EquipmentData weapon = new EquipmentData();
            weapon.infoKey = "Wand1";
            weapon.grade = Grade.Common;
            weapon.level = 1;
            weapon.quality = 0.5f;
            popup.rewards.Add(new Stuff(weapon));
            EquipmentData shield = new EquipmentData();
            shield.infoKey = "MagicOrb1";
            shield.grade = Grade.Common;
            shield.level = 1;
            shield.quality = 0.5f;
            popup.rewards.Add(new Stuff(shield));
            Data.AddWealth(Wealth.Type.reward,1);
        }

        popup.clear = true;
        popup.Setup();
    
        
        Data.AddStuffs(popup.rewards);
        //Data.AddWealth(wealth);
    }
    public void SetGameOver(){
        
        int now = 0;
        for(int i=0;i<Data.Player.dungeon.Count;i++){
            if(Data.DungeonName == Data.Player.dungeon[i].infoKey) now = i;
        }
        float rewardRate = (float)(now + 1)/Data.Player.dungeon.Count;
        //Debug.Log("보상 비율 : "+rewardRate);

        gameOver = true;
        Time.timeScale = 0.2f;
        MainCamera.CurtainIn(0.3f,()=>{
            Time.timeScale = 1f;
            GameOverPopup popup = UI.OpenPopup<GameOverPopup>("GameOver");
            Wealth wealth = new Wealth();
            //int exp = Mathf.FloorToInt((Time.time - gameStartTime));
            int exp = (manager.exp-1)*100;
            exp = Mathf.FloorToInt(exp * rewardRate);
            //int gold = manager.TotalCoin;
            int gold = exp/5;
            //wealth.exp = exp;
            //popup.wealth = wealth;
            popup.rewards = new List<Stuff>();
            if(exp>0)
                popup.rewards.Add(new Stuff(new Wealth(Wealth.Type.exp,exp)));
            if(gold>0)
                popup.rewards.Add(new Stuff(new Wealth(Wealth.Type.gold,gold)));

            if(Data.Player.wealth.reward == 0){
                EquipmentData weapon = new EquipmentData();
                weapon.infoKey = "Bow1";
                weapon.grade = Grade.Common;
                weapon.level = 1;
                weapon.quality = 0.5f;
                popup.rewards.Add(new Stuff(weapon));
                EquipmentData shield = new EquipmentData();
                shield.infoKey = "Quiver1";
                shield.grade = Grade.Common;
                shield.level = 1;
                shield.quality = 0.5f;
                popup.rewards.Add(new Stuff(shield));
                Data.AddWealth(Wealth.Type.reward,1);
            }
            else if(Data.Player.wealth.reward == 1){
                EquipmentData weapon = new EquipmentData();
                weapon.infoKey = "Wand1";
                weapon.grade = Grade.Common;
                weapon.level = 1;
                weapon.quality = 0.5f;
                popup.rewards.Add(new Stuff(weapon));
                EquipmentData shield = new EquipmentData();
                shield.infoKey = "MagicOrb1";
                shield.grade = Grade.Common;
                shield.level = 1;
                shield.quality = 0.5f;
                popup.rewards.Add(new Stuff(shield));
                Data.AddWealth(Wealth.Type.reward,1);
            }

            popup.clear = false;
            popup.Setup();
            
            //Data.AddWealth(wealth);
            Data.AddStuffs(popup.rewards);
        });
        
    }

    public void GoMain(){
        
        Loader.LoadScene("Main");
        
    }

    public void Option(){
        
        UI.OpenPopupSimple("Option");
        
    }

    /// <summary>
    /// 치트코드
    /// </summary>
    public void LevelUpCheat(){
        
        int level = Util.GetLevel(player.Exp);
        int maxExp = Util.GetExp(level+1);
        player.Exp = maxExp;
    }
    public void NextPointCheat(){
        if(manager != null){
            stageGauge.StartTime = manager.NextPointCheat();
        }
    }

    public void NextBossCheat(){
        if(manager != null){
            stageGauge.StartTime = manager.NextBossCheat();
        }
    }
}