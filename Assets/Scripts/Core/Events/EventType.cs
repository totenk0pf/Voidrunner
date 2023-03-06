namespace Core.Events {
    public enum EventType {
        TestEvent = 0,
        LogEvent,
        ItemRemoveEvent,
        ItemAddEvent,
        ItemPickEvent,
        InventoryHUDEvent,
        InventoryToggleEvent,
        InventoryUpdateEvent,
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
        RangedShotEvent,
        GetMovementStateEvent,
        SetMovementStateEvent,
        RequestIsOnGroundEvent,
        ReceiveIsOnGroundEvent,
        //Animation Play Events
        PlayMeleeAttackEvent,
        PlayRangedAttackEvent,
        //Animation Combat Events
        MeleeAttackBeginEvent,
        MeleeAttackEndEvent,
        EnemyDamageEvent,
    }
}