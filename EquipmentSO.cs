using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[CreateAssetMenu]
public class EquipmentSO : ScriptableObject
{
    public string itemName;
    public float attack;
    public float deffense;
    public float attackSpeed;
    public float criticalChance;
    public float criticalDamage;
    public float movementSpeed;
    public float health;
    public float hpRegeneration;
    public float mana;
    public float manaRegeneration;
    public float magicAttack;
    public float cooldown;
    
    public bool isEquipable = true; // Optional, for consistency
    public bool isUsable = false;   // Optional, if you want consumable gear later

    [Header("EQUIPMENT SLOT TYPE")]
    public EquipmentSlotType slotType = EquipmentSlotType.None;

    [TextArea] public string itemDescription;
    public void EquipItem()
    {
        PlayerStatsManager playerstatsmanager = GameObject.Find("StatManager").GetComponent<PlayerStatsManager>();
        playerstatsmanager.attack += attack;
        playerstatsmanager.defense += deffense;
        playerstatsmanager.attackSpeed += attackSpeed;
        playerstatsmanager.criticalChance += criticalChance;
        playerstatsmanager.criticalDamage += criticalDamage;
        playerstatsmanager.movementSpeed += movementSpeed;
        playerstatsmanager.health += health;
        playerstatsmanager.hpRegeneration -= hpRegeneration;
        playerstatsmanager.mana += mana;
        playerstatsmanager.manaRegeneration += manaRegeneration;
        playerstatsmanager.magicAttack += magicAttack;
        playerstatsmanager.cooldown += cooldown;
        playerstatsmanager.UpdateEquipmentStats();
    }

    public void UnEquipItem()
    {
        PlayerStatsManager playerstatsmanager = GameObject.Find("StatManager").GetComponent<PlayerStatsManager>();
        playerstatsmanager.attack -= attack;
        playerstatsmanager.defense -= deffense;
        playerstatsmanager.attackSpeed -= attackSpeed;
        playerstatsmanager.criticalChance -= criticalChance;
        playerstatsmanager.criticalDamage -= criticalDamage;
        playerstatsmanager.movementSpeed -= movementSpeed;
        playerstatsmanager.health -= health;
        playerstatsmanager.hpRegeneration -= hpRegeneration;
        playerstatsmanager.mana -= mana;
        playerstatsmanager.manaRegeneration -= manaRegeneration;
        playerstatsmanager.magicAttack -= magicAttack;
        playerstatsmanager.cooldown -= cooldown;
        playerstatsmanager.UpdateEquipmentStats();
    }
}
