using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public abstract class MazeGrid : MonoBehaviour
{
    [SerializeField] protected MazeCell _cellPrebaf;
    protected MazeCell[,] _cellGrid = null;
    [SerializeField] protected GameObject _pathPlatePrebaf;
    protected List<GameObject> _solutionPath = new();

    public abstract IEnumerator GenerateMaze(bool isAnimated);

    public void InitializeGrid(int rowCnt, int colCnt)
    {
        _cellGrid = new MazeCell[rowCnt, colCnt];

        float cellSize = 1f;

        //setting coordinates for top left corner of the grid
        var startPos = new Vector3(
            -(cellSize * colCnt / 2) + cellSize / 2, 
            (cellSize * rowCnt / 2) - cellSize / 2, 
            _cellPrebaf.transform.position.z);

        for (int i = 0; i < rowCnt; i++)
        {
            for (int j = 0; j < colCnt; j++)
            {
                //instantiating new cell at specific position and setting it as a child of MazeGrid
                _cellGrid[i, j] = Instantiate(
                    _cellPrebaf,
                    startPos + new Vector3(cellSize * j, -cellSize * i, 0),
                    Quaternion.identity,
                    this.transform);

                if (_cellGrid[i, j] == null)
                    throw new Exception("Error while instantiating the cell");

                _cellGrid[i, j].SetIndexes(i, j);
            }
        }

        DestroyExtraWalls();
    }

    //breadth first search based algorithm
    //creates the tree of the maze and builds the way out
    public void BuildSolutionPath(int iRow, int iCol)
    {
        if (_cellGrid == null)
        {
            throw new NotInitializedGridException();
        }
        if (!_cellGrid[0, 0].GetIsVisited())
        {
            throw new NotGeneratedGridException();
        }
        if (iRow < 0 || iCol < 0 ||
            iRow >= _cellGrid.GetLength(0) || iCol >= _cellGrid.GetLength(1))
        {
            throw new PlayerOutOfMazeException();
        }

        foreach (var cell in _cellGrid)
        {
            cell.Unvisit();
        }

        _cellGrid[iRow, iCol].Visit();
        var currentNode = new CellTree(_cellGrid[iRow, iCol]);

        var visitedCells = new Queue<CellTree>();
        visitedCells.Enqueue(currentNode);

        //building the tree of cells
        while (visitedCells.Count > 0 && currentNode.GetCell() != GetExitCell())
        {
            currentNode = visitedCells.Dequeue();

            foreach (var cell in GetAdjacentCells(currentNode.GetCell()))
            {
                if (!PathExist(currentNode.GetCell(), cell))
                    continue;
                cell.Visit();
                visitedCells.Enqueue(currentNode.AddChild(cell));
                if (cell == GetExitCell())
                    break;
            }
        }

        //creates the path from leaf to parent node until initial found
        while(currentNode != null)
        {
            _solutionPath.Add(Instantiate(_pathPlatePrebaf, currentNode.GetCell().transform.position, Quaternion.identity, this.transform));
            currentNode = currentNode.GetParent();
        }

        foreach (var cell in _cellGrid)
        {
            cell.Visit();
        }
    }

    public void ClearSolutionPath()
    {
        foreach (var plate in _solutionPath)
        {
            Destroy(plate);
        }
        _solutionPath.Clear();
    }

    protected void MakePassage(MazeCell currentCell, MazeCell nextCell)
    {
        if (currentCell.GetIndexes().iRow < nextCell.GetIndexes().iRow)
        {
            currentCell.DestroyBottomWall();
            nextCell.DestroyUpperWall();
        }
        else if (currentCell.GetIndexes().iRow > nextCell.GetIndexes().iRow)
        {
            currentCell.DestroyUpperWall();
            nextCell.DestroyBottomWall();
        }
        else if (currentCell.GetIndexes().iCol < nextCell.GetIndexes().iCol)
        {
            currentCell.DestroyRightWall();
            nextCell.DestroyLeftWall();
        }
        else if (currentCell.GetIndexes().iCol > nextCell.GetIndexes().iCol)
        {
            currentCell.DestroyLeftWall();
            nextCell.DestroyRightWall();
        }
    }

    protected List<MazeCell> GetAdjacentCells(MazeCell currentCell)
    {
        List<MazeCell> adjacentCells = new();

        int i = currentCell.GetIndexes().iRow;
        int j = currentCell.GetIndexes().iCol;

        if (j - 1 >= 0 && !_cellGrid[i, j - 1].GetIsVisited())
        {
            adjacentCells.Add(_cellGrid[i, j - 1]);
        }

        if (j + 1 < _cellGrid.GetLength(1) && !_cellGrid[i, j + 1].GetIsVisited())
        {
            adjacentCells.Add(_cellGrid[i, j + 1]);
        }

        if (i - 1 >= 0 && !_cellGrid[i - 1, j].GetIsVisited())
        {
            adjacentCells.Add(_cellGrid[i - 1, j]);
        }

        if (i + 1 < _cellGrid.GetLength(0) && !_cellGrid[i + 1, j].GetIsVisited())
        {
            adjacentCells.Add(_cellGrid[i + 1, j]);
        }

        return adjacentCells;
    }

    private bool PathExist(MazeCell cell1, MazeCell cell2)
    {
        if (cell1.GetIndexes().iRow < cell2.GetIndexes().iRow &&
            cell1.GetIndexes().iCol == cell2.GetIndexes().iCol)
        {
            return (!cell1.IsBottomWall() && !cell2.IsUpperWall());
        }

        if (cell1.GetIndexes().iRow > cell2.GetIndexes().iRow &&
            cell1.GetIndexes().iCol == cell2.GetIndexes().iCol)
        {
            return (!cell1.IsUpperWall() && !cell2.IsBottomWall());
        }

        if (cell1.GetIndexes().iCol < cell2.GetIndexes().iCol && 
            cell1.GetIndexes().iRow == cell2.GetIndexes().iRow)
        {
            return (!cell1.IsRightWall() && !cell2.IsLeftWall());
        }

        if (cell1.GetIndexes().iCol > cell2.GetIndexes().iCol && 
            cell1.GetIndexes().iRow == cell2.GetIndexes().iRow)
        {
            return (!cell1.IsLeftWall() && !cell2.IsRightWall());
        }

        return false;
    }


    // Destroys duplicated walls of two bordering cells
    //so there's only wall divides two cells
    private void DestroyExtraWalls()
    {
        for (int i = 0; i < _cellGrid.GetLength(0) - 1; i++)
        {
            for (int j = 0; j < _cellGrid.GetLength(1) - 1; j++)
            {
                _cellGrid[i, j].DestroyBottomWall();
                _cellGrid[i, j].DestroyRightWall();
            }
        }

        for (int i = 0; i < _cellGrid.GetLength(0) - 1; i++)
        {
            _cellGrid[i, _cellGrid.GetLength(1) - 1].DestroyBottomWall();
        }

        for (int i = 0; i < _cellGrid.GetLength(1) - 1; i++)
        {
            _cellGrid[_cellGrid.GetLength(0) - 1, i].DestroyRightWall();
        }
    }

    public MazeCell GetEntranceCell()
    {
        if (_cellGrid == null)
        {
            throw new NotInitializedGridException();
        }

        return _cellGrid[
            0, 
            (_cellGrid.GetLength(1) - 1) / 2];
    }

    public MazeCell GetExitCell()
    {
        if (_cellGrid == null)
        {
            throw new NotInitializedGridException();
        }

        return _cellGrid[
            _cellGrid.GetLength(0) - 1,
            (_cellGrid.GetLength(1) - 1) / 2];
    }

    public void OpenExit()
    {
        GetExitCell().DestroyBottomWall();
    }

    public void OpenEntrance()
    {
        GetEntranceCell().DestroyUpperWall();
    }

    public Vector3 GetCellPos(int iRow, int iCol)
    {
        if (_cellGrid == null)
        {
            throw new NotInitializedGridException();
        }

        if (iRow < 0 || iCol < 0 || 
            iRow >= _cellGrid.GetLength(0) || iCol >= _cellGrid.GetLength(1))
            throw new IndexOutOfRangeException();

        return _cellGrid[iRow, iCol].transform.position;
    }

    public void DestroyGrid()
    {
        if (_cellGrid == null)
        {
            throw new NotInitializedGridException();
        }

        foreach (var cell in _cellGrid)
        {
            Destroy(cell.GameObject());
        }
        _cellGrid = null;

        ClearSolutionPath();
    }
}
