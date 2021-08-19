using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using GoogleSheetsToUnity;
using UnityEngine.UI;

public class SubmitNilaiHandler : MonoBehaviour
{
    [SerializeField] TMP_InputField nilai = null;
    [SerializeField] Button submitBtn = null;
    [SerializeField] LoginData loginData = null;
    [SerializeField] float maxWaitingTime = 3f;
    [SerializeField] float popUpTime = 2f;
    [SerializeField] GameObject popUp = null;
    bool isWaitingResponse = false;
    float waitCounter = 0f;

    private void Update()
    {
        if (isWaitingResponse)
        {
            waitCounter += Time.deltaTime;
            if(waitCounter >= maxWaitingTime)
            {
                isWaitingResponse = false;
                StartCoroutine(ShowPopUp(false));
            }
        }
    }

    public void SubmitNilai()
    {
        submitBtn.interactable = false;
        isWaitingResponse = true;
        List<string> data = new List<string>
        {
            loginData.nomorInduk,
            loginData.nama,
            nilai.text
        };

        SpreadsheetManager.Append(new GSTU_Search(loginData.sheetId, loginData.workSheet), new ValueRange(data), SuccessSubmitting);
    }

    public void SuccessSubmitting()
    {
        isWaitingResponse = false;
        StartCoroutine(ShowPopUp(true));
    }

    IEnumerator ShowPopUp(bool success)
    {
        submitBtn.interactable = true;
        if (success)
        {
            popUp.transform.GetChild(1).gameObject.SetActive(true);
            popUp.transform.GetChild(2).gameObject.SetActive(false);
        }
        else
        {
            popUp.transform.GetChild(1).gameObject.SetActive(false);
            popUp.transform.GetChild(2).gameObject.SetActive(true);
        }
        popUp.SetActive(true);
        yield return new WaitForSecondsRealtime(popUpTime);
        popUp.SetActive(false);
    }
}
