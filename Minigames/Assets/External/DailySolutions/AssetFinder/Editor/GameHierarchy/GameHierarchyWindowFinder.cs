using System.Collections.Generic;
using System.Reflection;
using DailySolutions.AssetFinder.Utils;
using UnityEditor;
using UnityEngine;

namespace DailySolutions.AssetFinder.GameHierarchy
{
    [InitializeOnLoad]
    public class GameHierarchyWindowFinder : EditorWindow
    {
        private static object hierarchyEditor;
        private static SerializedObject serializedComponent;
        private static SerializedProperty propIterator;
        private static Object selectedObject;
        private static readonly List<int> ObjectsReferencingHierarchy = new List<int>();
        private static readonly List<int> ParentReferencingHierarchy = new List<int>();
        private static Component[] allComponents;
        private static Transform tempParent;
        private static Dictionary<string, Object> referencesDialogItems;
        private static ReferencesPopupWindowContent popupWindow;
        private static readonly GUIStyle ReferenceGUIStyle = new GUIStyle()
        {
            normal = new GUIStyleState() { textColor = Color.red },
            fontSize = 20,
            alignment = TextAnchor.MiddleRight,
        };

        static GameHierarchyWindowFinder()
        {
            referencesDialogItems = new Dictionary<string, Object>();
            EditorApplication.hierarchyWindowItemOnGUI += HandleHierarchyWindowItemOnGui;
            EditorApplication.projectWindowItemOnGUI += HandleProjectWindowItemOnGUI;
            Selection.selectionChanged += HandleSelectionChanged;
            HandleSelectionChanged();
        }
        
        #region OnGUI Handlers

        private static void HandleHierarchyWindowItemOnGui(int instanceID, Rect selectionRect)
        {
            if (selectedObject == null)
                return;

            GameObject obj = (GameObject) EditorUtility.InstanceIDToObject(instanceID);
            
            if(CheckIfIsReferencingHierarchy(instanceID))
            {
                EditorGUI.LabelField(selectionRect, "\u2190", ReferenceGUIStyle);
            }
            else if(CheckIfIsReferencingHierarchyAsParent(instanceID) && !IsGameObjectExpanded(obj))
            {
                EditorGUI.LabelField(selectionRect, "\u2193", ReferenceGUIStyle);
            }
        }

        private static void HandleProjectWindowItemOnGUI(string guid, Rect selectionRect)
        {
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
                x = selectionRect.x + (selectionRect.width * 0.5f)
            };
            
            if (GUI.Button(buttonRect, "Game Refs"))
            {
                PopupWindow.Show(buttonRect, popupWindow);
            }
        }

        #endregion

        private static bool CheckIfGameObjectHasReference(Object pObject)
        {
            bool value = false;
            serializedComponent = new SerializedObject(pObject);
            propIterator = serializedComponent.GetIterator();

            while (propIterator.NextVisible(true))
            {
                if (propIterator.propertyType == SerializedPropertyType.ObjectReference &&
                    propIterator.objectReferenceValue != null && selectedObject != null &&
                    propIterator.objectReferenceValue == selectedObject)
                {
                    value = true;
                }
            }

            propIterator.Dispose();
            serializedComponent.Dispose();
            return value;
        }

        private static bool CheckIfIsReferencingHierarchy(int instanceId)
        {
            return ObjectsReferencingHierarchy.Contains(instanceId);
        }

        private static bool CheckIfIsReferencingHierarchyAsParent(int instanceId)
        {
            return ParentReferencingHierarchy.Contains(instanceId);
        }

        
        private static void HandleSelectionChanged()
        {
            selectedObject = Selection.activeObject;
            if (selectedObject == null) 
                return;
            CheckReferencesHierarchy();
            GenerateDialogItems();
        }
        

        private static void CheckReferencesHierarchy()
        {
            allComponents = GetAllComponents();
            ObjectsReferencingHierarchy.Clear();
            ParentReferencingHierarchy.Clear();
            for (int i = 0; i < allComponents.Length; i++)
            {
                if(CheckIfGameObjectHasReference(allComponents[i]))
                {
                    ObjectsReferencingHierarchy.Add(allComponents[i].gameObject.GetInstanceID());
                    tempParent = allComponents[i].transform.parent;
                    while(tempParent != null)
                    {
                        ParentReferencingHierarchy.Add(tempParent.gameObject.GetInstanceID());
                        tempParent = tempParent.parent;
                    }
                }
            }

            System.Array.Clear(allComponents, 0, allComponents.Length);
            System.GC.Collect();
        }

        private static Component[] GetAllComponents()
        {
            var rootObjects = UnityEngine.SceneManagement.SceneManager
                .GetActiveScene()
                .GetRootGameObjects();
            List<Component> result = new List<Component>();
            foreach (var rootObject in rootObjects)
            {
                result.AddRange(rootObject.GetComponentsInChildren<Component>());
            }
            return result.ToArray();
        }

        private static bool IsGameObjectExpanded(GameObject go)
        {
            return GetExpandedGameObjects().Contains(go);
        }

        private static List<GameObject> GetExpandedGameObjects()
        {
            object sceneHierarchy = GetGameHierarchy();

            MethodInfo methodInfo = sceneHierarchy
                .GetType()
                .GetMethod("GetExpandedGameObjects");

            object result = methodInfo.Invoke(sceneHierarchy, new object[0]);

            return (List<GameObject>)result;
        }
        
        private static object GetGameHierarchy()
        {
            if (hierarchyEditor == null)
            {
                EditorWindow[] allWindows = Resources.FindObjectsOfTypeAll<EditorWindow>();

                for(int i = 0; i < allWindows.Length; i++)
                {
                    EditorWindow currentEditor = allWindows[i];
                    if (currentEditor.titleContent.text == "Hierarchy")
                    {
                        hierarchyEditor = currentEditor.GetType().Assembly.GetType("UnityEditor.SceneHierarchyWindow").GetProperty("sceneHierarchy")?.GetValue(currentEditor);
                        break;
                    }
                }
            }
            return hierarchyEditor;
        }
        private static void GenerateDialogItems()
        {
            referencesDialogItems = new Dictionary<string, Object>();
            if (ObjectsReferencingHierarchy == null || ObjectsReferencingHierarchy.Count == 0)
                return;

            for (int i = 0; i < ObjectsReferencingHierarchy.Count; i++)
            {
                int instanceId = ObjectsReferencingHierarchy[i];

                Object obj = EditorUtility.InstanceIDToObject(instanceId);

                if(obj != null && !referencesDialogItems.ContainsKey(obj.name))
                {
                    referencesDialogItems.Add(obj.name, obj);
                }
            }
            popupWindow = new ReferencesPopupWindowContent("Game Refs", referencesDialogItems);
        }

    }
}
