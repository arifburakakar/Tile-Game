using DG.Tweening;
using UnityEngine;

public class SelectAbility : BaseAbility
{
    public float SelectScale;
    public float SelectScaleDuration;
    public Ease SelectEase;
    public float DeselectScaleDuration;
    
    public bool Selectable = true;
    public SelectAbility(Item item) : base(item)
    {
        
    }

    public override void Execute()
    {
        base.Execute();
        item.transform.DOScale(SelectScale, SelectScaleDuration).SetEase(SelectEase);
    }

    public void Deselect()
    {
        item.transform.DOScale(1, DeselectScaleDuration).SetEase(SelectEase);
    }

    public void ToggleSelectable(bool toggle)
    {
        Selectable = toggle;
    }
}