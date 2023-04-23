using Core.Events;
using System.Timers;
using UI;
using UnityEngine;
using EventType = Core.Events.EventType;

public class Oxygen : MonoBehaviour {
    //Oxygen pool player has throughout the game
    public float oxygenPool;

    //Delay to regenerate oxygen (in seconds)
    public int regenerateTime;

    //Oxygen regen multipiler
    public float regenMultipiler;

    [Space] public float permanentOxygen;
    public float tempOxygen;

    [Space] public float totalOxygen; //sum of perm and temp oxygen\
    public float currentOxygen;
    public float oxygenRatio; //for UI

    //Check if can regenerate temp oxygen 
    private bool _canRegenOxygen = true;

    //Tracks regenerate current timer
    private int _currentRegenTimer;

    //System.Timers.Timer for regeneration
    private Timer _regenTimer;

    private void Awake() {
        permanentOxygen = oxygenPool;
        tempOxygen      = permanentOxygen;

        _currentRegenTimer = regenerateTime;

        totalOxygen   = permanentOxygen + tempOxygen;
        currentOxygen = totalOxygen;
        oxygenRatio   = currentOxygen / totalOxygen;
    }

    private void Start() {
        //Timer with interval of 1s 
        _regenTimer         =  new Timer(1000);
        _regenTimer.Enabled =  true;
        _regenTimer.Elapsed += OnTimedEvent;

        //Just in case
        if (oxygenPool == 0) Debug.LogWarning("Oxygen Pool might be null");
        if (regenerateTime == 0) Debug.LogWarning("Regen Time Pool might be null");
        if (regenMultipiler == 0) Debug.LogWarning("Regen Multiplier Pool might be null");
        
        this.FireEvent(EventType.UpdateOxygenModifiersEvent, this);
    }

    private void Update() {
        //Cap temp oxygen 
        if (tempOxygen >= permanentOxygen) tempOxygen = permanentOxygen;

        //Regenerate Oxygen with multiplier adds up overtime
        if (_currentRegenTimer <= 0) {
            _canRegenOxygen =  true;
            tempOxygen      += (regenMultipiler * Time.fixedDeltaTime);
        }
    }

    //For UI
    public float GetOxygenRatio() {
        currentOxygen = permanentOxygen + tempOxygen;
        oxygenRatio   = currentOxygen / totalOxygen;
        return oxygenRatio;
    }

    public void FireUIEvent() {
        EventDispatcher.Instance.FireEvent(EventType.UIBarChangedEvent, new BarUIMsg {
            type  = BarUI.BarType.Oxygen,
            value = GetOxygenRatio()
        });
    }

    /// <summary>
    /// Reduce Permanent Oxygen
    /// </summary>
    /// <param name="amount">Amount to decrease</param>
    public void ReducePermanentOxygen(float amount) {
        permanentOxygen -= amount;
        FireUIEvent();
    }

    /// <summary>
    /// Reduce Temp Oxygen
    /// </summary>
    /// <param name="amount">Amount to decrease</param>
    public void ReduceTempOxygen(float amount) {
        tempOxygen -= amount;

        _canRegenOxygen    = false;
        _currentRegenTimer = regenerateTime;
        FireUIEvent();
    }

    /// <summary>
    /// Adds Permanent Oxygen
    /// </summary>
    /// <param name="amount">Amount to add</param>
    public void AddPermanentOxygen(float amount) {
        permanentOxygen += amount;
        FireUIEvent();
    }

    /// <summary>
    /// Adds Temp Oxygen
    /// </summary>
    /// <param name="amount">Amount to add</param>
    public void AddTempOxygen(float amount) {
        tempOxygen += amount;
        FireUIEvent();
    }

    /// <summary>
    /// Check if enough temp oxygen to preform an action
    /// </summary>
    /// <param name="actionAmount">Oxygen cost to perform</param>
    /// <returns>(temp oxygen - actionAmount) > 0.1f</returns>
    public bool EnoughTempOxygen(float actionAmount) {
        return tempOxygen - actionAmount > 0.1f;
    }

    //For System.Timers.Timer _regenTimer
    private void OnTimedEvent(object source, ElapsedEventArgs e) {
        if (!_canRegenOxygen) {
            _currentRegenTimer--;
        }
    }
}