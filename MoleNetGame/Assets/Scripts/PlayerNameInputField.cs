﻿using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;
using System.Collections;

/// <summary>
/// Player name input field. Let the user input his name, will appear above the player in the game.
/// https://doc.photonengine.com/en-us/pun/v2/demos-and-tutorials/pun-basics-tutorial/lobby-ui
/// </summary>
[RequireComponent(typeof(InputField))] 
public class PlayerNameInputField : MonoBehaviour
{
    #region Private Constants

    const string playerNamePrefKey = "PlayerName";

    #endregion


    #region MonoBehaviour CallBacks

    // Start is called before the first frame update
    void Start()
    {
        string defaultName = string.Empty;
        InputField _inputField = this.GetComponent<InputField>();
        if(_inputField != null)
        {
            if (PlayerPrefs.HasKey(playerNamePrefKey))
            {
                defaultName = PlayerPrefs.GetString(playerNamePrefKey);
                _inputField.text = defaultName;
            }
        }

        PhotonNetwork.NickName = defaultName;
    }

    #endregion

    #region Public Methods
    
    /// <summary>
    /// Sets the name of the player, and saves it in the PlayerPrefs
    /// for future sessions
    /// </summary>
    /// <param name="value">The name of the Player</param>
    public void SetPlayerName(string value)
    {
        // #Important
        if (string.IsNullOrEmpty(value))
        {
            Debug.LogError("Player Name is null or empty");
            return;
        }
        PhotonNetwork.NickName = value;
        PlayerPrefs.SetString(playerNamePrefKey,value);
    }

    #endregion
}