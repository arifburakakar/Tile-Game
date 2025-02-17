using Unity.VisualScripting.ReorderableList;
using UnityEngine;

public class SelectAbility : BaseAbility
{
    public bool Selectable = true;
    public SelectAbility(Item item) : base(item)
    {
        
    }

    public override void Execute()
    {
        base.Execute();
        item.transform.localScale = Vector3.one * 1.2f;
    }

    public void Deselect()
    {
        item.transform.localScale = Vector3.one;
    }

    public void ToggleSelectable(bool toggle)
    {
        Selectable = toggle;
    }
}