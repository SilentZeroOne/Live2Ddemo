using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using live2d;

public class GameManager : MonoBehaviour
{
    private static GameManager instance;
    public static GameManager Instance
    {
        get { return instance; }
    }

    //玩家属性
    public int gold;
    public int favor;
    public int leftDays;

    public Text goldText;
    public Text favorText;
    public Text dateText;

    //private GameObject player;
    public LAppModelProxy modelProxy;
    public GameObject actionBtns;
    public GameObject gameOverBtns;
    public Live2DSimpleModel badBoyScript;
    public GameObject clickEffect;
    public Canvas canvas;
    public Texture2D missCuiNewCloth;
    //工作
    public GameObject workBtns;
    public Image workImg;
    public GameObject workUI;
    public Sprite[] workSprites;
    //聊天
    public GameObject chatUI;

    //约会
    public SpriteRenderer bgImg;
    public Sprite[] dateSprites;

    public GameObject talkLine;
    public Text talkText;
    public GameObject battleLog;
    public GameObject lastBtn;

    private AudioSource audioSource;
    public AudioClip[] clips;


    public Image mask;
    public bool toAnotherDay;
    private bool toBeDay;
    public bool isCatched;
    public bool isGameOver;
    private void Awake()
    {
        if (instance == null)
            instance = this;
        gold = favor = 0;
        leftDays = 20;
        //player = GameObject.FindGameObjectWithTag("Player");
        //modelProxy = player.GetComponent<LAppModelProxy>();
        UpdateUI();

        audioSource=GetComponent<AudioSource>();
        audioSource.loop = true;
        audioSource.clip = clips[0];
        audioSource.Play();
    }


    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Vector2 localPos;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(canvas.transform as RectTransform, Input.mousePosition, canvas.worldCamera, out localPos);
            GameObject go = Instantiate(clickEffect);
            go.transform.SetParent(canvas.transform);
            go.transform.localPosition = localPos;
            Destroy(go, 1);
        }

        if (isGameOver)
        {
            talkLine.SetActive(true);
            actionBtns.SetActive(false);
            gameOverBtns.SetActive(true);
            lastBtn.SetActive(false);
            if (favor >= 1500)
            {
                talkText.text = "二狗子终于追到了村里最漂亮的女孩翠花\n最后他们终于幸福的在一起了！";
            }else if (leftDays != 0&&isCatched)
            {
                talkText.text = "二狗子未能在流氓手中保护住翠花\n他们最终决裂了";
            }
            else
            {
                talkText.text = "二狗子在翠花出国前未能获取她的芳心\n他们最后没能在一起";
            }
        }

        if (toAnotherDay)
        {
            if (toBeDay)
            {
                Invoke("ToDay", 2);
            }
            else
            {
                ToDark();
            }
        }
    }

    public void ToBeDark()
    {
        toAnotherDay = true;
    }

    public void ToDark()
    {
        mask.color +=new Color(0,0,0, Mathf.Lerp(0, 1,0.005f));
        if (mask.color.a >= 0.8f)
        {
            mask.color = new Color(0, 0, 0,1);
            toBeDay = true;
            audioSource.clip = clips[0];
            audioSource.Play();
            ResetUI();
            UpdateUI();
        }
    }
    public void ToDay()
    {
        mask.color -= new Color(0, 0, 0, Mathf.Lerp(0,1, 0.005f));
        if (mask.color.a <= 0.2f)
        {
            mask.color = new Color(0, 0, 0, 0);
            toAnotherDay = false;
            toBeDay = false;

            //UpdateUI();
            
        }
    }

    public void CilckWorkBtn()
    {
        modelProxy.SetVisible(false);
        actionBtns.SetActive(false);
        workBtns.SetActive(true);
        PlayButtonSound();
        audioSource.clip = clips[2];
        audioSource.Play();
    }

    public void GetMoneny(int workIndex)
    {
        audioSource.PlayOneShot(clips[6]);
        workBtns.SetActive(false);
        ChangeGold((4 - workIndex) * 20);
        workUI.SetActive(true);
        workImg.sprite = workSprites[workIndex];
        talkLine.SetActive(true);
        talkText.text = "二狗子通过努力工作，挣得了" + ((4 - workIndex) * 20).ToString() + "元!";
    }

    public void ClickChatBtn()
    {
        PlayButtonSound();
        audioSource.clip = clips[1];
        audioSource.Play();
        actionBtns.SetActive(false);
        chatUI.SetActive(true);
        if (favor>=100)
        {
            modelProxy.GetModel().StartMotion("tap_body", 1, 2);
        }
        else
        {
            modelProxy.GetModel().StartMotion("tap_body",0,2);
        }
    }

    public void GetFavor(int chatIndex)
    {
        chatUI.SetActive(false);
        talkLine.SetActive(true);
        switch (chatIndex)
        {
            case 0:
                if (favor > 20)
                {
                    ChangeFavor(10);
                    talkText.text = "嘻嘻，谢谢，我也恰巧想看看你(*^_^*)";
                    modelProxy.GetModel().SetExpression("f05");
                    audioSource.PlayOneShot(clips[7]);
                }
                else
                {
                    ChangeFavor(2);
                    talkText.text = "哦，谢谢你哦。。。";
                    modelProxy.GetModel().SetExpression("f02");
                    audioSource.PlayOneShot(clips[7]);
                }
                break;
            case 1:
                if (favor > 60)
                {
                    ChangeFavor(20);
                    talkText.text = "啊。。嗯 谢谢哈。。";
                    modelProxy.GetModel().SetExpression("f07");
                    audioSource.PlayOneShot(clips[7]);
                }
                else
                {
                    ChangeFavor(-20);
                    talkText.text = "你 你干嘛啊 别动我!";
                    modelProxy.GetModel().SetExpression("f03");
                }
                break;
            case 2:
                if (favor > 100)
                {
                    ChangeFavor(40);
                    talkText.text = "那。。。咱们一起去吃饭，下午去哪玩?";
                    modelProxy.GetModel().SetExpression("f02");
                    audioSource.PlayOneShot(clips[7]);
                }
                else
                {
                    ChangeFavor(-40);
                    talkText.text = "你这人说话怎么这样啊，我又没得罪你。";
                    modelProxy.GetModel().SetExpression("f04");
                }
                break;
        }
    }

    public void CilckDateBtn()
    {
        PlayButtonSound();
        audioSource.clip = clips[3];
        audioSource.Play();
        actionBtns.SetActive(false);
        talkLine.SetActive(true);
        int randomNum = Random.Range(1, 4);
        bool hasEnoughGold = false;
        bgImg.sprite = dateSprites[randomNum];
        switch (randomNum)
        {
            case 1:
                if (gold >= 50)
                {
                    ChangeGold(-50);
                    ChangeFavor(150);
                    talkText.text = "学校门口原来有这么多好玩的地方，\n今天谢谢你了，二狗子。";
                    hasEnoughGold = true;
                }
                else
                {
                    talkText.text = "没事，不要在意，我最近零花钱比较多";
                    ChangeFavor(-50);
                }
                break;
            case 2:
                if (gold >= 150)
                {
                    ChangeGold(-150);
                    ChangeFavor(300);
                    talkText.text = "蟹黄汤包和烤鸭真的太好吃了，\n谢谢招待！";
                    hasEnoughGold = true;
                }
                else
                {
                    ChangeFavor(-150);
                    talkText.text = "下次你请我吃饭吧。";
                }
                break;
            case 3:
                if (gold >= 300)
                {
                    ChangeGold(-300);
                    ChangeFavor(500);
                    talkText.text = "今天真的很开心，\n还有谢谢你送的礼物，(●'◡'●)";
                    hasEnoughGold = true ;
                }
                else
                {
                    talkText.text = "那个娃娃真的好可爱哦，好像要啊。。。";
                    ChangeFavor(-300);
                }
                break;
            default:break;
        }
        if (hasEnoughGold)
        {
            modelProxy.GetModel().StartMotion("pinch_in",0,2);
            audioSource.PlayOneShot(clips[7]);
        }
        else
        {
            modelProxy.GetModel().StartMotion("flick_head", 0, 2);
        }
    }
    public void ClickLoveBtn()
    {
        PlayButtonSound();
        audioSource.clip = clips[4];
        audioSource.Play();
        actionBtns.SetActive(false);
        talkLine.SetActive(true);
        if (favor >= 1500)
        {
            talkText.text = "其实我也喜欢你很久了，二狗子。\n希望我们可以永远在一起!";
            modelProxy.GetModel().StartMotion("pinch_out", 0, 2);
            modelProxy.GetModel().SetExpression("f07");
            isGameOver = true; 
        }
        else
        {
            talkText.text = "二狗子，你。。你\n干嘛这么突然表白啊，我。。我们还不够了解彼此。";
            modelProxy.GetModel().StartMotion("shake", 0, 2);
            modelProxy.GetModel().SetExpression("f04");
        }
    }
    //更新玩家属性显示
    private void UpdateUI()
    {
        goldText.text = gold.ToString();
        favorText.text = favor.ToString();
        dateText.text = leftDays.ToString();
    }

    public void ChangeGold(int goldVal)
    {
        gold += goldVal;
        if (gold <= 0)
        {
            gold = 0;
        }
        UpdateUI();
    }
    public void ChangeFavor(int favorVal)
    {
        favor += favorVal;
        if (favor <= 0)
        {
            favor = 0;
        }
        UpdateUI();
    }

    private void ResetUI()
    {
        workUI.SetActive(false);
        talkLine.SetActive(false);
        actionBtns.SetActive(true);
        modelProxy.SetVisible(true);
        modelProxy.GetModel().SetExpression("f01");
        bgImg.sprite = dateSprites[0];
        leftDays--;
        if (leftDays == 5)
        {
            Invoke("CreateBadBoy", 2);
        }
        else if (leftDays == 10)
        {
            Live2DModelUnity live2DModelUnity = modelProxy.GetModel().GetLive2DModelUnity();
            live2DModelUnity.setTexture(2, missCuiNewCloth);
        }
        else if (leftDays == 0)
        {
            isGameOver = true;
        }
    }

    private void CreateBadBoy()
    {
        modelProxy.isRunningAway = true;
        badBoyScript.gameObject.SetActive(true);
        modelProxy.GetModel().SetExpression("f04");
        actionBtns.SetActive(false);
        battleLog.SetActive(true);
        audioSource.clip = clips[5];
        audioSource.Play();
    }

    public void CloseBattleLog()
    {
        battleLog.SetActive(false);
        PlayButtonSound();
    }
    public void DefeatBadBoy()
    {
        modelProxy.GetModel().StartMotion("shake", 0, 2);
        talkLine.SetActive(true);
        talkText.text = "刚才吓死我了，谢谢你及时救我，二狗子";
        ChangeFavor(300);
    }
    public void LoadScene(int sceneNum)
    {
        SceneManager.LoadScene(sceneNum);
        PlayButtonSound();
    }

    public void PlayButtonSound()
    {
        audioSource.PlayOneShot(clips[8]);
    }
    
}
