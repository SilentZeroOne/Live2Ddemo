using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectSpawn : MonoBehaviour
{
    public GameObject[] effects;
    public Transform canvasTF;

    public int randomAngleMin;
    public int randomAngleMax;
    //private Vector3 startPos;

    void Start()
    {
        InvokeRepeating("CreateEffect", 0, 2);
       
    }

    
    void Update()
    {
       
    }

    private void CreateEffect()
    {
        int randomIndex = Random.Range(0, 2);
        transform.rotation = Quaternion.Euler(new Vector3(0,0,Random.Range(randomAngleMin,randomAngleMax)));
        GameObject effectGo = Instantiate(effects[randomIndex],transform.position,transform.rotation);
        effectGo.transform.SetParent(canvasTF);
    }
}
