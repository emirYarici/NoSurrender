using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScaleUpDown : MonoBehaviour
{
    // Start is called before the first frame update
    Vector3 originalScale;
    void Start()
    {
        originalScale = transform.localScale;
        MoveLoop();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void MoveLoop()
    {
        transform.DOScale(originalScale * 1.2f, 0.4f).OnComplete(() =>
        {
            transform.DOScale(originalScale,0.4f).OnComplete(() => MoveLoop());
        });
    }
}
