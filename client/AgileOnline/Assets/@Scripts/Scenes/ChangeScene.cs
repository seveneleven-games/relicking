using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeScene : MonoBehaviour
{
    public Animator transitionAnimator;
    public float transitionTime = 1f;

    // 씬 전환 함수
    public void LoadScene(Define.EScene eScene)
    {
        StartCoroutine(Transition(eScene));
    }

    IEnumerator Transition(Define.EScene eScene)
    {
        // 애니메이션을 플레이합니다.
        transitionAnimator.SetTrigger("StartTransition");

        // 애니메이션 재생 시간만큼 대기합니다.
        yield return new WaitForSeconds(transitionTime);

        // 씬 로드
        Managers.Scene.LoadScene(eScene);
    }
}
