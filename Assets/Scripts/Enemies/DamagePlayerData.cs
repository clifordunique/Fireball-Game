using UnityEngine;

public class DamagePlayerData {

    public GameObject damagePlayerEffect;
    public bool customDamageSprite; // The damagePlayerEffect will not be used
    public int damageToPlayerHealth;
    public int damageToPlayerFireHealth;

    public Vector2 hitPos;
    public Transform transformInfo;

    // Should add camera shake amount here at some point

    ///// <summary>
    ///// 
    ///// </summary>
    ///// <param name="damagePlayerEffect">The effect to instantiate on hitting the player. If null, no effect will be instantiated.</param>
    ///// <param name="damageToPlayerHealth"></param>
    ///// <param name="damageToPlayerFireHealth"></param>
    ///// <param name="hitPos">The position of the effect hit.</param>
    ///// <param name="transformInfo">The transform of the enemy.</param>
    //public DamagePlayerData(GameObject _damagePlayerEffect, int _damageToPlayerHealth, int _damageToPlayerFireHealth, Vector2 _hitPos, Transform _transformInfo)
    //{
    //    damagePlayerEffect = _damagePlayerEffect;
    //    damageToPlayerHealth= _damageToPlayerHealth;
    //    damageToPlayerFireHealth= _damageToPlayerFireHealth;

    //    hitPos= _hitPos;
    //    transformInfo= _transformInfo;
    //}

    ///// <summary>
    ///// 
    ///// </summary>
    ///// <param name="damagePlayerEffect">The effect to instantiate on hitting the player. If null, no effect will be instantiated.</param>
    ///// <param name="damageToPlayerHealth"></param>
    ///// <param name="damageToPlayerFireHealth"></param>
    ///// <param name="hitPos">The position of the effect hit.</param>
    ///// <param name="transformInfo">The transform of the enemy.</param>
    //public DamagePlayerData(GameObject _damagePlayerEffect, int _damageToPlayerHealth, int _damageToPlayerFireHealth)
    //{
    //    damagePlayerEffect = _damagePlayerEffect;
    //    damageToPlayerHealth = _damageToPlayerHealth;
    //    damageToPlayerFireHealth = _damageToPlayerFireHealth;

    //    hitPos = Vector2.zero;
    //    transformInfo = null;
    //}

    /// <summary>
    /// 
    /// </summary>
    /// <param name="damagePlayerEffect">The effect to instantiate on hitting the player. If null, no effect will be instantiated.</param>
    /// <param name="damageToPlayerHealth"></param>
    /// <param name="damageToPlayerFireHealth"></param>
    /// <param name="hitPos">The position of the effect hit.</param>
    /// <param name="transformInfo">The transform of the enemy.</param>
    public DamagePlayerData()
    {
        damagePlayerEffect = null;
        damageToPlayerHealth = 0;
        damageToPlayerFireHealth = 0;

        hitPos = Vector2.zero;
        transformInfo = null;
        customDamageSprite = false;
    }
}
