using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ChatController : MonoBehaviour
{
    static public ChatController instance;

    [SerializeField]
    private TMP_Text ChatText;

    [SerializeField]
    public GameObject Script_Zone = null;
    [SerializeField]
    private TMP_Text To_Do = null;
    [SerializeField]
    private GameObject To_Title = null;
    [SerializeField]
    public TMP_Text Key_Text = null;

    public GameObject Collision_Zone;

    private string writerText = "";

    private string[] strings;
    private bool do_Check;
    private string[] list_do;

    public int t_num = 0;

    //타이핑 체크
    bool text_full;

    bool Not_Destroy = false;

    [SerializeField]
    private float delay;
    [SerializeField]
    private float Skip_delay;

    [SerializeField]
    private KeyCode next_Chat; //자막 넘기기

    bool isButtonClicked = false;

    public bool Scene_ = false;

    bool Story_On = false;

    public bool Script_On=false;

    private Player player;
    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
            Destroy(this.gameObject);

        DontDestroyOnLoad(this.gameObject);

        next_Chat = KeySetting.keys[(KeyAction)8];
    }
    void Start_()
    {
        Script_Zone.SetActive(false);
        To_Do.gameObject.SetActive(false);
        To_Title.SetActive(false);
    }

    void Update()
    {
        if (!GameManager.Instance.Option_)
        {
            if (Input.GetKeyDown(next_Chat))
            {
                isButtonClicked = true;
            }
            else if (Input.GetKeyUp(next_Chat))
                isButtonClicked = false;
        }

        if (!Scene_Check())
        {
            if (Script_Zone.activeSelf == true)
            {
                Script_On = true;
            }
            else
                Script_On = false;
        }
    }
    public void turnOn(GameObject t_Zone, string[] t_do, bool Check)
    {
        Script_Zone.SetActive(true);
        Collision_Zone = t_Zone;
        do_Check= Check;
        if (do_Check)
        {
            list_do=new string[t_do.Length];
            for(int i=0; i<t_do.Length;i++)
            {
                list_do[i] = t_do[i];
            }
        }
        StartCoroutine(TextPractice());
    }
    public void turnOn(GameObject t_Zone)
    {
        Script_Zone.SetActive(true);
        Collision_Zone = t_Zone;
        Not_Destroy = true;
        StartCoroutine(TextPractice());
    }
    public void turnOn_Story()
    {
        Script_Zone.SetActive(true);
        Not_Destroy = true;
        StartCoroutine(TextPractice());
    }
    IEnumerator NormalChat(string narration)
    {
        text_full = false;

        writerText = "";

        //텍스트 타이핑 효과
        for (int a = 0; a < narration.Length; a++)
        {
            writerText += narration[a];
            ChatText.text = writerText;

            if (isButtonClicked)
                break;
            yield return new WaitForSeconds(delay);
        }
        ChatText.text = narration;
        yield return new WaitForSeconds(Skip_delay);

        text_full = true;
        //키를 다시 누를 때 까지 무한정 대기
        while (true)
        {
            if (isButtonClicked&&text_full==true)
            {
                isButtonClicked = false;
                break;
            }
            yield return null;
        }
    }

    IEnumerator TextPractice()
    {
        int k = 0;

        while (k != strings.Length)
        {
            yield return StartCoroutine(NormalChat(strings[k]));
            k++;
        }

        if (Story_On==false)
            if (Not_Destroy)
            {
                yield return turnOff_B();
            }
            else
                yield return turnOff_A();
        else
            Return_mode();
    }
    IEnumerator turnOff_A()
    {
        Script_Zone.SetActive(false);
        if (do_Check)
        {
            To_Title.SetActive(true);
            To_Do.gameObject.SetActive(true);
            for (int i=0;i<list_do.Length;i++)
            {
                To_Do.text = list_do[i];
            }
            if (t_num == 2)
            {
                GameManager.Instance.Object_list_On();
                t_num = 0;
            }
        }
        if (this.Scene_ == true)
        {
            Scene_ = false;
            Story_On = true;
            StartCoroutine(sceneManager.Instance.Camera_Fade_Out());
        }
        else
        {
            player.Move_Ok = true;
            player.Now_Chat = false;
        }
        Collision_Zone.SetActive(false);
        yield return null;
    } 
    IEnumerator turnOff_B()
    {
        Script_Zone.SetActive(false);
        player.Move_Ok = true;
        player.Now_Chat = false;
        Not_Destroy = false;
        yield return null;
    }
    void Return_mode()
    {
        Script_Zone.SetActive(false);
        Story_On= false;
        Not_Destroy= false;
        StartCoroutine(sceneManager.Instance.Story_Fade_Out());
    }
    public void GetScribt(string[] t_strings)
    {
        strings=new string[t_strings.Length];
        for(int i=0;i<t_strings.Length;i++)
        {
            strings[i] = t_strings[i];
        }
    }
    public void Get_player(Player t_player)
    {
        player = t_player;
    }
    void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (!Scene_Check())
        {
            MatchUP();
        }
        else
            Match_Null();

    }
    void OnDisable()
    {
        // 델리게이트 체인 제거
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
    private bool Scene_Check()
    {
        if (sceneManager.Instance.Start_Check)
            return true;
        else
            return false;
    }
    private void MatchUP()
    {
        Script_Zone = GameObject.FindGameObjectWithTag("Script_Zone");
        ChatText = GameObject.FindGameObjectWithTag("Chat_Text").GetComponent<TMP_Text>();
        To_Do = GameObject.FindGameObjectWithTag("To_Do").GetComponent<TMP_Text>();
        To_Title = GameObject.FindGameObjectWithTag("To_Title");
        Key_Text = GameObject.FindGameObjectWithTag("Key_Text").GetComponent<TMP_Text>();

        Key_Text.text = "[" + (next_Chat == KeyCode.Mouse0 ? "LeftMouse" :
                next_Chat == KeyCode.Mouse1 ? "RightMouse" :
                next_Chat == KeyCode.Mouse2 ? "MouseWheel" : next_Chat.ToString()) + "]";

        Start_();
    }
    public void Clean_List()
    {
        To_Do.text = null;
        To_Do.gameObject.SetActive(false);
        To_Title.SetActive(false);
    }
    private void Match_Null()
    {
        Script_Zone = null;
        ChatText = null;
        To_Do = null;
        To_Title = null;
        Collision_Zone = null;
    }
    public void Set_Key()
    {
        next_Chat = KeySetting.keys[(KeyAction)8];

        try
        {
            Key_Text.text = "[" + (next_Chat == KeyCode.Mouse0 ? "LeftMouse" :
                    next_Chat == KeyCode.Mouse1 ? "RightMouse" :
                    next_Chat == KeyCode.Mouse2 ? "MouseWheel" : next_Chat.ToString()) + "]";
        }
        catch (NullReferenceException) { }
    }
}
