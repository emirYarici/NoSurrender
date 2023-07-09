using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

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
        InvokeRepeating("UpdateTarget", 0f, 1f);
    }

    // Update is called once per frame
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
            Vector3 dir = (targetObject.transform.position - transform.position).normalized;
            Vector3 targetPos = new Vector3(transform.position.x + dir.x, 0, transform.position.z + dir.z);
            transform.DOLookAt(targetPos, 0.8f);
        }
        else
        {
            transform.DOLookAt(Vector3.zero, 0.8F);
        }
    }

    private void Update()
    {
        if (GameManager.Instance.gameStarted == true)
        {
            MoveObjectForward();
        }
    }
    private void MoveObjectForward()
    {

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
                GetSpeedUp(other.gameObject);
                break;
            case "ScaleBoost":
                GetScaleUp(other.gameObject);
                break;
            case "Enemy":
                CollisionWithEnemyOrPlayer(other.gameObject, false);
                break;
            case "Player":
                CollisionWithEnemyOrPlayer(other.gameObject, false);
                break;
            case "Border":
                FallDown();
                break;
        }

    }
    public IEnumerator KeepOnMovingAfterHit()
    {
        yield return new WaitForSeconds(0.1f);
        UpdateTarget();
        move = true;
    }

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
    public void GetSpeedUp(GameObject otherGameObject)
    {
        otherGameObject.transform.parent = transform;
        otherGameObject.transform.DOLocalJump(Vector3.zero, 2, 1, 0.5f);
        otherGameObject.transform.DOScale(otherGameObject.transform.localScale * 0.5f, 0.5f).OnComplete(() =>
        {
            StartCoroutine(BoostSpeedForAWhile());
            GameManager.Instance.powerUpObjectsList.Remove(otherGameObject);
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
