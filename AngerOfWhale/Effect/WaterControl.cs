using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaterControl : MonoBehaviour
{
    public Material mat;

    public Color startColor;
    public Color sunsetColor;
    public Color nightColor;

    private Color currentColor;

    public float colorSpeed;

    public bool morning;
    public bool sunset;
    public bool night;

    private int nextColor;
    private int multiple;
    [SerializeField]
    private int changeColorScore = 2000;

    WaitForSeconds wait = new WaitForSeconds(0.1f);

    Coroutine changeWater;
    private void Start()
    {
        nextColor = 0;
        multiple = 1;

        mat.SetColor("_TopDarkColour", startColor);
    }

    private void LateUpdate()
    {
        CheckScore();

        if (morning)
        {
            currentColor = Color.Lerp(currentColor, startColor, Time.smoothDeltaTime * colorSpeed);
            mat.SetColor("_TopDarkColour", currentColor);
            //mat.SetColor("_TopDarkColour", startColor);
        }
        if (sunset)
        {
            currentColor = Color.Lerp(currentColor, sunsetColor, Time.smoothDeltaTime * colorSpeed);
            mat.SetColor("_TopDarkColour", currentColor);
            //mat.SetColor("_TopDarkColour", sunsetColor);
        }
        if (night)
        {
            currentColor = Color.Lerp(currentColor, nightColor, Time.smoothDeltaTime * colorSpeed);
            mat.SetColor("_TopDarkColour", currentColor);
            //mat.SetColor("_TopDarkColour", nightColor);
        }
    }
    public void CheckScore()
    {
        //색 변경 체크
        if (GameManager.score >= changeColorScore * multiple)
        {
            multiple++;
            nextColor++;

            //StartCoroutine(ChangeWater(nextColor));
        }
    }
    IEnumerator ChangeWater(int type)
    {
        while (true)
        {
            if (type == 0)
            {
                currentColor = Color.Lerp(currentColor, startColor, Time.smoothDeltaTime * colorSpeed);
                mat.SetColor("_TopDarkColour", currentColor);

                if (currentColor == startColor)
                    break;
            }
            else if (type == 1)
            {
                currentColor = Color.Lerp(currentColor, sunsetColor, Time.smoothDeltaTime * colorSpeed);
                mat.SetColor("_TopDarkColour", currentColor);
                Debug.Log("수줏ㅇ");
                if (currentColor == sunsetColor)
                    break;
            }
            else if (type == 2)
            {
                currentColor = Color.Lerp(currentColor, nightColor, Time.smoothDeltaTime * colorSpeed);
                mat.SetColor("_TopDarkColour", currentColor);

                if (currentColor == nightColor)
                    break;
            }
            yield return null;
        }
    }
}
