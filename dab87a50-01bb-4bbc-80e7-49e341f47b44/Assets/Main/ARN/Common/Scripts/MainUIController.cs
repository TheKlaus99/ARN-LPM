using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using ARUnit;
using GPSUnit;
using PositionUnit;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class MainUIController : MonoBehaviour
{

    const float zeroMapScaleFactor = 0.76f; //зум карты на котором карта целиком влезает в экран 


    public Canvas UITargetPointCanvas;
    public Transform targetPoint;

    public MapMarkerController mapMarkerController;
    public Animator mapMask, bigMapUI, HelpSuggestion, compassAnim, liftUIAnim;

    [Space]
    public GameObject OpenARButton;
    public GameObject OpenEasyARButton;

    [Space]
    public RectTransform northArrow;
    public RectTransform mapSprite;
    public RectTransform camPreview;
    public Image stopAR;
    public GameObject previewAR;

    [Space]
    public MapController mapController;
    //public NavUnit.PathFinder pathFinder;
    public MapHouseController mapHouseController;
    public FloorSelecter floorSelecter;
    public EasyARArrowRotator easyARArrowRotator;
    public GPSHelpTextController GPSHelpTextController;

    [Header("ARCam")]
    public Camera ARCam;
    public LayerMask camNormal, camHideAR;
    public RenderTexture targetTexture;

    [Header("EasyARCam")]
    public Animator imageDetecterAnim;
    public RawImage easyCameraImage;


    Vector3 lastImagePos = Vector3.zero;
    Vector3 lastGPSPos = Vector3.zero;
    public float lastGPSAcc = 100;
    Vector3 lastEasyARImagePos = Vector3.zero;
    [HideInInspector] public bool isMap = true;
    bool isEasyARMarkerShow = false;
    bool startAR = true;

    static void Show(Animator anim)
    {
        anim.SetTrigger("Show");
    }

    static void Hide(Animator anim)
    {
        anim.SetTrigger("Hide");
    }

    static void HideDeActive(Animator anim)
    {
        anim.SetTrigger("Hide");
        DisableObject.Disable(anim.gameObject, 0.3f);
    }

    bool easyAROpenButtonIsOn = false;
    void EasyAROpenButton(bool isOpen)
    {
        if (easyAROpenButtonIsOn != isOpen)
        {
            easyAROpenButtonIsOn = isOpen;
            bigMapUI.SetBool("EasyAR", isOpen);
        }

    }

    // Use this for initialization
    void Start()
    {
        Application.targetFrameRate = 60;

        SetTargetPoint(targetPoint.localPosition);

        PositionInterface.onStatusChange += OnPosStatusChange;
        GPSInterface.onGPSStatusUpdate += OnGPSStatusUpdate;
        ARInterface.onStatusChange += OnARStatusChange;
        GPSInterface.onGPSUpdate += OnGPSUpdate;
        DeviceChange.OnResolutionChange += OnResolutionChange;
        PositionInterface.onARCameraTramsformUpdate += OnARCameraTramsformUpdate;
        OnResolutionChange(new Vector2(Screen.width, Screen.height));

        mapHouseController.Set(OnTapHome);
        mapController.scrollRectMap.onScale += mapHouseController.OnZoom;

        if (PrivacyAcceser.CheckLocationAccess())
        {
            GPSInterface.StartGPS(1, 1);
            GPSInterface.StartCompass();
        }

        SetStateByPrivacy();

        ARCam.cullingMask = camHideAR;
        mapController.scrollRectMap.onClick.AddListener(OnMapClick);
        mapController.scrollRectMap.onLongClick.AddListener(OnMapLongClick);

        StartCoroutine(UpdateFloorHeightIE());


        if (!ARInterface.isSupport())
        {
            OpenARButton.SetActive(false);
        }
        EasyAROpenButton(true);

        Show(bigMapUI);

    }



    public void SetStateByPrivacy()
    {
        if (PrivacyAcceser.CheckLocationAccess())
        {
            if (GPSInterface.gpsStatus != GPSServiceStatus.Initializing || GPSInterface.gpsStatus != GPSServiceStatus.Running)
            {
                GPSInterface.StartGPS(1, 1);
                GPSInterface.StartCompass();
            }
        }
        else
        {
            if (GPSInterface.gpsStatus == GPSServiceStatus.Initializing || GPSInterface.gpsStatus == GPSServiceStatus.Running)
            {
                GPSInterface.StopCompass();
                GPSInterface.StopGPS();
            }
        }

        if (PrivacyAcceser.CheckCameraAccess())
        {
            OpenEasyARButton.SetActive(true);
            if (ARInterface.isSupport())
            {
                OpenARButton.SetActive(true);
            }
        }
        else
        {
            OpenEasyARButton.SetActive(false);
            OpenARButton.SetActive(false);
        }

    }

    private void OnResolutionChange(Vector2 res)
    {
        if (res.x / res.y > 1)
        {
            camPreview.localScale = new Vector3(res.x / res.y, 1, 1);
            easyCameraImage.GetComponent<RectTransform>().rotation = Quaternion.Euler(0, 0, 0);
        }
        else
        {
            camPreview.localScale = new Vector3(1, res.y / res.x, 1);
            easyCameraImage.GetComponent<RectTransform>().rotation = Quaternion.Euler(0, 0, 180);

        }
    }

    private void OnARStatusChange(ARStatus ARStatus)
    {
        if (ARStatus == ARStatus.Failed || ARStatus == ARStatus.Unsupported)
        {
            Debug.LogError("Петрович, всё говно, вырубай");
        }
        else if (ARStatus == ARStatus.Running)
        {
            if (isMap)
            {
                if (firstStart)
                {
                    firstStart = false;
                    Hide(mapMask);
                    Hide(bigMapUI);
                    bigMapUI.SetBool("floor", false);
                    isMap = false;
                    SuggestDialog(true);
                    mapHouseController.DeSelect(mapController.scrollRectMap.currentZoom < ARNSettings.settings.zoomToShowFloor);
                    //UpdateFloor();
                }
            }
        }
    }

    IEnumerator DisableGO(GameObject go, float t)
    {
        yield return new WaitForSeconds(t);
        go.SetActive(false);
    }

    private void OnGPSStatusUpdate(GPSServiceStatus status)
    {
        if (status == GPSServiceStatus.Failed || status == GPSServiceStatus.Disable)
        {
            Debug.LogError("Петрович, всё говно, вырубай");
        }
    }

    bool isGPSDistanceNormal = false;
    private void OnGPSUpdate(GPSInfo info)
    {


        if (GPSUtility.distance(info, new GPSInfo(ARNSettings.settings.GPSMap.latitude, ARNSettings.settings.GPSMap.longitude, 0)) > ARNSettings.settings.maxDistanceBetweenMapAndGPS)
        {
            isGPSDistanceNormal = false;
            lastGPSAcc = 100;
            GPSHelpTextController.ChangeGPSTextWhenARStarted(-1);
            return;
        }

        isGPSDistanceNormal = true;

        if (isEasyARMarkerShow && info.horizontalAccuracy < ARNSettings.settings.GPSAccuracyToSwitchFromEasyAR)
        {
            isEasyARMarkerShow = false;
            lastEasyARImagePos = Vector3.zero;
        }


        GPSInfo mapInfo = new GPSInfo(ARNSettings.settings.GPSMap.latitude, ARNSettings.settings.GPSMap.longitude, ARNSettings.settings.GPSMap.altitude);
        lastGPSPos = GPSUtility.GPSToVector(mapInfo, info) + ARNSettings.settings.GPSMap.localPos; //координата gps отсносительно карты 
        lastGPSAcc = info.horizontalAccuracy;

        mapController.UpdateGPS(lastGPSPos, info.horizontalAccuracy);

        if (PositionInterface.posStatus == PosStatus.unknown)
        {
            if (!isEasyARMarkerShow)
            {
                if (info.horizontalAccuracy < ARNSettings.settings.GPSAccuracyToHidePath)
                    NavUnit.NavInterface.UpdatePath(lastGPSPos, targetPoint.localPosition);
                else
                    NavUnit.NavInterface.ResetPath();
            }

            if (ARInterface.ARStatus == ARStatus.Running)
            {
                GPSHelpTextController.ChangeGPSTextWhenARStarted(lastGPSAcc);
            }

        }
    }



    private void OnPosStatusChange(PosStatus status)
    {
        if (status == PosStatus.normal)
        {
            SuggestDialog(false);
            mapMarkerController.mainArrowActive = true;

            ARCam.cullingMask = camNormal;
            PositionInterface.onARCameraTramsformUpdate += onARCameraAfterStatusChangeTramsformUpdate;
            if (!isMap)
            {
                mapController.mapView = false;
                UITargetPointCanvas.gameObject.SetActive(true);
                UITargetPointCanvas.sortingOrder = 6;
            }
        }
        else if (status == PosStatus.lost)
        {
            mapMarkerController.mainArrowActive = false;

            ARCam.cullingMask = camHideAR;
            UITargetPointCanvas.gameObject.SetActive(false);
            UITargetPointCanvas.sortingOrder = 4;

            if (!isMap)
            {
                mapController.mapView = true;
            }
        }
        else
        {
            mapMarkerController.mainArrowActive = false;


            ARCam.cullingMask = camHideAR;
            UITargetPointCanvas.gameObject.SetActive(false);
            UITargetPointCanvas.sortingOrder = 4;

            if (!isMap)
            {
                mapController.mapView = true;
            }
        }
    }

    private void onARCameraAfterStatusChangeTramsformUpdate(Vector3 position, Quaternion rotation)
    {
        PositionInterface.onARCameraTramsformUpdate -= onARCameraAfterStatusChangeTramsformUpdate;
        NavUnit.NavInterface.UpdatePath(position, targetPoint.localPosition);
    }

    void OnARCameraTramsformUpdate(Vector3 pos, Quaternion rot)
    {
        mapController.UpdateTransform(pos, rot);
    }


    bool firstStart = true;
    void StartAR()
    {
        GPSHelpTextController.ChangeGPSTextWhenARStarted(isGPSDistanceNormal ? lastGPSAcc : -1);
        EasyAROpenButton(false);
        if (EasyARInterface.ARStatus == ARStatus.Initializing || EasyARInterface.ARStatus == ARStatus.Running)
        {
            OnTapCloseEasyAR();
        }
        firstStart = true;
        PositionInterface.ResetPosition();
        ARInterface.StartARSession();
#if UNITY_EDITOR
        StartCoroutine(DebugStart());
#endif
    }
#if UNITY_EDITOR
    IEnumerator DebugStart()
    {
        yield return new WaitForSeconds(0.1f);
        ARInterface.ChangeStatus(ARStatus.Running);
        yield return new WaitForSeconds(0.5f);
        PositionInterface.ChangeArea(Area.outDoor);
        yield return new WaitForSeconds(0.1f);
        PositionInterface.ChangeStatus(PosStatus.normal);
    }
#endif

    bool isShow = false;
    void SuggestDialog(bool show)
    {

        if (isShow == show)
            return;

        if (show)
        {
            if (!isMap)
            {
                Show(HelpSuggestion);
                isShow = true;
            }
        }
        else
        {
            Hide(HelpSuggestion);
            isShow = false;
        }

    }

    bool inLift = false;
    public void OnLiftTriggerEnter()
    {
        if (!inLift)
        {
            inLift = true;
            liftUIAnim.gameObject.SetActive(true);
            PositionInterface.ChangeStatus(PosStatus.lost);
            ARToEstimate.onImageTrackUpdate += OnImageTrackUpdate;
        }
    }

    private void OnImageTrackUpdate(ARImage ARImage, float p)
    {
        ARToEstimate.onImageTrackUpdate -= OnImageTrackUpdate;
        if (inLift)
        {
            inLift = false;
            HideDeActive(liftUIAnim);
            ARInterface.ReStartARSession();
            PositionInterface.ResetPosition();
        }
    }

    public void OnARTap()
    {
        if (isMap && openMap == null)
        {
            ARCam.targetTexture = null;
            if (ARInterface.ARStatus != ARStatus.Running)
            {
                StartAR();
            }
            else
            {
                Hide(mapMask);
                Hide(bigMapUI);
                isMap = false;
                bigMapUI.SetBool("floor", false);
                mapHouseController.DeSelect();
                //mapHouseController.DeSelect(PositionInterface.area == Area.outDoor);
                UpdateFloor();
                if (PositionInterface.posStatus == PosStatus.normal)
                {
                    mapController.mapView = false;
                    UITargetPointCanvas.gameObject.SetActive(true);
                    UITargetPoint.UITargetPointInst.ChangeSortingOrder(6, 0.3f);
                }
            }
        }
    }

    public void OnMapClick(Vector2 screenPos)
    {
        if (!isMap)
        {
            Show(mapMask);
            Show(bigMapUI);
            isMap = true;
            mapController.mapView = true;
            UITargetPointCanvas.sortingOrder = 4;
            if (openMap == null)
            {
                openMap = StartCoroutine(OpenMapIE());
            }
        }
        else
        {
            HouseItem house = mapHouseController.HitInHouse(screenPos);
            if (!house)
            {
                mapHouseController.HideFloor();
                bigMapUI.SetBool("floor", false);
            }
            else
            {
                mapHouseController.OnTapHouse(house);
            }
        }
    }

    private void OnMapLongClick(Vector2 screenPos)
    {
#if UNITY_EDITOR
        if (isMap)
#else
            if (isMap && Input.touchCount == 1)
#endif
        {
            float y = 0;
            HouseItem house = mapHouseController.HitInHouse(screenPos);
            if (house)
            {
                y = house.currentFloor.height + .3f;
            }
            mapMarkerController.UpdateTargetPos(screenPos);
            RectTransformUtility.ScreenPointToLocalPointInRectangle(mapController.mapUIRT, screenPos, null, out screenPos);
            SetTargetPoint(new Vector3(screenPos.x / ARNSettings.settings.pixelsInMeter.x, y, screenPos.y / ARNSettings.settings.pixelsInMeter.y));

        }
    }

    public void OnPlusTap()
    {
        if (zoom == null) zoom = StartCoroutine(ZoomIE(mapController.scrollRectMap.currentZoom * 1.4f));
    }

    public void OnMinusTap()
    {
        if (zoom == null) zoom = StartCoroutine(ZoomIE(mapController.scrollRectMap.currentZoom / 1.4f));
    }

    public void OnTapToMe()
    {
        if (focus == null)
        {
            if (PositionInterface.posStatus == PosStatus.normal)
            {
                focus = StartCoroutine(FocusMapIE(mapController.camRT.position));
            }
            else if (isGPSDistanceNormal && GPSInterface.gpsStatus == GPSServiceStatus.Running)
            {
                focus = StartCoroutine(FocusMapIE(mapController.GPSPointMover.GetComponent<RectTransform>().position));
            }
            else
            {
                focus = StartCoroutine(FocusMapIE(mapController.mapUIRT.position));

            }
        }
    }

    bool easyARStatusChangeSubscribes = false;
    bool easyARImagesSubscribes = false;
    public void OnTapOpenEasyAR()
    {
        FixEasyAR();
        imageDetecterAnim.SetTrigger("Open");
        imageDetecterAnim.SetBool("HideFrame", false);
        easyCameraImage.enabled = false;
        easyARStatusChangeSubscribes = true;
        EasyARInterface.onStatusChange += OnEasyARStatusChange;
        EasyARInterface.StartEasyAR();
    }

    void FixEasyAR()
    {
#if UNITY_ANDROID && !UNITY_EDITOR
        easyCameraImage.GetComponent<RectTransform>().rotation = Quaternion.Euler(0, 0, 180);
#endif
    }

    private void OnEasyARStatusChange(ARStatus ARStatus)
    {

        if (ARStatus == ARStatus.Running)
        {
            easyARStatusChangeSubscribes = false;
            EasyARInterface.onStatusChange -= OnEasyARStatusChange;
            easyCameraImage.enabled = true;
            easyARImagesSubscribes = true;
            EasyARInterface.onImageAdd += OnEasyARImageAdd;
            EasyARInterface.onImageRemoved += OnEasyARImageRemove;
        }
    }

    private void OnEasyARImageRemove(ARImage ARImage)
    {
        imageDetecterAnim.SetBool("HideFrame", false);
    }

    private void OnEasyARImageAdd(ARImage ARImage)
    {
        SetEasyARTarget(ARImage.name);
        imageDetecterAnim.SetBool("HideFrame", true);
    }

    void SetEasyARTarget(string name)
    {
        ARMap.ARImageTransform imageAnchor = ARNSettings.settings.ARMap.imageAnchors.ToList().Find(x => x.name == name);

        easyARArrowRotator.Set(imageAnchor);
        if (imageAnchor == null)
            return;

        lastEasyARImagePos = imageAnchor.position;
        HouseItem house = mapHouseController.IsPointInHouse(new Vector2(lastEasyARImagePos.x, lastEasyARImagePos.z) * ARNSettings.settings.pixelsInMeter);
        if (house)
        {
            house.ChangeFloor(house.GetFloorIDByHeight(lastEasyARImagePos.y));
            floorSelecter.Set(house, house.ChangeFloor);
        }

        mapMarkerController.UpdateEasyARPos(lastEasyARImagePos);
        isEasyARMarkerShow = true;

        if (focus == null)
        {
            focus = StartCoroutine(FocusMapIE(mapMarkerController.easyARPosAnim.GetComponent<RectTransform>().position, 3));
        }
        if (zoom == null && mapController.scrollRectMap.currentZoom < ARNSettings.settings.zoomToShowFloor)
        {
            zoom = StartCoroutine(ZoomIE(ARNSettings.settings.zoomToShowFloor + .3f, false, 3));
        }

        NavUnit.NavInterface.UpdatePath(lastEasyARImagePos, targetPoint.localPosition);

    }

    public void OnTapCloseEasyAR()
    {
        imageDetecterAnim.SetTrigger("Close");

        if (easyARStatusChangeSubscribes)
        {
            easyARStatusChangeSubscribes = false;
            EasyARInterface.onStatusChange -= OnEasyARStatusChange;

        }

        if (easyARImagesSubscribes)
        {
            easyARImagesSubscribes = false;
            EasyARInterface.onImageAdd -= OnEasyARImageAdd;
            EasyARInterface.onImageRemoved -= OnEasyARImageRemove;
        }
        EasyARInterface.StopEasyAR();
    }

    float timeDown = 0;
    public void OnPointerDownAR()
    {
        timeDown = Time.time;
        if (!firstStart)
        {
            if (StopARUI == null)
            {
                StopARUI = StartCoroutine(StopARUIIE());
            }
        }
    }

    public void OnPointerUpAR()
    {
        if (StopARUI != null)
        {
            StopCoroutine(StopARUI);
            StopARUI = null;
            stopAR.fillAmount = 0;
        }

        if (Time.time - timeDown < 0.3f)
        {
            OnARTap();
        }

    }

    public void OnTapHome(HouseItem house)
    {
        bigMapUI.SetBool("floor", true);
        floorSelecter.Set(house, house.ChangeFloor);
    }

    public void SetTargetPoint(Vector3 pos)
    {
        mapController.SetTarget(pos);
        targetPoint.localPosition = pos;
        if (PositionInterface.posStatus != PosStatus.normal)
        {
            if (isEasyARMarkerShow)
            {
                NavUnit.NavInterface.UpdatePath(lastEasyARImagePos, targetPoint.localPosition);
            }
            else
            {
                if (lastGPSPos != Vector3.zero && lastGPSAcc < ARNSettings.settings.GPSAccuracyToHidePath)
                {
                    NavUnit.NavInterface.UpdatePath(lastGPSPos, targetPoint.localPosition);
                }
                else
                {
                    NavUnit.NavInterface.ResetPath();
                }
            }
        }
        else if (PositionInterface.posStatus == PosStatus.normal)
        {
            NavUnit.NavInterface.UpdatePathBetweenTransform();
        }
    }

    public void OnTapSearch()
    {
        Search.SearchUIController.Open(ARNSettings.settings.searchData, SearchCollback, null);
    }

    void SearchCollback(Search.SearchSer searchSer)
    {
        if (lastGPSPos != Vector3.zero || PositionInterface.posStatus == PosStatus.normal)
        {
            NavUnit.NavInterface.onPathFound += OnPathFound;
        }
        SetTargetPoint(searchSer.pos);
        Vector2 screenPos = new Vector2(searchSer.pos.x * ARNSettings.settings.pixelsInMeter.x, searchSer.pos.z * ARNSettings.settings.pixelsInMeter.y);
        HouseItem house = mapHouseController.IsPointInHouse(screenPos);
        mapMarkerController.UpdateTargetPos(screenPos, searchSer.name);

        mapMarkerController.targetHouseInfo.house = house;
        if (house)
        {
            int id = house.GetFloorIDByHeight(searchSer.pos.y);
            if (id != house.selectedFloorID)
            {
                house.ChangeFloor(id);
            }
            mapMarkerController.targetHouseInfo.floorID = id;
            mapHouseController.currentHouse = null;
            mapHouseController.OnTapHouse(house);
        };
        FocusMap(house, screenPos);
    }

    private void OnPathFound(Vector3[] points)
    {
        NavUnit.NavInterface.onPathFound -= OnPathFound;

        if (points.Length == 0)
            return;

        float minX = points[0].x;
        float minZ = points[0].x;
        float maxX = points[0].x;
        float maxZ = points[0].z;

        foreach (var item in points)
        {
            minX = Mathf.Min(minX, item.x);
            minZ = Mathf.Min(minZ, item.z);
            maxX = Mathf.Max(maxX, item.x);
            maxZ = Mathf.Max(maxZ, item.z);
        }

        Vector2 pos = new Vector2(maxX + minX, maxZ + minZ) / 2f * ARNSettings.settings.pixelsInMeter;
        mapController.contentRT.pivot = Vector2.one / 2;
        mapController.mapRT.pivot = (pos + mapController.mapRT.rect.size / 2) / mapController.mapRT.rect.size;
        mapController.mapRT.anchoredPosition = Vector2.zero;

        float scaleFactor = 1024f / Mathf.Max((maxX - minX) * ARNSettings.settings.pixelsInMeter.x, (maxZ - minZ) * ARNSettings.settings.pixelsInMeter.y) * zeroMapScaleFactor * .9f;

        mapController.scrollRectMap.currentZoom = scaleFactor;

        Vector2 rectPos;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(mapController.contentRT.parent.GetComponent<RectTransform>(), new Vector2(Screen.width, Screen.height) / 2, null, out rectPos);
        mapController.contentRT.anchoredPosition = rectPos;
    }

    void FocusMap(bool isHouse, Vector2 pos)
    {
        mapController.mapRT.rotation = Quaternion.Euler(0, 0, 0);
        if (!(lastGPSPos != Vector3.zero || PositionInterface.posStatus == PosStatus.normal))
        {
            mapController.contentRT.pivot = Vector2.one / 2;
            mapController.mapRT.pivot = (pos + mapController.mapRT.rect.size / 2) / mapController.mapRT.rect.size;
            mapController.mapRT.anchoredPosition = Vector2.zero;
            mapController.scrollRectMap.currentZoom = (isHouse) ? ARNSettings.settings.inDoorZoom : ARNSettings.settings.outDoorZoom;
            Vector2 rectPos;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(mapController.contentRT.parent.GetComponent<RectTransform>(), new Vector2(Screen.width, Screen.height) / 2, null, out rectPos);
            mapController.contentRT.anchoredPosition = rectPos;

        }
    }



    Coroutine StopARUI = null;
    IEnumerator StopARUIIE()
    {
        float t = 0;
        while (t < 1.3f)
        {
            t += Time.deltaTime * 2f;
            stopAR.fillAmount = t - 0.3f;
            yield return new WaitForEndOfFrame();
        }

        if (!firstStart)
        {
            firstStart = true;

            ARInterface.onStatusChange += onARStatusChange;
            ARInterface.StopARSession();

            previewAR.SetActive(true);
            stopAR.fillAmount = 0;
#if UNITY_EDITOR
            ARInterface.ChangeStatus(ARStatus.Stopped);
#endif
        }
        EasyAROpenButton(true);


        StopARUI = null;
    }

    private void onARStatusChange(ARStatus ARStatus)
    {
        ARInterface.onStatusChange -= onARStatusChange;
        if (ARStatus == ARStatus.Stopped)
        {
            PositionInterface.ChangeStatus(PosStatus.unknown);
            PositionInterface.ChangeArea(Area.unknown);
            Debug.Log(lastGPSAcc + " " + ARNSettings.settings.GPSAccuracyToHidePath);
            if (lastGPSPos != Vector3.zero && lastGPSAcc < ARNSettings.settings.GPSAccuracyToHidePath)
                NavUnit.NavInterface.UpdatePath(lastGPSPos, targetPoint.localPosition);
            else
                NavUnit.NavInterface.ResetPath();

            SuggestDialog(false);
        }
    }

    Coroutine focus = null;
    IEnumerator FocusMapIE(Vector2 pos, float speed = 2f)
    {
        RectTransform rt = mapController.contentRT;
        mapController.scrollRectMap.SetPivot(pos);

        yield return new WaitForEndOfFrame();
        Vector2 startPos = rt.position;
        Vector2 targetPos = new Vector2(Screen.width / 2f, Screen.height / 2f);
        float t = 0;

        while (t < 1)
        {
            rt.position = Vector2.Lerp(startPos, targetPos, t);
            t += Time.deltaTime * speed;
            yield return new WaitForEndOfFrame();
        }
        rt.position = targetPos;
        focus = null;
    }

    Coroutine openMap = null;
    IEnumerator OpenMapIE()
    {
        yield return new WaitForSeconds(0.2f);
        previewAR.SetActive(false);
        ARCam.targetTexture = targetTexture;
        openMap = null;
    }

    Coroutine rotateC = null;
    public void RotateToNorth()
    {
        if (rotateC == null)
        {
            rotateC = StartCoroutine(RotateToNorthIE());
        }
    }

    IEnumerator RotateToNorthIE()
    {
        float t = 0;
        mapController.scrollRectMap.SetPivot(new Vector2(Screen.width / 2f, Screen.height / 2f));
        Quaternion startRotation = mapSprite.rotation;

        while (t < 1)
        {
            mapSprite.rotation = Quaternion.Lerp(startRotation, Quaternion.Euler(0, 0, 0), t);
            t += Time.deltaTime * 3;
            yield return new WaitForEndOfFrame();
        }

        mapSprite.rotation = Quaternion.Euler(0, 0, 0);

        rotateC = null;
    }

    Coroutine zoom = null;
    IEnumerator ZoomIE(float targetZoom, bool toCenter = true, float speed = 8f)
    {

        targetZoom = Mathf.Clamp(targetZoom, ARNSettings.settings.minZoomFactor, ARNSettings.settings.maxZoomFactor);

        if (toCenter)
            mapController.scrollRectMap.SetPivot(new Vector2(Screen.width / 2f, Screen.height / 2f));
        float startZoom = mapController.scrollRectMap.currentZoom;
        float t = 0;
        while (t < 1)
        {
            t += Time.deltaTime * speed;
            mapController.scrollRectMap.currentZoom = startZoom + (targetZoom - startZoom) * t;
            yield return new WaitForEndOfFrame();
        }
        mapController.scrollRectMap.currentZoom = targetZoom;
        zoom = null;
    }

    IEnumerator UpdateFloorHeightIE()
    {
        while (true)
        {
            if (!isMap && PositionInterface.posStatus == PosStatus.normal)
            {
                UpdateFloor();
            }
            yield return new WaitForSeconds(1);
        }
    }

    void UpdateFloor()
    {
        mapHouseController.SetHeight(ARCam.transform.localPosition.y);
    }

    // Update is called once per frame
    void Update()
    {
        if (isMap)
        {
            northArrow.rotation = mapSprite.rotation;
            if (Math.Abs(mapSprite.rotation.eulerAngles.z) < 15)
            {
                if (Input.touchCount == 0)
                {
                    compassAnim.SetBool("Show", false);
                    if (Math.Abs(mapSprite.rotation.eulerAngles.z) > 2)
                        RotateToNorth();
                }
            }
            else
            {
                compassAnim.SetBool("Show", true);
            }
        }
    }
}
