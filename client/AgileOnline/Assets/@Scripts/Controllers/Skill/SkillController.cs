using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillController : BaseController
{
    private CreatureController _owner;

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
