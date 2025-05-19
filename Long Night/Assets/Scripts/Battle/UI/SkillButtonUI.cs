using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SkillButton : MonoBehaviour
{
    private ItemData _skill;

    [Header("Настройки навыка")]
    [SerializeField] private int manaCost;
    public void Setup(ItemData skill)
    {
        _skill = skill;
        GetComponent<Button>().onClick.AddListener(UseSkill);
    }

    private void UseSkill()
    {
            // Определяем тип навыка через ItemID
            switch (_skill.itemID)
            {
                case ItemID.LifeImpulse:
                    BattleManager.Instance.UseLifeImpulse(_skill, manaCost);
                    break;
                case ItemID.FlashFury:
                    BattleManager.Instance.UseFuryFlash(_skill, manaCost);
                    break;
            case ItemID.GuardianShieldRuna:
                BattleManager.Instance.UseGuardianShield(_skill, manaCost);
                break;
            case ItemID.RealityRift:
                BattleManager.Instance.UseRealityRift(_skill, manaCost);
                break;
            default:
                    break;
            }
    }
}