using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Damage : MonoBehaviour
{
    public TextMeshPro text;
    public float moveSpeed;
    public float alphaSpeed;
    public float destroyTime;
    private Color alpha;
    public int damage;

    private void OnEnable()
    {
        text = GetComponent<TextMeshPro>();
        alpha = text.color;
        Invoke("ReturnToPool", destroyTime);
    }

    private void Start()
    {
        text.text = damage.ToString();
    }

    private void Update()
    {
        transform.Translate(new Vector3(0, moveSpeed * Time.deltaTime, 0));
        alpha.a = Mathf.Lerp(alpha.a, 0, Time.deltaTime * alphaSpeed);
        text.color = alpha;
    }

    private void ReturnToPool()
    {
        Managers.Pool.Push(gameObject);
    }

    private void OnDisable()
    {
        // 풀에 반환될 때 텍스트와 알파값 초기화
        text.text = "";
        alpha.a = 1;
        text.color = alpha;
    }
}