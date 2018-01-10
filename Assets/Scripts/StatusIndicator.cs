using UnityEngine;

public class StatusIndicator : MonoBehaviour {

    [SerializeField]
    private RectTransform healthBarRect;
    [SerializeField]
    private RectTransform fireHealthBarRect;

    void Start()
    {
        if (healthBarRect == null)
        {
            Debug.LogError("No health bar object reference!");
        }
    }

    public void SetHealth(int _cur, int _max)
    {
        float _value = (float)_cur / _max;

        healthBarRect.localScale = new Vector3(_value, healthBarRect.localScale.y, healthBarRect.localScale.z);
    }

    public void SetFireHealth(int _cur, int _max)
    {
        float _value = (float)_cur / _max;

        fireHealthBarRect.localScale = new Vector3(_value, fireHealthBarRect.localScale.y, fireHealthBarRect.localScale.z);
    }
}
