using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunModule : MonoBehaviour
{
    [Header("References")]
    public GameObject turret;
    public Transform firePoint;
    public ModuleController module;
    public GameObject player;

    [Header("Turning Settings")]
    public float turnSpeed;
    public float angle;

    [Header("Attack Settings")]
    public bool canShoot;
    public float attackSpeed;
    public GameObject bullet;
    public GameObject enemyBullet;

    [Header("Sound Effects")]
    public AudioSource shootSound;
    // Start is called before the first frame update
    void Start()
    {
        module = gameObject.GetComponent<ModuleController>();
        player = GameObject.FindGameObjectWithTag("Player");
    }

    // Update is called once per frame
    void Update()
    {   
        if (!module.isBot){
            if (module.attached){
                Turret();
            }
        }   else{
            BotTurret();
        }
        
    }

    void Turret(){
        // Move turret to mouse direction
        Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        Vector2 direction =
            new Vector2(mousePosition.x - turret.transform.position.x,
                        mousePosition.y - turret.transform.position.y);
        angle = Mathf.LerpAngle(angle, Mathf.Atan2(direction.y, direction.x)*180/Mathf.PI, turnSpeed);
        if (Mathf.Abs(angle) > 360){
            angle -= 360*Mathf.Sign(angle);
        }
        if (mousePosition.x > turret.transform.position.x){
            turret.transform.localRotation = Quaternion.Euler(0.0f, 0.0f, ((angle+0*Mathf.Sign(angle))*Mathf.PI/180) * 60);
        }else {
            
            turret.transform.localRotation = Quaternion.Euler(0.0f, 0.0f, ((angle-9*Mathf.Sign(angle))*Mathf.PI/180) * 60);
        }

        // Fire projectiles
        if (canShoot && Input.GetMouseButton(0)){
            StartCoroutine(ShootTimer());
        }
    }

    void BotTurret(){
        // Move turret to mouse direction
        Vector2 playerPosition = player.transform.position;

        Vector2 direction =
            new Vector2(playerPosition.x - turret.transform.position.x,
                        playerPosition.y - turret.transform.position.y);

        angle = Mathf.LerpAngle(angle, Mathf.Atan2(direction.y, direction.x)*180/Mathf.PI, turnSpeed);
        if (Mathf.Abs(angle) > 360){
            angle -= 360*Mathf.Sign(angle);
        }
        
        if (direction.y > turret.transform.position.x){
            turret.transform.localRotation = Quaternion.Euler(0.0f, 0.0f, ((angle-0*Mathf.Sign(angle))*Mathf.PI/180) * 60);
        }else {
            
            turret.transform.localRotation = Quaternion.Euler(0.0f, 0.0f, ((angle-10*Mathf.Sign(angle))*Mathf.PI/180) * 60);
        }
        

        if (canShoot){
            StartCoroutine(EnemyShootTimer());
        }
    }

    IEnumerator ShootTimer(){
        turret.GetComponent<Animator>().SetTrigger("Fire");

        shootSound.Play();
        canShoot = false;
        BulletController thing = Instantiate(bullet, firePoint.position, firePoint.rotation).GetComponent<BulletController>();
        thing.startingPosition = firePoint.position;
        yield return new WaitForSeconds(attackSpeed);
        canShoot = true;
    }
    IEnumerator EnemyShootTimer(){
        turret.GetComponent<Animator>().SetTrigger("Fire");

        canShoot = false;
        BulletController thing = Instantiate(enemyBullet, firePoint.position, firePoint.rotation).GetComponent<BulletController>();
        thing.startingPosition = firePoint.position;
        yield return new WaitForSeconds(attackSpeed);
        canShoot = true;
    }
}
