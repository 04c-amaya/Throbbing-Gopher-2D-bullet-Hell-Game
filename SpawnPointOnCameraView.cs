using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnPointOnCameraView : MonoBehaviour
{
    WaveSpawner waveSpawner;
    private void Awake()
    {
        waveSpawner = GameObject.Find("Wave Spawner").GetComponent<WaveSpawner>();
    }
     void OnBecameInvisible()
    {
        Debug.Log("Invis");
        waveSpawner.spawnPoints.Add(this.gameObject);
    }
     void OnBecameVisible()
    {
        Debug.Log("vis");
        waveSpawner.spawnPoints.Remove(this.gameObject);
    }
}
