#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace oisinr.finder
{

    public class Finder
    {
        public enum PlacesToLook { Children, Transform, Scene }
        public PlacesToLook placesToLook;
        public PlacesToLook rememberPlace;
        Transform[] transforms;
        enum TypeOfThingToFind { Monoscript, MonoBehaviour, Component }
        TypeOfThingToFind findType;
        public UnityEngine.Object thingToFind;
        [Tooltip("If the thing to find is a script, this will search for subclasses of that script")]
        public bool getSubclasses;
        public List<UnityEngine.Object> foundObjects = new List<UnityEngine.Object>();

        bool isSubclass(Type type1, Type type2)
        {
            if (type1 == type2)
            {
                return true;
            }
            else if (type1.BaseType == typeof(MonoBehaviour))
            {
                return false;
            }
            else
            {
                return isSubclass(type1.BaseType, type2);
            }
        }



        public void GoFind()
        {
            if (thingToFind == null) { return; }

            findType = TypeOfThingToFind.Component;
            try
            {
                MonoBehaviour script = (MonoBehaviour)thingToFind;
                findType = TypeOfThingToFind.MonoBehaviour;
            }
            catch
            {

            }
            try
            {
                MonoScript script = (MonoScript)thingToFind;
                findType = TypeOfThingToFind.Monoscript;
            }
            catch
            {

            }

            switch (findType)
            {
                case TypeOfThingToFind.MonoBehaviour:
                    GetScriptFromMonoBehaviour();
                    break;
                case TypeOfThingToFind.Monoscript:
                    GetScriptFromMonoScript();
                    break;
                case TypeOfThingToFind.Component:
                    GetComponent();
                    break;
            }
        }


        //From scene
        void GetScriptFromMonoBehaviour()
        {
            foundObjects.Clear();
            transforms = GetPlacesToLook();
            foreach (Transform t in transforms)
            {

                MonoBehaviour[] attachedScripts = t.GetComponents<MonoBehaviour>();
                if (getSubclasses)
                {
                    foreach (MonoBehaviour script in attachedScripts)
                    {
                        if (isSubclass(script.GetType(), thingToFind.GetType()))
                        {
                            foundObjects.Add(script);
                        }
                    }
                }
                else
                {
                    foreach (MonoBehaviour script in attachedScripts)
                    {
                        if (script.GetType() == thingToFind.GetType())
                        {
                            foundObjects.Add(script);
                        }
                    }
                }

            }
        }

        //From project folder
        void GetScriptFromMonoScript()
        {
            foundObjects.Clear();
            transforms = GetPlacesToLook();
            foreach (Transform t in transforms)
            {
                MonoScript scriptToFind = (MonoScript)thingToFind;
                MonoBehaviour[] attachedScripts = t.GetComponents<MonoBehaviour>();

                if (getSubclasses)
                {
                    foreach (MonoBehaviour script in attachedScripts)
                    {
                        if (isSubclass(script.GetType(), scriptToFind.GetClass()))
                        {
                            foundObjects.Add(script);
                        }
                    }
                }
                else
                {
                    foreach (MonoBehaviour script in attachedScripts)
                    {
                        if (script.GetType() == scriptToFind.GetClass())
                        {
                            foundObjects.Add(script);
                        }
                    }
                }
            }
        }

        void GetComponent()
        {
            foundObjects.Clear();
            transforms = GetPlacesToLook();
            foreach (Transform t in transforms)
            {

                Component[] attachedComponents = t.GetComponents<Component>();
                foreach (Component component in attachedComponents)
                {
                    if (component.GetType() == thingToFind.GetType())
                    {
                        foundObjects.Add(component);
                    }
                }
            }
        }


        Transform[] GetPlacesToLook()
        {
            switch (placesToLook)
            {
                case PlacesToLook.Children:
                    Transform[] ts = Selection.activeGameObject.GetComponentsInChildren<Transform>(); //To avoid including self
                    Transform[] ts2 = new Transform[ts.Length - 1];
                    Array.ConstrainedCopy(ts, 1, ts2, 0, ts.Length - 1);
                    return ts2;
                case PlacesToLook.Transform:
                    return Selection.activeGameObject.transform.root.GetComponentsInChildren<Transform>();
                case PlacesToLook.Scene:
                    return UnityEngine.Object.FindObjectsOfType<Transform>();
                default:
                    return Selection.activeGameObject.GetComponentsInChildren<Transform>();
            }
        }
    }
}

#endif
