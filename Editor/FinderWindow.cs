#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System;
using System.Collections.Generic;

namespace oisinr.finder
{
    public class FinderWindow : EditorWindow
    {
        private Finder finder;
        private bool getSubclasses = false;
        private bool rememberPlace = false;


        [MenuItem("Window/Finder")]
        public static void ShowWindow()
        {
            EditorWindow.GetWindow<FinderWindow>("Finder");
        }

        private void OnGUI()
        {
            if (finder == null)
            {
                finder = new Finder();
            }

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Find Objects");
            EditorGUILayout.Space();

            EditorGUI.BeginChangeCheck();
            finder.placesToLook = (Finder.PlacesToLook)EditorGUILayout.EnumPopup("Places to Look", finder.placesToLook);
            finder.thingToFind = EditorGUILayout.ObjectField("Thing to Find", finder.thingToFind, typeof(UnityEngine.Object), true);
            getSubclasses = EditorGUILayout.Toggle(new GUIContent("Get Subclasses", "If the thing to find is a script, this will search for subclasses of that script"), getSubclasses);

            if (Selection.activeGameObject == null && rememberPlace)
            {
                finder.rememberPlace = finder.placesToLook;
                finder.placesToLook = Finder.PlacesToLook.Scene;
                rememberPlace = false;
            }
            else if (!rememberPlace && Selection.activeGameObject != null)
            {
                finder.placesToLook = finder.rememberPlace;
                rememberPlace = true;
            }


            if (finder.thingToFind != null)
            {
                if (finder.thingToFind.GetType() == typeof(GameObject))
                {
                    finder.thingToFind = null;
                }
            }
            if (EditorGUI.EndChangeCheck())
            {
                finder.getSubclasses = getSubclasses;
                if (Selection.activeGameObject == null)
                {
                    finder.placesToLook = Finder.PlacesToLook.Scene;
                }
            }

            EditorGUILayout.Space();
            if (GUILayout.Button("Find"))
            {
                finder.GoFind();
            }

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Found Objects");

            foreach (UnityEngine.Object obj in finder.foundObjects)
            {
                EditorGUILayout.ObjectField(obj, obj.GetType(), true);
            }
        }
    }
}
#endif