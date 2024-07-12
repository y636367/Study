using System.Collections.Generic;
using UnityEngine;

public class EffectPool : MonoBehaviour
{
    [SerializeField]
    private GameObject[] Effects;
    private List<GameObject>[] EffectList;

    [Space(10f)]
    public GameObject Bubbles_P;
    public GameObject Buliets_P;
    public GameObject Jumps_P;
    public GameObject Landings_P;
    public GameObject Shots_P;
    public GameObject Sparks_P;

    [Space(10f)]
    public GroundEffect GE;

    private void Awake()
    {
        EffectList = new List<GameObject>[Effects.Length];

        for(int index=0; index<Effects.Length; index++)
        {
            EffectList[index] = new List<GameObject>();
        }
    }

    public GameObject SpawnEffect(int effect_num, Transform t_position)
    {
        GameObject select = null;

        foreach (var effect in EffectList[effect_num])
        {
            if (!effect.activeSelf)
            {
                select = effect;
                select.SetActive(true);
                break;
            }
        }
        if (!select)
        {
            select = Instantiate(Effects[effect_num], t_position);

            EffectList[effect_num].Add(select);
        }
        return select;
    }
}
