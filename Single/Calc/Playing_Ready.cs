using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using BackEnd;

public class Playing_Ready : MonoBehaviour
{
    [SerializeField]
    private Progress progress;                                  // ���൵�� ���� Off_ ��ư Ȱ��ȭ ���θ� ���� ��ũ��Ʈ
    [SerializeField]
    private SceneNames nextScene;                               // ���� ������ �Ѿ�� ������ �� �̸�

    [SerializeField]
    private Text Guide_text;                                    // �ȳ� �ؽ�Ʈ
    [SerializeField]
    private Loadding_Monster LM;                                // �ε� �� ǥ�õ� ����
    [SerializeField]
    private Button Off_;                                        // �ε��� �Ϸ�� �� �����׼ǰ��� �Է¹޾� �ൿ�ϱ� ���� ��ư

    [SerializeField]
    private float Text_blink;                                   // Text�� ��ũ�� �ӵ��ݿ� ��

    [Header("Loadding_Sound")]                                  // �ε� �Ϸ�� ����� ����
    [SerializeField]
    private string Loadding_Sound;

    private void Awake()
    {
        Setup_init();                                           // �ʱ� ����
        FrameRate_Setting();                                    // ������ ����
        Off_.enabled = false;                                   // ��ư ��Ȱ��ȭ
    }
    private void Setup_init()
    {
        Application.runInBackground = true;                     // ��Ȱ��ȭ������ ���� ��� ����

        Screen.sleepTimeout = SleepTimeout.NeverSleep;          // ȭ���� ������ �ʰ� ����

        progress.Play(OnAfterProgress);                         // Progress�� Play�Լ� �� ����� �׼�
    }
    private void OnAfterProgress()                              
    {
        Changed_Monster();                                      // �ε��� �Ϸ�� �� ó�� �Լ�_1
        Next_();           

        //���� ����ȭ
    }
    private void Changed_Monster()
    {
        Guide_text.text = "Touch to Start!";                                        // ���� Now Loadding... �ؽ�Ʈ�� ��ȯ

        LM.transform.GetChild(0).GetComponent<Animator>().SetTrigger("Death");      // �ε��� �Ϸ������ �ð������� ǥ���ϱ� ���� ǥ�õ� ���� ��� ó��

        StartCoroutine(Text_Blink_out());                                           // �ε��� �Ϸ������ �ð������� ǥ���ϱ� ���� �ؽ�Ʈ ������ ȿ���� ���� �ڷ�ƾ ����
    }
    /// <summary>
    /// �ؽ�Ʈ�� ���İ��� 0���� Ŭ�� ���� ����
    /// </summary>
    private IEnumerator Text_Blink_out()
    {
        while (Guide_text.color.a > 0)                                              
        {
            Guide_text.color=new Color(Guide_text.color.r, Guide_text.color.g,
                Guide_text.color.b,Guide_text.color.a-(Time.deltaTime*Text_blink));

            yield return null;
        }

        StartCoroutine(Text_Blink_in());
    }
    /// <summary>
    /// �ؽ�Ʈ�� ���İ��� 1���� ������ ���� ����
    /// </summary>
    private IEnumerator Text_Blink_in()
    {
        while (Guide_text.color.a < 1)
        {
            Guide_text.color = new Color(Guide_text.color.r, Guide_text.color.g,
                Guide_text.color.b, Guide_text.color.a + (Time.deltaTime * Text_blink));

            yield return null;
        }

        StartCoroutine(Text_Blink_out());
    }
    private void Next_()
    {
        Off_.enabled = true;                                    // ���� �׼� ������ ���� ��ư Ȱ��ȭ
        Off_.onClick.AddListener(Loadding_sound);               // ��ư�� �׼� �߰�(Ŭ�� �� ��ġ�� ���� ���)
        Off_.onClick.AddListener(Scene_Change);                 // ��ư�� �׼� �߰�(�� ����)
    }
    private void Loadding_sound()
    {
        soundManager.Instance.PlaySoundEffect(Loadding_Sound);
    }
    public void Scene_Change()
    {
        Quit_Game.Instance.Limit = false;                                               // �ڷΰ��� ��ư �� Escape Ŭ�� �� ��ġ �� ������ �̺�Ʈ ������ ���� ���� �� ����

        string message = string.Empty;                                                  // ���� ���� �� �ȳ� �޽��� ������ ���� ���� ����

        bool autoLoginEnabled = PlayerPrefs.GetInt("AutoLogin") == 1;                   // PlayerPrefs�� ���� ��⿡ ����� �ڵ��α��� ���� �� �޾ƿ���
        PlayerPrefs.Save();                                                             // PlayerPrefs ����

        if (autoLoginEnabled)                                                           // �ڵ� �α����� ���̶��
        {
            SendQueue.Enqueue(Backend.BMember.LoginWithTheBackendToken, callback =>     // ����(�ڳ�) �񵿱� ť�� ��ȣ ����
            {
                if (callback.IsSuccess())                                               // ��ȣ �۽� ���� ��
                {
                    message = Backend_GameData.Instance.GetDatas();

                    if (message != "")                                                  // User�� ������ �ҷ�����, �Ľ� �˻�(������ �ҷ����� Ȯ��) ���� üũ
                    {
                        Utils.Instance.LoadScene(SceneNames.Loadding);                  // ���� �߻� �ʱ�ȭ������ �̵�
                        return;
                    }
                    else
                    {
                        Utils.Instance.LoadScene(nextScene + 1);                        // �ٷ� Main������ �̵�
                    }
                }
                else
                {
                    Utils.Instance.LoadScene(nextScene);                                // ��ȣ �۽� ����, Loggin ������ �̵�
                }
            });
        }
        else
        {
            Utils.Instance.LoadScene(nextScene);                                        // �ڵ� �α��� ���� �̶��, Loggin ������ �̵�
        }
    }
    private void FrameRate_Setting()
    {
        Application.targetFrameRate = 70;                       // �ּ� ������ �ӵ� ����

                                                                // �ִ� ������ �ӵ� ����
        QualitySettings.vSyncCount = 0;                         // ���� ����ȭ ��Ȱ��ȭ
    }
}
