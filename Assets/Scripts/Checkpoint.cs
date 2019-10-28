using System;
using System.Collections;
using System.Collections.Generic;
using Outclaw;
using UnityEngine;
using Zenject;

public class Checkpoint : MonoBehaviour {

    [Inject] 
    private ISpawnManager spawnManager;
    
    [SerializeField] private string checkpointName;

    public string CheckpointName => checkpointName;

    // Start is called before the first frame update
    void Start() {
        spawnManager.Checkpoints.Add(this);
    }

    public void UpdateLastCheckpoint() {
        spawnManager.LastCheckpoint = checkpointName;
        Debug.Log(checkpointName);
    }
}
