//////////////////////////////////////////////////////////////////////////
// File: <GameManager.cs>
// Author: <Alex Kitching>
// Date Created: <8/03/17>
// Brief: <Script handling GameController Functionality.>
/////////////////////////////////////////////////////////////////////////

using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class GameManager : MonoBehaviour
{

    #region Variables
    // Public Access of GameManager
    public static GameManager instance;

    public RoundSettings roundSettings;

    [SerializeField]
    private GameObject sceneCamera;

    private const string PLAYER_ID_PREFIX = "Player ";

    private static Dictionary<string, Player> players = new Dictionary<string, Player>();
    #endregion

    void Awake()
    {
        // Check for accidental creation of 2 GameManagers
        if(instance != null)
        {
            Debug.LogError("More than one GameManager in scene");
        }
        else
        {   // Sets current instance
            instance = this;
        }
    }

    public void SetSceneCameraActive(bool a_bIsActive)
    {
        if (sceneCamera == null) // No Scene Camera - Exit
            return;

        sceneCamera.SetActive(a_bIsActive); // Set Sceen Camera to Active/Inactive
    }

    #region PlayerTracking



    public static void RegisterPlayer(string a_sNetID, Player a_player)
    {
        // Adds player reference to Dictionary
        string sPlayerID = PLAYER_ID_PREFIX + a_sNetID;
        players.Add(sPlayerID, a_player);
        // Sets Player gameObjects name.
        a_player.transform.name = sPlayerID;
    }

    public static void UnRegisterPlayer(string a_sPlayerID)
    {
        // Removes Player from Dictionary
        players.Remove(a_sPlayerID);
    }

    public static Player GetPlayer(string a_sPlayerID)
    {
        // Returns Player from Dictionary
        return players[a_sPlayerID];
    }

    public static Player[] GetAllPlayers()
    { 
        // Returns all players 
        return players.Values.ToArray();
    }
    #endregion

}
