using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;

public class NetworkEnemy : NetworkBehaviour{

    public GameObject enemyPrefab;
    public Vector3 spawnValues;
    public int wavesNumber;
    public int enemyCount;
    public float spawnWait;
    public float startWait;
    public float waveWait;
    public float waitBeforeVictory;


    public override void OnStartServer()
    {
        StartCoroutine(SpawnWaves());
    }


    /// <summary>
    /// this coroutine instantiate all the enemies by waves based on an enemy prefab and spawn them on the server.
    /// the number of enemy is incremented by 5 every wave and after a certain amount of waves, it loads the victory scene.
    /// </summary>
    /// <returns></returns>
    IEnumerator SpawnWaves()
    {
        yield return new WaitForSeconds(startWait);

        for (int i = 0; i < wavesNumber; i++)
        {
            for (int j = 0; j < enemyCount; j++)
            {
                Vector3 spawnPosition = new Vector3(Random.Range(-spawnValues.x, spawnValues.x), spawnValues.y, spawnValues.z);
                Quaternion spawnRotation = Quaternion.identity;
                var enemy = (GameObject)Instantiate(enemyPrefab, spawnPosition, spawnRotation);
                NetworkServer.Spawn(enemy);
                yield return new WaitForSeconds(spawnWait);
            }
            enemyCount = enemyCount + 5;
            yield return new WaitForSeconds(waveWait);
        }

        yield return new WaitForSeconds(waitBeforeVictory);

        SceneManager.LoadScene("Victory");

    }
}
