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

    public bool isFire()
    {
        return curFireHealth > 0;
    }

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
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
