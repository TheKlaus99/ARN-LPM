using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ForceLayoutRebuilder : MonoBehaviour
{
    RectTransform m_rt;
    RectTransform rt
    {
        get
        {
            if (m_rt == null)
                m_rt = GetComponent<RectTransform>();
            return m_rt;
        }
    }

    private void OnEnable()
    {
        UpdateLayout();
    }

    public void UpdateLayout()
    {
        LayoutRebuilder.ForceRebuildLayoutImmediate(rt);
    }
}
