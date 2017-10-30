//////////////////////////////////////////////////////////////////////////
// File: <CheckMatchMaker.cs>
// Author: <Alex Kitching>
// Date Created: <31/03/17>
// Brief: <Script handling the Toggling of the Match Maker.>
/////////////////////////////////////////////////////////////////////////

using UnityEngine;

public class CheckMatchMaker : MonoBehaviour
{

    public bool bMatchMakerActive = false;

    public void ToggleMatchMaker()
    {
        // Toggles Match Maker
        bMatchMakerActive = !bMatchMakerActive;
    }
}
