using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;


namespace CTemplate
{
    [CustomEditor(typeof(LevelCreator))]
    public class LevelCreatorEditor : Editor
    {
        public LevelCreator LevelCreators;


        public override void OnInspectorGUI()
        {
            this.DrawDefaultInspector();
            if (this.LevelCreators == null)
            {
                this.LevelCreators = (LevelCreator)this.target;
            }
            {
                if (GUILayout.Button("Add Connector")) {
                    LevelCreators.AddConnector();
               }
                if (GUILayout.Button("Add Multi Connectos")) {
                    LevelCreators.AddMultiConnector();
                }
                if (GUILayout.Button("Add Portal")) {
                    LevelCreators.AddPortal();
                }
                if (GUILayout.Button("Disable Dot")) {
                    LevelCreators.AddDisabled();
                }
                if (GUILayout.Button("Remove Dot")) {
                    LevelCreators.AddRemoved();
                }
                if (GUILayout.Button("Save Level")) {
                    LevelCreators.SaveLevel();
                }
                if (GUILayout.Button("Load Levels")) {
                    LevelCreators.LoadLevels();
                }
                if (GUILayout.Button("Reset Level")) {
                    LevelCreators.CreateNewLevel();
                }
                
            }
        }
    }
}