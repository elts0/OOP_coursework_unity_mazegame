using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//A tree for storing data about breadth first search of maze
//Every node stores an object of class MazeCell and references to cells that are adjacent to current
//Can have unlimited number of children however it's limited to 3 according to maze properties
public class CellTree
{
    private List<CellTree> _children = new();

    private CellTree _parent;

    private MazeCell _cell;

    public CellTree(MazeCell cell)
    {
        _cell = cell;
    }

    public MazeCell GetCell()
    {
        return _cell;
    }

    public CellTree AddChild(MazeCell cell)
    {
        var child = new CellTree(cell) { _parent = this };
        _children.Add(child);

        return child;
    }

    public CellTree GetParent()
    {
        return _parent;
    }
}
