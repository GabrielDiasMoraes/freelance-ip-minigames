using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace DailySolutions.AssetFinder.Utils
{
    public class ReferencesPopupWindowContent : PopupWindowContent
    {
        private Dictionary<string, Object> _referencesDialogItems;
        private Vector2 _scrollPos;
        private string _panelName;

        private static readonly GUIStyle TitleStyle = new GUIStyle()
        {
            fontSize = 20,
            fontStyle = FontStyle.Bold,
            alignment = TextAnchor.MiddleCenter,
            normal = new GUIStyleState() { textColor = Color.white },
        };

        public ReferencesPopupWindowContent(string panelName, Dictionary<string, Object> pReferencesDialogItems)
        {
            this._panelName = panelName;
            _referencesDialogItems = pReferencesDialogItems;
        }

        public override void OnGUI(Rect rect)
        {
            GUIStyle buttonStyle = new GUIStyle(GUI.skin.button)
            {
                alignment = TextAnchor.MiddleLeft
            };

            GUILayout.Space(15f);
            
            GUILayout.Label(_panelName, TitleStyle);
            
            GUILayout.Space(15f);
            
            GUILayout.BeginVertical();
            _scrollPos = GUILayout.BeginScrollView(_scrollPos);
            int count = 1;
            foreach (var item in _referencesDialogItems)
            {
                if (GUILayout.Button($"{count} - {item.Key}", buttonStyle))
                {
                    EditorGUIUtility.PingObject(item.Value);
                }
                count++;
            }
            GUILayout.EndScrollView();
            GUILayout.EndVertical();
        }
    }
}
