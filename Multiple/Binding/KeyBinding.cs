using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class KeyBinding
{
    // 저장 및 불러오기 시 폴더 - 파일 - 확장자 - 고유번호
    public string localDirectioryPath = "Settings";                                                         // 폴더 명 (Assets/Settings)
    public string fileName = "InputBindingPreset";                                                          // 파일 이름 (InputBindingPreset.txt)
    public string extName = "txt";                                                                          // 확장자
    public string id = "001";                                                                               // 고유번호
    public Dictionary<Action, KeyCode> Bindings => bindingDict;
    private Dictionary<Action, KeyCode> bindingDict;

    /// <summary>
    /// 객체 생성 시 자동 호출 되게 생성자로
    /// </summary>
    /// <param name="initalized"></param>
    public KeyBinding(bool initalized = true)
    {
        bindingDict = new Dictionary<Action, KeyCode>();
    }
    /// <summary>
    /// 바인딩 지정 함수
    /// </summary>
    /// <param name="action"></param>
    /// <param name="key"></param>
    /// <param name="allowOverlap"></param>
    public void Bind(KeyBinding t_bind, in Action action, in KeyCode key, bool allowOverlap = false)
    {
        if (!allowOverlap && t_bind.bindingDict.ContainsValue(key))                                                 // 중복 키를 사용 하지 않고 변경하려는 Action에 키가 바인딩 되어 있다면
        {
            var copy = new Dictionary<Action, KeyCode>(t_bind.bindingDict);                                         // 키 값 비교를 위한 변수 생성 및 변경하려는 Action 할당

            foreach (var kv in copy)
            {
                if (kv.Value.Equals(key))                                                                           // 다른 Action 들 중 변경하려는 키와 동일한 키가 이미 할당 되어 있다면
                {
                    t_bind.bindingDict[kv.Key] = KeyCode.None;                                                      // 그 Action에 키값을 None으로 설정
                }
            }
        }
        t_bind.bindingDict[action] = key;                                                                           // 변경하고자 하는 Action에 키 바인딩
    }
    /// <summary>
    /// 키 바인딩 초기화
    /// </summary>
    /// <summary>
    /// SerializableInputBinding 클래스를 이 클래스에서 빠르게 변환할 수 있도록 새로운 생성자로
    public void ResetAll(KeyBinding t_bind)
    {
        Bind(t_bind ,Action.MoveForward, KeyCode.W);
        Bind(t_bind, Action.MoveBackward, KeyCode.S);
        Bind(t_bind, Action.MoveLeft, KeyCode.A);
        Bind(t_bind, Action.MoveRight, KeyCode.D);

        Bind(t_bind, Action.Dash, KeyCode.LeftShift);
        Bind(t_bind, Action.Jump, KeyCode.Space);

        Bind(t_bind, Action.RotateCamera, KeyCode.Mouse1);
        Bind(t_bind, Action.Ragdoll, KeyCode.R);
        Bind(t_bind, Action.ForcedDeath, KeyCode.F);
        Bind(t_bind, Action.Option, KeyCode.Tab);
    }
    public KeyBinding(SerializableInputBinding sib)
    {
        bindingDict = new Dictionary<Action, KeyCode>();

        foreach (var kv in sib.bindPairs)
        {
            bindingDict[kv.key] = kv.value;                                                                 // SerializableInputBinding의 직렬화를 위해 생성했던 클래스를 가져와 할당
        }
    }
    /// <summary>
    /// 새로운 바인딩 설정 적용 함수
    /// </summary>
    /// <param name="newBinding"></param>
    public void ApplyNewBindings(SerializableInputBinding newBinding, KeyBinding t_keybinding, SerializableInputBinding t_sib)
    {
        t_keybinding.bindingDict.Clear();                                                                                               // 내용 비우고
            
        for(int index = 0; index < newBinding.bindPairs.Length; index++)
        {
            for (int j = 0; j < t_sib.bindPairs.Length; j++)
            {
                if (newBinding.bindPairs[index].key == t_sib.bindPairs[j].key)
                {
                    t_keybinding.bindingDict[newBinding.bindPairs[index].key] = newBinding.bindPairs[index].value;                          // 새로운 바인딩 설정
                    t_sib.bindPairs[j].value = newBinding.bindPairs[index].value;
                }
            }
        }
    }
    /// <summary>
    /// 바인딩된 내용을 Json으로 저장
    /// </summary>
    public void SaveFile(SerializableInputBinding t_sib)
    {
        string jsonStr = JsonUtility.ToJson(t_sib);                                                         // 현재 바인딩 된 값들을Json으로 변환하여 string 변수로 담아서

        LocalFileIOHandler.Save(jsonStr, localDirectioryPath, $"{fileName}_{id}", extName);                 // 내용 ,경로, 파일 이름 + 고유번호 전달하여 저장진행
    }
   /// <summary>
   /// 과거 저장한 바인딩 내용 불러오기
   /// </summary>
   /// <param name="t_keybinding"></param>
   /// <param name="t_sib"></param>
    public void LoadFromFile(KeyBinding t_keybinding, SerializableInputBinding t_sib)
    {
        string jsonStr = LocalFileIOHandler.Load(localDirectioryPath, $"{fileName}_{id}", extName);

        if (jsonStr == null)
            return;

        var sib = JsonUtility.FromJson<SerializableInputBinding>(jsonStr);                                  // json 파일 변환
        ApplyNewBindings(sib, t_keybinding, t_sib);                                                         // 바인딩
    }
}
