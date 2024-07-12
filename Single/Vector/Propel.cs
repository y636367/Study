using System.Collections;
using UnityEngine;

public class Propel : MonoBehaviour
{
    [SerializeField]
    private Quaternion isRotateValue;                                                   // �� ������Ʈ�� ȸ����
    [SerializeField]
    private bool anotherValue;                                                          // Ư�� ȸ������ �־���ϴ°�?
    [SerializeField]
    private float rotationspeed;                                                        // ȸ�� �ӵ�

    [Tooltip("0-x 1-y 2-z")]
    [SerializeField]
    private int Axis_rotation;                                                          // ȸ���ϰ��� �ϴ� ��
    private void Awake()
    {
        if (anotherValue)
            transform.rotation = isRotateValue;
    }
    private void Start()
    {
        StartCoroutine(nameof(rotate));
    }
    private IEnumerator rotate()
    {
        while(true)
        {
            if (GameManager.instance.isPause)
                yield return null;

            switch (Axis_rotation)
            {
                case 0:
                    transform.Rotate(Vector3.right, rotationspeed * Time.deltaTime);
                    break;
                case 1:
                    transform.Rotate(Vector3.up, rotationspeed * Time.deltaTime);
                    break;
                case 2:
                    transform.Rotate(Vector3.forward, rotationspeed * Time.deltaTime);
                    break;
            }

            yield return null;
        }
    }
}
