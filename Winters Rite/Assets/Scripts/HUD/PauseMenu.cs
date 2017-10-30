//////////////////////////////////////////////////////////////////////////
// File: <PauseMenu.cs>
// Author: <Alex Kitching>
// Date Created: <31/3/17>
// Brief: <Script handling Pause Menu Functionality.>
/////////////////////////////////////////////////////////////////////////

using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Networking.Match;

public class PauseMenu : MonoBehaviour
{
    public static bool bIsOn = false;

    private NetworkManager networkManager;

    private bool bMatchMakerActive;

    void Start()
    {
        // Set networkManager to singleton.
        networkManager = NetworkManager.singleton;
    }

    public void LeaveRoom ()
    {
        // Check whether we are in a matchmaking game or not.
        bMatchMakerActive = networkManager.GetComponent<CheckMatchMaker>().bMatchMakerActive;

        if(bMatchMakerActive) // We are in a matchmaking game.
        {
            // Drop Matchmaking Connection.
            MatchInfo matchInfo = networkManager.matchInfo;
            networkManager.matchMaker.DropConnection(matchInfo.networkId, matchInfo.nodeId, 0, networkManager.OnDropConnection);
        }
        // Stops network manager.
        networkManager.StopHost();
    }
}
