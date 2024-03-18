using System;
using System.Collections.Generic;
using UnityEngine;

public class MazeManager : MonoBehaviour
{
    private int _rowCnt = 3, _colCnt = 3;
    [SerializeField] private MazeGrid[] _mazeGrids;
    private MazeGrid _currentMazeGrid;
    [SerializeField] private Player _player;
    [SerializeField] private GameObject _keyPrefab;
    [SerializeField] private Camera _mainCamera;
    private readonly List<GameObject> _keys = new();
    private readonly int _keyCnt = 3;
    private bool _isInitialized = false;
    private bool _isGenerated = false;
    private bool _isSolved = false;

    private bool _isAnimated = true;
    private bool _isKeyGenerated = true;
    private bool _is3DEnabled = true;

    void Start()
    {
        _currentMazeGrid = _mazeGrids[0];
    }

    public void SetColCnt(float value)
    {
        _colCnt = (int)value;
        if (_colCnt <= 0 || _colCnt >= 100)
        {
            throw new Exception("Unexpected error");
        }
    }

    public void SetRowCnt(float value)
    {
        _rowCnt = (int)value;
        if (_rowCnt <= 0 || _rowCnt >= 100)
        {
            throw new Exception("Unexpected error");
        }
    }
    public void SetAnimated(bool value)
    {
        _isAnimated = value;
    }

    public void SetKeyGenerated(bool value)
    {
        _isKeyGenerated = value;
    }
    public void Set3DEnabled(bool value)
    {
        _is3DEnabled = value;
        _mainCamera.orthographic = !_is3DEnabled;
        SetCameraPOV();
    }

    public void SetGenerationMethod(int value)
    {
        if (value < 0 || value >= _mazeGrids.Length)
        {
            throw new Exception("Unexpected error");
        }

        ClearGrid();
        _currentMazeGrid = _mazeGrids[value];

        if (_currentMazeGrid == null)
        {
            throw new Exception("Unexpected error");
        }
    }

    public void QuitGame()
    {
        Application.Quit();
    }


    void SetCameraPOV()
    {
        if (_is3DEnabled)
        {
            //field of view calculates as angle between top and bottom part of maze grid (in degrees) relative to the camera
            float fieldOfView = MathF.Atan(Math.Max(_rowCnt, _colCnt) * 1f / 2 / Math.Abs(_mainCamera.transform.position.z)) * (180 / (float)Math.PI) * 2;
            _mainCamera.fieldOfView = fieldOfView + 5f;
        }
        else
        {
            //orthographic size is the distance between the center of the screen and its top part
            _mainCamera.orthographicSize = Math.Max(_rowCnt, _colCnt) / 2 + 2f;
        }
    }

    void PlaceKeys()
    {
        for (int i = 0; i < _keyCnt; i++)
        {
            Vector3 keyPos;

            bool isOverlaped;
            //find postition to place key that is not occupied but other object
            do
            {
                isOverlaped = false;
                int random = UnityEngine.Random.Range(0, _rowCnt * _colCnt - 1);
                keyPos = new Vector3(random % _colCnt, -random / _colCnt);
                keyPos += _currentMazeGrid.GetCellPos(0, 0);

                if (keyPos == _currentMazeGrid.GetEntranceCell().transform.position)
                {
                    isOverlaped = true;
                    continue;
                }

                foreach (var key in _keys)
                {
                    if (keyPos == key.transform.position)
                    {
                        isOverlaped = true;
                        break;
                    }
                }

            } while (isOverlaped);

            _keys.Add(Instantiate(_keyPrefab, keyPos, Quaternion.identity, this.transform));
        }
    }

    public void OpenExit()
    {
        _currentMazeGrid.OpenExit();
    }

    public void CreateGrid()
    {
        if (_isInitialized)
        {
            ClearGrid();
        }

        SetCameraPOV();
        _currentMazeGrid.InitializeGrid(_rowCnt, _colCnt);
        _isInitialized = true;
    }

    public void GenerateMaze()
    {
        if (_isGenerated)
        {
            CreateGrid();
        }

        try
        {
            StartCoroutine(_currentMazeGrid.GenerateMaze(_isAnimated));

            _currentMazeGrid.OpenEntrance();
            if (_isKeyGenerated)
            {
                PlaceKeys();
            }
            else
            {
                _currentMazeGrid.OpenExit();
            }

            _player.transform.position = _currentMazeGrid.GetEntranceCell().transform.position;
            _player.gameObject.SetActive(true);

            _isGenerated = true;
        }
        catch (NotInitializedGridException exception)
        {
            exception.ShowMessageWindow();
        }
        catch (Exception)
        {
            throw new Exception("Unexpected error");
        }

    }

    public void ClearGrid()
    {
        try
        {
            _currentMazeGrid.DestroyGrid();
        }
        catch (NotInitializedGridException ex)
        {
            ex.ShowMessageWindow();
        }
        catch (Exception)
        {
            throw new Exception("Unexpected error");
        }

        _player.ResetPlayer(_keyCnt);

        _player.gameObject.SetActive(false);

        foreach (var key in _keys)
            Destroy(key);
        _keys.Clear();

        _isInitialized = false;
        _isGenerated = false;
        _isSolved = false;
    }

    public void ShowSolution()
    {
        if (_isSolved)
        {
            _currentMazeGrid.ClearSolutionPath();
        }

        try
        {
            int iRow = -(int)Math.Round((_player.transform.position - _currentMazeGrid.GetCellPos(0, 0)).y);
            int iCol = (int)Math.Round((_player.transform.position - _currentMazeGrid.GetCellPos(0, 0)).x);

            _currentMazeGrid.BuildSolutionPath(iRow, iCol);

            _isSolved = true;
        }
        catch (NotInitializedGridException ex)
        {
            ex.ShowMessageWindow();
        }
        catch (NotGeneratedGridException ex)
        {
            ex.ShowMessageWindow();
        }
        catch (PlayerOutOfMazeException ex)
        {
            ex.ShowMessageWindow();
        }
        catch (Exception)
        {
            throw new Exception("Unexpected error");
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            CreateGrid();
        }
        else if (Input.GetKeyDown(KeyCode.Space))
        {
            GenerateMaze();
        }
        else if (Input.GetKeyDown(KeyCode.Escape))
        {
            ClearGrid();
        }
        else if (Input.GetKeyDown(KeyCode.Tab))
        {
            ShowSolution();
        }
    }
}
