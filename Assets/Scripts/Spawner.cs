using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Spawner : MonoBehaviour
{
    private List<SpawnPoint> spawnPointList;
    private List<Character> spawnedCharacter;
    private bool hasSpawned;
    public UnityEvent AllSpawnedEnemyEliminated;
    private void Awake() {
        var spawnPointArray = transform.parent.GetComponentsInChildren<SpawnPoint>();
        spawnPointList = new List<SpawnPoint>(spawnPointArray);
        spawnedCharacter = new List<Character>();
    }
    private void Update() {
        if(!hasSpawned || spawnedCharacter.Count==0){
            return;
        }

        bool allSpawnedDead = true;
        foreach(Character c in spawnedCharacter){
            if(c.CurrentState!=Character.CharacterState.Dead){
                allSpawnedDead = false;
                break;
            }
        }

        if(allSpawnedDead){
            if(AllSpawnedEnemyEliminated != null)
                AllSpawnedEnemyEliminated.Invoke();

            spawnedCharacter.Clear();
        }
    }

    public void SpawnCharacters(){
        if(hasSpawned){
            return;
        }
        hasSpawned = true;

        foreach (SpawnPoint point in spawnPointList){
            if(point.EnemyToSpawn!=null){
                GameObject spawnedGameObject = Instantiate(point.EnemyToSpawn, point.transform.position, point.transform.rotation);
                spawnedCharacter.Add(spawnedGameObject.GetComponent<Character>());        
            }
        }
    }

    private void OnTriggerEnter(Collider other) {
        if(other.tag=="Player"){
            SpawnCharacters();
        }
    }
}
