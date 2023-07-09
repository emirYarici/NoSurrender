using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerTriggerManager : MonoBehaviour
{
    // Start is called before the first frame update
    public PlayerController playerScript;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    /*
     The main player script is in the parent. To work with capsule collider properly, and also to manage scale up effects
     this trigger scripts detect the trigger scenerios and calss the right func for the occasion
     */
    private void OnTriggerEnter(Collider other)
    {
        switch (other.transform.tag)
        {
            case "SpeedBoost":
                playerScript.GetSpeedUp(other.gameObject);
                break;
            case "ScaleBoost":
                playerScript.GetScaleUp(other.gameObject);
                break;
            case "Enemy":
                Debug.Log("sa");
                playerScript.CollisionWithEnemy(other.gameObject,false);
                break;
            case "Border":
                playerScript.FallDown();
                break;
            case "CriticHitPoint":
                playerScript.CollisionWithEnemy(other.gameObject, true);
                break;
        }
    }

    private void OnTriggerExit(Collider other)
    {
       
    }
}
