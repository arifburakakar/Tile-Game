using DG.Tweening;
using UnityEngine;

[CreateAssetMenu(fileName = "SelectAbilityConfig", menuName = "Abilities/Select Ability Config")]
public class SelectAbilityConfig : AbilityConfig
{
    public float SelectScale;
    public float SelectScaleDuration;
    public Ease SelectEase;
    public float DeselectScaleDuration;
    public override IAbility CreateAbility(Item item)
    {
        SelectAbility ability = new SelectAbility(item);

        ability.SelectEase = SelectEase;
        ability.SelectScale = SelectScale;
        ability.DeselectScaleDuration = DeselectScaleDuration;
        ability.SelectScaleDuration = SelectScaleDuration;

        return ability;
    }
}