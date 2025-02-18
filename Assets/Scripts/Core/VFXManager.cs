using System.Collections.Generic;
using UnityEngine;

public class VFXManager : SingletonGameSystem<VFXManager>
{
    private GameObject parentObject;
    private Dictionary<string, Queue<ParticleSystem>> pools = new Dictionary<string, Queue<ParticleSystem>>();
    private List<ParticleSystem> activeSystems = new List<ParticleSystem>();
    private int updateCount = 5;
    private int initialCount = 10;
    
    protected override void OnCreate()
    {
        base.OnCreate();

        parentObject = new GameObject("VFX Manager");
        GameItemsConfig gameItemsConfig = LevelManager.Instance.GameItemsConfig;

        foreach (ItemDataSerializer itemDataSerializer in gameItemsConfig.ItemDataSerializers)
        {
            foreach (ItemData itemData in itemDataSerializer.ItemData)
            {
                for (int i = 0; i < initialCount; i++)
                {
                    CreateParticle(itemData.DestroyParticle);
                }     
            }
        }

        Main.Instance.MainUpdate += Update;
    }
    
    private void CreateParticle(string objectName)
    {
        GameObject gameObject = Resources.Load<GameObject>(objectName);
        if (gameObject == null)
        {
            return;
        }
        ParticleSystem instance = Object.Instantiate(gameObject, parentObject.transform).GetComponent<ParticleSystem>();
        instance.gameObject.SetActive(false);
        instance.name = gameObject.name;
        instance.transform.eulerAngles = gameObject.transform.eulerAngles;
        
        if (pools.ContainsKey(gameObject.name))
        {
            pools[gameObject.name].Enqueue(instance);
        }
        else
        {
            var queue = new Queue<ParticleSystem>();
            pools.Add(gameObject.name, queue);
            queue.Enqueue(instance);
        }
    }
    
    private void CreateParticle(GameObject gameObject)
    {
        ParticleSystem instance = Object.Instantiate(gameObject, parentObject.transform).GetComponent<ParticleSystem>();
        instance.gameObject.SetActive(false);
        instance.name = gameObject.name;
        if (pools.ContainsKey(gameObject.name))
        {
            pools[gameObject.name].Enqueue(instance);
        }
        else
        {
            var queue = new Queue<ParticleSystem>();
            pools.Add(gameObject.name, queue);
            queue.Enqueue(instance);
        }
    }

    public void Play(string particleName, Vector3 position)
    {
        if (particleName.Equals("") || particleName == string.Empty)
        {
            return;
        }
        
        if (!pools.ContainsKey(particleName) || pools[particleName].Count == 0)
        {
            for (int i = 0; i < updateCount; i++)
            {
                CreateParticle(particleName);
            }
        }

        ParticleSystem system = pools[particleName].Dequeue();
        system.transform.position = position;
        system.gameObject.SetActive(true);
        system.Play();

        activeSystems.Add(system);
    }

    public void Play(GameObject particle, Vector3 position)
    {
        if (!pools.ContainsKey(particle.name) || pools[particle.name].Count == 0)
        {
            for (int i = 0; i < updateCount; i++)
            {
                CreateParticle(particle);
            }
        }

        ParticleSystem system = pools[particle.name].Dequeue();
        system.transform.position = position;
        system.gameObject.SetActive(true);
        system.Play();

        activeSystems.Add(system);
    }

    private void Update()
    {
        for (int i = activeSystems.Count - 1; i >= 0; i--)
        {
            ParticleSystem system = activeSystems[i];
            if (!system.IsAlive(true))
            {
                system.gameObject.SetActive(false);
                pools[system.name].Enqueue(system);
                activeSystems.RemoveAt(i);
            }
        }
    }
}