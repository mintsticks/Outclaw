﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace Outclaw{
  [CustomEditor(typeof(ObjectDialogueData))]
  public class ObjectDialogueDataEditor : Editor
  {
    public override void OnInspectorGUI(){
      ObjectDialogueData data = (ObjectDialogueData)target;
      EditorGUILayout.LabelField("Progress:", "" + data.Progress);
    }
  }
}
