using UnityEngine;

public class Panel_Registration : MonoBehaviour
{
    /// <summary>
    /// �ش� ��ũ��Ʈ Ȱ��ȭ �ɶ����� Quit�� �г� List�� (�ش� ��ũ��Ʈ ������Ʈ�� �߰��Ǿ��ִ�) ���� ������Ʈ �߰�
    /// </summary>
    private void OnEnable()
    {
        Sounds.instance.WindowOpen_Sound();
        Quit_Game.Instance.Panel_In(this.gameObject);
    }
}
