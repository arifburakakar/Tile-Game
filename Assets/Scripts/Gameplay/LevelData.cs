using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class LevelData
{
    public LevelType LevelType;
    public List<GeneratorsItemData> Fillers;
    public List<CellData> GridData;
}

[Serializable]
public struct GeneratorsItemData
{
    public OID OID;
}

[Serializable]
public struct CellData
{
    public Vector3Int Index;
    public Vector3 WorldPosition;
    public OID OID;
}

public enum LevelType
{
    NORMAL,
    HARD,
    SUPER_HARD
}