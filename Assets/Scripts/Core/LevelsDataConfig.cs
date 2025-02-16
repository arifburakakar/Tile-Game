using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "LevelsDataConfig", fileName = "LevelsDataConfig", order = 0)]
public class LevelsDataConfig : ScriptableObject
{
    public List<TextAsset> Levels;
}