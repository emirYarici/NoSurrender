using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/* 
 * This class makes Camera Follow our main character 
 * */
public class CameraFollow : MonoBehaviour
{
    public GameObject player;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        //if our player is not fall down yet, cameras transform equalize players position + constant offset
        if (player != null) {
            transform.position = new Vector3(player.transform.position.x, player.transform.position.y + 15, player.transform.position.z - 15);
        }
    }
}
