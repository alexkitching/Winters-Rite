//////////////////////////////////////////////////////////////////////////
// File: <NetworkSyncTransform.cs>
// Author: <Alex Kitching>
// Date Created: <8/03/17>
// Brief: <Script handling Network Transform Syncing.>
/////////////////////////////////////////////////////////////////////////

using UnityEngine;
using UnityEngine.Networking;

public class NetworkSyncTransform : NetworkBehaviour
{

    #region Variables
    [SerializeField]
    private GameObject playerCamera;
    [SerializeField]
    private float fPositionLerpRate = 15;
    [SerializeField]
    private float fRotationLerpRate = 15;
    [SerializeField]
    private float fPositionThreshold = 0.1f;
    [SerializeField]
    private float fRotationThreshold = 1f;

    [SyncVar]
    private Vector3 v3LastPosition;

    [SyncVar]
    private Vector3 v3LastRotation;

    [SyncVar]
    private Vector3 v3LastCameraRotation;
    #endregion

    void Update ()
	{
		if (isLocalPlayer) // Exit if we are the local player, we only want to sync other players
			return;
        // Interpolate Movement
		InterpolatePosition(); 
		InterpolateRotation();
		InterpolateCameraRotation();
	}

	void FixedUpdate()
	{
		if (!isLocalPlayer) // If we are not the local player return, only run on local player
			return;

		bool bPosChanged = IsPositionChanged(); // Has the Position Changed

		if(bPosChanged) // Position has changed
		{
			CmdSendPosition(transform.position); // Send Position to Network
            v3LastPosition = transform.position; // Sets last position to current position
		}

		bool bRotationChanged = IsRotationChanged(); // Has the rotation changed

		if(bRotationChanged) // Rotation has changed
		{
			CmdSendRotation(transform.localEulerAngles); // Send Rotation to Network
            v3LastRotation = transform.localEulerAngles; // Sets last rotation to current rotation
		}

		bool bCameraRotationChanged = IsCameraRotationChanged(); // Has Camera Rotation Changed

		if(bCameraRotationChanged) // Rotation has changed
		{
			CmdSendCameraRotation(playerCamera.transform.localEulerAngles); // Sends rotation to Network
            v3LastCameraRotation = playerCamera.transform.localEulerAngles; // Sets last rotation to current rotation
		}

	}

	public void InterpolatePosition()
	{   // Lerps the Players Position
		transform.position = Vector3.Lerp(transform.position, v3LastPosition, Time.deltaTime * fPositionLerpRate);
	}

	private void InterpolateRotation()
	{   // Lerps the Players Rotation
		transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(v3LastRotation), Time.deltaTime * fRotationLerpRate);
	}

	private void InterpolateCameraRotation()
	{   // Lerps the players Camera rotation 
		playerCamera.transform.localRotation = Quaternion.Lerp(playerCamera.transform.localRotation, Quaternion.Euler(v3LastCameraRotation), Time.deltaTime * fRotationLerpRate);
	}

	[Command(channel = Channels.DefaultUnreliable)] 
	public void CmdSendPosition(Vector3 a_vPos)
    {   // Sets players last position and sends over network.
        v3LastPosition = a_vPos; 
	}

	[Command(channel = Channels.DefaultUnreliable)]
	public void CmdSendRotation(Vector3 a_vRot)
    {   // Sets players last rotation and sends over network.
        v3LastRotation = a_vRot; 
    }
	[Command(channel = Channels.DefaultUnreliable)]
	public void CmdSendCameraRotation(Vector3 a_vRot)
    {   // Sets cameras last rotation and sends over network.
        v3LastCameraRotation = a_vRot; 
    }

	private bool IsPositionChanged()
    {   // Checks Distance between current position and last position and whether it is greater than threshhold set.
        return Vector3.Distance(transform.position, v3LastPosition) > fPositionThreshold;
	}

	private bool IsRotationChanged()
    {   // Checks Distance between current rotation angle and last rotation angle and whether it is greater than threshold set.
        return Vector3.Distance(transform.localEulerAngles, v3LastRotation) > fRotationThreshold;
	}

	private bool IsCameraRotationChanged()
    {   // Checks Distance between current camera rotation angle and last camera rotation angle and whether it is greater than threshold set.
        return Vector3.Distance(playerCamera.transform.localEulerAngles, v3LastCameraRotation) > fRotationThreshold;
	}

	public override int GetNetworkChannel()
    {   // Returns Network Channel.
        return Channels.DefaultUnreliable;
	}

	public override float GetNetworkSendInterval()
    {   // Returns Network Poll time.
        return 0.01f;
	}
}
