using UnityEngine;
using System.Collections;

public class PlayerHealth : UnityEngine.MonoBehaviour, IHealth
{

    #region Variables and editor configuration

    public int max = 100;
    public AudioClip damageClip;
    public GUITexture damageTint;
    public int regenerationRate = 8;
    public float regenerationDelay = 1f;

    bool isDead = false;
    bool isDamaged = false;

    private int current;
    private float timer = 0f;

    AudioSource src;


    #endregion
    #region Unity hooks
    void Awake () {
        current = max;

        src = gameObject.GetComponentInParent<AudioSource>();

        // Cover the whole screen with the damageTint texture
        Rect r = new Rect(-Screen.width / 2, -Screen.height / 2, Screen.width, Screen.height);
        damageTint.pixelInset = r;
	}
	
	void Update () {
        if (current < max)
        {
            TintScreenFromDamage();
            Regenerate();
        }
        else timer = 0;
	}
    #endregion
    # region Interface implementation
    public void Damage(int damage)
    {
        current -= damage;
        timer = 0;


        Debug.Log("Taking " + damage + " damage...");
        Debug.Log("Health remaining: " + current);
        // TODO: Play damage clip
		src.PlayOneShot(damageClip);
    }

    public void Heal(int damage)
    {
        current += damage;
		if (current > max)
			current = max;

        Debug.Log("Healing " + damage + " damage...");
        Debug.Log("Health remaining: " + current);
    }

    public void Regenerate(int damage, float rate, float duration)
    {

    }

    public void Regenerate()
    {
        timer += Time.deltaTime;
        if (timer >= regenerationDelay + regenerationRate)
        {
            Heal((int)regenerationRate);
            timer = regenerationDelay;
        }
    }
    # endregion
    #region Extra methods
    void TintScreenFromDamage()
    {
        float alpha = 1 - ((float)current / (float)max);
        Color c = new Color(damageTint.color.r, damageTint.color.g, damageTint.color.b, alpha);
        damageTint.color = c;
    }
    #endregion
}
