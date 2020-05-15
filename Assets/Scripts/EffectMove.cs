using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectMove : MonoBehaviour
{
    private float moveSpeed=100;
    private float timeVal;
    private int ramdomYPos;
    // Start is called before the first frame update
    void Start()
    {
        Destroy(gameObject, 8);
    }

    // Update is called once per frame
    void Update()
    {
        transform.Translate(-transform.right * moveSpeed*Time.deltaTime);
        if (timeVal >= 1)
        {
            timeVal = 0;
            ramdomYPos = Random.Range(-1, 2);
        }
        else
        {
            transform.Translate(transform.up * ramdomYPos * Time.deltaTime*moveSpeed/5);
            timeVal += Time.deltaTime;
        }
    }
}
