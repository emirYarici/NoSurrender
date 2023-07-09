using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
/* this class 
 * makes enemy move To the nearest power up
 * also makes enemy get power ups, and get hit effect by other players or other enemies  
 * 
 * */
public class EnemyController : MonoBehaviour
{
    public Rigidbody rigidBody;
    public float moveSpeed;
    public Vector3 touchStartPos;
    public bool move = true;
    public float score;
    public GameObject targetObject;
    public GameObject modelObject;
    public float pushPower;
    // Start is called before the first frame update
    void Start()
    {
        //Look for a new target every one second
        InvokeRepeating("UpdateTarget", 0f, 1f);
    }

    // this func decleares a new target to the enemy to go towards, target selection is based on distance
    public void UpdateTarget()
    {
        float minDistance = 10000;
        for (int counter = 0; counter < GameManager.Instance.powerUpObjectsList.Count; counter++)
        {
            float curDistance = Vector3.Distance(GameManager.Instance.powerUpObjectsList[counter].transform.position, transform.position);
            if (curDistance < minDistance)
            {
                minDistance = curDistance;
                targetObject = GameManager.Instance.powerUpObjectsList[counter];
            }
        }
        if (targetObject != null)
        {
            //get the direction Vector between target and enemy
            Vector3 dir = (targetObject.transform.position - transform.position).normalized;
            //add this vector to position vector of the enemy, this is the point that our enemy should look at
            Vector3 targetPos = new Vector3(transform.position.x + dir.x, 0, transform.position.z + dir.z);
            //look at the target position but in 0.8 seconds;
            transform.DOLookAt(targetPos, 0.8f);
        }
        else
        {
            //if there is no targets left, go to the middle of the environment
            transform.DOLookAt(Vector3.zero, 0.8F);
        }
    }

    private void Update()
    {
        //start moving after game start
        if (GameManager.Instance.gameStarted == true)
        {
            MoveObjectForward();
        }
    }
    private void MoveObjectForward()
    {
        //make enemy move forward by changing the velocity through rigidbody element of it
        //by using move parameter, I disabled forward movement if there is a collision
        if (move)
        {
            rigidBody.velocity = transform.forward * 1 * moveSpeed;
        }
        else
        {
            rigidBody.velocity = Vector3.zero;
        }
    }
    
    private void OnTriggerEnter(Collider other)
    {
        switch (other.transform.tag)
        {
            case "SpeedBoost":
                //make enemy speed up
                GetSpeedUp(other.gameObject);
                break;
            case "ScaleBoost":
                //make enemy speed up
                GetScaleUp(other.gameObject);
                break;
            case "Enemy":
                //collision with enemy
                CollisionWithEnemyOrPlayer(other.gameObject, false);
                break;
            case "Player":
                //collision with player
                CollisionWithEnemyOrPlayer(other.gameObject, false);
                break;
            case "Border":
                //if goes outside of the area
                FallDown();
                break;
        }

    }
    //this func enables forward movements after collision effect completed
    public IEnumerator KeepOnMovingAfterHit()
    {

        yield return new WaitForSeconds(0.1f);
        UpdateTarget();
        move = true;
    }

    /*this function manages collision with other capsule objects
     the collision effect has two parts;moving to the pushed position, the strenght 
     of the push is based on score ratio between colliders
     Also if our enemy get hit from back, "aka criticSpot" the ratio multiplide by a constant 

     rotation and moving parts made by using DOTween functions
     */
    public void CollisionWithEnemyOrPlayer(GameObject otherGameObject, bool isOnCriticHitPoint)
    {
        
        float opponentScore;
        if (otherGameObject.GetComponent<EnemyController>() != null)//it is a Enemy
        {
            opponentScore = otherGameObject.transform.parent.parent.GetComponent<EnemyController>().score;
        }
        else// it is our player 
        {
            opponentScore = GameManager.Instance.score;
        }
        float massRatio = 1;
        if (score > 0)
        {
            massRatio = opponentScore / score;
        }
        else
        {
            massRatio = opponentScore / 100;
        }
        move = false;
        rigidBody.velocity = Vector3.zero;
        transform.DOKill();
        modelObject.transform.DOComplete();
        modelObject.transform.DOPunchScale(new Vector3(0.2f, 0.2f, 0.2f), 0.5f, 2, 0.4f);
        Vector3 dir = (transform.position - otherGameObject.transform.position).normalized;
        Vector3 targetPos;
        if (isOnCriticHitPoint == true)// is being hitted from critic spot
        {
            targetPos = new Vector3((transform.position.x + dir.x * pushPower * massRatio) * 1.25f, 0, (transform.position.z + dir.z * pushPower * massRatio) * 1.25f);
        }
        else
        {
            targetPos = new Vector3(transform.position.x + dir.x * pushPower * massRatio, 0, transform.position.z + dir.z * pushPower * massRatio);
        }
        modelObject.transform.DOComplete();
        modelObject.transform.DOPunchRotation(dir * 40, 0.5F, 1);
        transform.DOMove(targetPos, 0.5f).SetEase(Ease.OutBack).OnComplete(() =>
        {
            StartCoroutine(KeepOnMovingAfterHit());
        });
    }

    /*make scaleUpObject a little animation and scale up enemy object at the end of it 
     */
    public void GetScaleUp(GameObject otherGameObject)
    {
        otherGameObject.transform.parent = transform;
        otherGameObject.transform.DOLocalJump(Vector3.zero, 2, 1, 0.5f);
        otherGameObject.transform.DOScale(otherGameObject.transform.localScale * 0.5f, 0.5f).OnComplete(() =>
        {
            score += 100;
            modelObject.transform.DOScale(modelObject.transform.localScale * 1.1f, 0.3f);
            GameManager.Instance.powerUpObjectsList.Remove(otherGameObject);
            GameManager.Instance.ControlScaleUpAmount();
            Destroy(otherGameObject);
        });
    }
    //go down and control if game is ended or not
    public void FallDown()
    {
        rigidBody.isKinematic = true;
        transform.DOMoveY(-3, 1f).OnComplete(() =>
        {
            GameManager.Instance.enemyList.Remove(gameObject);
            GameManager.Instance.peopleLeft--;
            GameManager.Instance.GameEnd();
            Destroy(gameObject);
        });
    }

    /*
     
     make speed up object a little animation and speed up enemy for a while 
     */
    public void GetSpeedUp(GameObject otherGameObject)
    {
        otherGameObject.transform.parent = transform;
        otherGameObject.transform.DOLocalJump(Vector3.zero, 2, 1, 0.5f);
        otherGameObject.transform.DOScale(otherGameObject.transform.localScale * 0.5f, 0.5f).OnComplete(() =>
        {
            StartCoroutine(BoostSpeedForAWhile());
            GameManager.Instance.powerUpObjectsList.Remove(otherGameObject);
            GameManager.Instance.ControlScaleUpAmount();
            Destroy(otherGameObject);
        });
    }

    public IEnumerator BoostSpeedForAWhile()
    {
        moveSpeed *= 2;
        yield return new WaitForSeconds(7);
        moveSpeed /= 2;
    }
}
