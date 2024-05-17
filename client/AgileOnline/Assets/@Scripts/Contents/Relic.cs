using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class Relic
{
    public int key = 0; // 루키스는 string
    public Data.RelicData RelicData;
    bool _isEquipped = false;

    public bool IsEquipped
    {
        get
        {
            // 장비중인 아이템인지 확인
            return _isEquipped;
        }
        set
        {
            _isEquipped = value;
        }
    }

    
    
    // 인게임 관련

    // 생성자!!!!
    public Relic(int key)
    {
        this.key = key;

        // 어떤 Relic이니?
        RelicData = Managers.Data.RelicDic[key];

    }
    
    // 인게임 쪽
    
    
}
