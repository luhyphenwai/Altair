using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    [Header("References")]
    public GameObject player;
    public Rigidbody2D rb;
    public LayerMask enemyLayer;

    [Header("Settings")]
    public float speed;
    public float distance;
    public float margin;
    public float repelDistance;
    public float repelStrength;
    public float repelCap;
    public float repelMargin;
    

    // Start is called before the first frame update
    void Start()
    {
        rb = gameObject.GetComponent<Rigidbody2D>();
        player = GameObject.FindGameObjectWithTag("Player");
    }

    // Update is called once per frame
    void Update()
    {
        CheckDead();
        Move();
    }

    void CheckDead(){
        if (transform.childCount <= 0 || transform.GetChild(0).GetComponent<ModuleController>().moduleType != ModuleController.ModuleType.Core){
            transform.DetachChildren();
            Destroy(gameObject);
        }
    }

    void Move(){
        Vector2 velocity = Vector2.zero;
        if (Vector2.Distance(transform.position, player.transform.position)-distance > margin){
            velocity = new Vector2(Mathf.Sign(player.transform.position.x-transform.position.x)*speed, Mathf.Sign(player.transform.position.y-transform.position.y)*speed);
        }  else if (Vector2.Distance(transform.position, player.transform.position)-distance < margin && Vector2.Distance(transform.position, player.transform.position)-distance>0){
            velocity = Vector2.zero;
        }
        else {
            velocity = new Vector2(-Mathf.Sign(player.transform.position.x-transform.position.x)*speed, -Mathf.Sign(player.transform.position.y-transform.position.y)*speed);
        }

        RaycastHit2D ray = Physics2D.BoxCast(transform.position, new Vector2(repelDistance, repelDistance), 0f, Vector2.up, 0, enemyLayer);
        
        if (ray) {
            float enemyDistance = Vector2.Distance(transform.position, ray.point);
            if (enemyDistance < repelDistance ) {
                Vector2 direction = ray.point - (Vector2)transform.position;
                velocity.x = velocity.x - Mathf.Clamp((1/enemyDistance)*repelStrength, 0, repelCap) * direction.normalized.x;
                velocity.y = velocity.y - Mathf.Clamp((1/enemyDistance)*repelStrength, 0, repelCap) * direction.normalized.y;
            }
        }
        // Check for nearby enemies
        rb.velocity = velocity;
    }
}
