using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


[ExecuteInEditMode]
public class DebugMapPoints : MonoBehaviour
{


    public SettingsScriptable settings;
    public TMPro.TMP_Text distanceT;

    public Transform[] transforms;
    public RectTransform[] points;



    void Update()
    {
        for (int i = 0; i < transforms.Length && i < points.Length; i++)
        {
            points[i].anchoredPosition = new Vector2(transforms[i].localPosition.x, transforms[i].localPosition.z) * settings.pixelsInMeter;
        }

        if (transforms.Length >= 2 && points.Length >= 2)
        {
            distanceT.GetComponent<RectTransform>().anchoredPosition = points[0].anchoredPosition + (points[1].anchoredPosition - points[0].anchoredPosition) / 2;
            distanceT.text = Vector3.Distance(transforms[0].position, transforms[1].position).ToString("0.00");
        }
    }
}
