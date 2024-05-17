using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InitBase : MonoBehaviour
{
    protected bool _init = false;
    protected Vector3 _initialScale;

    public virtual bool Init()
    {
        if (_init)
            return false;

        _init = true;
        _initialScale = transform.localScale;
        return true;
    }

    private void Awake()
    {
        Init();
    }
    
    private void Update()
    {
        UpdateController();
    }

    public virtual void UpdateController()
    {
        
    }
}
