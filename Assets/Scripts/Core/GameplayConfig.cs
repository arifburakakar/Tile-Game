using DG.Tweening;
using UnityEngine;

[CreateAssetMenu(menuName = "GameplayConfig", fileName = "GameplayConfig", order = 0)]
public class GameplayConfig : ScriptableObject
{
    [Header("Holder")] 
    public float ItemCollectMovementDuration;
    public Ease ItemCollectMovementEase;
}