using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class KeyBinding
{
    // ���� �� �ҷ����� �� ���� - ���� - Ȯ���� - ������ȣ
    public string localDirectioryPath = "Settings";                                                         // ���� �� (Assets/Settings)
    public string fileName = "InputBindingPreset";                                                          // ���� �̸� (InputBindingPreset.txt)
    public string extName = "txt";                                                                          // Ȯ����
    public string id = "001";                                                                               // ������ȣ
    public Dictionary<Action, KeyCode> Bindings => bindingDict;
    private Dictionary<Action, KeyCode> bindingDict;

    /// <summary>
    /// ��ü ���� �� �ڵ� ȣ�� �ǰ� �����ڷ�
    /// </summary>
    /// <param name="initalized"></param>
    public KeyBinding(bool initalized = true)
    {
        bindingDict = new Dictionary<Action, KeyCode>();
    }
    /// <summary>
    /// ���ε� ���� �Լ�
    /// </summary>
    /// <param name="action"></param>
    /// <param name="key"></param>
    /// <param name="allowOverlap"></param>
    public void Bind(KeyBinding t_bind, in Action action, in KeyCode key, bool allowOverlap = false)
    {
        if (!allowOverlap && t_bind.bindingDict.ContainsValue(key))                                                 // �ߺ� Ű�� ��� ���� �ʰ� �����Ϸ��� Action�� Ű�� ���ε� �Ǿ� �ִٸ�
        {
            var copy = new Dictionary<Action, KeyCode>(t_bind.bindingDict);                                         // Ű �� �񱳸� ���� ���� ���� �� �����Ϸ��� Action �Ҵ�

            foreach (var kv in copy)
            {
                if (kv.Value.Equals(key))                                                                           // �ٸ� Action �� �� �����Ϸ��� Ű�� ������ Ű�� �̹� �Ҵ� �Ǿ� �ִٸ�
                {
                    t_bind.bindingDict[kv.Key] = KeyCode.None;                                                      // �� Action�� Ű���� None���� ����
                }
            }
        }
        t_bind.bindingDict[action] = key;                                                                           // �����ϰ��� �ϴ� Action�� Ű ���ε�
    }
    /// <summary>
    /// Ű ���ε� �ʱ�ȭ
    /// </summary>
    /// <summary>
    /// SerializableInputBinding Ŭ������ �� Ŭ�������� ������ ��ȯ�� �� �ֵ��� ���ο� �����ڷ�
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
            bindingDict[kv.key] = kv.value;                                                                 // SerializableInputBinding�� ����ȭ�� ���� �����ߴ� Ŭ������ ������ �Ҵ�
        }
    }
    /// <summary>
    /// ���ο� ���ε� ���� ���� �Լ�
    /// </summary>
    /// <param name="newBinding"></param>
    public void ApplyNewBindings(SerializableInputBinding newBinding, KeyBinding t_keybinding, SerializableInputBinding t_sib)
    {
        t_keybinding.bindingDict.Clear();                                                                                               // ���� ����
            
        for(int index = 0; index < newBinding.bindPairs.Length; index++)
        {
            for (int j = 0; j < t_sib.bindPairs.Length; j++)
            {
                if (newBinding.bindPairs[index].key == t_sib.bindPairs[j].key)
                {
                    t_keybinding.bindingDict[newBinding.bindPairs[index].key] = newBinding.bindPairs[index].value;                          // ���ο� ���ε� ����
                    t_sib.bindPairs[j].value = newBinding.bindPairs[index].value;
                }
            }
        }
    }
    /// <summary>
    /// ���ε��� ������ Json���� ����
    /// </summary>
    public void SaveFile(SerializableInputBinding t_sib)
    {
        string jsonStr = JsonUtility.ToJson(t_sib);                                                         // ���� ���ε� �� ������Json���� ��ȯ�Ͽ� string ������ ��Ƽ�

        LocalFileIOHandler.Save(jsonStr, localDirectioryPath, $"{fileName}_{id}", extName);                 // ���� ,���, ���� �̸� + ������ȣ �����Ͽ� ��������
    }
   /// <summary>
   /// ���� ������ ���ε� ���� �ҷ�����
   /// </summary>
   /// <param name="t_keybinding"></param>
   /// <param name="t_sib"></param>
    public void LoadFromFile(KeyBinding t_keybinding, SerializableInputBinding t_sib)
    {
        string jsonStr = LocalFileIOHandler.Load(localDirectioryPath, $"{fileName}_{id}", extName);

        if (jsonStr == null)
            return;

        var sib = JsonUtility.FromJson<SerializableInputBinding>(jsonStr);                                  // json ���� ��ȯ
        ApplyNewBindings(sib, t_keybinding, t_sib);                                                         // ���ε�
    }
}
