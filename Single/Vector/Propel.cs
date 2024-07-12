using System.Collections;
using UnityEngine;

public class Propel : MonoBehaviour
{
    [SerializeField]
    private Quaternion isRotateValue;                                                   // 현 오브젝트의 회전값
    [SerializeField]
    private bool anotherValue;                                                          // 특정 회전값을 넣어야하는가?
    [SerializeField]
    private float rotationspeed;                                                        // 회전 속도

    [Tooltip("0-x 1-y 2-z")]
    [SerializeField]
    private int Axis_rotation;                                                          // 회전하고자 하는 축
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
