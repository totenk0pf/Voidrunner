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
    private float GetOxygenRatio() {
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

    public void ReducePermanentOxygen(float amount) {
        permanentOxygen -= amount;
        if (permanentOxygen <= 0) {
            EventDispatcher.Instance.FireEvent(EventType.OnPlayerDie);
        }
        FireUIEvent();
    }

    public void ReduceTempOxygen(float amount) {
        tempOxygen -= amount;
        _canRegenOxygen    = false;
        _currentRegenTimer = regenerateTime;
        FireUIEvent();
    }
    
    public void AddPermanentOxygen(float amount) {
        permanentOxygen += amount;
        FireUIEvent();
    }
    
    public void AddTempOxygen(float amount) {
        tempOxygen += amount;
        FireUIEvent();
    }

    public bool EnoughTempOxygen(float actionAmount) => tempOxygen - actionAmount > 0.1f;
    
    private void OnTimedEvent(object source, ElapsedEventArgs e) {
        if (!_canRegenOxygen) {
            _currentRegenTimer--;
        }
    }
}