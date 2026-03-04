using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using System.Collections.Generic;
using System;

public class UI : MonoBehaviour
{
    protected static UI i;
    public static UI I
    {
        get
        {
            if (i == null)
            {
                i = FindObjectOfType<UI>();
                if (i == null)
                {
                    GameObject singleton = new GameObject(typeof(UI).ToString());
                    i = singleton.AddComponent<UI>();
                    //DontDestroyOnLoad(singleton);
                }
            }
            return i;
        }
    }
    [SerializeField]
    private RectTransform body,upper,lower,popupBody;
    public static RectTransform Body { get => I.body; }

    [SerializeField]
    private MainUI main;
    public static MainUI Main { get => I.main; }

    [SerializeField]
    private GameUI game;
    public static GameUI Game { get => I.game; }

    [SerializeField]
    private TitleUI title;
    public static TitleUI Title { get => I.title; }

    [SerializeField]
    private Canvas popupCanvas;
    public static Canvas PopupCanvas { get => I.popupCanvas; }

    [SerializeField]
    private MainCamera mainCamera;
    public static MainCamera MainCamera { get => I.mainCamera; }

    private bool loaded = false;
    public static bool Loaded { get => I.loaded; }

    [SerializeField]
    private Transform SmallNoticePosition;

    private int smallNoticeCount = 0;
    public static int SmallNoticeCount { get => I.smallNoticeCount; set => I.smallNoticeCount = value; }

    private Vector2 screen;

    
    private Dictionary<string, GameObject> popupDic = new Dictionary<string, GameObject>();

    private void Awake()
    {
        if(i == null)
        {
            DontDestroyOnLoad(gameObject);
            i = this;
        }
        else{
            Destroy(gameObject);
        }
        this.SetBody();
        //Debug.Log(Screen.SafeArea);
    }

    public static void Load(){
        I.StartCoroutine(I.LoadCoroutine());
    }

    void SetBody(){
        //body
        Vector2 minAnchor = Screen.safeArea.min;
        Vector2 maxAnchor = Screen.safeArea.max;

        minAnchor.x /= Screen.width;
        minAnchor.y /= Screen.height;

        maxAnchor.x /= Screen.width;
        maxAnchor.y /= Screen.height;
        
        body.anchorMin = minAnchor;
        body.anchorMax = maxAnchor;

        popupBody.anchorMin = minAnchor;
        popupBody.anchorMax = maxAnchor;

        upper.anchorMin = new Vector2(0f, maxAnchor.y);;
        lower.anchorMax = new Vector2(1f, minAnchor.y);

        screen = new Vector2(Screen.width, Screen.height);
    }

    void Update(){
        if(screen.x != Screen.width || screen.y != Screen.height){
            SetBody();
        }
    }


    IEnumerator LoadCoroutine(){
        AsyncOperationHandle<IList<GameObject>> handle =  Addressables.LoadAssetsAsync<GameObject>("UI", obj =>
        {
            //Gets called for every loaded asset
            

            if(obj.tag == "Popup"){
                GameObject _obj = Instantiate(obj,this.popupBody);
                _obj.name = obj.name;
                _obj.SetActive(false);
                popupDic.Add(obj.name, _obj);
                return;
            }
            else{
                Debug.Log("UI : "+obj.name);
                GameObject _obj = Instantiate(obj,this.body);
                _obj.SetActive(false);
                switch(obj.name){
                    case "MainUI":
                        main = _obj.GetComponent<MainUI>();
                    break;
                    case "GameUI":
                        game = _obj.GetComponent<GameUI>();
                    break;
                    case "TitleUI":
                        title = _obj.GetComponent<TitleUI>();
                    break;
                    //case "Popups":
                    // case "PopupCanvas":
                    //     popupCanvas = _obj;
                    //     popupCanvas.transform.SetParent(null);
                    //     _obj.SetActive(true);
                    //     Transform popupsParent = _obj.transform;
                    //     foreach(Transform child in popupsParent){
                    //         Debug.Log(child.name);
                    //         popupDic.Add(child.name, child.gameObject);
                    //     }
                    // break;
                }
                
                _obj.name = obj.name;
                _obj.transform.SetAsFirstSibling();
            }
            
        });
        yield return handle;
        Debug.Log("UI Loaded");


        PlayFabManager.Connect();//UI로드완료시점에서네트워크시작
        
        //popups.transform.SetAsLastSibling();
        loaded = true;
    }

    public static void OpenPopupSimple(string name){//UnityEvent때문에
        Debug.Log("OpenPopupSimple : "+name);
        I.popupDic[name].SetActive(true);
    }
    public static void ClosePopupSimple(string name){
        I.popupDic[name].SetActive(false);
    }
    public static T OpenPopup<T>(string name){
        I.popupDic[name].SetActive(true);
        return I.popupDic[name].GetComponent<T>();
    }

    public static void Confirm(Action confirm,string content,string title = null,Action cancel = null){
        I.popupDic["Confirm"].SetActive(true);
        ConfirmPopup popup = I.popupDic["Confirm"].GetComponent<ConfirmPopup>();
        if(title != null){
            popup.title.gameObject.SetActive(true);
            popup.title.text = Util.LocalStr("UI",title);
        }
        else{
            popup.title.gameObject.SetActive(false);
        }
        if(content != null){
            popup.content.gameObject.SetActive(true);
            popup.content.text = Util.LocalStr("UI",content);
        }
        else{
            popup.content.gameObject.SetActive(false);
        }
        popup.actions["confirm"] = confirm;
        if(cancel != null){
            popup.cancelButton.gameObject.SetActive(true);
            popup.actions["cancel"] = cancel;
        }
        else{
            popup.cancelButton.gameObject.SetActive(false);
        }
    }

    public static void SetScene(string sceneName){
        switch(sceneName){
            case "Title":
                I.main.gameObject.SetActive(false);
                I.game.gameObject.SetActive(false);
                I.title.gameObject.SetActive(true);
            break;
            case "Main":
                I.main.gameObject.SetActive(true);
                I.game.gameObject.SetActive(false);
                I.title.gameObject.SetActive(false);
            break;
            case "Game":
                I.main.gameObject.SetActive(false);
                I.game.gameObject.SetActive(true);
                I.title.gameObject.SetActive(false);
            break;
            case "Dungeon":
                I.main.gameObject.SetActive(false);
                I.game.gameObject.SetActive(true);
                I.title.gameObject.SetActive(false);
            break;
        }
    }

    public static void SmallNotice(string key){
        SmallNotice(key,null);
    }
    public static void SmallNotice(string key,object[] argu){
        if(I.smallNoticeCount >= 5) return;
        SmallNotice notice = Pool.GetObject<SmallNotice>("SmallNotice");
        //notice.transform.SetParent(I.SmallNoticePosition);
        notice.transform.SetParent(I.popupCanvas.transform);
        notice.transform.position = I.SmallNoticePosition.position;
        notice.content = Util.LocalStr("SmallNotice",key,argu);
        //notice.gameObject.SetActive(true);
        notice.InitObject();
    }

    
}