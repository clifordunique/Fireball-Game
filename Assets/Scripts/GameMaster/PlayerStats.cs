using UnityEngine;

public class PlayerStats : MonoBehaviour {

    public static PlayerStats instance;

    public int maxHealth = 30;
    public int _curHealth;
    public int curHealth
    {
        get { return _curHealth; }
        set { _curHealth = Mathf.Clamp(value, 0, maxHealth); }
    }

    public int maxFireHealth = 30;
    public int _curFireHealth;
    public int curFireHealth
    {
        get { return _curFireHealth; }
        set { _curFireHealth = Mathf.Clamp(value, 0, maxFireHealth); }
    }

    public bool isFire()
    {
        return _curFireHealth > 0;
    }

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }
}
