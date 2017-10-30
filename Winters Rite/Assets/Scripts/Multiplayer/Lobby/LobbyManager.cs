//////////////////////////////////////////////////////////////////////////
// File: <LobbyManager.cs>
// Author: <Alex Kitching>
// Date Created: <31/03/17>
// Brief: <Script handling Lobby Functionality.>
/////////////////////////////////////////////////////////////////////////

using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using UnityEngine.Networking;
using UnityEngine.Networking.Match;
using System.Collections;

public class LobbyManager : MonoBehaviour
{

	#region Variables
	//Host Game Variables
	[SerializeField]
	private uint iRoomSize = 4;

	private string sRoomName;

	//Join Game Variables
	List<GameObject> roomList = new List<GameObject>();

	[SerializeField]
	private Text status;

	[SerializeField]
	private Text lanStatus;

	[SerializeField]
	private Button btnLanHost;

	[SerializeField]
	private Button btnLanJoin;

	[SerializeField]
	private InputField inputRoomName;

    [SerializeField]
    private InputField ipInput;

	[SerializeField]
	private Button btnCreateRoom;

	[SerializeField]
	private Button btnRefresh;


	[SerializeField]
	private GameObject roomListItemPrefab;

	[SerializeField]
	private Transform roomListParent;

	// General Variables
	private NetworkManager networkManager;

	#endregion

	void Start()
	{   //Sets up Network Manager and Match Maker
		networkManager = NetworkManager.singleton;

		if(networkManager.matchMaker == null) // Match Maker Inactive
		{
            // Toggle Match Maker
            networkManager.StartMatchMaker();
			networkManager.GetComponent<CheckMatchMaker>().ToggleMatchMaker(); 
		}

        // Refreshes Room List on load
        RefreshRoomList();
	}

	#region Match Host Game
	public void SetRoomName(string a_sName) // Sets Room Name
	{
        sRoomName = a_sName;
	}
	
	public void CreateRoom() // Creates MatchMaking Room
	{
		if(sRoomName != null && sRoomName != "") // If Room name has been entered and is not blank
		{
			Debug.Log("Creating Room:" + sRoomName + " with room for " + iRoomSize + " players.");
			// Create room
			networkManager.matchMaker.CreateMatch(sRoomName, iRoomSize, true,"","","",0,0, networkManager.OnMatchCreate);
		}
	}
	#endregion

	#region Match Join Game
	public void RefreshRoomList()
	{
        // Clears Current Room List
		ClearRoomList();

		if (networkManager.matchMaker == null)
		{
			networkManager.StartMatchMaker(); // Start Matchmaker if inactive
		}

		networkManager.matchMaker.ListMatches(0, 20, "", true, 0, 0, OnMatchList); // Lists matches
		status.text = "Loading..."; 
	}

	public void OnMatchList(bool a_bSuccess, string a_sExtendedInfo, List<MatchInfoSnapshot> a_matchList)
	{
		status.text = "";  

		if (!a_bSuccess) // Unable to Connect
		{
			status.text = "Couldn't retrieve matches";
			return;
		}

        if (a_matchList == null) // No Matches found
        {
            status.text = "No Matches found";
            return;
        }

		foreach (MatchInfoSnapshot match in a_matchList) // Creates room list item for each match found
		{
			GameObject _roomListItemGO = Instantiate(roomListItemPrefab);
			_roomListItemGO.transform.SetParent(roomListParent, false);

			RoomListItem _roomListItem = _roomListItemGO.GetComponent<RoomListItem>();

			if (_roomListItem != null) // List Item not setup.
			{
				_roomListItem.Setup(match, JoinRoom); // Runs Setup
			}

			roomList.Add(_roomListItemGO); // Adds Item
		}

		if (roomList.Count == 0) // No Rooms Available
		{
			status.text = "No rooms available.";
		}
	}

	void ClearRoomList() // Clears Room List
	{
		for (int i = 0; i < roomList.Count; i++)
		{
			Destroy(roomList[i]); // Destroys all room lists
		}

		roomList.Clear(); // Clears Room List
	}

	public void JoinRoom(MatchInfoSnapshot a_match) // Joins Room
	{
		networkManager.matchMaker.JoinMatch(a_match.networkId, "", "", "", 0, 0, networkManager.OnMatchJoined);
		StartCoroutine(WaitForJoin()); // Starts Waiting for Join 
	}

	IEnumerator WaitForJoin()
	{
		ClearRoomList(); // Clears Room List

		int iCountdown = 10; 

		while (iCountdown > 0) // While still counting down
		{
			DisableInteraction(); // Disables Buttons
			status.text = "Joining... (" + iCountdown + ")";

			yield return new WaitForSeconds(1); // Wait a second

            iCountdown--; // Reduce Countdown
		}

		//Failed to connect
		status.text = "Failed to connect.";
		yield return new WaitForSeconds(1); // Wait a second

		MatchInfo matchInfo = networkManager.matchInfo;
		if (matchInfo != null) // No Match connection after 10 seconds
		{
			EnableInteraction(); // Renable Buttons
			networkManager.matchMaker.DropConnection(matchInfo.networkId, matchInfo.nodeId, 0, networkManager.OnDropConnection); // Drop Matchmaker Connection Attempt
			networkManager.StopHost(); // Stop Host
		}

		RefreshRoomList(); // Refreshes Room List
	}
	#endregion

	#region Manual Game
	public void LocalHost()
	{
		if (!NetworkClient.active && !NetworkServer.active) // Sever or Client not already running
		{
			networkManager.GetComponent<CheckMatchMaker>().ToggleMatchMaker(); // Toggles Match Maker (Disables)
			networkManager.StartHost(); // Starts Hosting
		}
	}

	public void LocalJoin()
	{
		if(!NetworkClient.active && !NetworkServer.active) // Sever or Client not already running
        {
            networkManager.networkAddress = ipInput.text; // Sets Network Address
			networkManager.GetComponent<CheckMatchMaker>().ToggleMatchMaker(); // Toggles Match Maker (Disables)
            networkManager.StartClient(); // Starts Client
			StartCoroutine(WaitForLocalJoin()); // Starts Waiting to Join
		}
	}

	IEnumerator WaitForLocalJoin()
	{
		int iCountdown = 18; // 18 Second Countdown

		while(iCountdown > 0) // While still counting down
        {
			DisableInteraction(); // Disables Buttons
            lanStatus.text = "Joining... (" + iCountdown + ")";

			yield return new WaitForSeconds(1); // Wait a second

            iCountdown--; // Reduce Countdown
        }

		//Failed to connect
		lanStatus.text = "Failed to connect.";
		yield return new WaitForSeconds(1); // Wait a second
        if (networkManager.isNetworkActive == false) // No Connection Made
		{
			EnableInteraction(); // Renable Buttons
            networkManager.StopHost(); // Stop Host
            networkManager.GetComponent<CheckMatchMaker>().ToggleMatchMaker(); // Turn Matchmaker Back On
		}
	}

	void DisableInteraction() // Disables Buttons
	{
		inputRoomName.interactable = false;
		btnLanHost.interactable = false;
		btnLanJoin.interactable = false;
		btnCreateRoom.interactable = false;
		btnRefresh.interactable = false;
	}
	void EnableInteraction() // Enables Buttons
	{
		inputRoomName.interactable = true;
		btnLanHost.interactable = true;
		btnLanJoin.interactable = true;
		btnCreateRoom.interactable = true;
		btnRefresh.interactable = true;
	}


	#endregion


}
