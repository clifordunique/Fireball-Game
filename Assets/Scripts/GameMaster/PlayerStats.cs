using UnityEngine;

public class PlayerStats : MonoBehaviour {

    public static PlayerStats instance;

    // Powerups
    private bool zoom;
    public bool Zoom
    {
        get { return zoom; }
        set { zoom = value; }
    }

    private int maxHealth = 30;
    private int curHealth;
    private int maxFireHealth = 30;
    private int curFireHealth;
    private int intervalBetweenFireHealing = 5;

    public int MaxHealth
    {
        get { return maxHealth; }
    }
    public int CurHealth
    {
        get { return curHealth; }
        set { curHealth = Mathf.Clamp(value, 0, MaxHealth); }
    }
    public int MaxFireHealth
    {
        get { return maxFireHealth; }
    }
    public int CurFireHealth
    {
        get { return curFireHealth; }
        set { curFireHealth = Mathf.Clamp(value, 0, MaxFireHealth); }
    }

    public bool IsFire()
    {
        return curFireHealth > 0;
    }

    float time;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        time = Time.time;
    }

    void Update()
    {        
        if(Time.time - time > intervalBetweenFireHealing && curFireHealth < maxFireHealth)
        {
            time = Time.time;
            curFireHealth++;
        }
    }

    public void EnablePowerup(Utilities.PowerUps powerup)
    {
        switch (powerup)
        {
            case Utilities.PowerUps.ZOOM:
                zoom = true;
                break;
            default:
                Debug.Log("Powerup is not in PlayerStats.");
                break;
        }
    }
}
