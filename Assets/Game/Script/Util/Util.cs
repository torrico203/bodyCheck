//BOMIN
using UnityEngine;
using System.Globalization;
using UnityEngine.Localization;
using UnityEngine.Localization.Components;
using UnityEngine.Localization.Settings;
using System;
using TMPro;
using UnityEngine.Localization.SmartFormat.PersistentVariables;

public static class Util {

    // 누적 확률 테이블 (CDF)
    public static readonly float[] cumulativeProbabilities = new float[]
    {
        0.0229f, 0.0458f, 0.0687f, 0.0916f, 0.1145f, 0.1374f, 0.1603f, 0.1832f, 0.2061f, 0.229f,
        0.2519f, 0.2733f, 0.2947f, 0.3161f, 0.3375f, 0.3589f, 0.3803f, 0.4017f, 0.4231f, 0.4445f,
        0.4659f, 0.4836f, 0.5013f, 0.519f, 0.5366f, 0.5542f, 0.5718f, 0.5894f, 0.607f, 0.6246f,
        0.6422f, 0.6561f, 0.67f, 0.6839f, 0.6978f, 0.7117f, 0.7256f, 0.7395f, 0.7534f, 0.7673f,
        0.7812f, 0.7913f, 0.8014f, 0.8115f, 0.8216f, 0.8317f, 0.8418f, 0.8519f, 0.862f, 0.8721f,
        0.8822f, 0.8885f, 0.8948f, 0.9011f, 0.9074f, 0.9137f, 0.92f, 0.9263f, 0.9326f, 0.9389f,
        0.9452f, 0.9478f, 0.9504f, 0.953f, 0.9556f, 0.9582f, 0.9607f, 0.9632f, 0.9657f, 0.9682f,
        0.9707f, 0.972f, 0.9733f, 0.9745f, 0.9757f, 0.9769f, 0.9781f, 0.9793f, 0.9805f, 0.9817f,
        0.9827f, 0.9837f, 0.9847f, 0.9857f, 0.9867f, 0.9877f, 0.9887f, 0.9897f, 0.9907f, 0.9917f,
        0.9927f, 0.9935f, 0.9943f, 0.9951f, 0.9958f, 0.9965f, 0.9972f, 0.9979f, 0.9986f, 0.9993f,
        1f
    };

    public static float randomKey = 0.9163f;
    public static T DeepCopy<T>(T obj){
        string json = JsonUtility.ToJson(obj);
        return JsonUtility.FromJson<T>(json);
    }

    public static float GenerateQuality()
    {
        float r = UnityEngine.Random.value;

        for (int i = 0; i < cumulativeProbabilities.Length; i++)
        {
            if (r <= cumulativeProbabilities[i])
            {
                return i/100f;
            }
        }

        return 0f; // fallback (정상적이면 절대 도달하지 않음)
    }
    public static float QualityChance(int currentQuality)
    {
        if (currentQuality < 0) return 1f;
        if (currentQuality >= 100) return 0f;

        return 1f - cumulativeProbabilities[currentQuality];
    }

    // public static int GetExp(int level){ //해당 레벨이 경험치가 몇 이상이어야 하는지
    //     //return (5*(int)Mathf.Pow((level-1),2)+5*(level-1));
    //     return ((level-1)*((level-1)+5))/2;
    // }
    // public static int GetLevel(int exp){ //해당 경험치가 어느 레벨에 해당하는지
    //     //return Mathf.FloorToInt(Mathf.Sqrt(25+20*exp)-5)/10+1; 
    //     return Mathf.FloorToInt(Mathf.Sqrt(25+8*exp)-5)/2+1; 
    // }
    public static int GetExp(int level)
    {
        return ((level+1)*level/2-1)*10;
    }
    public static int GetLevel(int exp)
    {
        //return Mathf.FloorToInt(Mathf.Sqrt(exp + 1));
        return Mathf.FloorToInt((-1f + Mathf.Sqrt(1f + 8f * (exp/10 + 1))) / 2f);
    }

    public static int GetGameExp(int level){ 
        return (5*(int)Mathf.Pow((level-1),2)+5*(level-1));
    }
    public static int GetGameLevel(int exp){ 
        return Mathf.FloorToInt(Mathf.Sqrt(25+20*exp)-5)/10+1;  
    }
    public static string StatToExplain(Stat stat){
        string text = "";
        for(int i=0;i<stat.Count();i++){
            if(stat[i] != 0){
                text += Util.LocalStr("Stat",((Stat.Type)i).ToString());
                
                string valueStr = Util.StatToString(stat[i],(Stat.Type)i);

                if(stat[i] > 0){
                    text += " " +Util.LocalStr("UI","increase",new[]{new{value = (valueStr)}});
                }
                else{
                    text += " " +Util.LocalStr("UI","decrease",new[]{new{value = (valueStr)}});
                }
                if(i!=stat.Count()-1) text+="\n";
            }
        }
        return text;
    }
    public static string StatToString(Stat stat,Stat.Type type,bool abs = false){
        return StatToString(stat[(int)type],type,abs);
    }
    public static string StatToString(float stat,Stat.Type type,bool abs = false){
        string value = "0";
        if(abs) stat = Mathf.Abs(stat);
        if(Info.IntStat[(int)type] != 0)
            value = Mathf.RoundToInt(stat).ToString();
        else if(Info.FloatStat[(int)type] != 0)
            value = (Mathf.Round(stat*100f)/100f).ToString("F2");
        else
            value = (Mathf.Round(stat*100f)).ToString()+"%";
        return value;
    }

    public static int FixedRandom(int min, int max, int seed = 1){
        int val = (int)(Mathf.PI*Util.randomKey*seed)&int.MaxValue;
        //Debug.Log(val);
        int result = (val % (max-min+1)) + min;
        return result;
    }

    public static Deal CalcDeal(DealStat dealStat, Stat stat){
        Deal deal = new Deal();
        
        return deal;
    }

    public static string ColorToHex(Color color, bool includeAlpha = false)
    {
        Color32 color32 = color;
        if (includeAlpha)
            return $"#{color32.r:X2}{color32.g:X2}{color32.b:X2}{color32.a:X2}";
        else
            return $"#{color32.r:X2}{color32.g:X2}{color32.b:X2}";
    }


    public static string GetThousandCommaText(int data)
    {
        return data.ToString("N0", CultureInfo.InvariantCulture);
    }
    public static string LocalStr(string type, string str, object[] argu = null){
        LocalizedString ls = new LocalizedString{
            TableReference = type,
            TableEntryReference = str
        };
        
        var op = ls.GetLocalizedStringAsync(argu);
        op.WaitForCompletion();
        if (op.IsDone) return op.Result;
        else return "";
    }

    public static long GetCurrentTimeStamp()
    {
        DateTime dt = DateTime.Now;
        return ((DateTimeOffset)dt).ToUnixTimeSeconds();
    }

    public static Vector2 Rotate(Vector2 v, float degrees)
    {
        float radians = degrees * Mathf.Deg2Rad;
        float sin = Mathf.Sin(radians);
        float cos = Mathf.Cos(radians);

        float x = v.x * cos - v.y * sin;
        float y = v.x * sin + v.y * cos;

        return new Vector2(x, y);
    }

    public static float GetAngle(Vector2 dir)
    {
        float angle = Mathf.Atan2(-dir.x, dir.y) * Mathf.Rad2Deg;
        return (angle + 360) % 360; // 0~360도 범위로 정규화
    }

    public static long DateTimeToTimeStamp(DateTime value)
    {
        return ((DateTimeOffset)value).ToUnixTimeSeconds();
    }

    public static DateTime TimeStampToDateTime(long value)
    {
        DateTime dt = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
        dt = dt.AddSeconds(value).ToLocalTime();
        return dt;
    }

    public static string FormatNumber(long num)
    {
        if (num >= 1000000)
        {
            return (num / 1000000f).ToString("F1") + "M"; 
        }
        else if (num >= 1000)
        {
            return (num / 1000f).ToString("F1") + "K"; 
        }
        else
        {
            return num.ToString();
        }
    }
    
    public static void SetLocalizedText(TextMeshProUGUI textComponent, string type, string str, object[] argu = null){
        if(textComponent == null) return;
        
        LocalizedString ls = new LocalizedString{
            TableReference = type,
            TableEntryReference = str
        };
        
        var op = ls.GetLocalizedStringAsync(argu);
        op.Completed += (handle) => {
            if(handle.IsDone){
                textComponent.text = handle.Result;
            }
        };
    }
}