
/*****************************************************************************************************************
 - CreditsUI.cs -
-----------------------------------------------------------------------------------------------------------------
 Author:             Victor Dang
 Game/Program Name:  Gravity
 Engine Version:     Unity 2019.1.3f1
-----------------------------------------------------------------------------------------------------------------
 Description: 
     Contains logic for buttons within the credits UI.
*****************************************************************************************************************/

using UnityEngine;
using UnityEngine.UI;

public class CreditsUI : MonoBehaviour
{
    [SerializeField] Button buttonSappheirosLink,
                            buttonZapSplatLink,
                            buttonCreditsExit;


    /// <summary>
    /// Called when the game object is enabled.
    /// 
    /// Sets up corresponding button logic.
    /// </summary>
    void Start()
    {
        buttonSappheirosLink.onClick.AddListener(delegate { Application.OpenURL(Data.SAPPHEIROS_URL); });
        buttonZapSplatLink.onClick.AddListener(delegate { Application.OpenURL(Data.ZAPSPLAT_URL); });
        buttonCreditsExit.onClick.AddListener(MainMenuManager.Instance.CreditsExitButtonClicked);
    }
}