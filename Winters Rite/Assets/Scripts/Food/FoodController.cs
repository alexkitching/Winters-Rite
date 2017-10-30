//////////////////////////////////////////////////////////////////////////
// File: <FoodController.cs>
// Author: <Alex Kitching>
// Date Created: <20/03/17>
// Brief: <Script handling Food gameObject Functionality.>
/////////////////////////////////////////////////////////////////////////

using UnityEngine;

public class FoodController : MonoBehaviour
{

    #region Variables
    private const string c_sPLAYER_TAG = "Player";

    [SerializeField]
    private string sFoodName = "Apple";

    [SerializeField]
    private int iHungerIncreaseAmount = 10;
    #endregion

    void OnTriggerEnter(Collider a_other)
    {
        if(a_other.gameObject.tag== c_sPLAYER_TAG) // Player has entered collider.
        {
            // Call Players Increase Hunger function.
            a_other.gameObject.GetComponent<Player>().CmdPlayerHungerIncrease(gameObject, a_other.gameObject.name, sFoodName, iHungerIncreaseAmount);
        }
    }

    public void DestroyFood()
    {
        // Destroys gameObject attached to this script.
        Destroy(gameObject);
    }
}
