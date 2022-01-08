using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    public GameObject carToFollow = null;
    private bool movingRight = false;
    private bool movingLeft = true;
    public bool followingCar = false;
    
    void Update()
    {
        if (followingCar == false)
        { 
            if (Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.A))
            {
                movingLeft = true;
                movingRight = false;
                followingCar = false;
            }
            if (Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.D))
            {
                movingLeft = false;
                movingRight = true;
                followingCar = false;
            }
            if (Input.GetKeyUp(KeyCode.LeftArrow) || Input.GetKeyUp(KeyCode.A))
            {
                movingLeft = false;
                followingCar = false;
            }
            if (Input.GetKeyUp(KeyCode.RightArrow) || Input.GetKeyUp(KeyCode.D))
            {
                movingRight = false;
                followingCar = false;
            }
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            carToFollow = null;
            followingCar = false;
        }

        if (carToFollow != null)
        {
            movingLeft = false;
            movingRight = false;
            followingCar = true;
        }

        if (carToFollow == null)
        {
            followingCar = false;
            transform.position = new Vector3(transform.position.x, 0f, transform.position.z);
        }
    }

    private void FixedUpdate()
    {
        if (movingRight)
        {
            MoveRight();
        }    
        if (movingLeft)
        {
            MoveLeft();
        }
        if (followingCar)
        {
            FollowCar();
        }
    }

    private void MoveLeft()
    {
        if (transform.position.x > 0)
        {
            Vector3 newPos = new Vector3(transform.position.x - 10f, transform.position.y, -10f);
            transform.position = newPos;
        }
    }

    private void MoveRight()
    {
        if (transform.position.x < 28445)
        {
            Vector3 newPos = new Vector3(transform.position.x + 10f, transform.position.y, -10f);
            transform.position = transform.position = newPos;
        }
    }

    private void FollowCar()
    {
        Debug.Log("clicking on carToFollow");
        Vector3 newPos = carToFollow.transform.position+ new Vector3(0f,0f,-10f);
        transform.position = newPos;
    }
}
