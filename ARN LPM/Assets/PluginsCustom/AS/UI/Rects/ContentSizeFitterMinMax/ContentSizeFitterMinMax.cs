using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[AddComponentMenu("Layout/Content Size Fitter MinMax", 142)]
public class ContentSizeFitterMinMax : ContentSizeFitter
{


    [SerializeField]
    public Vector2 minSize, maxSize;

    [NonSerialized]
    private RectTransform m_Rect;
    private DrivenRectTransformTracker m_Tracker;


    private RectTransform rectTransform
    {
        get
        {
            if ((UnityEngine.Object) this.m_Rect == (UnityEngine.Object) null)
                this.m_Rect = this.GetComponent<RectTransform>();
            return this.m_Rect;
        }
    }

    private void HandleSelfFittingAlongAxis(int axis)
    {

        FitMode fitting = (axis == 0 ? horizontalFit : verticalFit);
        if (fitting == FitMode.Unconstrained)
            return;

        m_Tracker.Add(this, rectTransform,
            (axis == 0 ? DrivenTransformProperties.AnchorMaxX : DrivenTransformProperties.AnchorMaxY) |
            (axis == 0 ? DrivenTransformProperties.SizeDeltaX : DrivenTransformProperties.SizeDeltaY));

        // Set anchor max to same as anchor min along axis
        Vector2 anchorMax = rectTransform.anchorMax;
        anchorMax[axis] = rectTransform.anchorMin[axis];
        rectTransform.anchorMax = anchorMax;

        // Set size to min size
        Vector2 sizeDelta = rectTransform.sizeDelta;
        if (fitting == FitMode.MinSize)
            sizeDelta[axis] = Mathf.Min(maxSize[axis], Mathf.Max(minSize[axis], LayoutUtility.GetMinSize(m_Rect, axis)));
        else
            sizeDelta[axis] = Mathf.Min(maxSize[axis], Mathf.Max(minSize[axis], LayoutUtility.GetPreferredSize(m_Rect, axis)));
        rectTransform.sizeDelta = sizeDelta;
    }


    public override void SetLayoutHorizontal()
    {
        this.m_Tracker.Clear();
        this.HandleSelfFittingAlongAxis(0);
    }

    public override void SetLayoutVertical()
    {
        this.HandleSelfFittingAlongAxis(1);
    }


    protected override void OnEnable()
    {
        base.OnEnable();
        this.SetDirty();
    }

    protected override void OnDisable()
    {
        this.m_Tracker.Clear();
        LayoutRebuilder.MarkLayoutForRebuild(this.rectTransform);
        base.OnDisable();
    }

    protected override void OnRectTransformDimensionsChange()
    {
        this.SetDirty();
    }
}
