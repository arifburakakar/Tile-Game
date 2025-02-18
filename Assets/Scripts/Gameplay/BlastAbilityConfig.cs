using UnityEngine;

[CreateAssetMenu(fileName = "BlastAbilityConfig", menuName = "Abilities/Blast Ability Config")]
public class BlastAbilityConfig : AbilityConfig
{
    public int BlastBlockLevel;
    public BlastType MinBlastCap;

    public override IAbility CreateAbility(Item item)
    {
        BlastAbility ability = new BlastAbility(item);
        ability.BlastBlockLevel = BlastBlockLevel;
        ability.MinBlastCap = MinBlastCap;
        return ability;
    }
}