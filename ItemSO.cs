using System.Collections;
using UnityEngine;

[CreateAssetMenu(fileName = "NewItem", menuName = "Inventory/Item")]
public class ItemSO : ScriptableObject
{
    [Header("ITEM INFO")]
    public string itemName;
    [TextArea] public string itemDescription;

    [Header("ITEM FLAGS")]
    public bool isUsable = false;
    public bool isEquipable = false;
    public bool isStackable = false;

    [Header("EQUIPMENT SLOT TYPE")]
    public EquipmentSlotType slotType = EquipmentSlotType.None;

    [Header("POTION TYPE (if usable)")]
    public PotionType potionType = PotionType.None;

    [Header("STAT CHANGES")]
    public StatModifier[] statModifiers;

    [Header("TEMPORARY BUFF SETTINGS")]
    public bool isTemporaryBuff = false;
    public float buffDuration = 1800f; // 30 min by default

   // [Header("VISUAL EFFECTS (Optional)")]
   // public GameObject useEffectPrefab;

    public void UseItem()
    {
        var playerStats = GameObject.Find("StatManager").GetComponent<PlayerStatsManager>();

        // Optional visual effect
       // if (useEffectPrefab)
       // {
        //    GameObject.Instantiate(useEffectPrefab, playerStats.transform.position, Quaternion.identity);
       // }

        foreach (var mod in statModifiers)
        {
            if (mod.stat == StatToChange.None) continue;

            if (isTemporaryBuff)
                playerStats.AddTemporaryStat(mod.stat, mod.amount, buffDuration);
            else
                playerStats.AddStats(mod.stat, mod.amount);

            // Direct health/mana recovery
            if (mod.stat == StatToChange.Health)
                GameObject.Find("Player").GetComponent<PlayerHealth>().ChangeHealth(mod.amount);
            if (mod.stat == StatToChange.Mana)
                GameObject.Find("Player").GetComponent<PlayerMana>().RegenerateMana(mod.amount);
        }
    }

    [System.Serializable]
    public struct StatModifier
    {
        public StatToChange stat;
        public int amount;
    }

    public enum StatToChange
    {
        None,
        Attack,
        Deffense,
        AttackSpeed,
        CriticalChance,
        CriticalDamage,
        Movementspeed,
        Health,
        HpRegeneration,
        Mana,
        ManaRegeneration,
        MagicAttack,
        Cooldown,
    }

    public enum PotionType
    {
        None,
        Health,
        Mana,
        Buff,
        Mixed
    }
}
