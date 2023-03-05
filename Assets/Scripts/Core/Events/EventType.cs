namespace Core.Events {
    public enum EventType {
        TestEvent = 0,
        LogEvent,
        UIBarChangedEvent,
        OxygenChangeEvent,
        LevelChangeEvent,
        WeaponChangedEvent,
        WeaponFiredEvent,
        WeaponRechargedEvent,
        AugmentChangedEvent,
        AugmentDrainEvent,
        RangedShotEvent,
        GetMovementStateEvent,
        SetMovementStateEvent,
        RequestIsOnGroundEvent,
        ReceiveIsOnGroundEvent,
        //Animation Combat Events
        MeleeAttackBeginEvent,
        MeleeAttackEndEvent,
        EnemyDamageEvent,
    }
}