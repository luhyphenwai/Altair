using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletController : MonoBehaviour
{
    public Rigidbody2D rb;
    public float speed;
    public int attackDamage;
    public Vector2 startingPosition;
    public float maxDistance;

    // Start is called before the first frame update
    void Start()
    {
        rb = gameObject.GetComponent<Rigidbody2D>();
        rb.velocity = transform.right * speed;
    }

    private void Update() {
        if (Vector2.Distance(startingPosition, transform.position) > maxDistance){
            Destroy(gameObject);
        }
    }
    IEnumerator destroy()
    {
        rb.velocity = Vector2.zero;
        Destroy(transform.GetChild(0).gameObject);
        yield return new WaitForSeconds(1);
        Destroy(gameObject);
    }
}
