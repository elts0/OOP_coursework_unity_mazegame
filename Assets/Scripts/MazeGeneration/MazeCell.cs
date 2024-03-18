using UnityEngine;


public class MazeCell : MonoBehaviour
{
    private bool _isVisited = false;
    private int _iRow, _iCol;
    [SerializeField] private GameObject _leftWall;
    [SerializeField] private GameObject _rightWall;
    [SerializeField] private GameObject _upperWall;
    [SerializeField] private GameObject _bottomWall;
    [SerializeField] private GameObject _cellCube;

    public void SetIndexes(int iRow, int iCol)
    {
        _iRow = iRow;
        _iCol = iCol;
    }

    public (int iRow, int iCol) GetIndexes()
    {
        return (_iRow, _iCol);
    }

    public bool GetIsVisited()
    {
        return _isVisited;
    }

    public void Visit()
    {
        _isVisited = true;

        if (_cellCube != null)
            Destroy(_cellCube);
    }

    public void Unvisit()
    {
        _isVisited = false;
    }

    public void DestroyLeftWall()
    {
        if (_leftWall != null)
            Destroy(_leftWall);
    }

    public void DestroyRightWall()
    {
        if (_rightWall != null)
            Destroy(_rightWall);
    }

    public void DestroyUpperWall()
    {
        if (_upperWall != null)
            Destroy(_upperWall);
    }

    public void DestroyBottomWall()
    {
        if (_bottomWall != null)
            Destroy(_bottomWall);
    }

    public bool IsLeftWall()
    {
        return _leftWall != null;
    }

    public bool IsRightWall()
    {
        return _rightWall != null;
    }

    public bool IsUpperWall()
    {
        return _upperWall != null;
    }

    public bool IsBottomWall()
    {
        return _bottomWall != null;
    }
}
