using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ModuleController : MonoBehaviour
{
    public enum ModuleType {Basic, Gun, Engine, Core};

    [Header("References")]
    public LayerMask connectionLayer;
    private Animator anim;
    private Rigidbody2D rb;


    [Header("Settings")]
    public ModuleType moduleType;

    // Points it can connect to other modules
    public BoxCollider2D[] attachmentPoints;

    // Points that other modules can connect to 
    public BoxCollider2D[] connectionPoints;
    public List<ModuleController> connectedModules;
    public BoxCollider2D attachmentPoint;
    public BoxCollider2D connectionPoint;
    public bool isBot;

    [Header("Health")]
    public int health;
    public bool reset;
    
    [Header("Locking settings")]
    public float margin;
    public bool doNotConnect;
    public bool locked;
    public Vector2 lockingPosition;
    public float connectSpeed;
    public bool attached;

    [Header("Sound Effects")]
    public AudioSource hit;
    public AudioSource attachSound;
    public GameObject explosion;
    public bool die;
    // Set references
    private void Awake() {
        rb = gameObject.GetComponent<Rigidbody2D>();
        anim = gameObject.GetComponent<Animator>();
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {   
        if (die) Die();


        
        // Check if not attached 
        if (!attached){
            CheckAttachmentPoints();
        }   else if (!doNotConnect && !locked){
            ConnectAttachmentPoints();
        }   else if (locked && transform.parent != null){
            transform.localPosition = lockingPosition;
        }
        
        // Check for health
        if (health <= 0 || reset){
            Reset();
        }

        if (!locked && !doNotConnect) gameObject.layer = LayerMask.NameToLayer("Default");
        anim.SetBool("isBot", isBot);
    }

    // Search for connection points
    void CheckAttachmentPoints(){
        // Loop through all attachment points and check for nearby connection points
        for (int i = 0; i < attachmentPoints.Length; i++ ) {
            RaycastHit2D[] boxCasts = Physics2D.BoxCastAll(attachmentPoints[i].bounds.center, attachmentPoints[i].bounds.size, 0f, Vector2.up, 0f, connectionLayer);

            // Loop through all connection points found and check if they are viable
            for (int j = 0; j < boxCasts.Length; j++ ) {

                // Check if box cast hit, is not on own object, and is not already attached
                if (boxCasts[j] && boxCasts[j].collider.gameObject.transform.parent.gameObject != gameObject && !boxCasts[j].collider.gameObject.GetComponent<ConnectionPoint>().attached){

                    // Check if parent module of point is attached
                    if (boxCasts[j].collider.gameObject.transform.parent.GetComponent<ModuleController>().attached){
                        // Mark module as attached 
                        attached = true;

                        // Mark connection point as attached
                        boxCasts[j].collider.gameObject.GetComponent<ConnectionPoint>().attached = true;

                        // Set connection and attachemnt points
                        connectionPoint = boxCasts[j].collider.gameObject.GetComponent<BoxCollider2D>();
                        attachmentPoint = attachmentPoints[i];
                        
                        // Set parent
                        transform.parent = boxCasts[j].collider.transform.parent.parent;

                        // Set module
                        boxCasts[j].collider.transform.parent.gameObject.GetComponent<ModuleController>().connectedModules.Add(this);

                        if (!isBot){
                            // Set layer so no collisions
                            gameObject.layer = LayerMask.NameToLayer("Player");
                        }   else {
                            // Set layer so no collisions
                            gameObject.layer = LayerMask.NameToLayer("Enemy");
                        }
                    }
                }
            }
        }
    }

    void ConnectAttachmentPoints(){
        // Connection and attachment point positions
        Vector2 attachmentPosition = attachmentPoint.gameObject.transform.position;
        Vector2 connectionPosition = connectionPoint.gameObject.transform.position;

        Vector2 parentPosition = transform.parent.position;

        // Calculate locking position
        if (Mathf.Abs(transform.position.y - connectionPosition.y) > Mathf.Abs(transform.position.x - connectionPosition.x)){
            lockingPosition.x = connectionPoint.gameObject.transform.position.x-parentPosition.x-(attachmentPosition.x-transform.position.x);
            lockingPosition.y = connectionPoint.gameObject.transform.position.y - (attachmentPoint.gameObject.transform.position.y-transform.position.y)-parentPosition.y;
        }   else if (Mathf.Abs(transform.position.y - connectionPosition.y) < Mathf.Abs(transform.position.x - connectionPosition.x)){
            lockingPosition.y = connectionPoint.gameObject.transform.position.y-parentPosition.y-(attachmentPosition.y-transform.position.y);
            lockingPosition.x = connectionPoint.gameObject.transform.position.x - (attachmentPoint.gameObject.transform.position.x-transform.position.x)-parentPosition.x;
        }
        
        if (!isBot){
            // Set layer so no collisions
            gameObject.layer = LayerMask.NameToLayer("Player");
        }   else {
            // Set layer so no collisions
            gameObject.layer = LayerMask.NameToLayer("Enemy");
        }

        locked = true;

        // Play sound
        attachSound.Play();
        
    }

    public void Die(){
        // Go through all connected modules
        for(int i = 0; i < connectedModules.Count; i++){
            // Reset attached
            connectedModules[i].GetComponent<ModuleController>().die = true;
        }
        Instantiate(explosion, transform.position, Quaternion.identity);
        Destroy(gameObject);
    }

    public void Reset(){
        // Go through all connections
        for(int i = 0; i < connectionPoints.Length; i++){
            // Reset attached
            connectionPoints[i].GetComponent<ConnectionPoint>().attached = false;
        }
        
        // Go through all connected modules
        for(int i = 0; i < connectedModules.Count; i++){
            // Reset attached
            connectedModules[i].GetComponent<ModuleController>().reset = true;
        }

        // Check for health
        if (health <= 0){
            if (!isBot && moduleType == ModuleType.Core){
                Die();
            }   else {
                if (connectionPoint != null ) connectionPoint.transform.parent.GetComponent<ModuleController>().connectedModules.Remove(this);
                Instantiate(explosion, transform.position, Quaternion.identity);
                Destroy(gameObject);
            }
        }

        // Reset attached
        attached = false;
        locked = false;
        doNotConnect =false;

        // Set layer so no collisions
        gameObject.layer = LayerMask.NameToLayer("Default");

        // Unparent
        transform.parent = null;

        // knockback
        if (rb != null) rb.velocity = new Vector2(Random.Range(-2f, 2f),Random.Range(-2f, 2f));

        // Finish reset
        reset = false;

        // Mark as not a bot
        isBot = false;

        if (moduleType == ModuleType.Gun){
            gameObject.GetComponent<GunModule>().canShoot = true;
        }

        
    }

    // Check for bullet collision

    private void OnCollisionEnter2D(Collision2D other) {
        if (other.gameObject.tag == "Enemy Bullet" && !isBot){
            health -= other.gameObject.GetComponent<BulletController>().attackDamage;
            anim.SetTrigger("Hit");
            hit.Play();
            Destroy(other.gameObject);
        }

        if (isBot && other.gameObject.tag == "Bullet" ){
            health -= other.gameObject.GetComponent<BulletController>().attackDamage;
            anim.SetTrigger("Hit");
            hit.Play();
            Destroy(other.gameObject);
        }
    }
    
}
