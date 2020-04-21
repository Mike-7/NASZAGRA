using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Bar : MonoBehaviour
{
    public Image bar;
    int dir = 1;

    void FixedUpdate()
    {
        if(bar.rectTransform.localPosition.x >= -100)
        {
            dir = -1;
        }
        if(bar.rectTransform.localPosition.x <= -400)
        {
            dir = 1;
        }

        bar.rectTransform.localPosition += new Vector3(dir * 30 * Time.deltaTime, 0);
    }
}
