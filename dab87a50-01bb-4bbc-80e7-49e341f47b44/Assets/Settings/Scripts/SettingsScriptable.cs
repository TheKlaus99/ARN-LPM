using UnityEngine;

[CreateAssetMenu(fileName = "Settings", menuName = "Settings/MainSettings", order = 0)]
public class SettingsScriptable : ScriptableObject
{
    [Header("Keys")]
    [TextArea(1, 5)]
    public string easyARKey;

    [Header("Maps")]
    public GPSUnit.GPSMap GPSMap;
    public ARUnit.ARMap ARMap;

    [Header("Position")]
    //максимальная погрешность при которой будет происходить коррекция пивота (если вышел за пределы новой погрешности)
    public float maxAccToPivotByOutOfAcc = 15;

    //коэффициент привышения новой погрешности над старой для коррекции пивота
    public float exceedKToPivotByMinAcc = 1.2f;

    //погрешность аркита на метр
    public float accuracyByMeter = 0.1f; //1 метр на 10

    //расстояние для поиска ближайшей дороги
    public float accuracyToSample = 20f;

    //погрешность после которой позиционирование считается верным 
    public float minAngleAccuracyToStart = 120f;


    [Header("GPS2Estimate")]
    //минимальная погрешность компаса при конвертации из GPS в эстимейт 
    public float minCompasAccuracy = 60;

    //минималное расстояние от пивота карты на котором GPS будет считаться корректным. в КМ
    public float maxDistanceBetweenMapAndGPS = 5f;

    [Header("AR2Estimate")]
    //время распознания
    public float timeToGet = 2f;

    //минимальная дистанция для начала обновления после старта
    public float minDistanceToUpdate = .05f;

    //минимальный угол для начала обновления после старта
    public float minAngleToUpdate = 10f;

    //погрешность угла созданного эстимейта
    public float angleAccuracy = 20f;

    //горизонтальная погрешность созданного эстимейта
    public float horizontalAccuracy = .1f;


    [Header("Navigation")]
    //расстояние для перерасчёта маршрута 
    public float distanceToRecalculate = 5;


    [Header("Draw path")]
    //расстояние между стрелками 
    public float arrowDistance = 1;

    //скорость стрелок
    public float arrowSpeed = 1;

    //максимальная дистанция прорисовки пути
    public float maxPathDrawDistance = 100f;



    [Header("UI/Map")]
    //спрайт для ui карты
    public Sprite UIMap;

    //пикселей в метре на карте = 1024 / map.size / 2
    public Vector2 pixelsInMeter;

    //толщина линии пути при размере карты = 1
    public float pathLineThickness = 8f;

    //погрешность GPS при которой выключается матка "вы здесь"
    public float GPSAccuracyToSwitchFromEasyAR = 20f;

    //погрешность GPS после которой не будет отображаться путь
    public float GPSAccuracyToHidePath = 20f;

    [Space]
    //максимальное отдаление
    public float minZoomFactor = 0.05f;

    //максимальное приближение
    public float maxZoomFactor = 5;

    //размер карты при запуске
    public float startZoomFactor = 1;

    //размер карты после которого будут помещения внутри невыделенных домов
    public float zoomToShowFloor = 2;

    //размер в который перейдёт карты при включение АР режима в помещение 
    public float inDoorZoom = 2.1f;

    //размер в который перейдёт карты при включение АР режима на улице  
    public float outDoorZoom = 1.2f;

    [Header("UI/Search")]

    //список кабинетов
    public Search.DataScriptable searchData;

}
