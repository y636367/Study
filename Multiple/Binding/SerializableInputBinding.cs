using System;
using UnityEngine;

/// <summary>
/// InputBinding의 Dictionary 직렬화 불가 하기에 Json 직렬화 가능 하게
/// </summary>
[Serializable]
public class SerializableInputBinding
{
    public BindPair[] bindPairs;

    /// <summary>
    /// 객체 생성시 자동 호출 생성자로
    /// </summary>
    /// <param name="binding"></param>
    public SerializableInputBinding(KeyBinding binding)
    {
        int len = binding.Bindings.Count;                                               // 바인딩 개수
        int index = 0;

        bindPairs = new BindPair[len];                                                  // 바인딩 할 개수 만큼 배열 할당
        
        foreach(var pair in binding.Bindings)
        {
            bindPairs[index++] = new BindPair(pair.Key, pair.Value);                    // 바인딩 할 딕셔너리형 하나씩 받아옴
        }
    }
}
/// <summary>
/// 직렬화를 위한 새로운 클래스 생성
/// </summary>
[Serializable]
public class BindPair
{
    public Action key;                                                                  // KeyBinding의 Bindings의 Key값
    public KeyCode value;                                                               // KeyBinding의 Bindings의 Value값

    /// <summary>
    /// 할당
    /// </summary>
    /// <param name="key"></param>
    /// <param name="value"></param>
    public BindPair(Action key, KeyCode value) 
    {
        this.key = key;
        this.value = value;
    }
}
