using UnityEngine;
using PlayFab;
using PlayFab.ClientModels;
using System;
using System.Collections.Generic;

public class PlayFabManager : MonoBehaviour
{
    public static PlayFabManager I { get; private set; }

    private bool isLogin = false;
    public static bool IsLogin{
        get{
            return I.isLogin;
        }
    }

    private string id;
    public static string Id{
        get{
            return I.id;
        }
    }

    void Awake()
    {
        if (I == null)
        {
            I = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
    public static void Connect()
    {
        #if UNITY_ANDROID && !UNITY_EDITOR
                string androidId = SystemInfo.deviceUniqueIdentifier;
        #else
                string androidId = "EditorTestDeviceId"; // 에디터에서는 임의의 ID 사용
        #endif

        var request = new LoginWithAndroidDeviceIDRequest
        {
            AndroidDeviceId = androidId,
            CreateAccount = true,
            TitleId = PlayFabSettings.TitleId // 설정되어 있지 않다면 수동 지정 가능
        };
        UI.OpenPopupSimple("NetworkLoad");
        PlayFabClientAPI.LoginWithAndroidDeviceID(request, (result)=>{
            UI.ClosePopupSimple("NetworkLoad");
            Debug.Log("PlayFab 로그인 성공! 플레이어 ID: " + result.PlayFabId);
            I.isLogin = true;
            I.id = result.PlayFabId;
        }, (error)=>{
            UI.ClosePopupSimple("NetworkLoad");
            UI.Confirm(()=>{
                Application.Quit();
            },"PlayFab 로그인 실패: " + error.GenerateErrorReport());
            //Debug.LogError("PlayFab 로그인 실패: " + error.GenerateErrorReport());
        });
    }

    public static void UpdateLeaderboard(string name, int value){
        var request = new UpdatePlayerStatisticsRequest
        {
            Statistics = new List<StatisticUpdate>
            {
                new StatisticUpdate
                {
                    StatisticName = name, // PlayFab의 통계 이름과 일치해야 함
                    Value = value
                }
            }
        };

        PlayFabClientAPI.UpdatePlayerStatistics(request, 
        (result)=>{
            Debug.Log(name + "기록 성공 : "+value.ToString());
        }, 
        (error)=>{
            Debug.Log(name + "기록 실패 : "+error.GenerateErrorReport());
        });
    }

    public static void GetUserData(Action<Dictionary<string,UserDataRecord>> callback = null)
    {
        
        UI.OpenPopupSimple("NetworkLoad");
        var request = new GetUserDataRequest
        {
            // 특정 유저의 데이터를 가져오려면 PlayFabId를 명시적으로 지정할 수 있음
            // PlayFabId = "다른 유저 ID", // 생략하면 현재 로그인한 유저 기준
        };
        PlayFabClientAPI.GetUserData(request, (result)=>{
            
            UI.ClosePopupSimple("NetworkLoad");
            Dictionary<string,UserDataRecord> userData = new Dictionary<string, UserDataRecord>();
            if (result.Data == null || result.Data.Count == 0)
            {
                Debug.Log("유저 데이터가 없습니다.");   
                userData = null;
            }
            else
            {
                userData = result.Data;
            }
            if(callback != null){
                callback(userData);
                callback = null;
            }
        }, (error) => {
            UI.ClosePopupSimple("NetworkLoad");
            UI.Confirm(()=>{
                Application.Quit();
            },"유저 데이터 로드 실패: " + error.GenerateErrorReport());
            //Debug.LogError("유저 데이터 로드 실패: " + error.GenerateErrorReport());
        });
    }

    public static void UpdateUserData(Dictionary<string,string> saveDic){
        var request = new UpdateUserDataRequest
        {
            Data = saveDic
        };

        PlayFabClientAPI.UpdateUserData(request, 
        (result)=>{
            Debug.Log("PlayFab 저장 완료");
        },
        (error) => {
            Debug.LogError("유저 데이터 저장 실패: " + error.GenerateErrorReport());
        });
        
    }



}
