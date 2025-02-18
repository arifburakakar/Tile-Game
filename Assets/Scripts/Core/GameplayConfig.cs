using DG.Tweening;
using UnityEngine;

[CreateAssetMenu(menuName = "GameplayConfig", fileName = "GameplayConfig", order = 0)]
public class GameplayConfig : ScriptableObject
{
    [Header("Holder")] 
    public float ItemCollectMovementSpeed;
    public float ItemMergeDuration = .2f;
    public float ItemMergeDespawnDelay = .02f;
    public Ease ItemMergeEase = Ease.Linear;
}