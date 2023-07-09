using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCriticPosTrigger : MonoBehaviour
{
    public PlayerController playerScript;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void OnTriggerEnter(Collider other)
    {
        if (CompareTag("Enemy"))
        {
            playerScript.CollisionWithEnemy(other.gameObject, true);
        }
    }
}
