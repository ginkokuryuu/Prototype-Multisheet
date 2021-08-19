using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Login Data", menuName = "Create Login Data")]
public class LoginData : ScriptableObject
{
    public string username;
    public string nama;
    public string nomorInduk;
    public string sekolah;
    public string kelas;
    public string sheetId;
    public string workSheet;

    private void OnEnable()
    {
        hideFlags = HideFlags.DontUnloadUnusedAsset;
    }
}
