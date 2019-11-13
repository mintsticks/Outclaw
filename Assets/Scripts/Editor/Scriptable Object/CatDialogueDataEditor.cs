using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace Outclaw.City{
  [CustomEditor(typeof(CatDialogueData))]
  public class CatDialogueDataEditor : Editor
  {

    private GameStateData state;

    public override void OnInspectorGUI(){
      DrawSingle();
    }

    private void DrawSingle(){
      CatDialogueData data = (CatDialogueData)target;

      GameStateList stateList = Resources.Load<GameStateList>("Game State Data/Game State List");

      EditorGUILayout.LabelField("Relationship Rank:", "" + data.Rank);
      EditorGUILayout.LabelField("State Ranks");
      foreach(GameStateData stateData in stateList){
        int rank = data.GetGameStateRank(stateData);
        if(rank > 0){
          EditorGUILayout.LabelField(stateData.name, "" + rank);
        }
      }

      EditorGUILayout.LabelField("Relationship Rank");
      if(GUILayout.Button("Force Increase")){
        data.IncreaseRank();
      }
      if(GUILayout.Button("Force Reset")){
        data.ResetRank();
      }
      EditorGUILayout.LabelField("State Rank");
      state = (GameStateData)EditorGUILayout.ObjectField("State", state, typeof(GameStateData), false);
      if(GUILayout.Button("Force Increase")){
        data.IncreaseGameStateRank(state);
      }
      if(GUILayout.Button("Force Reset")){
        data.ResetStateRank(state);
      }
      if(GUILayout.Button("Force Reset All")){
        data.ResetStateRanks();
      }
    }
  }
}
