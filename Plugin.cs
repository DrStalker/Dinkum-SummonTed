using System;
using System.Collections;
using System.Linq;
using BepInEx;
using BepInEx.Configuration;
using Mirror;
using UnityEngine;

namespace SummonTed;

[BepInPlugin(PluginInfo.PLUGIN_GUID, PluginInfo.PLUGIN_NAME, PluginInfo.PLUGIN_VERSION)]
public class Plugin : BaseUnityPlugin
{
    private readonly ConfigEntry<KeyCode> _hotKey;
    public Plugin()
    {

    _hotKey        = Config.Bind("General", "HotKey",        KeyCode.F11,"Press to summon Ted");

    }
    
    private void Awake()
    {
        // Plugin startup logic
        Logger.LogInfo($"Plugin {PluginInfo.PLUGIN_GUID} is loaded!");
    }

    private void Update()
    {
        if (Input.GetKeyDown(_hotKey.Value))
        {
            Logger.LogInfo($"Attempting to summon Ted");
            int playerX=0;
            int playerY=0;
            int z=0;
            bool iFoundTed=false;

            // Is Ted already here?
            for (int i = 0; i < NPCManager.manage.npcsOnMap.Count; i++)
            {   
                if (NPCManager.manage.npcsOnMap[i].npcId==5)
                {
                    iFoundTed=true;
                    NotificationManager.manage.createChatNotification("Ted Selly is already somewhere on the island");
                }
            }

            if (!iFoundTed) 
            {
                //Summon Ted
                AnimalManager.manage.trapperCanSpawn = false;  //so he doesn't spawn a second time

                //Even though locations are done using vector3 the x & y paramaters given to functions get multiplied by 2 to form the vector and also Y is stored in the vector's Z.
                playerX=(int)(NetworkMapSharer.share.localChar.transform.position.x/2);
                playerY=(int)(NetworkMapSharer.share.localChar.transform.position.z/2);

                Logger.LogInfo(String.Format("Player location: {0} {1}",playerX,playerY));
                NotificationManager.manage.createChatNotification(String.Format("Player location: {0} {1}",playerX,playerY));


                NPCManager.manage.npcsOnMap.Add(new NPCMapAgent(5, playerX, playerY));
                NetworkMapSharer.share.RpcPlayTrapperSound(new Vector3((float)(playerX * 2), 0f, (float)(playerY * 2)));
                
                NotificationManager.manage.createChatNotification(String.Format("Ted Selly has been summoned!"));
            }
            
        }

    }



}

