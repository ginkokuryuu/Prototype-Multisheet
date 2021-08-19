using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GoogleSheetsToUnity;

public class GoogleSheetConnector : MonoBehaviour
{
    public static GoogleSheetConnector INSTANCE;
    public string sheetId = "";
    public string workSheet = "";

    private void Awake()
    {
        if(INSTANCE == null)
        {
            INSTANCE = this;
        }
        else
        {
            Destroy(this.gameObject);
        }
    }

    public void GetData(UnityEngine.Events.UnityAction<GstuSpreadSheet> callback)
    {
        SpreadsheetManager.Read(new GSTU_Search(sheetId, workSheet), callback);
    }
}
