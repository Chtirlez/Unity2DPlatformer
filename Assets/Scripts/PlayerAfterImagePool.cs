using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAfterImagePool : MonoBehaviour
{
    //These two lines of code declare a private GameObject variable named afterImagePrefab and a private Queue of GameObjects named availableObjects.
    //The purpose of these variables is not immediately clear without the rest of the code,
    //but it seems like they might be used to create and manage "afterimages" of the player character in the game.
    //An afterimage is a visual effect created by rapidly displaying a series of still images of an object in motion,
    //giving the illusion of multiple copies of the object trailing behind it.
    [SerializeField]

    private GameObject afterImagePrefab;

    private Queue<GameObject> availableObjects = new Queue<GameObject>();

    public static PlayerAfterImagePool Instance { get; private set; }


    //In the Awake() method, the Instance field is set to the current instance of the ObjectPooler class using Instance = this;.
    //This field is used to access the ObjectPooler instance from other scripts without having to create a new instance of the class every time.
    //The GrowPool() method is called to create and add objects to the pool.
    private void Awake()
    {
        Instance = this;
        GrowPool();
    }
    //n this code, the GrowPool() method is called during the Awake() method.
    //This method is responsible for instantiating and adding objects to the pool until it reaches
    //the maximum capacity of 10.

    //The method uses a for loop that iterates 10 times,
    //creating an instance of the afterImagePrefab GameObject and adding it to the pool using the AddToPool()
    //method.The transform.SetParent(transform) line sets the parent of the instantiated object to be the same as
    //the object pool's transform.

    //This method is called during Awake() to ensure that the object pool is ready for use before any objects
    //are needed. By pre-populating the object pool, there will always be enough objects available to create
    //afterimages during gameplay.
    private void GrowPool()
    {
         for(int i= 0; i< 10; i++)
        {
            var instanceToAdd = Instantiate(afterImagePrefab);
            instanceToAdd.transform.SetParent(transform);
            AddToPool(instanceToAdd);
        }
    }
    //This method adds a given GameObject instance to the object pool by deactivating it and adding it to the availableObjects queue.
    //This means that the object is no longer visible or interactable in the scene,
    //but it is kept in the pool for later reuse. By keeping a pool of already instantiated objects,
    //the game can avoid the overhead of creating and destroying objects on the fly, which can improve performance.
    public void AddToPool(GameObject instance)
    {
        instance.SetActive(false);
        availableObjects.Enqueue(instance);
    }


    //The GetFromPool method checks if there are any available objects in the availableObjects queue.
    //If there are no objects available, it calls the GrowPool method to create new objects and add them
    //to the queue.Then, it dequeues an object from the queue, sets its active state to true,
    //and returns the object to the calling code.This way, objects are reused from the pool instead of creating
    //and destroying them every time they are needed, which can be more efficient in terms of memory usage and
    //performance.

    public GameObject GetFromPool()
    {
        if(availableObjects.Count == 0)
        {
            GrowPool();
        }

        var instance = availableObjects.Dequeue();
        instance.SetActive(true);
        return instance;
    }
} 
