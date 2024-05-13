using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using static Define;

public static class Extension
{
    public static T GetOrAddComponent<T>(this GameObject go) where T : UnityEngine.Component
    {
        return Util.GetOrAddComponent<T>(go);
    }

    public static void BindEvent(this GameObject go, Action action = null, Action<BaseEventData> dragAction = null, EUIEvent type = EUIEvent.Click)
    {
        UI_Base.BindEvent(go, action, dragAction, type);
    }

    public static bool IsValid(this GameObject go)
    {
        return go != null && go.activeSelf;
    }
    
    public static bool IsValid(this BaseController bc)
    {
        return bc != null && bc.isActiveAndEnabled;
    }

    public static void DestroyChilds(this GameObject go)
    {
        foreach (Transform child in go.transform)
            Managers.Resource.Destroy(child.gameObject);
    }

    public static void TranslateEx(this Transform transform, Vector3 dir)
    {
        BaseController bc = transform.gameObject.GetComponent<BaseController>();
        if (bc != null)
            bc.TranslateEx(dir);
    }

    public static void Shuffle<T>(this IList<T> list)
    {
        int n = list.Count;

        while (n > 1)
        {
            n--;
            int k = UnityEngine.Random.Range(0, n + 1);
            (list[k], list[n]) = (list[n], list[k]); //swap
        }
    }

    public static int[] RandomIntList(int length, int min, int max, HashSet<int> maxSkillTypes)
    {
        int[] result = new int[length];
        Array.Fill(result, -1);
        
        List<int> rangePool = new();
        
        for (int i = min; i <= max; i++)
        {
            if(maxSkillTypes.Contains(i)) continue;
            rangePool.Add(i);
        }
        
        for (int j = 0; j < length; j++)
        {
            if (rangePool.Count == 0) break;
            
            int index = UnityEngine.Random.Range(0, rangePool.Count);
            result[j] = rangePool[index];
            rangePool.RemoveAt(index);
        }
        
        return result;
    }

    public static int[] RandomSkillList(int length, List<int> skillList, HashSet<int> maxSkillTypes)
    {
        
        int[] result = new int[length];
        Array.Fill(result, -1);

        int skillType = 0;
        
        // 딥카피 부분
        List<int> rangePool = new();
        
        Debug.Log("스킬 리스트 반환 함수 들어옴!");
        
        foreach (int skillId  in skillList)
        {
            skillType = skillId / 10;
            if(maxSkillTypes.Contains(skillType)) continue;
            rangePool.Add(skillType);
        }
        
        for (int j = 0; j < length; j++)
        {
            if (rangePool.Count == 0) break;
            
            int index = UnityEngine.Random.Range(0, rangePool.Count);
            // 스킬 풀이 비어서 더 못 꺼내요 ㅋㅋ
            result[j] = rangePool[index];
            rangePool.RemoveAt(index);
        }
        
        return result;
    }

    public static float RoundThird(float value)
    {
        Debug.Log((int)(value * 1000));
        Debug.Log((int)(value * 1000) / 1000f);

        return (int)(value * 1000) / 1000f;
    }
    
}