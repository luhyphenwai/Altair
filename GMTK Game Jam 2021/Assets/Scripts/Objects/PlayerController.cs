using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("References")]
    private Rigidbody2D rb;

    [Header("Settings")]
    public int modules;
    public bool movementLocked;
    public float movementUnlockTime;

    [Header("Acceleration Speed")]
    public float moduleAccelDecreaseSpeed;
    public float engineIncreaseSpeed;
    public float defaultSpeed;
    public float speed;

    [Header("Deceleration Speed")]
    public float moduleDecelIncreaseSpeed;
    public float engineDecreaseSpeed;
    public float defaultDecelerateSpeed;
    public float decelerateSpeed;

    [Header("Speed Settings")]
    public float moduleDecreaseMaxSpeed;
    public float engineIncreaseMaxSpeed;
    public float defaultMaxSpeed;
    public float maxSpeed;
    public Vector2 velocity;
    
    // Set references
    private void Awake() {
        rb = gameObject.GetComponent<Rigidbody2D>();
    }
    // Start is called before the first frame update
    void Start()
    {
        speed = defaultSpeed;
        decelerateSpeed = defaultDecelerateSpeed;
        maxSpeed = defaultMaxSpeed;
        ModuleSpeed();
    }

    // Update is called once per frame
    void Update()
    {   
        if (movementLocked){
            StartCoroutine(unlockMovement());
        }
        if (!movementLocked){
            Move();
        }
        ModuleSpeed();
    }
    IEnumerator unlockMovement(){
        yield return new WaitForSeconds(movementUnlockTime);
        movementLocked = false;
        StopAllCoroutines();
    }
    void Move(){    
        // Get player input
        Vector2 playerInput = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));

        // Add to velocity
        velocity += speed * playerInput * Time.deltaTime;

        // Clamp it to max speed
        velocity = Vector2.ClampMagnitude(velocity, maxSpeed);

        // Check if not accelerating in directions, and if so decelerate
        if (playerInput.x == 0){
            velocity -= Vector2.right * decelerateSpeed * Mathf.Sign(velocity.x) * Time.deltaTime;
        }   
        if (playerInput.y == 0){
            velocity -= Vector2.up * decelerateSpeed * Mathf.Sign(velocity.y) * Time.deltaTime;
        }

        // Set velocity
        rb.velocity = velocity;
    }

    void ModuleSpeed(){
        if (transform.childCount != modules){
            
            // Find amount of engine modules
            int engineModules = 0;
            for (int i = 0; i < transform.childCount; i++){
                if (transform.GetChild(i).GetComponent<ModuleController>().moduleType == ModuleController.ModuleType.Engine){
                    engineModules++;
                }
            }
            
            speed = defaultSpeed - transform.childCount*moduleAccelDecreaseSpeed + engineIncreaseSpeed*engineModules;
            decelerateSpeed = defaultDecelerateSpeed - transform.childCount*moduleDecelIncreaseSpeed + engineDecreaseSpeed*engineModules;
            maxSpeed = defaultMaxSpeed - transform.childCount*moduleDecreaseMaxSpeed + engineModules*engineIncreaseMaxSpeed;

            modules = transform.childCount;
        }
    }
}
