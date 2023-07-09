using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpeedUpManager : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        transform.localScale = Vector3.one * 0.2f;
        ScaleLoop();
        transform.DOScale(Vector3.one * 0.7f, 0.3f);

    }

    // Update is called once per frame
    void Update()
    {
        transform.Rotate(new Vector3(0, 1, 0), 0.5f);
    }
    public void ScaleLoop()
    {
        transform.DOScale(Vector3.one * 0.7f, 0.7f).OnComplete(() =>
        {
            transform.DOScale(Vector3.one * 0.5f, 0.5f).OnComplete(() => ScaleLoop());
        });
    }
}
