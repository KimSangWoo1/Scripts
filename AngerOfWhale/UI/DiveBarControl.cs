using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DiveBarControl : MonoBehaviour
{
    // Component
    public Transform target;
    public Image gage; //게이지 이미지

    // Variable
    [Range(0f, 1f)]
    public float diveTimeSpeed = 0.5f; //잠수 시간 속도
    [Range(250, 350)]
    public float height = 300f;

    private float diveBarTime; //잠수 시간

    private bool dive; // 잠수 체크

    // Property
    public bool Diving { get { return dive; } }

    private void OnEnable()
    {
        //초기화
        dive = true;
        gage.fillAmount = 1f;
        diveBarTime = 1f;
        StartCoroutine(GageOver());
    }

    private void OnDisable()
    {
        //초기화 + 코루틴 끄기
        dive = false;
        StopCoroutine(GageOver());
    }
    private void Update()
    {
        Vector3 playerPosition = Camera.main.WorldToScreenPoint(target.transform.position);
        transform.position = playerPosition + new Vector3(0f, height, 0f);
            
    }
    // 잠수 게이지 코루틴
    IEnumerator GageOver()
    {
        while (diveBarTime >= 0f)
        {
            diveBarTime -= Time.deltaTime * diveTimeSpeed;
            gage.fillAmount = diveBarTime;
            yield return null;
        }
        OffDiveBar();
    }
    
    public void OnDiveBar()
    {
        gameObject.SetActive(true);
    }

    public void OffDiveBar()
    {
        gameObject.SetActive(false);
    }
}
