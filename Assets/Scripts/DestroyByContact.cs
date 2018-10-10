using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyByContact : MonoBehaviour {

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "LaserCanon" || other.tag == "AllyBigShot" || other.tag == "Shield")
        {
            Destroy(gameObject);
            return;
        }
        else if (other.tag == "AllyShot")
        {
            Destroy(gameObject);
            Destroy(other.gameObject);
            return;
        }
        else
        {
            return;
        }
    }
}
