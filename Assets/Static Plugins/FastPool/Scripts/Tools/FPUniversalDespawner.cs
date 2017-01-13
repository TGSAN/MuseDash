using UnityEngine;
using System.Collections;


public class FPUniversalDespawner : MonoBehaviour, IFastPoolItem
{
    /// <summary>
    /// ID of the pool to despawn this game object into
    /// </summary>
    public int TargetPoolID
    {
        get { return targetPoolID; }
        set { targetPoolID = value; }
    }
    /// <summary>
    /// "Despawn the object after the specified period of time
    /// </summary>
    public bool DespawnDelayed
    { get { return despawnDelayed; } }
    /// <summary>
    /// Despawn the object after the specified period of time
    /// </summary>
    public float Delay
    { get { return delay; } }
    /// <summary>
    /// Despawn the object after the particles on this GameObject will die
    /// </summary>
    public bool DespawnOnParticlesDead
    { get { return despawnOnParticlesDead; } }
    /// <summary>
    /// Reset particle system on despawn. Useful when the game object will be despawned before than all particles will die
    /// </summary>
    public bool ResetParticleSystem
    { get { return resetParticleSystem; } }
    /// <summary>
    /// Despawn the object after the AudioSource on this GameObject will stop playing
    /// </summary>
    public bool DespawnOnAudioSourceStop
    { get { return despawnOnAudioSourceStop; } }


    [SerializeField]
    int targetPoolID;
    [SerializeField]
    bool despawnDelayed = false;
    [SerializeField]
    float delay = 0;
    [SerializeField]
    bool despawnOnParticlesDead = false;
    [SerializeField]
    bool resetParticleSystem = false;
    [SerializeField]
    bool despawnOnAudioSourceStop = false;


    bool needCheck = false;
    AudioSource aSource;
    ParticleSystem pSystem;



    void Start()
    {
        if (despawnDelayed)
            StartCoroutine(Despawn(delay));


        if (despawnOnAudioSourceStop)
        {
            aSource = GetComponentInChildren<AudioSource>();
            needCheck = true;
        }

        if (despawnOnParticlesDead)
        {
            pSystem = GetComponentInChildren<ParticleSystem>();
            needCheck = true;
        }

        if (needCheck)
            StartCoroutine(CheckAlive());
    }

    public void OnFastInstantiate()
    {
        if (despawnDelayed)
            StartCoroutine(Despawn(delay));

        if (needCheck)
            StartCoroutine(CheckAlive());

        //Autoplay Particle System on Spawn
        if (despawnOnParticlesDead && (pSystem != null) && resetParticleSystem)
            pSystem.Play(true);
    }

    public void OnFastDestroy()
    {
        StopAllCoroutines();
        if (despawnOnParticlesDead && (pSystem != null) && resetParticleSystem)
            pSystem.Clear(true);
    }


    IEnumerator Despawn(float despawn_delay)
    {
        yield return new WaitForSeconds(despawn_delay);
        StopAllCoroutines();

        if (FastPoolManager.Instance != null)
            FastPoolManager.GetPool(targetPoolID, gameObject, true).FastDestroy(gameObject);
        else
            Debug.LogError("FastPoolManager is not present in the scene or being disabled! AutoDespawn will not working on GameObject " + name);
    }

    IEnumerator CheckAlive()
    {
        while (true)
        {
            yield return new WaitForSeconds(1);
            if (despawnOnAudioSourceStop && (aSource != null) && !aSource.isPlaying)
            {
                StartCoroutine(Despawn(0));
                break;
            }

            if (despawnOnParticlesDead && (pSystem != null) && !pSystem.IsAlive(true))
            {
                StartCoroutine(Despawn(0));
                break;
            }
        }
    }
}
