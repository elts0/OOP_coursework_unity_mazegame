using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HuntKillMaze : DepthFirstMaze
{
    //similar to deapth first search algorithm
    //but instead of backtracking, goes through maze grid in search of unvisited cells (hunting)
    public override IEnumerator GenerateMaze(bool isAnimated)
    {
        if (_cellGrid == null)
        {
            throw new NotInitializedGridException();
        }

        MazeCell initialCell = _cellGrid[0, 0];
        MazeCell currentCell = initialCell;
        currentCell.Visit();

        int huntPos = 0;
        do
        {
            MazeCell nextCell = GetNextCell(currentCell);
            if (nextCell == null)
            {
                currentCell = HuntMode(ref huntPos);
                if (currentCell != null)
                {
                    currentCell.Visit();
                    RemoveWallAdjacent(currentCell);
                }
                continue;
            }
            MakePassage(currentCell, nextCell);
            nextCell.Visit();
            currentCell = nextCell;

            if (isAnimated)
                yield return new WaitForSeconds(0.02f);

        } while (currentCell != null);
    }

    private MazeCell HuntMode(ref int huntPos)
    {
        for (int i = huntPos / _cellGrid.GetLength(1); i < _cellGrid.GetLength(0); i++)
        {
            for (int j = huntPos % _cellGrid.GetLength(1); j < _cellGrid.GetLength(1); j++)
            {
                huntPos++;
                if (!_cellGrid[i, j].GetIsVisited())
                {
                    return _cellGrid[i, j];
                }
            }
        }

        return null;
    }

    private void RemoveWallAdjacent(MazeCell cell)
    {
        int i = cell.GetIndexes().iRow;
        int j = cell.GetIndexes().iCol;

        if (j - 1 >= 0 && _cellGrid[i, j - 1].GetIsVisited())
        {
            MakePassage(cell, _cellGrid[i, j - 1]);
            return;
        }
        if (i - 1 >= 0 && _cellGrid[i - 1, j].GetIsVisited())
        {
            MakePassage(cell, _cellGrid[i - 1, j]);
            return;
        }
        if (i + 1 >= 0 && _cellGrid[i + 1, j].GetIsVisited())
        {
            MakePassage(cell, _cellGrid[i + 1, j]);
            return;
        }
        if (j + 1 >= 0 && _cellGrid[i, j + 1].GetIsVisited())
        {
            MakePassage(cell, _cellGrid[i, j + 1]);
            return;
        }
    }
}
