using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CanonLaserOnTouching : MonoBehaviour {


    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "EnemyShot")
        {
            Destroy(other.gameObject);
            return;
        }
        else
        {
            return;
        }
    }
}
