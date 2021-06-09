using UnityEngine;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

public class FPSGameManager : MonoBehaviour
{
    public static FPSGameManager instance;
    public GameObject enemyprefab;
    public GameObject bulleytprefab;
    public GameObject boomParticle;

    public AudioSource audioSource;
    public AudioClip ShootClip;
    public AudioClip boomClip;
    public AudioClip beHitClip;

    private EntityManager _manager;
    //blobAssetStore是一个提供缓存的类，缓存能让你对象创建时更快。
    private BlobAssetStore _blobAssetStore;
    private GameObjectConversionSettings _settings;

    public Entity enemyEntity;
    public Entity bulletEntity;
    public Entity boomEntity;
  
    
    public Entity test;
    void Start()
    {
        
        instance = this;
        _manager = World.DefaultGameObjectInjectionWorld.EntityManager;
        _blobAssetStore = new BlobAssetStore();
        _settings = GameObjectConversionSettings.FromWorld(World.DefaultGameObjectInjectionWorld, _blobAssetStore);
        enemyEntity = GameObjectConversionUtility.ConvertGameObjectHierarchy(enemyprefab, _settings);
        bulletEntity = GameObjectConversionUtility.ConvertGameObjectHierarchy(bulleytprefab, _settings);
        boomEntity = GameObjectConversionUtility.ConvertGameObjectHierarchy(boomParticle, _settings);

        test = _manager.Instantiate(boomEntity);

        Translation translation = new Translation
        {
            Value = float3.zero
        };

        _manager.SetComponentData(test, translation);
      
        
    }

    public void PlayShoot()
    {
        audioSource.PlayOneShot(ShootClip);
    }
    public void PlayBoom()
    {
        audioSource.PlayOneShot(boomClip);
    }

    public void PlayBehit()
    {
        audioSource.PlayOneShot(beHitClip);
    }


    private void OnDestroy()
    {
        _blobAssetStore.Dispose();

    }
}
