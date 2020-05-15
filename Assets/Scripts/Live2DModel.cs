using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using live2d;
using live2d.framework;

public class Live2DModel : MonoBehaviour
{
    public TextAsset modelFile;
    public Texture2D[] textures;
    public TextAsset[] motionFiles;

    private Live2DMotion[] motions;
    //0.idle 1.点头 2.摇头 3.左摇 4.开心 5.生气 6.失落 7.做错事 8.做错事失落 9.叉腰抛媚眼
    //10.惊讶 11.害羞 出眼泪 12.很生气 13.逃避 14.害羞

    private Live2DModelUnity live2DModel;

    private Matrix4x4 live2DCanvasPos;

    private L2DMotionManager motionManager;
    //优先级的设置标准
    //1.动作未进行的状态，优先级为0
    //2.idle发生时，优先级为1
    //3.其他动作发生时，优先级为2
    //4.无视优先级，强制发生的动作，优先级为3

    //private MotionQueueManager motionQueueManager;
    //private MotionQueueManager motionQueueManagerB;

    private int motionIndex = 0;
    //private Live2DMotion live2DMotionIdle;

    //private Dictionary<string, Live2DMotion> dict;

    //自动眨眼
    private EyeBlinkMotion eyeBlinkMotion;

    //鼠标拖拽引起变化
    private L2DTargetPoint drag;

    //套用物理运算的设定
    private PhysicsHair sideHairLeft;
    private PhysicsHair sideHairRight;
    private PhysicsHair backHairLeft;
    private PhysicsHair backHairRight;

    //表情
    public TextAsset[] expressionFiles;
    public L2DExpressionMotion[] expressions;
    private MotionQueueManager expressionManager;
    void Start()
    {
        //初始化
        Live2D.init();

        //读取模型
        //Live2DModelUnity.loadModel(Application.dataPath+"/Resources/Epsilon/runtime/Epsilon.moc");

        //第二种读取形式
        //TextAsset mocFile = Resources.Load<TextAsset>("Epsilon/runtime/Epsilon.moc");
        live2DModel= Live2DModelUnity.loadModel(modelFile.bytes);
        #region
        //与贴图建立关联
        //Texture2D texture2D1 = Resources.Load<Texture2D>("Epsilon/runtime/Epsilon.1024/texture_00");
        //Texture2D texture2D2 = Resources.Load<Texture2D>("Epsilon/runtime/Epsilon.1024/texture_01");
        //Texture2D texture2D3 = Resources.Load<Texture2D>("Epsilon/runtime/Epsilon.1024/texture_02");

        //live2DModel.setTexture(0,texture2D1);
        //live2DModel.setTexture(1,texture2D2);
        //live2DModel.setTexture(2,texture2D3);
        #endregion
        for (int i = 0; i < textures.Length; i++)
        {
            live2DModel.setTexture(i, textures[i]);
        }

        //指定显示位置与尺寸
        float modelWidth = live2DModel.getCanvasWidth();

        live2DCanvasPos = Matrix4x4.Ortho(0,modelWidth,modelWidth,0,-50,50);

        //播放动作
        //实例化动作
        //live2DMotionIdle = Live2DMotion.loadMotion(Application.dataPath + "/Resources/Epsilon/runtime/motions/Epsilon_idle_01");
        motions = new Live2DMotion[motionFiles.Length];
        for (int i = 0; i < motionFiles.Length; i++)
        {
            motions[i] = Live2DMotion.loadMotion(motionFiles[i].bytes);
        }
        #region
        //设置某一动画的一些属性
        //重复播放不淡入
        motions[0].setLoopFadeIn(false);
        motions[0].setFadeOut(1000);//毫秒
        motions[0].setFadeIn(1000);
        motions[0].setLoop(true);

        //motionQueueManager = new MotionQueueManager();
        //motionQueueManager.startMotion(motions[motionIndex]);

        //motions[5].setLoop(true);
        //motionQueueManagerB = new MotionQueueManager();
        //motionQueueManagerB.startMotion(motions[5]);
        #endregion

        //动作优先级使用
        motionManager = new L2DMotionManager();

        //眨眼
        eyeBlinkMotion = new EyeBlinkMotion();

        drag = new L2DTargetPoint();
        #region 头发物理效果处理
        sideHairLeft = new PhysicsHair();
        sideHairRight = new PhysicsHair();
        backHairLeft = new PhysicsHair();
        backHairRight = new PhysicsHair();

        //套用物理运算
        sideHairLeft.setup(0.2f,0.5f,0.14f);
        sideHairRight.setup(0.2f, 0.5f, 0.14f);
        PhysicsHair.Src srcX = PhysicsHair.Src.SRC_TO_X;//横向摇摆
        PhysicsHair.Src srcZ = PhysicsHair.Src.SRC_TO_G_ANGLE;
        
        sideHairLeft.addSrcParam(srcX, "PARAM_ANGLE_X", 0.005f, 1);
        sideHairRight.addSrcParam(srcX, "PARAM_ANGLE_X", 0.005f, 1);

        backHairLeft.setup(0.24f,0.5f,0.18f);
        backHairRight.setup(0.24f, 0.5f, 0.18f);

        backHairLeft.addSrcParam(srcX, "PARAM_ANGLE_X", 0.005f, 1);
        backHairLeft.addSrcParam(srcZ, "PARAM_ANGLE_Z", 0.8f, 1);
        backHairRight.addSrcParam(srcX, "PARAM_ANGLE_X", 0.005f, 1);
        backHairRight.addSrcParam(srcZ, "PARAM_ANGLE_Z", 0.8f, 1);

        //设置输出表现
        PhysicsHair.Target target = PhysicsHair.Target.TARGET_FROM_ANGLE;
        sideHairLeft.addTargetParam(target, "PARAM_HAIR_SIDE_L",0.005f,1);
        sideHairRight.addTargetParam(target, "PARAM_HAIR_SIDE_R", 0.005f, 1);
        backHairLeft.addTargetParam(target, "PARAM_HAIR_BACK_L", 0.005f, 1);
        backHairRight.addTargetParam(target, "PARAM_HAIR_BACK_R", 0.005f, 1);
        #endregion

        //表情
        expressionManager = new MotionQueueManager();
        expressions = new L2DExpressionMotion[expressionFiles.Length];
        for(int i=0; i < expressionFiles.Length; i++)
        {
            expressions[i] = L2DExpressionMotion.loadJson(expressionFiles[i].bytes);
        }
    }


    void Update()
    {
        live2DModel.setMatrix(transform.localToWorldMatrix*live2DCanvasPos);
        #region
        //if (Input.GetMouseButtonDown(0))
        //{
        //    motionIndex++;
        //    if (motionIndex >= motions.Length)
        //    {
        //        motionIndex = 0;
        //    }
        //    motionQueueManager.startMotion(motions[motionIndex]);
        //}

        //motionQueueManager.updateParam(live2DModel);
        //motionQueueManagerB.updateParam(live2DModel);
        

        //判断待机动作
        //if (motionManager.isFinished())
        //{
        //    StartMotion(0, 1);
        //}
        //else if (Input.GetKeyDown(KeyCode.Space))
        //{
        //    StartMotion(14, 2);
        //}
        //motionManager.updateParam(live2DModel);

        //设置参数
        // live2DModel.setParamFloat(2, 30, 1);
        //参数的保存与回复  整个模型的参数
        //live2DModel.saveParam();
        //live2DModel.loadParam();

        //设定模型某一部分的透明度
        //live2DModel.setPartsOpacity("PARTS_01_EYE_BALL_001", 0);
#endregion
        //眨眼
        eyeBlinkMotion.setParam(live2DModel);

        //获取鼠标位置
        Vector3 pos = Input.mousePosition;
        if (Input.GetMouseButton(0))
        {   //通过公式转换得到live2d鼠标检测点的比例值是-1到1
            //通过这个值取设置我们的参数
            //按照这个值带来的影响度来影响模型动作
            drag.Set(pos.x/Screen.width*2-1,pos.y/Screen.height*2-1);
        }
        else
        {
            drag.Set(0, 0);
        }
        drag.update();

        //模型转向
        if (drag.getX() != 0)
        {
            live2DModel.setParamFloat("PARAM_ANGLE_X", 30 * drag.getX());
            live2DModel.setParamFloat("PARAM_ANGLE_Y", 30 * drag.getY());
            live2DModel.setParamFloat("PARAM_EYE_BALL_X",  drag.getX());//取-值眼睛只盯着玩家
            live2DModel.setParamFloat("PARAM_EYE_BALL_Y",  drag.getY());
            live2DModel.setParamFloat("PARAM_BODY_ANGLE_X",10* drag.getX());

        }

        long time = UtSystem.getUserTimeMSec();

        sideHairLeft.update(live2DModel,time);
        sideHairRight.update(live2DModel, time);
        backHairLeft.update(live2DModel, time);
        backHairRight.update(live2DModel, time);

        //表情使用
        if (Input.GetMouseButtonDown(0))
        {
            motionIndex++;
            if (motionIndex >= expressions.Length)
            {
                motionIndex = 0;
            }
            expressionManager.startMotion(expressions[motionIndex]);
        }

        expressionManager.updateParam(live2DModel);


        //更新模型状态
        live2DModel.update();
    }

    private void OnRenderObject()
    {
        //绘制模型
        live2DModel.draw();
    }

    private void StartMotion(int motionIndex,int priority)
    {
        if (motionManager.getCurrentPriority() >= priority)
        {
            return;
        }
        motionManager.startMotion(motions[motionIndex]);
    }
}
