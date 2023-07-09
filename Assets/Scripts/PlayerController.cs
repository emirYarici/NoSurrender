using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;
/* this class 
 * makes player move To the nearest power up
 * also makes player get power ups, and get hit effect by other players or other enemies  
 * 
 * */
public class PlayerController : MonoBehaviour
{
    public Rigidbody rigidBody;
    public float moveSpeed;
    public Vector3 touchStartPos;
    public Vector3 curTouchPosition;
    public bool moveByInput = true;
    public GameObject playerObject;
    public float pushPower;
    public Color originalColor;
    public MeshRenderer modelMeshRenderer;
    // Start is called before the first frame update
    void Start()
    {
        originalColor = modelMeshRenderer.material.color;
    }

    // Update is called once per frame
    void Update()
    {
        DetectButtonPress();
        if (GameManager.Instance.gameStarted)
        {
            MoveStraight();
        }
    }
    private void MoveStraight()
    {
        if (moveByInput)
        {
            rigidBody.velocity = transform.forward * 1 * moveSpeed;
        }
        else
        {
            rigidBody.velocity = Vector3.zero;
        }
    }
    private void DetectButtonPress()
    {
        if (Input.touchCount > 0)
        {
            Vector3 localRotationParent = transform.localEulerAngles;
            Vector3 localRotationBody = transform.localEulerAngles;

            if (Input.GetTouch(0).phase == TouchPhase.Began) // This is actions when finger/cursor hit screen
            {
                touchStartPos = Input.GetTouch(0).position;
            }
            if (Input.GetTouch(0).phase == TouchPhase.Moved)
            {
                curTouchPosition = Input.GetTouch(0).position;
                //get the direction Vector, between the touch start pos and current touch pos
                Vector3 dir = (curTouchPosition - touchStartPos).normalized;
                //add this vector to position vector of the player, this is the point that our player should look at
                Vector3 targetPos = new Vector3(transform.position.x + dir.x, 0, transform.position.z + dir.y);
                //look at the target position but in 0.8 seconds;

                transform.DOLookAt(targetPos, 0.4f);
            }
            if (Input.GetTouch(0).phase == TouchPhase.Ended)
            {
                //reset touch posses
                touchStartPos = Vector3.zero;
                curTouchPosition = Vector3.zero;
            }
        }

    }

    /*this function manages collision with other capsule objects
     the collision effect has two parts;moving to the pushed position, the strenght 
     of the push is based on score ratio between colliders
     Also if our player get hit from back, "aka criticSpot" the ratio multiplide by a constant 

     rotation and moving parts made by using DOTween functions
     */
    public void CollisionWithEnemy(GameObject otherGameObject, bool isOnCriticHitPoint)
    {
        moveByInput = false;
        rigidBody.velocity = Vector3.zero;
        float enemyScore = otherGameObject.transform.parent.parent.GetComponent<EnemyController>().score;
        float massRatio = 1;
        if (GameManager.Instance.score > 0)
        {
            massRatio = enemyScore / GameManager.Instance.score;
        }
        else
        {
            massRatio = enemyScore / 100;
        }
        transform.DOKill();
        transform.DOPunchScale(new Vector3(0.2f, 0.2f, 0.2f), 0.5f, 2, 0.4f);
        Vector3 dir = (transform.position - otherGameObject.transform.position).normalized;
        Vector3 targetPos;
        if (isOnCriticHitPoint == true)
        {
            targetPos = new Vector3(transform.position.x + (dir.x * pushPower * massRatio) * 1.25f, 0, transform.position.z + (dir.z * pushPower * massRatio) * 1.25f);

        }
        else
        {
            targetPos = new Vector3(transform.position.x + dir.x * pushPower * massRatio, 0, transform.position.z + dir.z * pushPower * massRatio);
        }
        playerObject.transform.DOComplete();
        playerObject.transform.DOPunchRotation(dir * 40, 0.5F, 1);
        transform.DOMove(targetPos, 0.5f).SetEase(Ease.OutBack).OnComplete(() =>
        {
            StartCoroutine(KeepOnMovingAfterHit());
        });
    }

    public void GetScaleUp(GameObject otherGameObject)
    {

        otherGameObject.transform.parent = transform;
        otherGameObject.transform.DOLocalJump(Vector3.zero, 2, 1, 0.5f);
        otherGameObject.transform.DOScale(otherGameObject.transform.localScale * 0.5f, 0.5f).OnComplete(() =>
        {
            playerObject.transform.DOScale(playerObject.transform.localScale * 1.1f, 0.2f);
            GameManager.Instance.score += 100;
            GameManager.Instance.powerUpObjectsList.Remove(otherGameObject);
            GameManager.Instance.ControlScaleUpAmount();
            Destroy(otherGameObject);
        });
    }
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
    public void FallDown()
    {

        rigidBody.isKinematic = true;
        transform.DOMoveY(-3, 1f).OnComplete(() =>
        {
            Destroy(gameObject);
            GameManager.Instance.GameLose();
        });
    }
    public IEnumerator KeepOnMovingAfterHit()
    {
        yield return new WaitForSeconds(0.1f);
        moveByInput = true;
    }
    //the player object turns red slowy and turns to the normal color after speed up effect finished
    public IEnumerator BoostSpeedForAWhile()
    {
        moveSpeed *= 2;
        modelMeshRenderer.material.DOKill();
        modelMeshRenderer.material.DOColor(Color.red, 0.5f);
        yield return new WaitForSeconds(7);
        modelMeshRenderer.material.DOColor(originalColor, 0.5f);
        moveSpeed /= 2;
    }
}
