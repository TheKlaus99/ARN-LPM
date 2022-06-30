using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CanvasScalerM : CanvasScaler
{
    [System.Serializable]
    public struct scaleFactors
    {
        public float diagonalInch;
        public float scaleFactor;
    }

    public scaleFactors[] m_scales = {
        new scaleFactors
        {
        diagonalInch = 8,
        scaleFactor = 0.85f
        },
        new scaleFactors
        {
        diagonalInch = 10,
        scaleFactor = 1f
        }
    };

    public float getCurrentScreenScale(int h, int w, float dpi)
    {
        if (m_scales == null || m_scales.Length == 0)
            return 1;

        float dI = Mathf.Sqrt(h * h + w * w) / dpi;
        int i = 0;
        while ((i + 1) < m_scales.Length && m_scales[i].diagonalInch < dI)
        {
            i++;
        }

        return m_scales[i].scaleFactor;
    }

    protected override void HandleConstantPhysicalSize()
    {
        float currentDpi = Screen.dpi;
        float dpi = (currentDpi == 0 ? m_FallbackScreenDPI : currentDpi);
        float targetDPI = 1;
        switch (m_PhysicalUnit)
        {
            case Unit.Centimeters:
                targetDPI = 2.54f;
                break;
            case Unit.Millimeters:
                targetDPI = 25.4f;
                break;
            case Unit.Inches:
                targetDPI = 1;
                break;
            case Unit.Points:
                targetDPI = 72;
                break;
            case Unit.Picas:
                targetDPI = 6;
                break;
        }

        float scale = getCurrentScreenScale(Screen.height, Screen.width, dpi);
        SetScaleFactor(dpi / targetDPI * scale);
        SetReferencePixelsPerUnit(m_ReferencePixelsPerUnit * targetDPI / m_DefaultSpriteDPI / scale);
    }
}
