///Created by Neodrop. neodrop@unity3d.ru

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SaveAndLoad : MonoBehaviour
{
    public string fileName = "SaveMe.neo";
	public GameObject SavingObject;
    Rect redRect, greenRect, blueRect, resetRect, loadRect;

    void Awake()
    {
        redRect = new Rect(10, 10, 100, 30);
        greenRect = new Rect(redRect.x + redRect.width + 10, redRect.y, redRect.width, redRect.height);
        blueRect = new Rect(greenRect.x + greenRect.width + 10, redRect.y, redRect.width, redRect.height);
        resetRect = new Rect(blueRect.x + blueRect.width + 10, redRect.y, redRect.width, redRect.height);
        loadRect = new Rect(resetRect.x + resetRect.width + 10, redRect.y, redRect.width, redRect.height);
    }


    GameObject go = null;
    List<GameObject> objects = new List<GameObject>();
    void OnGUI()
    {
        bool created = false;
        if (GUI.RepeatButton(redRect, "Capsule"))
        {
			go = Instantiate(SavingObject) as GameObject;
			TypeHolder typeH = go.AddComponent(typeof(TypeHolder)) as TypeHolder;
            typeH.type = 1;
			created = true;
			go.name = "GameObject" + go.GetInstanceID();
			
//            go = GameObject.CreatePrimitive(PrimitiveType.Capsule);
//            TypeHolder typeH = go.AddComponent(typeof(TypeHolder)) as TypeHolder;
//            typeH.type = PrimitiveType.Capsule;
//            created = true;
//            go.name = "Capsule" + go.GetInstanceID();
        }

        if (GUI.RepeatButton(greenRect, "Cube"))
        {
//            go = GameObject.CreatePrimitive(PrimitiveType.Cube);
//            TypeHolder typeH = go.AddComponent(typeof(TypeHolder)) as TypeHolder;
//            typeH.type = PrimitiveType.Cube;
//            created = true;
//            go.name = "Cube" + go.GetInstanceID();
        }

        if (GUI.RepeatButton(blueRect, "Cylinder"))
        {
//            go = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
//            TypeHolder typeH = go.AddComponent(typeof(TypeHolder)) as TypeHolder;
//            typeH.type = PrimitiveType.Cylinder;
//            created = true;
//            go.name = "Cylinder" + go.GetInstanceID();
        }

        if (GUI.Button(resetRect, "SAVE"))
        {
            Hashtable toSave = new Hashtable();
            int count = objects.Count;
            for (int i = 0; i < count; i++)
            {
                GameObject obj = objects[i]; // номер обекта
                TypeHolder th = obj.GetComponent(typeof(TypeHolder)) as TypeHolder;
                if (toSave.ContainsKey(obj.name)) obj.name += obj.GetInstanceID(); 
                toSave.Add(obj.name, new ObjectSaver(obj, th.type)); // добавляет в табличку сейва объект(-ы) obj
            }

            for (int i = 0; i < count; i++) Destroy(objects[i]);
            objects.Clear();

            BinarySaver.Save(toSave, fileName); // сохраняет таблицу toSave в файл fileName
        }

        if (GUI.Button(loadRect, "LOAD"))
        {
            Hashtable toLoad = BinarySaver.Load(fileName) as Hashtable;
            if (toLoad == null)
            {
                Debug.Log("No File Found");
                return;
            }
            ICollection coll = toLoad.Values;

            foreach (ObjectSaver obj in coll)
            {
				
				GameObject g = Instantiate(SavingObject) as GameObject;
				g.transform.position = obj.GetPosition();
                g.GetComponent<Renderer>().material.color = obj.GetColor();
                g.name = obj.objectName;
                TypeHolder th = g.AddComponent(typeof(TypeHolder)) as TypeHolder;
                th.type = obj.GetObjectType();
                objects.Add(g);
				
            }
        }

        if (!created) return;
        go.transform.position = Random.insideUnitSphere * 5;
        go.GetComponent<Renderer>().material.color = new Color(Random.insideUnitSphere.x, Random.insideUnitSphere.y, Random.insideUnitSphere.z);
        objects.Add(go);
    }
}

[System.Serializable]
public class ObjectSaver
{
    /// <summary>
    /// Сериализация Цвета
    /// </summary>
    [System.Serializable]
    public class ColorSL 
    {
        float R, G, B, A;
        public ColorSL(UnityEngine.Color color)
        {
            R = color.r;
            G = color.g;
            B = color.b;
            A = color.a;
        }

        public Color GetColor()
        {
            return new Color(R, G, B, A);
        }
    }

    [System.Serializable]
    public class VectorSL
    {
        float X, Y, Z;

        public VectorSL(Vector3 pos)
        {
            X = pos.x;
            Y = pos.y;
            Z = pos.z;
        }

        public Vector3 GetPosition()
        {
            return new Vector3(X, Y, Z);
        }
    }

//===========================================SAVE AND LOAD======================================
    string objectType;
    ColorSL color;
    VectorSL position;
    public string objectName;

    public ObjectSaver(GameObject obj, int objectType)
    {
        switch (objectType)
        {
            case 1 :
                this.objectType = "Map";
                break;
//            case PrimitiveType.Cube:
//                this.objectType = "Cube";
//                break;
//            case PrimitiveType.Cylinder :
//                this.objectType = "Cylinder";
//                break;
        }

        color = new ColorSL(obj.GetComponent<Renderer>().material.color);
        position = new VectorSL(obj.transform.position);
        objectName = obj.name;
    }

    public int GetObjectType()
    {
        switch (objectType)
        {
            case "Map" :
                return 1;
//            case "Cube" :
//                return PrimitiveType.Cube;
//            case "Cylinder" :
//                return PrimitiveType.Cylinder;
        }

        return 0; // нечего сюда вставить. Уж не обессудьте.
    }

    public Color GetColor()
    {
        return color.GetColor();
    }

    public Vector3 GetPosition()
    {
        return position.GetPosition();
    }
}
