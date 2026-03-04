//BOMIN
#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using System.Collections;

//차후에 테스트 용으로 사용할 수 있음
public class EditorUtil : MonoBehaviour
{

    [MenuItem("PlayerPrefs/Delete All")]
    static void DeletePlayerPrefs()
    {
        PlayerPrefs.DeleteAll();
        Debug.Log("All PlayerPrefs deleted");
    }

    [MenuItem("PlayerPrefs/Save All")]
    static void SavePlayerPrefs()
    {
        PlayerPrefs.Save();
        Debug.Log("All PlayerPrefs saved");
    }

    [MenuItem("Tools/Sprite/Set Pivot to Center")]
    static void SetPivotToCenter()
    {
        Object[] selectedObjects = Selection.objects;
        if (selectedObjects.Length == 0)
        {
            Debug.LogWarning("스프라이트를 선택해주세요.");
            return;
        }

        foreach (Object obj in selectedObjects)
        {
            if (obj is Texture2D texture)
            {
                string path = AssetDatabase.GetAssetPath(texture);
                TextureImporter importer = AssetImporter.GetAtPath(path) as TextureImporter;
                
                if (importer != null)
                {
                    importer.spritePivot = new Vector2(0.5f, 0f);
                    importer.SaveAndReimport();
                }
            }
        }
        
        AssetDatabase.Refresh();
        Debug.Log("선택된 스프라이트들의 pivot이 중앙으로 설정되었습니다.");
    }

    [MenuItem("Tools/Sprite/Set Pivot to Bottom Center")]
    static void SetPivotToBottomCenter()
    {
        Object[] selectedObjects = Selection.objects;
        if (selectedObjects.Length == 0)
        {
            Debug.LogWarning("스프라이트를 선택해주세요.");
            return;
        }

        foreach (Object obj in selectedObjects)
        {
            if (obj is Texture2D texture)
            {
                string path = AssetDatabase.GetAssetPath(texture);
                TextureImporter importer = AssetImporter.GetAtPath(path) as TextureImporter;
                
                if (importer != null)
                {
                    importer.spritePivot = new Vector2(0.5f, 0f);
                    importer.SaveAndReimport();
                }
            }
        }
        
        AssetDatabase.Refresh();
        Debug.Log("선택된 스프라이트들의 pivot이 하단 중앙으로 설정되었습니다.");
    }

    [MenuItem("Tools/Material/Set Shader to CartoonCoffee/Particle Additive &q")]
    static void SetShaderToCartoonCoffeeParticleAdditive()
    {
        Object[] selectedObjects = Selection.objects;
        if (selectedObjects.Length == 0)
        {
            Debug.LogWarning("머티리얼을 선택해주세요.");
            return;
        }

        Shader targetShader = Shader.Find("CartoonCoffee/Particle Additive");
        if (targetShader == null)
        {
            Debug.LogError("Shader 'CartoonCoffee/Particle Additive'를 찾을 수 없습니다.");
            return;
        }

        int changedCount = 0;
        foreach (Object obj in selectedObjects)
        {
            if (obj is Material mat)
            {
                // 기존 텍스처 저장
                var mainTex = mat.HasProperty("_MainTex") ? mat.GetTexture("_MainTex") : null;
                var color = mat.HasProperty("_Color") ? mat.GetColor("_Color") : Color.white;

                Undo.RecordObject(mat, "Change Shader");
                mat.shader = targetShader;

                // 새 셰이더에도 _MainTex, _Color가 있으면 복원
                if (mat.HasProperty("_MainTex"))
                    mat.SetTexture("_MainTex", mainTex);
                if (mat.HasProperty("_Color"))
                    mat.SetColor("_Color", color);

                EditorUtility.SetDirty(mat);
                changedCount++;
            }
        }

        if (changedCount > 0)
        {
            AssetDatabase.SaveAssets();
            Debug.Log($"{changedCount}개의 머티리얼의 셰이더가 변경되었습니다.");
        }
        else
        {
            Debug.LogWarning("선택한 오브젝트 중 머티리얼이 없습니다.");
        }
    }

    [MenuItem("Tools/Material/Set ParticleSystem Materials Shader to CartoonCoffee/Particle Additive &w")]
    static void SetParticleSystemMaterialsShaderToCartoonCoffeeParticleAdditive()
    {
        Object[] selectedObjects = Selection.objects;
        if (selectedObjects.Length == 0)
        {
            Debug.LogWarning("게임 오브젝트를 선택해주세요.");
            return;
        }

        Shader targetShader = Shader.Find("CartoonCoffee/Particle Additive");
        if (targetShader == null)
        {
            Debug.LogError("Shader 'CartoonCoffee/Particle Additive'를 찾을 수 없습니다.");
            return;
        }

        int changedCount = 0;
        foreach (Object obj in selectedObjects)
        {
            GameObject go = null;
            if (obj is GameObject)
                go = obj as GameObject;
            else if (obj is Component)
                go = (obj as Component).gameObject;
            if (go == null) continue;

            // 모든 자식 포함 파티클 시스템 찾기
            var particleSystems = go.GetComponentsInChildren<ParticleSystem>(true);
            foreach (var ps in particleSystems)
            {
                var renderer = ps.GetComponent<ParticleSystemRenderer>();
                if (renderer == null) continue;
                var mats = renderer.sharedMaterials;
                bool changed = false;
                for (int i = 0; i < mats.Length; i++)
                {
                    var mat = mats[i];
                    if (mat == null) continue;
                    // 기존 텍스처/컬러 저장
                    var mainTex = mat.HasProperty("_MainTex") ? mat.GetTexture("_MainTex") : null;
                    var color = mat.HasProperty("_Color") ? mat.GetColor("_Color") : Color.white;

                    Undo.RecordObject(mat, "Change Shader");
                    mat.shader = targetShader;
                    if (mat.HasProperty("_MainTex"))
                        mat.SetTexture("_MainTex", mainTex);
                    if (mat.HasProperty("_Color"))
                        mat.SetColor("_Color", color);
                    EditorUtility.SetDirty(mat);
                    changed = true;
                    changedCount++;
                }
                if (changed)
                    AssetDatabase.SaveAssets();
            }
        }
        if (changedCount > 0)
        {
            Debug.Log($"{changedCount}개의 파티클 머티리얼의 셰이더가 변경되었습니다.");
        }
        else
        {
            Debug.LogWarning("선택한 오브젝트의 파티클 시스템에서 변경된 머티리얼이 없습니다.");
        }
    }

    [MenuItem("Tools/Projectile/Sync dealDuration with duration")]
    static void SyncDealDurationWithDuration()
    {
        Object[] selectedObjects = Selection.objects;
        if (selectedObjects.Length == 0)
        {
            Debug.LogWarning("Projectile 프리팹을 선택해주세요.");
            return;
        }

        int changedCount = 0;
        foreach (Object obj in selectedObjects)
        {
            GameObject go = null;
            if (obj is GameObject)
                go = obj as GameObject;
            else if (obj is Component)
                go = (obj as Component).gameObject;
            if (go == null) continue;

            // 프리팹인지 확인
            var prefabType = PrefabUtility.GetPrefabAssetType(go);
            if (prefabType == PrefabAssetType.NotAPrefab) continue;

            var projectile = go.GetComponent<Projectile>();
            if (projectile != null)
            {
                Undo.RecordObject(projectile, "Sync dealDuration with duration");
                var so = new SerializedObject(projectile);
                var durationProp = so.FindProperty("duration");
                var dealDurationProp = so.FindProperty("dealDuration");
                if (durationProp != null && dealDurationProp != null)
                {
                    dealDurationProp.floatValue = durationProp.floatValue;
                    so.ApplyModifiedProperties();
                    EditorUtility.SetDirty(projectile);
                    changedCount++;
                }
            }
        }
        if (changedCount > 0)
        {
            AssetDatabase.SaveAssets();
            Debug.Log($"{changedCount}개의 Projectile 프리팹의 dealDuration이 duration과 동일하게 변경되었습니다.");
        }
        else
        {
            Debug.LogWarning("선택한 오브젝트 중 Projectile 컴포넌트가 있는 프리팹이 없습니다.");
        }
    }
}
#endif