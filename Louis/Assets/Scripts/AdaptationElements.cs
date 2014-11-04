using UnityEngine;
using System.Collections;

public class AdaptationElements : MonoBehaviour {
    
    GameObject player;
    TaskManagement taskManagement;
    Camera mainCamera;
    Inventory inventory;
    public Transform textTriggerPrefab;
    
    List<string> acidTips = new List<string>{"communic_lewisChargeTip1","communic_lewisAcidTip1", "communic_lewisFormulaTip1"};
    List<string> navigationTips = new List<string>{"communic_navigationTipQuantity1"};
    List<string> ethanolTips = new List<string>{"communic_ethanolTip1"};
    
    void Start()
    {
        DontDestroyOnLoad(transform.gameObject);
        
        taskManagement = GameObject.Find("TaskManagement").GetComponent<TaskManagement>();
        player = GameObject.Find("Player");
        inventory = GameObject.Find("Inventory").GetComponent<Inventory>();
        mainCamera = Camera.main;
    }
    
    OnLevelWasLoaded()
    {
        Start();
    }
    
    void CreateNewTrigger(string triggerName)
	{
		Vector3 toSpawn = player.transform.position;
		UnityEngine.Object newTrigger = Instantiate(textTriggerPrefab,toSpawn,Quaternion.identity);
		newTrigger.name = triggerName;
	}
    
    IEnumerator Earthquake()
    {
        playerLook = player.GetComponent<PlayerLook>();
        //playerMove = player.GetComponent<PlayerMove>();
        
        playerLook.SetActive(false);
        //playerMove.SetActive(false);
        //Vector3 originPosition = mainCamera.transform.position;
        Quaternion originRotation = mainCamera.transform.rotation;
        for (int i = 0; i < 5; i++)
        {
            mainCamera.transform.rotation = Quaternion(originRotation.x + Random.Range(-0.4,0.4)*0.2f, originRotation.y + Random.Range(-0.4,0.4)*0.2f, originRotation.z + Random.Range(-0.4,0.4)*0.2f, originRotation.w + Random.Range(-0.4,0.4)*0.2f);
            yield return null;
        }
        
        //mainCamera.transform.position = originPosition;
        mainCamera.transform.rotation = originRotation;
        playerLook.SetActive(true);
        //playerMove.SetActive(true);
    }
    
    public void Challenge()
    {
        if (Application.loadedLevelName == "cavernblabla1")
        {
            StartCoroutine("Earthquake");
            CreateNewTrigger("communic_earthquake1");
        }
    }
    
    public void Assist()
    {
        
        if (taskManagement.progressionIndex == 1)
        {
            return;
        }
        if (taskManagement.progressionIndex == 2)
        {
            if (inventory.inventory["O"] < 4 && !inventory.inventoryOpen)
            {
                int choice = Random.Range(0,navigationTips.Length);
                CreateNewTrigger(navigationTips[choice]);
                navigationTips.RemoveAt(choice);
                return;
            }
            
            GiveHints(acidTips);
        }
        if (taskManagement.progressionIndex == 5)
        {
            GiveHints(ethanolTips);
        }
    }
    
    void GiveHint(List<string> someTips)
    {
        if (someTips.Length == 0)
            {
                //gives answer
                return;
            }
            
            CreateNewTrigger(someTips[0]);
            someTips.RemoveAt(0);
            return;
    }

}
