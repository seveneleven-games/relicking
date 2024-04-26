using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Define;

public class SkillController : BaseController
{
    public ESkillType SkillType { get; protected set; } = ESkillType.None;
    
    public override bool Init()
    {
        if (base.Init() == false)
            return false;

        ObjectType = EObjectType.Skill;
        return true;
    }
    
    #region Destroy

    private Coroutine _coDestroy;

    public void StartDestroy(float delaySeconds)
    {
        StopDestroy();
        _coDestroy = StartCoroutine(CoDestroy(delaySeconds));
    }

    public void StopDestroy()
    {
        if (_coDestroy != null)
        {
            StopCoroutine(_coDestroy);
            _coDestroy = null;
        }
    }

    IEnumerator CoDestroy(float delaySeconds)
    {
        yield return new WaitForSeconds(delaySeconds);

        if (this.IsValid())
        {
            Managers.Object.Despawn(this);
        }
    }

    #endregion
}
