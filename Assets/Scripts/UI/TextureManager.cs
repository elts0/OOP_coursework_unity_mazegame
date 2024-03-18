using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TextureManager : MonoBehaviour
{
    [SerializeField] private Material _backgroundPlane;
    [SerializeField] private Material _cellCube;
    [SerializeField] private Material _cellWall;
    [SerializeField] private Material _player;
    [SerializeField] private Material _key;
    [SerializeField] private Material _pathPlate;

    private readonly List<Action> _actions = new();

    private void Awake()
    {
        SetTextureDefault();
        _actions.Add(() => SetTextureDefault());
        _actions.Add(() => SetTextureAlternative());
        _actions.Add(() => SetTextureDark());
    }

    public void SetTexturePreset(int value)
    {
        if (value < 0 || value >= _actions.Count)
            throw new Exception("Incorrect value for texture preset");

        _actions[value]();
    }

    private void SetTextureDefault()
    {
        _backgroundPlane.SetColor("_Color", new Color32(255, 255, 255, 255));
        _cellCube.SetColor("_Color", new Color32(0, 0, 0, 255));
        _cellWall.SetColor("_Color", new Color32(255, 0, 0, 255));
        _player.SetColor("_Color", new Color32(83, 255, 56, 255));
        _key.SetColor("_Color", new Color32(240, 255, 23, 255));
        _pathPlate.SetColor("_Color", new Color32(64, 164, 255, 255));
    }

    private void SetTextureAlternative()
    {
        _backgroundPlane.SetColor("_Color", new Color32(255, 139, 242, 255));
        _cellCube.SetColor("_Color", new Color32(21, 0, 255, 255));
        _cellWall.SetColor("_Color", new Color32(47, 185, 255, 255));
        _player.SetColor("_Color", new Color32(255, 56, 101, 255));
        _key.SetColor("_Color", new Color32(255, 150, 23, 255));
        _pathPlate.SetColor("_Color", new Color32(64, 255, 95, 255));
    }

    private void SetTextureDark()
    {
        _backgroundPlane.SetColor("_Color", new Color32(0, 0, 0, 255));
        _cellCube.SetColor("_Color", new Color32(17, 157, 11, 255));
        _cellWall.SetColor("_Color", new Color32(30, 77, 30, 255));
        _player.SetColor("_Color", new Color32(0, 71, 145, 255));
        _key.SetColor("_Color", new Color32(255, 216, 23, 255));
        _pathPlate.SetColor("_Color", new Color32(24, 0, 68, 255));
    }
}
