using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

[System.Serializable]
public class Boundary
{
    public float xMin, xMax, yMin, yMax;
}


public class BigShipController : NetworkBehaviour
{

    private Rigidbody2D rb2d;
    public Boundary boundary;
    public GameObject shot;
    public GameObject laserCanon;
    public Transform laserSpawn;
    public Transform shotSpawnLeft;
    public Transform shotSpawnRight;
    private RectTransform healthBar;
    private AudioSource soundShoot;
    private AudioSource soundLaser;
    [SyncVar(hook = "OnChangeHealth")] public int health;
    public float speed;
    public float fireRate;
    public float laserCanonDuration;
    public float laserCanonCooldown;
    private float nextFire;
    private float nextFireLaserCanon;

    // Use this for initialization
    void Start()
    {
        rb2d = GetComponent<Rigidbody2D>();
        if (isLocalPlayer)
        {
            this.transform.position = new Vector3(0.0f, -3.0f, 0.0f);
        }
        healthBar = GameObject.Find("ForegroundMotherShip").GetComponent<RectTransform>();
        soundShoot = GetComponent<AudioSource>();
        soundLaser = transform.Find("LaserCanonSpawner").GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!isLocalPlayer)
        {
            return;
        }
        if (Input.GetKey(KeyCode.Space) && Time.time > nextFire)
        {
            nextFire = Time.time + fireRate;
            soundShoot.Play();
            Cmd_FireMotherShip();
        }
        if (Input.GetKey("e") && Time.time > nextFireLaserCanon)
        {
            nextFireLaserCanon = Time.time + laserCanonCooldown;
            soundLaser.Play();
            Cmd_LaserCanonActivate();
        }
    }

    private void FixedUpdate()
    {
        float moveHorizontal = Input.GetAxisRaw("Horizontal");

        Vector3 movement = new Vector3(moveHorizontal, 0.0f, 0.0f);
        rb2d.velocity = movement * speed;

        rb2d.position = new Vector3
        (
            Mathf.Clamp(rb2d.position.x, boundary.xMin, boundary.xMax),
            Mathf.Clamp(rb2d.position.y, boundary.yMin, boundary.yMax),
            0.0f
        );
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "EnemyShip")
        {
            Destroy(other.gameObject);
            TakeDamage(2);
            if (health == 0)
            {
                Destroy(gameObject);
                SceneManager.LoadScene("Defeat");
            }
        }
        else if (other.tag == "EnemyShot")
        {
            Destroy(other.gameObject);
            TakeDamage(1);
            if (health == 0)
            {
                Destroy(gameObject);
                SceneManager.LoadScene("Defeat");
            }
        }
        else
        {
            return;
        }
    }

    public void TakeDamage(int amount)
    {
        health -= amount;
        if (health <= 0)
        {
            health = 0;
        }
    }

    /// <summary>
    /// this function take the value health of the player and fill visually the health bar by the amount of health the player has.
    /// and as the health is syncronised through the server it syncronise visually the health of the players on the server and all clients.
    /// </summary>
    /// <param name="healthValue"></param>
    void OnChangeHealth(int healthValue)
    {
        healthBar.sizeDelta = new Vector2(healthValue * 4, healthBar.sizeDelta.y);
    }


    /// <summary>
    /// this command function tell the server to instantiate a bullet on a position and spawn it on the server.
    /// </summary>
    [Command]
    void Cmd_FireMotherShip()
    {
        // Create the Bullet from the Bullet Prefab
        var bulletLeft = (GameObject)Instantiate(shot, shotSpawnLeft.position, shotSpawnLeft.rotation);
        var bulletRight = (GameObject)Instantiate(shot, shotSpawnRight.position, shotSpawnRight.rotation);

        // Spawn the bullet on the Clients
        NetworkServer.Spawn(bulletLeft);
        NetworkServer.Spawn(bulletRight);

    }

    /// <summary>
    /// this command function tell the server to instantiate a big bullet on a position and spawn it on the server with a certain duration
    /// before destroying it.
    /// </summary>
    [Command]
    void Cmd_LaserCanonActivate()
    {
        var laser = (GameObject)Instantiate(laserCanon, laserSpawn.transform.position, laserSpawn.transform.rotation);
        NetworkServer.Spawn(laser);
        Destroy(laser, laserCanonDuration);
    }

}
