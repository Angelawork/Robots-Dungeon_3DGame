using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnPoint : MonoBehaviour
{
    public GameObject EnemyToSpawn;
    private void OnDrawGizmos() {
        Gizmos.color = Color.green;
        Vector3 center = transform.position + new Vector3(0,0.75f,0f);
        Gizmos.DrawWireCube(center,Vector3.one);
        Gizmos.DrawLine(center,center+transform.forward*2);
    }
}
