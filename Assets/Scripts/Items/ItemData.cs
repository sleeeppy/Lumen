using System;
using System.Collections.Generic;

/// <summary>
/// 아이템 식별자. JSON 저장(정수 인덱스)과 인스펙터 배열 순서에 강결합되어 있으므로
/// 값의 "순서"는 바꾸지 말 것. 항목 추가는 항상 맨 뒤에.
/// </summary>
public enum ItemId
{
    Ring1,
    Ring2,
    Ring3,
    Ring4,
    Ring5,
    Bracelet1,
    Bracelet2,
    Bracelet3,
    Nail1,
    Nail2,
}

/// <summary>아이템 분류. 분류별로 동시 장착 가능 개수가 다르다.</summary>
public enum ItemCategory
{
    None,
    Ring,
    Bracelet,
    Nail,
}

/// <summary>아이템 한 종류에 대한 모든 정적 데이터(이름/설명/분류/효과).</summary>
public class ItemDefinition
{
    public ItemId Id;
    public string DisplayName;
    public string Description;
    public ItemCategory Category;

    /// <summary>장착 시 적용할 효과. 효과가 없는 아이템은 null.</summary>
    public Action ApplyEffect;
}

/// <summary>
/// 모든 아이템 데이터의 단일 소스(Single Source of Truth).
/// 이전에는 Inventory.cs 안의 여러 switch 문(타입/장착제한/설명/효과)에 흩어져 있었다.
/// </summary>
public static class ItemDatabase
{
    private static readonly Dictionary<ItemCategory, int> EquipLimits = new Dictionary<ItemCategory, int>
    {
        { ItemCategory.Ring, 2 },
        { ItemCategory.Bracelet, 1 },
        { ItemCategory.Nail, 1 },
    };

    private static readonly Dictionary<ItemId, ItemDefinition> Definitions = BuildDefinitions();

    public static ItemDefinition Get(ItemId id) => Definitions[id];

    /// <summary>해당 분류를 동시에 몇 개까지 장착할 수 있는지.</summary>
    public static int GetEquipLimit(ItemCategory category)
        => EquipLimits.TryGetValue(category, out int limit) ? limit : 0;

    private static Dictionary<ItemId, ItemDefinition> BuildDefinitions()
    {
        var defs = new[]
        {
            new ItemDefinition
            {
                Id = ItemId.Ring1, Category = ItemCategory.Ring,
                DisplayName = "하트 로켓", Description = "최대 체력이 1 증가합니다.",
                ApplyEffect = ItemEffects.IncreaseMaxLife,
            },
            new ItemDefinition
            {
                Id = ItemId.Ring2, Category = ItemCategory.Ring,
                DisplayName = "별의 반지", Description = "최대 에너지가 소폭 증가합니다.",
                ApplyEffect = ItemEffects.IncreaseMaxGauge,
            },
            // Ring3/4/5 는 효과가 같으므로 같은 함수를 공유한다 (아이템마다 클래스를 나눌 필요 없음).
            new ItemDefinition
            {
                Id = ItemId.Ring3, Category = ItemCategory.Ring,
                DisplayName = "Ring3", Description = "공격력을 증가시킵니다.",
                ApplyEffect = ItemEffects.IncreaseAttackPower,
            },
            new ItemDefinition
            {
                Id = ItemId.Ring4, Category = ItemCategory.Ring,
                DisplayName = "Ring4", Description = "공격력을 증가시킵니다.",
                ApplyEffect = ItemEffects.IncreaseAttackPower,
            },
            new ItemDefinition
            {
                Id = ItemId.Ring5, Category = ItemCategory.Ring,
                DisplayName = "Ring5", Description = "공격력을 증가시킵니다.",
                ApplyEffect = ItemEffects.IncreaseAttackPower,
            },
            new ItemDefinition
            {
                Id = ItemId.Bracelet1, Category = ItemCategory.Bracelet,
                DisplayName = "에리스의 팔찌", Description = "공격 타입이 기본 공격으로 변경 됩니다.",
                ApplyEffect = ItemEffects.SetBasicAttack,
            },
            new ItemDefinition
            {
                Id = ItemId.Bracelet2, Category = ItemCategory.Bracelet,
                DisplayName = "루나의 팔찌", Description = "공격 타입이 레이저 공격으로 변경됩니다.",
                ApplyEffect = ItemEffects.SetLaserAttack,
            },
            new ItemDefinition
            {
                Id = ItemId.Bracelet3, Category = ItemCategory.Bracelet,
                DisplayName = "자석 수갑", Description = "공격 타입이 유도 공격으로 변경됩니다.",
                ApplyEffect = ItemEffects.SetHomingAttack,
            },
            new ItemDefinition
            {
                Id = ItemId.Nail1, Category = ItemCategory.Nail,
                DisplayName = "별의 메아리", Description = "플레이어 주변 탄막을 제거합니다.",
                ApplyEffect = ItemEffects.EquipNail1,
            },
            new ItemDefinition
            {
                Id = ItemId.Nail2, Category = ItemCategory.Nail,
                DisplayName = "레인보우 스파크",
                Description = "지속시간동안 무적 상태가 되며 공격속도가 대폭 증가합니다.\n (개발중입니다.)",
                ApplyEffect = ItemEffects.EquipNail2,
            },
        };

        var map = new Dictionary<ItemId, ItemDefinition>(defs.Length);
        foreach (var def in defs)
            map.Add(def.Id, def);
        return map;
    }
}
