using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DepthFirstMaze : MazeGrid
{
    //depth first search algorithm to generate maze
    //basically creates a tree within maze grid
    public override IEnumerator GenerateMaze(bool isAnimated)
    {
        if (_cellGrid == null)
        {
            throw new NotInitializedGridException();
        }

        MazeCell initialCell = _cellGrid[0, 0];
        MazeCell currentCell = initialCell;
        currentCell.Visit();
        var visitedCells = new Stack<MazeCell>();
        visitedCells.Push(currentCell);

        do
        {
            //randomly choose next cell over unvisited adjacent to current
            MazeCell nextCell = GetNextCell(currentCell);
            //if there're no unvisited adjacent cell, backtrack over stack
            if (nextCell == null)
            {
                visitedCells.Pop();
                currentCell = visitedCells.Peek();
                continue;
            }
            MakePassage(currentCell, nextCell);
            nextCell.Visit();
            visitedCells.Push(nextCell);
            currentCell = nextCell;

            if (isAnimated)
                yield return new WaitForSeconds(0.02f);

        } while (currentCell != initialCell);
    }


    protected MazeCell GetNextCell(MazeCell currentCell)
    {
        var adjacentCells = GetAdjacentCells(currentCell);

        if (adjacentCells.Count == 0)
        {
            return null;
        }
        else
        {
            System.Random random = new();
            return adjacentCells[random.Next() % adjacentCells.Count];
        }
    }
}
