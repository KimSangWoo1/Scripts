using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HitBlinking : MonoBehaviour
{
    private Image redHit;

    private Color red;
    private float blinkTime;
    private bool blink;
    void Start()
    {
        redHit = GetComponent<Image>();
        red = new Color(1f, 0f, 0f, 0f);
        redHit.color = red;
        blinkTime = 0f;
        blink = false;       
    }

    // Update is called once per frame
    void Update()
    {
        if (blink)
        {
            blinkTime += Time.deltaTime *0.5f;
            redHit.color = new Color(1f, 0f, 0f, blinkTime);
            if (blinkTime >= 0.1f)
            {
                blinkTime = 0;
                blink = false;
                redHit.color = red;
            }
        }    
    }

    public void Blinking(bool _blink)
    {
        blink = _blink;
    }

}
