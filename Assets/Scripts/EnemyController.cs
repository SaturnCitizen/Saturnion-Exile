using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;


public class EnemyController : NetworkBehaviour{

    private Rigidbody2D rb2d;
    public float speed;
    public GameObject shot;
    public Transform shotSpawn;
    public float fireRate;
    private float nextFire;

    // Use this for initialization
    void Start () {
        rb2d = GetComponent<Rigidbody2D>();
        rb2d.velocity = transform.up * speed;
    }
	
	// Update is called once per frame
	void Update () {
        if (!isServer)
        {
            return;
        }
        if (Time.time > nextFire)
        {
            nextFire = Time.time + fireRate;
            Fire();
        }
    }


    /// <summary>
    /// this function instantiate and spawn all the enemy bullets on the server according to a certain fire rate.
    /// </summary>
    void Fire()
    {
        // Create the Bullet from the Bullet Prefab
        var bullet = (GameObject)Instantiate(
            shot,
            shotSpawn.position,
            shotSpawn.rotation);

        // Spawn the bullet on the Clients
        NetworkServer.Spawn(bullet);
    }
}
