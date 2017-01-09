using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;


public class PlayerHealth : MonoBehaviour
{
    public int startingHealth = 100;
    public int currentHealth;
    //public AudioClip deathClip;
    public float flashSpeed = 5f;
    public Color flashColour = new Color(1f, 0f, 0f, 0.1f);


    
    //AudioSource playerAudio;
    //PlayerShooting playerShooting;
    bool isDead;
    bool damaged;


    void Awake ()
    {
       // playerAudio = GetComponent <AudioSource> ();
        //playerShooting = GetComponentInChildren <PlayerShooting> ();
        currentHealth = startingHealth;
    }


    void Update ()
    {
        damaged = false;
    }


    public void TakeDamage (int amount)
    {
        damaged = true;

        currentHealth -= amount;
        Debug.Log(currentHealth + "% of health left");



       // playerAudio.Play ();

        if(currentHealth <= 0 && !isDead)
        {
            Death ();
        }
    }


    void Death ()
    {
        isDead = true;
        //this.GetComponent<Rigidbody>().useGravity = true;


        //playerShooting.DisableEffects ();

        //anim.SetTrigger ("Die");

        //playerAudio.clip = deathClip;
        //playerAudio.Play ();

        //playerMovement.enabled = false;
        //playerShooting.enabled = false;
        //SceneManager.LoadScene(1);
    }


    public void RestartLevel ()
    {
        SceneManager.LoadScene (0);
    }
}
