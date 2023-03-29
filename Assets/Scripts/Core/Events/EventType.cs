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
        WeaponRechargedEvent,
        AugmentChangedEvent,
        AugmentChargeEvent,
        AugmentDrainEvent,
        EmpowerDamageEnemyEvent,
        GetMovementStateEvent,
        SetMovementStateEvent,
        RequestIsOnGroundEvent,
        ReceiveIsOnGroundEvent,
        //Weapon Init Events
        RefreshRangedAttributesEvent,
        //Weapon Attack/Activation Events
        WeaponFiredEvent, //common for melee & ranged
        WeaponMeleeFiredEvent, //only for melee
        WeaponRangedFiredEvent, //only for ranged
        WeaponPostRangedFiredEvent, //only for ranged (UI/UX handle)
        //Modifiers Events
        RefreshModifiersEvent,
        UpdateCombatModifiersEvent,
        UpdateOxygenModifiersEvent,
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