using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AdaptationElements : MonoBehaviour {
    
    GameObject player;
    TaskManagement taskManagement;
    Camera mainCamera;
    public Inventory inventory;
    public Transform textTriggerPrefab;
    
	List<string> generalTips = new List<string>{"gameinfo_lewisCenterTip1", "gameinfo_lewisValenceTip1"};
	List<string> heatTips = new List<string>{"gameinfo_lewisCTFETip1"};
    List<string> acidTips = new List<string>{"gameinfo_lewisChargeTip1","gameinfo_lewisAcidTip1", "gameinfo_lewisFormulaTip1"};
	List<string> navigationTips = new List<string>{"gameinfo_navigationTipQuantity1","gameinfo_navigationTip1"};
    List<string> ethanolTips = new List<string>{"gameinfo_ethanolTip1"};

	//List<string> challenges = new List<string>{"MoveStairs","Earthquake"};

	bool earthquaked = false;
	bool movedstairs = false;
    
    void Start()
    {
        DontDestroyOnLoad(transform.gameObject);
        
        taskManagement = GameObject.Find("Tasks").GetComponent<TaskManagement>();
        player = GameObject.Find("Player");
        inventory = GameObject.Find("Inventory").GetComponent<Inventory>();
        mainCamera = Camera.main;
    }
    
    void OnLevelWasLoaded()
    {
        Start();
    }
    
    void CreateNewTrigger(string triggerName)
	{
		Vector3 toSpawn = player.transform.position;
		UnityEngine.Object newTrigger = Instantiate(textTriggerPrefab,toSpawn,Quaternion.identity);
		newTrigger.name = triggerName;
	}

	void MoveStairs()
	{
		GameObject[] stairs = GameObject.FindGameObjectsWithTag ("RemovableStairs");
		foreach(GameObject stair in stairs)
		{
			Destroy(stair);
		}
	}
    
    IEnumerator Earthquake()
    {
        PlayerLook playerLook = mainCamera.GetComponent<PlayerLook>();
        //playerMove = player.GetComponent<PlayerMove>();
        
		playerLook.enabled = false;
        //playerMove.SetActive(false);
        //Vector3 originPosition = mainCamera.transform.position;
		float originalStrength = 0.007f;
        Quaternion originRotation = mainCamera.transform.rotation;
        for (int i = 30; i > 0; i--)
        {
			float strength = originalStrength*i;
			mainCamera.transform.rotation = new Quaternion(originRotation.x + Random.Range(-strength,strength), originRotation.y + Random.Range(-strength,strength), originRotation.z + Random.Range(-strength,strength), originRotation.w + Random.Range(-strength,strength));
			//yield return new WaitForSeconds(0.1f);
			yield return new WaitForSeconds(0.01f);
			yield return null;
        }
        
        //mainCamera.transform.position = originPosition;
        mainCamera.transform.rotation = originRotation;
        playerLook.enabled = true;
		CreateNewTrigger("communic_earthquake1");
        //playerMove.SetActive(true);
    }
    
    public void Challenge()
    {
		if (Application.loadedLevelName == "cavernScene" && taskManagement.progressionIndex > 0 && !movedstairs)
		{
			MoveStairs();
			movedstairs = true;
		}

        if (Application.loadedLevelName == "cavernScene" && taskManagement.progressionIndex > 0 && !earthquaked)
        {
			AudioSource[] audios = player.GetComponents<AudioSource>();
			audios[1].Play();
			if (inventory.inventoryOpen)
			{
				inventory.ToolToggle();
			}
			StartCoroutine(Earthquake());
			earthquaked = true;
        }
    }
    
    public void Assist()
    {

        if (taskManagement.progressionIndex == 2)
        {
            if (inventory.inventory["O"] < 4 && !inventory.inventoryOpen && navigationTips.Count > 1)
            {
				GiveHint(navigationTips);
				return;
            }
            
            GiveHint(acidTips);
			return;
        }

        if (taskManagement.progressionIndex == 5)
        {
            GiveHint(ethanolTips);
			return;
        }

		if (taskManagement.progressionIndex == 4)
		{
			if (!inventory.inventory.ContainsKey("F") || !inventory.inventory.ContainsKey("Cl"))
			{
				if (navigationTips.Count> 1)
				{
					navigationTips.RemoveAt(0);
				}
				GiveHint(navigationTips);
				return;
			}
			GiveHint (heatTips);
			return;
		}

		if (taskManagement.progressionIndex > 0)
		{
			GiveHint(generalTips);
			return;
		}
    }
    
    void GiveHint(List<string> someTips)
    {
        if (someTips.Count == 0)
            {
                //gives answer?
                return;
            }
            
            CreateNewTrigger(someTips[0]);
            someTips.RemoveAt(0);
    }
}
