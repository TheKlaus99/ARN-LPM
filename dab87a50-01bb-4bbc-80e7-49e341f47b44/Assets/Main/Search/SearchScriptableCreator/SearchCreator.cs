using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


#if UNITY_EDITOR
using UnityEditor;
public class SearchCreator : EditorWindow
{
    bool showPosition = true;
    string status = "Select a GameObject";
    int selectedID = 0;
    Search.DataScriptable sourceData;
    GameObject pointerGO;
    Vector3 pos;

    // Add menu item named "My Window" to the Window menu
    [MenuItem("Window/Serach data creator")]
    public static void ShowWindow()
    {
        //Show existing window instance. If one doesn't exist, make one.
        EditorWindow.GetWindow(typeof(SearchCreator));
    }

    Vector2 scrollPos;
    Vector2 scrollInfo;
    public void OnGUI()
    {
        //GUILayout.Label("Data base creator", EditorStyles.boldLabel);
        EditorGUILayout.Space();
        EditorGUILayout.BeginHorizontal();
        {
            EditorGUILayout.BeginVertical(GUILayout.Width(200));
            {
                EditorGUILayout.BeginHorizontal();
                {
                    GUILayout.Label("Source");
                    var t = EditorGUILayout.ObjectField(sourceData, typeof(Search.DataScriptable), true) as Search.DataScriptable;
                    if (t != sourceData)
                    {
                        selectedID = 0;
                        if (sourceData != null)
                        {
                            if (sourceData.data == null)
                                sourceData.data = new List<Search.SearchSer>();
                            Open(0);
                        }
                    }

                    sourceData = t;
                }
                EditorGUILayout.EndHorizontal();

                DrawLeftScroll();

            }
            EditorGUILayout.EndVertical();

            EditorGUILayout.Space();

            DrawMenuInsp();

        }
        EditorGUILayout.EndHorizontal();

    }

    void DrawLeftScroll()
    {
        if (sourceData)
        {
            //if (sourceData.data != null)
            var data = DrowSearchAndSort();

            scrollPos = EditorGUILayout.BeginScrollView(scrollPos, GUILayout.Width(250));
            for (int i = 0; i < data.Count; i++)
            {
                EditorGUILayout.BeginHorizontal();
                {
                    if (selectedID == sourceData.data.IndexOf(data[i]))
                    {
                        var style = new GUIStyle(GUI.skin.button);
                        style.normal.textColor = Color.green;
                        if (GUILayout.Button(data[i].name, style))
                        {
                            Open(sourceData.data.IndexOf(data[i]));
                        }
                    }
                    else
                    {
                        if (GUILayout.Button(data[i].name))
                        {
                            Open(sourceData.data.IndexOf(data[i]));
                        }
                    }

                    data[i].sortingOrder = EditorGUILayout.IntField(data[i].sortingOrder, GUILayout.Width(30));

                    if (GUILayout.Button("X", GUILayout.Width(20)))
                    {
                        Remove(sourceData.data.IndexOf(data[i]));
                    }
                }
                EditorGUILayout.EndHorizontal();
            }
            if (GUILayout.Button("Add"))
            {
                Add();
            }

            EditorGUILayout.EndScrollView();
        }
        else
        {
            GUILayout.Label("Choose source scriptable", EditorStyles.boldLabel);
        }
    }

    string searchText = "";
    List<Search.SearchSer> DrowSearchAndSort()
    {
        if (sourceData == null || sourceData.data == null)
            return new List<Search.SearchSer>();

        EditorGUILayout.BeginHorizontal();
        {
            GUILayout.Label("Search");
            searchText = EditorGUILayout.TextField(searchText);
            if (GUILayout.Button("X", GUILayout.Width(20)))
            {
                searchText = "";
            }
        }
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        {
            if (GUILayout.Button("Sort alphabet", GUILayout.Width(119)))
            {
                sourceData.data = sourceData.data.OrderBy(x => x.name).ThenByDescending(x => x.sortingOrder).ToList();
            }
            if (GUILayout.Button("Sort sortingOrder", GUILayout.Width(119)))
            {
                sourceData.data = sourceData.data.OrderByDescending(x => x.sortingOrder).ThenBy(x => x.name).ToList();
            }
        }
        EditorGUILayout.EndHorizontal();


        if (string.IsNullOrWhiteSpace(searchText))
            return sourceData.data;
        return sourceData.data.Where(x => x.name.ToLower().Contains(searchText.ToLower())).OrderByDescending(x => x.sortingOrder).ThenBy(x => x.name).ToList();
    }

    string tmpName = "";
    string tmpInfo = "";
    Vector3 tmpPos = Vector3.zero;
    void DrawMenuInsp()
    {
        EditorGUILayout.BeginVertical();
        {
            EditorGUILayout.BeginHorizontal();
            {
                GUILayout.Label("Pointer");
                pointerGO = EditorGUILayout.ObjectField(pointerGO, typeof(GameObject), true) as GameObject;
            }
            EditorGUILayout.EndHorizontal();

            if (!sourceData || sourceData.data == null)
                return;

            if (sourceData.data.Count > 0 && sourceData.data[selectedID].name.CompareTo(tmpName) == 0)
            {
                EditorGUILayout.PrefixLabel("Name:");
            }
            else
            {
                EditorGUILayout.PrefixLabel("[Changed]Name:");
            }
            tmpName = EditorGUILayout.TextField(tmpName);

            //var style = new GUIStyle(GUI.skin.textArea);
            //style.stretchHeight = true;


            if (sourceData.data.Count > 0 && sourceData.data[selectedID].info.CompareTo(tmpInfo) == 0)
            {
                EditorGUILayout.PrefixLabel("Info:");
            }
            else
            {
                EditorGUILayout.PrefixLabel("[Changed]Info:");
            }

            tmpInfo = EditorGUILayout.TextArea(tmpInfo, EditorStyles.textArea, GUILayout.MinHeight(100), GUILayout.Height(250));

            if (pointerGO)
            {
                if (EditorWindow.focusedWindow == this)
                {
                    pointerGO.transform.localPosition = tmpPos;
                }
                else
                {
                    tmpPos = pointerGO.transform.localPosition;
                }
            }

            if (sourceData.data.Count > 0 && sourceData.data[selectedID].pos == tmpPos)
            {
                tmpPos = EditorGUILayout.Vector3Field("Pos:", tmpPos);
            }
            else
            {
                tmpPos = EditorGUILayout.Vector3Field("[Changed]Pos:", tmpPos);
            }

            EditorGUILayout.Space();
            EditorGUILayout.Space();
            EditorGUILayout.Space();

            EditorGUILayout.BeginHorizontal();
            {
                EditorGUI.BeginDisabledGroup(
                    sourceData.data == null ||
                    selectedID >= sourceData.data.Count ||
                    selectedID < 0 ||
                    (sourceData.data[selectedID].pos == tmpPos &&
                        sourceData.data[selectedID].info.CompareTo(tmpInfo) == 0 &&
                        sourceData.data[selectedID].name.CompareTo(tmpName) == 0)
                );
                if (GUILayout.Button("Revert"))
                {
                    Open(selectedID);
                }

                if (GUILayout.Button("Apply"))
                {
                    Apply();
                }
                EditorGUI.EndDisabledGroup();

            }
            EditorGUILayout.EndHorizontal();
            if (GUILayout.Button("Add as new"))
            {
                if (sourceData)
                {
                    sourceData.data.Add(new Search.SearchSer(tmpName, tmpInfo, tmpPos, 0));
                    selectedID = sourceData.data.Count - 1;
                }
            }


        }
        EditorGUILayout.EndVertical();
    }

    void OnInspectorUpdate()
    {
        this.Repaint();
    }

    void Remove(int id)
    {
        if (sourceData)
        {
            sourceData.data.RemoveAt(id);
            if (selectedID > sourceData.data.Count - 1)
            {
                Open(sourceData.data.Count - 1);
            }
            else if (selectedID == id)
            {
                Open(selectedID);
            }
        }
    }

    void Apply()
    {
        sourceData.data[selectedID].name = tmpName;
        sourceData.data[selectedID].info = tmpInfo;
        sourceData.data[selectedID].pos = tmpPos;

        Save();
    }

    void Add()
    {
        if (sourceData)
        {
            if (sourceData.data == null)
                sourceData.data = new List<Search.SearchSer>();
            sourceData.data.Add(new Search.SearchSer("NewItem", "", tmpPos, 0));
            Open(sourceData.data.Count - 1);
        }
    }

    void Open(int id)
    {
        GUI.FocusControl("");
        selectedID = id;
        if (id < sourceData.data.Count && id >= 0)
        {
            tmpName = sourceData.data[id].name;
            tmpInfo = sourceData.data[id].info;
            tmpPos = sourceData.data[id].pos;
        }
        else
        {
            tmpName = "";
            tmpInfo = "";
            tmpPos = Vector3.zero;

        }
    }

    void Save()
    {
        if (sourceData)
        {
            EditorUtility.SetDirty(sourceData);
        }
    }

}
#endif
