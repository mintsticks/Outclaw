using System.Collections.Generic;
using UnityEngine;
using Yarn.Unity;

namespace Outclaw {
  public class CatVariableStorage : VariableStorageBehaviour {
    Dictionary<string, Yarn.Value> variables = new Dictionary<string, Yarn.Value>();

    void Awake() {
      ResetToDefaults();
    }

    public override void ResetToDefaults() {
      Clear();
    }

    public override void SetValue(string variableName, Yarn.Value value) {
      variables[variableName] = new Yarn.Value(value);
    }

    public override Yarn.Value GetValue(string variableName) {
      return variables.ContainsKey(variableName) == false ? Yarn.Value.NULL : variables[variableName];
    }

    public override void Clear() {
      variables.Clear();
    }
  }
}