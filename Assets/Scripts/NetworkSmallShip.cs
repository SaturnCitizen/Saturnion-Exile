using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Networking;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class NetworkSmallShip : NetworkBehaviour
{
    private Rigidbody2D rb2d;
    public Boundary boundary;
    public GameObject shot;
    public Transform shotSpawn;
    private RectTransform healthBar;
    private SpriteRenderer sprite;
    private AudioSource soundShoot;
    private AudioSource soundShield;
    [SyncVar (hook = "OnChangeHealth")] public int health;
    public float fireRate;
    public float speed;
    public float shieldCooldown;
    public float shieldDuration;
    private float nextFire;
    private float nextShield;
    private bool shielded = false;


    void Start()
    {
        rb2d = GetComponent<Rigidbody2D>();
        healthBar = GameObject.Find("ForegroundSmallShip").GetComponent<RectTransform>();
        sprite = GetComponent<SpriteRenderer>();
        soundShoot = GetComponent<AudioSource>();
        soundShield = transform.Find("AudioShield").GetComponent<AudioSource>();
    }
    void Update()
    {
        if (!isLocalPlayer)
        {
            return;
        }
        if (Input.GetKey(KeyCode.Space) & Time.time > nextFire)
        {
            nextFire = Time.time + fireRate;
            soundShoot.Play();
            CmdFireSmallShip();
        }
        if (Input.GetKey("e") & Time.time > nextShield)
        {
            nextShield = Time.time + shieldCooldown;
            StartCoroutine("ShieldActive");
        }

    }

    private void FixedUpdate()
    {
            float moveHorizontal = Input.GetAxisRaw("Horizontal");
            float moveVertical = Input.GetAxisRaw("Vertical");

            Vector3 movement = new Vector3(moveHorizontal, moveVertical, 0.0f);
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
            if (shielded == false)
            {
                TakeDamage(2);
                if (health == 0)
                {
                    Destroy(gameObject);
                    SceneManager.LoadScene("Defeat");
                }
            }
        }
        else if (other.tag == "EnemyShot")
        {
            Destroy(other.gameObject);
            if (shielded == false)
            {
                TakeDamage(1);
                if (health == 0)
                {
                    Destroy(gameObject);
                    SceneManager.LoadScene("Defeat");
                }
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
    void OnChangeHealth(int healthValue)
    {
        healthBar.sizeDelta = new Vector2(healthValue * 10, healthBar.sizeDelta.y);
    }


    /// <summary>
    /// this command function tell the server to instantiate a bullet on a position and spawn it on the server.
    /// </summary>
    [Command]
    void CmdFireSmallShip()
    {
        // Create the Bullet from the Bullet Prefab
        var bullet = (GameObject)Instantiate(shot, shotSpawn.position, shotSpawn.rotation);

        // Spawn the bullet on the Clients
        NetworkServer.Spawn(bullet);
    }

    

    /// <summary>
    /// those 4 command functions are used to synchronise the color of the ship to simulate visually the shield
    /// and to set the boolean in order to prevent the player of taking damages or not.
    /// the callbacks functions make sure that all clients are synchronised.
    /// </summary>
    [Command]
    void Cmd_ProvideColorShieldToServer()
    {
        sprite.color = Color.blue;
    }

    [Command]
    void Cmd_ProvideColorNormalToServer()
    {
        sprite.color = Color.white;
    }

    [Command]
    void Cmd_ActivateShield()
    {
        shielded = true;
    }

    [Command]
    void Cmd_DesactivateShield()
    {
        shielded = false;
    }

    [ClientCallback]
    void TransmitColorShield()
    {
        if (isLocalPlayer)
        {
            Cmd_ProvideColorShieldToServer();
        }
    }

    [ClientCallback]
    void TransmitColorNormal()
    {
        if (isLocalPlayer)
        {
            Cmd_ProvideColorNormalToServer();
        }
    }

    [ClientCallback]
    void TransmitShieldActivation()
    {
        if (isLocalPlayer)
        {
            Cmd_ActivateShield();
        }
    }

    [ClientCallback]
    void TransmitShieldDesactivation()
    {
        if (isLocalPlayer)
        {
            Cmd_DesactivateShield();
        }
    }

    /// <summary>
    /// this coroutine activate the shield by making the player invincible and changing his color to blue for a short duration.
    /// it calls the function to synchronised the color and the boolean when needed. 
    /// </summary>
    /// <returns></returns>
    IEnumerator ShieldActive()
    {
        soundShield.Play();
        shielded = true;
        TransmitShieldActivation();
        sprite.color = Color.blue;
        TransmitColorShield();
        yield return new WaitForSeconds(shieldDuration);
        TransmitColorNormal();
        shielded = false;
        TransmitShieldDesactivation();
        sprite.color = Color.white;
        soundShield.Stop();
        StopCoroutine("ShieldActive");
    }
}

