using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Asteroids : MonoBehaviour
{
    public float knockback;
    private void OnCollisionEnter2D(Collision2D other) {
        if (other.gameObject.layer == LayerMask.NameToLayer("Player")){
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            player.GetComponent<PlayerController>().movementLocked = true;
            Vector2 velocity = new Vector2(-Mathf.Sign(transform.position.x-player.transform.position.x)*knockback, -Mathf.Sign(transform.position.y-player.transform.position.y)*knockback);
            player.GetComponent<Rigidbody2D>().velocity = velocity;
        }
    }
}
