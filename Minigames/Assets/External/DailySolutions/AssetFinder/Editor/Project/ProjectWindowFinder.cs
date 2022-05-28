using System.Collections.Generic;
using DailySolutions.AssetFinder.Project.Scriptable;
using DailySolutions.AssetFinder.Utils;
using UnityEditor;
using UnityEngine;

namespace DailySolutions.AssetFinder.Project
{
    [InitializeOnLoad]
    public class ProjectWindowFinder : EditorWindow
    {
        private static readonly GUIStyle IndicatorStyle = new GUIStyle()
        {
            normal = new GUIStyleState() { textColor = Color.red },
            fontSize = 20,
            alignment = TextAnchor.MiddleRight,
        };

        private static Object selectedObject;
        private static ProjectReference selectedReference;
        private static ScriptableFinderCacheScriptable cachedValues;
        private static Dictionary<string, Object> referencesDialogItems;
        private static ReferencesPopupWindowContent popupWindow;

        static ProjectWindowFinder()
        {
            referencesDialogItems = new Dictionary<string, Object>();
            EditorApplication.projectWindowItemOnGUI = HandleProjectWindowItemOnGUI;
            Selection.selectionChanged += HandleSelectionChanged;
            HandleSelectionChanged();
            RefreshScriptable();
        }

        public static void RefreshScriptable()
        {
            cachedValues = AssetDatabase.LoadAssetAtPath<ScriptableFinderCacheScriptable>(ProjectFinderEditorCache.CachePath);
        }

        private static void HandleProjectWindowItemOnGUI(string guid, Rect selectionRect)
        {
            if (CheckIfIsReferencingProject(guid))
            {
                EditorGUI.LabelField(selectionRect, "\u2190", IndicatorStyle);
            }

            if (selectedObject == null) 
                return;
            if (!AssetDatabase.TryGetGUIDAndLocalFileIdentifier(selectedObject, out string pGuid, out long localID))
                return;
            if (pGuid != guid || referencesDialogItems.Count <= 0) 
                return;
            
            Rect buttonRect = new Rect(selectionRect)
            {
                height = 15f,
                width = selectionRect.width * 0.25f,
                x = selectionRect.x + (selectionRect.width * 0.75f)
            };
            
            if (GUI.Button(buttonRect, "Proj. Refs"))
            {
                PopupWindow.Show(buttonRect, popupWindow);
            }
        }

        private static bool CheckIfIsReferencingProject(string guid)
        {
            if(selectedReference.referencesGuid == null || selectedReference.referencesGuid.Length == 0)
                return false;

            return System.Array.IndexOf(selectedReference.referencesGuid, guid) != -1;
        }

        private static void HandleSelectionChanged()
        {
            selectedObject = Selection.activeObject;
            if (selectedObject != null)
            {
                GetProjectReferencesFor(selectedObject);
            }
        }

        private static void GetProjectReferencesFor(Object selectedObject)
        {
            if (cachedValues == null)
            {
                RefreshScriptable();
                return;
            }

            if(AssetDatabase.TryGetGUIDAndLocalFileIdentifier(selectedObject, out string guid, out long localID)) 
            {
                cachedValues.TryGetValue(guid, out selectedReference);
            }
            else
            {
                selectedReference = new ProjectReference();
            }
            GenerateDialogItems();
        }

        private static void GenerateDialogItems()
        {
            referencesDialogItems = new Dictionary<string, Object>();
            if (selectedReference.referencesGuid == null)
                return;

            for (int i = 0; i < selectedReference.referencesGuid.Length; i++)
            {
                string guid = selectedReference.referencesGuid[i];

                string path = AssetDatabase.GUIDToAssetPath(guid);

                Object obj = AssetDatabase.LoadAssetAtPath<Object>(path);

                if(obj != null)
                {
                    referencesDialogItems[obj.name] = obj;
                }
            }
            popupWindow = new ReferencesPopupWindowContent("Project Refs", referencesDialogItems);
        }
    }
}
