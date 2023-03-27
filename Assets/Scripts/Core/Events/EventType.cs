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
        //Animation Events
        PlayAnimationEvent,
        PlayMeleeAttackEvent,
        ResumeMovementEvent, //resume movement after melee window
        StopMovementEvent, //stop movement on melee atk
        PlayRangedAttackEvent,
        CancelMeleeAttackEvent,
        //Animation Clips Combat Events
        MeleeAttackBeginEvent,
        MeleeAttackEndEvent,
        EnemyDamageEvent, //duplicate of DamageEnemyEvent (testing purposes, may update later)
    }
}