using System;
using System.Collections;
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

    public static int[] RandomIntList(int length, int min, int max)
    {
        int[] result = new int[length];
        
        List<int> rangePool = new();
        
        for (int i = min; i < max; i++)
        {
            rangePool.Add(i);
        }
        
        for (int j = 0; j < length; j++)
        {
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
        foreach (int skillId  in skillList)
        {
            skillType = skillId / 10;
            if(maxSkillTypes.Contains(skillType)) continue;
            rangePool.Add(skillType);
        }
        
        for (int j = 0; j < length; j++)
        {
            int index = UnityEngine.Random.Range(0, rangePool.Count);
            result[j] = rangePool[index];
            rangePool.RemoveAt(index);
        }
        
        return result;
    }
}