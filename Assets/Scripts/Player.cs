using UnityEngine;

//Class Player that is controllable by user
//Can collide with Maze Walls
public class Player : MonoBehaviour
{
    private float _speed = 5f;
    private int _keysCnt = 3;
    private Vector3 _direction = new(0, 0, 0);
    [SerializeField] MazeManager _mazeManager;

    void FixedUpdate()
    {
        CalculateMovement();
    }

    void CalculateMovement()
    {
        _direction.x = Input.GetAxisRaw("Horizontal");
        _direction.y = Input.GetAxisRaw("Vertical");

        transform.Translate(_speed * Time.deltaTime * _direction);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        //check if collided with key
        //if so - decrement the number of keys to be found for opening an exit
        //and send a message to MazeManager class
        if (collision.tag == "key")
        {
            _keysCnt--;
            Destroy(collision.gameObject);
            if (_keysCnt == 0)
                _mazeManager.OpenExit();
        }
    }

    public void ResetPlayer(int keyCnt)
    {
        _keysCnt = keyCnt;
    }
}
