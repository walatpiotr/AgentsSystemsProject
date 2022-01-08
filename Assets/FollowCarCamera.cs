using UnityEngine;

public class FollowCarCamera : MonoBehaviour
{
    private void OnMouseDown()
    {
        var cameraFollowScript = Camera.main.GetComponent<CameraMovement>();
        cameraFollowScript.carToFollow = gameObject;
        cameraFollowScript.followingCar = true;
    }
}
