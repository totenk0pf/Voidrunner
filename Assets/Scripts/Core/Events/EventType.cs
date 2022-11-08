namespace Core.Events {
    public enum EventType {
        TestEvent = 0,
        LogEvent,
        ItemRemoveEvent,
        ItemAddEvent,
        InventoryUIEvent,
        UIBarChangedEvent,
        OxygenChangeEvent,
        LevelChangeEvent,
        WeaponChangedEvent,
        WeaponFiredEvent,
        WeaponRechargedEvent,
        AugmentChangedEvent,
        AugmentChargeEvent,
        AugmentDrainEvent,
        DamageEnemyEvent,
        RangedShotEvent
    }
}