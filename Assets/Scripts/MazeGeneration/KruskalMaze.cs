using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class KruskalMaze : MazeGrid
{
    //kruskal algorithm for building minimal spanning tree
    //creates a set for every cell and then merges these sets
    public override IEnumerator GenerateMaze(bool isAnimated)
    {
        if (_cellGrid == null)
        {
            throw new NotInitializedGridException();
        }

        var setList = CreateSetList();
        var pathList = CreatePathList();

        //shuffles the elements of the list
        pathList = pathList.OrderBy(_ => Guid.NewGuid()).ToList();

        foreach (var path in pathList)
        {
            var set1 = FindCellSet(setList, path[0]);
            if (set1.Contains(path[1]))
                continue;

            var set2 = FindCellSet(setList, path[1]);
            set1.UnionWith(set2);
            setList.Remove(set2);

            path[0].Visit();
            path[1].Visit();
            MakePassage(path[0], path[1]);

            if (isAnimated)
                yield return new WaitForSeconds(0.02f);
        }
    }

    //return the set in which the cell exists
    private HashSet<MazeCell> FindCellSet(List<HashSet<MazeCell>> setList, MazeCell cell)
    {
        foreach (var set in setList)
        {
            if (set.Contains(cell))
                return set;
        }

        throw new System.Exception("Error: couldnt locate the set");
    }

    //creates a alist of sets in which there's only one cell
    private List<HashSet<MazeCell>> CreateSetList()
    {
        var setList = new List<HashSet<MazeCell>>();

        foreach (var cell in _cellGrid)
        {
            setList.Add(new HashSet<MazeCell>() { cell });
        }

        return setList;
    }

    //generates a list that contains every two adjacent cells
    private List<MazeCell[]> CreatePathList()
    {
        var pathList = new List<MazeCell[]>();

        for (int i = 0; i < _cellGrid.GetLength(0); i++)
        {
            for (int j = 0; j < _cellGrid.GetLength(1); j++)
            {
                if (j + 1 < _cellGrid.GetLength(1))
                    pathList.Add(new MazeCell[2] { _cellGrid[i, j], _cellGrid[i, j + 1] });
                if (i + 1 < _cellGrid.GetLength(0))
                    pathList.Add(new MazeCell[2] { _cellGrid[i, j], _cellGrid[i + 1, j] });
            }
        }

        return pathList;
    }
}
