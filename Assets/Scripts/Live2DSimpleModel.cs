using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using live2d;

public class Live2DSimpleModel : MonoBehaviour
{
    public TextAsset modelFile;
    public Texture2D texture;
    public TextAsset idleMotionFile;
    public GameObject missCui;

    private Live2DModelUnity model;
    private Matrix4x4 live2DCanvasPos;

    private Live2DMotion motionIdle;
    private MotionQueueManager motionQueueManager;

    private EyeBlinkMotion eyeBlinkMotion;

    public float moveSpeed=5;

    private Vector3 initPos;
    public bool isDefeat;
    public int hitCount;
    void Start()
    {
        Live2D.init();
        model = Live2DModelUnity.loadModel(modelFile.bytes);
        model.setTexture(0, texture);
        float modelWidth = model.getCanvasWidth();
        live2DCanvasPos = Matrix4x4.Ortho(0, modelWidth, modelWidth, 0,-50,50);

        motionIdle = Live2DMotion.loadMotion(idleMotionFile.bytes);
        motionIdle.setLoop(true);

        motionQueueManager = new MotionQueueManager();
        eyeBlinkMotion = new EyeBlinkMotion();
        motionQueueManager.startMotion(motionIdle);
        initPos = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        model.setMatrix(transform.localToWorldMatrix * live2DCanvasPos);
        motionQueueManager.updateParam(model);
        eyeBlinkMotion.setParam(model);

        model.update();
        if (GameManager.Instance.isGameOver)
        {
            return;
        }
        if ((missCui.transform.position.x-transform.position.x) < 3)
        {
            GameManager.Instance.isGameOver = true;
            GameManager.Instance.isCatched = true;
        }
        if (isDefeat)
        {
            transform.position = Vector3.Lerp(transform.position, initPos, 0.1f);
            Destroy(gameObject);
        }
        else
        {
            transform.Translate(Vector3.right*moveSpeed*Time.deltaTime);
        }
    }

    private void OnRenderObject()
    {
        model.draw();
    }
    private void OnMouseDown()
    {
        if (GameManager.Instance.isGameOver) { return; }
        if (hitCount >= 20)
        {
            isDefeat = true;
            GameManager.Instance.DefeatBadBoy();
        }
        else
        {
            hitCount++;
        }

    }
}
