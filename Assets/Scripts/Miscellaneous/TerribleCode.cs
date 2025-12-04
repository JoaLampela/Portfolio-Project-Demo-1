using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TerribleCode : MonoBehaviour
{
    [SerializeField] private GameObject _prefab;
    [SerializeField] private Transform spawnParent;
    [SerializeField] private string resourcePath = "Crate";

    private int _counter;

    private void Update()
    {
        GameObjectFind();
        // ComponentGetComponent();
        // CameraMain();
        // DebugLog();
        // InstantiateAndDestroy();
        // HeapMemoryAllocations();
        // Linq();
        // LoadResources();
        // LargePhysicsSphere();
    }
    
    private void GameObjectFind()
    {
        GameObject player = GameObject.Find("Player");

        if (player)
        {
            Vector3 pos = player.transform.position;
        }
    }
    
    private void ComponentGetComponent()
    {
        Transform transformComponent = GetComponent<Transform>();
    }
    
    private void CameraMain()
    {
        Camera cam = Camera.main;
        
        if (cam)
        {
            Vector3 pos = cam.transform.position;
        }
    }
    
    private void DebugLog()
    {
        _counter++;
        
        Debug.Log("Update count = " + _counter + " at time " + Time.time);
    }
    
    private void InstantiateAndDestroy()
    {
        GameObject instance = Instantiate(_prefab, Random.insideUnitSphere * 5f, Quaternion.identity, spawnParent);
        Destroy(instance);
    }
    
    private void HeapMemoryAllocations()
    {
        List<Vector3> tempList = new();

        for (int i = 0; i < 100; i++)
        {
            tempList.Add(new Vector3(i, i * 2, i * 3));
        }
        
        int[] tempArray = new int[100];
        
        for (int i = 0; i < tempArray.Length; i++)
        {
            tempArray[i] = i * i;
        }
    }
    
    private void Linq()
    {
        List<int> numbers = Enumerable.Range(0, 100).ToList();
        List<int> evenNumbers = numbers.Where(n => n % 2 == 0).ToList();
    }
    
    private void LoadResources()
    {
        GameObject loadedPrefab = Resources.Load<GameObject>(resourcePath);
    }
    
    private void LargePhysicsSphere()
    {
        Collider[] hits = Physics.OverlapSphere(transform.position, 50f);
        
        foreach (Collider hit in hits)
        {
            hit.enabled = hit.enabled;
        }
    }
}
