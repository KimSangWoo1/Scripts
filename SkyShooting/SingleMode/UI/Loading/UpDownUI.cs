using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UpDownUI : MonoBehaviour
{
    public bool animationOn;
    public Image[] img;
    private float upTime;

    Vector2 firstPos;
    Vector2 secondPos;
    Vector2 thirdPos;

    float posY;


    private void Awake()
    {
        firstPos = img[0].rectTransform.localPosition;
        secondPos = img[1].rectTransform.localPosition;
        thirdPos = img[2].rectTransform.localPosition;
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
        posY = Mathf.PingPong(upTime, 20f);
        
        img[0].rectTransform.localPosition = new Vector2(img[0].rectTransform.localPosition.x, firstPos.y + posY);
        yield return new WaitForEndOfFrame();

        img[1].rectTransform.localPosition = new Vector2(img[1].rectTransform.localPosition.x, secondPos.y + posY *1.5f);
        yield return new WaitForEndOfFrame();

        img[2].rectTransform.localPosition = new Vector2(img[2].rectTransform.localPosition.x, thirdPos.y + posY *2f);
        yield return new WaitWhile(()=> !animationOn);

    }
    private void OnDisable()
    {
        img[0].rectTransform.localPosition = firstPos;
        img[1].rectTransform.localPosition = secondPos;
        img[2].rectTransform.localPosition = thirdPos;

        animationOn = false;
        upTime = 0f;
    }
}
