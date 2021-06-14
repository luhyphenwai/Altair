using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaveManager : MonoBehaviour
{
    [Header("References")]
    public GameObject player;
    public Camera mainCamera;
    public Animator alertAnim;
    public LayerMask asteroidLayer;
    public LayerMask enemyLayer;

    [Header("Wave Settings")]
    public bool waiting;
    public float waitTime;

    [Header("Enemy Prefabs")]
    public GameObject[] enemies;
    [Header("Sound")]
    public AudioSource enemySpawnSound;
    private void Awake() {
        mainCamera = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();
        player = GameObject.FindGameObjectWithTag("Player");
    }
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(StartingSpawnTime());
    }

    // Update is called once per frame
    void Update()
    {
        // Check if there are still enemies alive
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");

        // Start timer if enemies are all dead
        if (enemies.Length <= 0 && !waiting)  {
            StartCoroutine(WaitSpawnTime());
        }
    }

    void SpawnWave(){
        // Check player modules
        int modules = player.transform.childCount;

        // Different amount of enemies with levels depending on modules
        if (modules > 0){
            // If less than or equal to 12 modules
            if (modules <= 11){
                // Spawn 1 level 1
                SpawnEnemy(enemies[Random.Range(0,2)]);
            } else if (modules <= 14){
                // Spawn 3 level 1
                for (int i = 0; i < 2; i++){
                    SpawnEnemy(enemies[Random.Range(0,2)]);
                }
            } else if (modules <= 18){
                // Spawn 1 level 1
                SpawnEnemy(enemies[Random.Range(0,2)]);
                // Spawn 3 level 2
                for (int i = 0; i < 2; i++){
                    SpawnEnemy(enemies[Random.Range(3,5)]);
                }
            }   else if (modules <= 22){
                // Spawn 1 level 1
                SpawnEnemy(enemies[Random.Range(0,2)]);
                // Spawn 1 level 1
                SpawnEnemy(enemies[Random.Range(0,2)]);
                // Spawn 4 level 2
                for (int i = 0; i < 4; i++){
                    SpawnEnemy(enemies[Random.Range(3,5)]);
                }
            }   else if (modules <= 25){
                // Spawn 1 level 2
                SpawnEnemy(enemies[Random.Range(3,5)]);
                // Spawn 3 level 3
                for (int i = 0; i < 4; i++){
                    SpawnEnemy(enemies[Random.Range(6,9)]);
                }
            }   else if (modules <= 30){
                // Spawn 5 level 3
                for (int i = 0; i < 6; i++){
                    SpawnEnemy(enemies[Random.Range(6,9)]);
                }
            }   else if (modules <= 40){
                // Spawn 3 level 2
                for (int i = 0; i < 4; i++){
                    SpawnEnemy(enemies[Random.Range(3,5)]);
                }
                // Spawn 5 level 3
                for (int i = 0; i < 6; i++){
                    SpawnEnemy(enemies[Random.Range(6,9)]);
                }
            }   else if (modules <= 45){
                
                // Spawn 9 level 3
                for (int i = 0; i < 10; i++){
                    SpawnEnemy(enemies[Random.Range(6,9)]);
                }
            }
            else if (modules >= 50){
                for (int i = 0; i < modules/3; i++){
                    SpawnEnemy(enemies[Random.Range(0,enemies.Length-1)]);
                }
            }
        }
    }
    void SpawnEnemy(GameObject enemy){
        bool foundPosition = false;

        while (!foundPosition){
            // Find random position outside radius of camera
            Vector2 position = new Vector2(mainCamera.orthographicSize*Mathf.Sign(Random.Range(-1,1))*(2), Random.Range(-mainCamera.orthographicSize, mainCamera.orthographicSize));
            position.x = transform.position.x+position.x;
            position.y = transform.position.y+position.y;
            
            // Raycast position to find asteroids
            RaycastHit2D ray = Physics2D.BoxCast(position, new Vector2(mainCamera.orthographicSize, mainCamera.orthographicSize),0,Vector2.up,0,asteroidLayer);
            RaycastHit2D enemyNearby = Physics2D.BoxCast(position, new Vector2(mainCamera.orthographicSize/2, mainCamera.orthographicSize/2),0,Vector2.up,0,enemyLayer);
            
            // Instantiate enemy if no asteroids
            if (!ray && !enemyNearby){
                Instantiate(enemy, position, Quaternion.identity);
                foundPosition = true;
            }
        }
    }

    IEnumerator WaitSpawnTime(){
        waiting = true;
        yield return new WaitForSeconds(waitTime+(player.transform.childCount/2));
        alertAnim.SetTrigger("Alert");
        yield return new WaitForSeconds(0.5f);
        enemySpawnSound.Play();
        SpawnWave();
        
        waiting = false;
    }

    IEnumerator StartingSpawnTime(){
        waiting = true;
        yield return new WaitForSeconds(12);
        alertAnim.SetTrigger("Alert");
        yield return new WaitForSeconds(0.5f);
        enemySpawnSound.Play();
        SpawnWave();
        
        waiting = false;
    }
}
