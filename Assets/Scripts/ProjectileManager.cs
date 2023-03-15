using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ProjectileType { FriendlyRegular, FriendlyCharged, HostileRegular }

public class ProjectileManager : MonoBehaviour {
    public static Dictionary<ProjectileType, List<GameObject> > currentProjectiles = new Dictionary<ProjectileType, List<GameObject> >();

    private static Transform awakeQueue;
    private static GameObject projectilePrefab;
    private static AudioSource projectileAudiosource;
    private static AudioClip shootFriendlyClip;
    private static AudioClip shootHostileClip;


    public void Awake() {
        awakeQueue = transform.Find("AwakeQueue");
        projectilePrefab = Resources.Load<GameObject>("Prefabs/Prefab_Projectile");
        projectileAudiosource = gameObject.AddComponent<AudioSource>();
        shootFriendlyClip = Resources.Load<AudioClip>("Audio/shootPlayer");
        shootHostileClip = Resources.Load<AudioClip>("Audio/shootHostile");
        
        foreach (ProjectileType pt in Enum.GetValues(typeof(ProjectileType))) {
            currentProjectiles.Add(pt, new List<GameObject>());
        } 
    }

    public static void createProjectile(Transform rootTransform, ProjectileType type) {
        GameObject projectile = Instantiate<GameObject>(projectilePrefab, awakeQueue);
        projectile.GetComponent<Projectile>().Init(rootTransform, type);
        currentProjectiles[type].Add(projectile);
        projectile.transform.parent = null;
        AudioClip audioClip = shootFriendlyClip;
        float audioVolume = 0.4f;
        if (type == ProjectileType.HostileRegular) {
            audioClip = shootHostileClip;
            audioVolume = 1.0f;
        }
        projectileAudiosource.PlayOneShot(audioClip, audioVolume);
        Debug.Log(string.Format("Projectile created: {1}", projectile.tag, type));
    }

    public static void removeProjectile(GameObject projectile, ProjectileType type) {
        currentProjectiles[type]?.Remove(projectile);
    }

    public static List<GameObject> getFriendlyProjectiles() {
        List<GameObject> allFriendlies = currentProjectiles[ProjectileType.FriendlyRegular];
        allFriendlies.AddRange(currentProjectiles[ProjectileType.FriendlyCharged]);
        return allFriendlies;
    }

    public static List<GameObject> getHostileProjectiles() {
        return currentProjectiles[ProjectileType.HostileRegular];
    }
}
