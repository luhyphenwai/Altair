using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [Header("References")]
    public Animator fadeOutAnim;
    public Animator pauseAnim;
    public Animator endGameAnim;
    public GameObject mainCamera;
    public GameObject player;

    [Header("Scene Change Settings")]
    public float fadeDelay;
    public float fadeTime;
    public bool changingScenes;
    public Scene lastScene;

    [Header("Game Settings")]
    public bool playerDead;
    public float endGameTime;

    [Header("Pause Settings")]
    public bool paused;

    [Header("Sound Settings")]
    public AudioSource menuButtonSound;

    private void Awake() {
        DontDestroyOnLoad(gameObject);
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Pause();
        OnNewScene();
        InGame();
    }

    // Pause menu
    void Pause(){
        if (Input.GetKeyDown(KeyCode.Escape)) paused = !paused;
        
        if (paused) {
            pauseAnim.SetBool("Paused", true);
            Time.timeScale = 0;
        }   else {
            pauseAnim.SetBool("Paused", false);
            Time.timeScale = 1;
        }
    }

    // Functions for menu objects
    public void ReturnToMainMenu(){
        menuButtonSound.Play();
        paused = false;
        Debug.Log("Return to menu");
        if (!changingScenes) StartCoroutine(FadeOut(SceneUtility.GetScenePathByBuildIndex(0)));
    }
    public void Play(){
        menuButtonSound.Play();
        paused = false;
        if (!changingScenes) StartCoroutine(FadeOut(SceneUtility.GetScenePathByBuildIndex(1)));
    
    }
    public IEnumerator FadeOut(string scene){
        paused = false;

        // Mark as changing scenes
        changingScenes = true;

        // Wait for fade delay time
        yield return new WaitForSeconds(fadeDelay);

        // Start fade out animation
        fadeOutAnim.SetBool("Fade", true);

        // Wait for fade out animation time
        yield return new WaitForSeconds(fadeTime);

        // Load scene
        SceneManager.LoadScene(scene);

        // Unfade
        fadeOutAnim.SetBool("Fade", false);

        // Finish changing scenes
        changingScenes = false;

        // Check in in main menu
        yield return new WaitForEndOfFrame();
        yield return new WaitForEndOfFrame();
        if (SceneManager.GetActiveScene().buildIndex == 0) Destroy(gameObject);
    }

    void OnNewScene(){
        if (lastScene != SceneManager.GetActiveScene()){
            mainCamera = GameObject.FindGameObjectWithTag("MainCamera");

            transform.GetChild(0).GetComponent<Canvas>().worldCamera = mainCamera.GetComponent<Camera>();
            
            if (SceneManager.GetActiveScene().buildIndex == 1){
                player = GameObject.FindGameObjectWithTag("Player");
            }   
        }
    }

    void InGame(){
        if (SceneManager.GetActiveScene().buildIndex == 1){
            if (player.transform.childCount > 0){
                if (player.transform.GetChild(0).GetComponent<ModuleController>().moduleType != ModuleController.ModuleType.Core){
                endGameAnim.SetBool("End", true);
                }
            }   else {
                endGameAnim.SetBool("End", true);
            }
            
        }
    }
}
