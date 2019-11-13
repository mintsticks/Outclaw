using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace Outclaw{
  [CustomEditor(typeof(Task))]
  public class TaskEditor : Editor
  {
    public override void OnInspectorGUI(){
      Task task = (Task)target;
      EditorGUILayout.LabelField("Is Complete:", "" + task.IsComplete);
      if(GUILayout.Button("Force Complete")){
        task.Complete();
      }
      if(GUILayout.Button("Force Reset")){
        task.Reset();
      }
    }
  }
}
