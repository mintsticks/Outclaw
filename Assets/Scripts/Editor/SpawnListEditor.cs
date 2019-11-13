using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace Outclaw{
  [CustomEditor(typeof(SpawnList))]
  public class SpawnListEditor : Editor
  {
    public override void OnInspectorGUI(){
      EditorGUILayout.HelpBox("Make any enterances a child of this because the spawn locations are built from its children.",
        MessageType.Info);
    }
  }
}