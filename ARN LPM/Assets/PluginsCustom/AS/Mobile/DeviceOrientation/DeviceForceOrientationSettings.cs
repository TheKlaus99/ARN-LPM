using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace AS.DeviceOrientation
{
    [CreateAssetMenu(fileName = "DeviceForceOrientation", menuName = "Settings/DeviceForceOrientation", order = 0)]
    public class DeviceForceOrientationSettings : ScriptableObject
    {
        [Header("Phone")]
        public bool mobileLandscapeLeft;
        public bool mobileLandscapeRight;
        public bool mobilePortrait;
        public bool mobilePortraitUpsideDown;

        [Header("Tablet")]
        public bool tabletLandscapeLeft;
        public bool tabletLandscapeRight;
        public bool tabletPortrait;
        public bool tabletPortraitUpsideDown;

        [Space]
        [Range(0f, 10f)]
        public float tabletInch;
    }
}
