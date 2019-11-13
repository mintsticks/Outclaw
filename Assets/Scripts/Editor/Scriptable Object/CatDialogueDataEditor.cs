using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace Outclaw.City{
  [CustomEditor(typeof(CatDialogueData))]
  public class CatDialogueDataEditor : Editor
  {
    public override void OnInspectorGUI(){
      CatDialogueData data = (CatDialogueData)target;

      GameStateList stateList = Resources.Load<GameStateList>("Game State Data/Game State List");

      EditorGUILayout.LabelField("Relationship Rank:", "" + data.Rank);
      EditorGUILayout.LabelField("State Ranks");
      foreach(GameStateData state in stateList){
        int rank = data.GetGameStateRank(state);
        if(rank > 0){
          EditorGUILayout.LabelField(state.name, "" + rank);
        }
      }
    }
  }
}
