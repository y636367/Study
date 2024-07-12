using System.IO;
using UnityEngine;

/// <summary>
///  로컬 파일로 저장 및 로드 하기 위한 클래스
/// </summary>
public static class LocalFileIOHandler
{
    private static string RootPath => $"{Application.dataPath}/";                                                   // 저장되고 불러올 파일 경로

    /// <summary>
    /// 파일 경로 결합 후 내용 저장
    /// </summary>
    /// <param name="text"> 바인딩 된 내용</param>
    /// <param name="dir"> 저장될 로컬 경로</param>
    /// <param name="file"> 저장될 파일 이름</param>
    /// <param name="ext"> 저장될 파일 형식</param>
    /// <returns></returns>
    public static bool Save(in string text, in string dir, in string file, in string ext)
    {
        string path = Path.Combine(RootPath, dir, $"{file}.{ext}");                                                 // 파일경로 결합

        FileInfo fi = new FileInfo(path);                                                                           // 파일 경로 변수화
        DirectoryInfo di = fi.Directory;                                                                            // 경로 디렉토리화

        if (!di.Exists)                                                                                             // 파일이 존재하지 않다면
        {
            di.Create();                                                                                            // 디렉토리 파일 생성
        }

        File.WriteAllText(path, text);                                                                              // 디렉토리 파일에 경로와 내용 삽입

        return true;
    }
    /// <summary>
    /// 파일 경로 결합 후 결합된 경로에서 파일 불러오기
    /// </summary>
    /// <param name="dir"></param>
    /// <param name="file"></param>
    /// <param name="ext"></param>
    /// <returns></returns>
    public static string Load(in string dir, in string file, in string ext)
    {
        string path = Path.Combine(RootPath, dir, $"{file}.{ext}");                                                 // 파일경로 결합

        FileInfo fi = new FileInfo(path);                                                                           // 파일 경로 변수화

        if (!fi.Exists)                                                                                             // 파일이 존재하지 않다면
            return null;                                                                                            // null 반환

        return File.ReadAllText(path);                                                                              // 파일 안의 내용 모두 읽어옴
    }
}
