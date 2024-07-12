using System.IO;
using UnityEngine;

/// <summary>
///  ���� ���Ϸ� ���� �� �ε� �ϱ� ���� Ŭ����
/// </summary>
public static class LocalFileIOHandler
{
    private static string RootPath => $"{Application.dataPath}/";                                                   // ����ǰ� �ҷ��� ���� ���

    /// <summary>
    /// ���� ��� ���� �� ���� ����
    /// </summary>
    /// <param name="text"> ���ε� �� ����</param>
    /// <param name="dir"> ����� ���� ���</param>
    /// <param name="file"> ����� ���� �̸�</param>
    /// <param name="ext"> ����� ���� ����</param>
    /// <returns></returns>
    public static bool Save(in string text, in string dir, in string file, in string ext)
    {
        string path = Path.Combine(RootPath, dir, $"{file}.{ext}");                                                 // ���ϰ�� ����

        FileInfo fi = new FileInfo(path);                                                                           // ���� ��� ����ȭ
        DirectoryInfo di = fi.Directory;                                                                            // ��� ���丮ȭ

        if (!di.Exists)                                                                                             // ������ �������� �ʴٸ�
        {
            di.Create();                                                                                            // ���丮 ���� ����
        }

        File.WriteAllText(path, text);                                                                              // ���丮 ���Ͽ� ��ο� ���� ����

        return true;
    }
    /// <summary>
    /// ���� ��� ���� �� ���յ� ��ο��� ���� �ҷ�����
    /// </summary>
    /// <param name="dir"></param>
    /// <param name="file"></param>
    /// <param name="ext"></param>
    /// <returns></returns>
    public static string Load(in string dir, in string file, in string ext)
    {
        string path = Path.Combine(RootPath, dir, $"{file}.{ext}");                                                 // ���ϰ�� ����

        FileInfo fi = new FileInfo(path);                                                                           // ���� ��� ����ȭ

        if (!fi.Exists)                                                                                             // ������ �������� �ʴٸ�
            return null;                                                                                            // null ��ȯ

        return File.ReadAllText(path);                                                                              // ���� ���� ���� ��� �о��
    }
}
