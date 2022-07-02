using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ARUnit
{

    public class ARTrakerStarter : MonoBehaviour
    {
        public GameObject ARKitTracker, ARCoreTracker;
        private void Awake()
        {
            if (ARInterface.isSupport())
            {
                CreateTrackeer();
            }
            else
            {
                Debug.Log("Full AR not support");

            }
            Destroy(this);
        }

#if UNITY_EDITOR
        void CreateTrackeer()
        {
            ARKitTracker.SetActive(true);
        }

#elif UNITY_IOS
        void CreateTrackeer()
        {
            ARKitTracker.SetActive(true);
            //ARCoreTracker.SetActive(true);
        }

#elif UNITY_ANDROID
        void CreateTrackeer()
        {
            ARCoreTracker.SetActive(true);
        }
#endif

    }
}
