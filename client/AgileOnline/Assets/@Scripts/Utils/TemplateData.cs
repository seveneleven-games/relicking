using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "TemplateData", menuName = "Custom/TemplateData", order = 1)]
public class TemplateData : ScriptableObject
{
    public int[] TemplateIds;
    public int TempNodeNum = 0;

    public event Action<int> OnSelectedClassIdChanged;
    private int selectedClassId = 1;
    public int SelectedClassId
    {
        get => selectedClassId;
        set
        { 
            if (selectedClassId != value)
            {
                selectedClassId = value;
                OnSelectedClassIdChanged?.Invoke(selectedClassId);
            }
        }
    }
    public int SelectedRelicId = 0;

}
