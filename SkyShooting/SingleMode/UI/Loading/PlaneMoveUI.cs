using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlaneMoveUI : MonoBehaviour
{
    public bool animationOn;

    public Image plane;

    private float upTime;
    float posY;

    Vector2 planePos;

    void Awake()
    {
        planePos = plane.rectTransform.localPosition;
    }
    private void OnEnable()
    {
        animationOn = true;
        upTime = 0f;
    }

    // Update is called once per frame
    void Update()
    {
        upTime += Time.deltaTime * 30f;
        if (animationOn)
        {
            StartCoroutine(UpDownMove());
        }
    }

    IEnumerator UpDownMove()
    {
        posY = Mathf.PingPong(upTime, 100f);

        plane.rectTransform.localPosition = new Vector2(plane.rectTransform.localPosition.x, planePos.y + posY);
        yield return new WaitWhile(() => !animationOn);

    }

    private void OnDisable()
    {
        animationOn = false;
        upTime = 0f;
        plane.rectTransform.localPosition = planePos;
    }
}
