//////////////////////////////////////////////////////////////////////////
// File: <RoomListItem.cs>
// Author: <Alex Kitching>
// Date Created: <31/03/17>
// Brief: <Script handling Room List Item Functionality.>
/////////////////////////////////////////////////////////////////////////

using UnityEngine;
using UnityEngine.Networking.Match;
using UnityEngine.UI;


public class RoomListItem : MonoBehaviour
{
    #region Variables
    public delegate void JoinRoomDelegate(MatchInfoSnapshot a_match);
    private JoinRoomDelegate joinRoomCallBack;

    [SerializeField]
    private Text roomNameText;

    private MatchInfoSnapshot match;
    #endregion

    public void Setup(MatchInfoSnapshot a_match, JoinRoomDelegate a_joinRoomCallback)
    {
        // Sets up Callbacks and References
        match = a_match;
        joinRoomCallBack = a_joinRoomCallback;

        // Sets RoomName Text
        roomNameText.text = match.name + " (" + match.currentSize + "/" + match.maxSize + ")";
    }

    public void JoinRoom()
    {
        joinRoomCallBack.Invoke(match); // Sends Callback to Join Room
    }
}
