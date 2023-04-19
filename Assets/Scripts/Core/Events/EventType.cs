namespace Core.Events {
    //TODO: Divide enum by category (ItemEventType, AnimEventType,...) -> Point of enum is to be clear,-
    //TODO: -making a single enum with too many types would nullify the point of an enum in the first place.
    //TODO: Give each enum a value, so that when comparing enum value, it doesn't get misconstrued
    //TODO: Reason for doing this cuz a senior walked by my screen and told me to do this. 
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
        LevelUpEvent,
        SpecUpEvent,
        WeaponChangedEvent,
        WeaponRechargedEvent,
        AugmentChangedEvent,
        AugmentChargeEvent,
        AugmentDrainEvent,
        EmpowerDamageEnemyEvent,
        RequestMovementStateEvent,
        ReceiveMovementStateEvent,
        SetMovementStateEvent,
        RequestIsOnGroundEvent,
        ReceiveIsOnGroundEvent,
        //Grapple Based Events
        CancelGrappleEvent,
        //Weapon Init Events
        RefreshRangedAttributesEvent,
        //Weapon Attack/Activation Events
        WeaponFiredEvent, //common for melee & ranged
        UpdateActiveWeaponEvent,
        WeaponMeleeFiredEvent, //only for melee
        WeaponRangedFiredEvent, //only for ranged
        WeaponPostRangedFiredEvent, //only for ranged (UI/UX handle)
        //Modifiers Events
        RefreshModifiersEvent,
        UpdateCombatModifiersEvent,
        UpdateOxygenModifiersEvent,
        //Animation Events
        PlayAnimationEvent,
        PlayAttackEvent,
        ResumeMovementEvent, //resume movement after melee window
        StopMovementEvent, //stop movement on melee atk
        CancelAttackEvent,
        //Animation Clips Combat Events
        AttackBeginEvent,
        OnInputWindowHold,
        AttackEndEvent,
        MeleeEnemyDamageEvent, //duplicate of DamageEnemyEvent (testing purposes, may update later)
        RangedEnemyDamageEvent,
        //Player Animator Init Events
        RequestPlayerAnimatorEvent,
        ReceivePlayerAnimatorEvent,
        //
        NotifyPlayerComboSequenceEvent,
        RunPlayerComboSequenceEvent,
        NotifyStopAllComboSequenceEvent,
        NotifyResumeAllComboSequenceEvent,
        InitWeaponRefEvent,
        //
        LockInputEvent,
        UnlockInputEvent,
        //
        RequestCurrentGrappleTypeEvent,
        ReceiveCurrentGrappleTypeEvent,
    }
}