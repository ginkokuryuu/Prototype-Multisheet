using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using GoogleSheetsToUnity;
using System.Text.RegularExpressions;
using System;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

enum LoginCode
{
    DEFAULT_VALUE,
    LoginSuccess,
    NoUserFound,
    WrongPassword,
    WrongSchool,
    WrongClass,
    ConnectionFailed
}

public class LoginHandler : MonoBehaviour
{
    [Header("User data Sheet")]
    [SerializeField] string sheetId = "";
    [SerializeField] string workSheet = "";
    [SerializeField] float maxWaitingTime = 3f;
    float waitCounter = 0f;
    LoginCode status = LoginCode.DEFAULT_VALUE;             

    [Header("Form data")]
    [SerializeField] TMP_InputField username = null;
    [SerializeField] TMP_InputField password = null;
    [SerializeField] TMP_InputField sekolah = null;
    [SerializeField] TMP_InputField kelas = null;
    [SerializeField] Button loginBtn = null;

    [Header("Pop Up")]
    [SerializeField] float popUpTime = 2f;
    [SerializeField] GameObject failedPopUp = null;
    [SerializeField] TMP_Text messageText = null;
    bool isWaitingResponse = false;

    [Header("Stored Login Data")]
    [SerializeField] LoginData loginData = null;

    private void Update()
    {
        if (isWaitingResponse)
        {
            waitCounter += Time.deltaTime;
            if(waitCounter >= maxWaitingTime)
            {
                isWaitingResponse = false;
                status = LoginCode.ConnectionFailed;
                StartCoroutine(ShowPopUp());
            }
        }
        else
        {
            waitCounter = 0f;
        }
    }

    public void Login()
    {
        if (FieldIsEmpty())
            return;

        loginBtn.interactable = false;
        status = LoginCode.DEFAULT_VALUE;
        SpreadsheetManager.Read(new GSTU_Search(sheetId, workSheet), HandleData);
        isWaitingResponse = true;
    }

    public void HandleData(GstuSpreadSheet spreadsheetRef)
    {
        isWaitingResponse = false;

        foreach (var _username in spreadsheetRef.columns["USERNAME"])
        {
            if(_username.value != username.text)
                continue;

            if (password.text != spreadsheetRef[_username.value, "PASSWORD"].value)
            {
                //wrong password;
                status = LoginCode.WrongPassword;
                break;
            }

            if(sekolah.text != spreadsheetRef[_username.value, "SEKOLAH"].value)
            {
                //wrong school
                status = LoginCode.WrongSchool;
                break;
            }

            if(kelas.text != spreadsheetRef[_username.value, "KELAS"].value)
            {
                //wrong class
                status = LoginCode.WrongClass;
                break;
            }

            //login success
            status = LoginCode.LoginSuccess;
            SaveLoginData(spreadsheetRef.rows[username.text]);
            SceneManager.LoadScene(1);
            return;
        }

        if(status == LoginCode.DEFAULT_VALUE)
        {
            //no user found
            status = LoginCode.NoUserFound;
        }

        //show pop up
        StartCoroutine(ShowPopUp());
        return;
    }

    void SaveLoginData(List<GSTU_Cell> datas)
    {
        loginData.username = datas[0].value;
        loginData.nama = datas[2].value;
        loginData.nomorInduk = datas[3].value;
        loginData.sekolah = datas[4].value;
        loginData.kelas = datas[5].value;
        loginData.sheetId = datas[6].value;
        loginData.workSheet = datas[7].value;
    }

    IEnumerator ShowPopUp()
    {
        loginBtn.interactable = true;
        messageText.text = SplitCamelCase(status.ToString());
        failedPopUp.SetActive(true);
        yield return new WaitForSecondsRealtime(popUpTime);
        failedPopUp.SetActive(false);
    }

    public string SplitCamelCase(string str)
    {
        return Regex.Replace(
            Regex.Replace(
                str,
                @"(\P{Ll})(\P{Ll}\p{Ll})",
                "$1 $2"
            ),
            @"(\p{Ll})(\P{Ll})",
            "$1 $2"
        );
    }

    private bool FieldIsEmpty()
    {
        return string.IsNullOrEmpty(username.text) || string.IsNullOrEmpty(password.text) || string.IsNullOrEmpty(sekolah.text) || string.IsNullOrEmpty(kelas.text);
    }
}
