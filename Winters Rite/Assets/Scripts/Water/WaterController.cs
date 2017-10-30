//////////////////////////////////////////////////////////////////////////
// File: <WaterController.cs>
// Author: <Alex Kitching>
// Date Created: <14/05/17>
// Brief: <Script handling Water gameObject Functionality.>
/////////////////////////////////////////////////////////////////////////

using UnityEngine;

public class WaterController : MonoBehaviour
{
    private const string c_sPLAYER_TAG = "Player";

    void OnTriggerEnter(Collider a_other)
    {
        if (a_other.gameObject.tag == c_sPLAYER_TAG) // Player has entered Collider.
        {
            // Call players thirst increase function
            a_other.gameObject.GetComponent<Player>().CmdPlayerStartThirstIncrease(a_other.gameObject.name);
        }
    }

    private void OnTriggerExit(Collider a_other)
    {
        if (a_other.gameObject.tag == c_sPLAYER_TAG) // Player has exited Collider
        {
            // Call players stop thirst increase function
            a_other.gameObject.GetComponent<Player>().CmdPlayerStopThirstIncrease(a_other.gameObject.name);
        }
    }
}
