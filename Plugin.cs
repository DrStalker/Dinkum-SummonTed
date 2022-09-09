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

    _hotKey = Config.Bind("General", "HotKey", KeyCode.F11,"Press to summon Ted");

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
            float playerX=0;
            float playerY=0;
            float tedX=0;
            float tedY=0;
            float dX=0;
            float dY=0;
            float tedDistance=0;
            float tedDirection=0;
            bool iFoundTed=false;
            string tedDirName="UnknownDirection";
            int tedNPCId = -1;

            // Is Ted already here?
            for (int i = 0; i < NPCManager.manage.npcsOnMap.Count; i++)
            {   
                if (NPCManager.manage.npcsOnMap[i].npcId==5)
                {
                    iFoundTed=true;
                    tedNPCId=i;

                }
            }

            if (!iFoundTed) 
            {
                //Summon Ted

                AnimalManager.manage.trapperCanSpawn = false;  //so he doesn't spawn a second time
                
                //Place him on top of the player
                //Even though locations are done using vector3 the x & y paramaters given to functions get multiplied by 2 to form the vector and also Y is stored in the vector's Z.
                playerX=(int)(NetworkMapSharer.share.localChar.transform.position.x/2);
                playerY=(int)(NetworkMapSharer.share.localChar.transform.position.z/2);

                //spawn NPC & play the Ted Selly whistle
                NPCManager.manage.npcsOnMap.Add(new NPCMapAgent(5, (int)playerX, (int)playerY));
                NetworkMapSharer.share.RpcPlayTrapperSound(new Vector3((float)(playerX * 2), 0f, (float)(playerY * 2)));
                
                NotificationManager.manage.createChatNotification(String.Format("Ted Selly has been summoned!"));
            }
            else //describe where Ted is, relative to the player
            {
                //NotificationManager.manage.createChatNotification("Ted Selly is already somewhere on the island!");
                //get your location, ted's location, work out the distance and direction.
                //this can probably be done better using Unity's vector3 class functions instead of pullingout the components like this, but given how weirdly the vector3 class gets used this requires a lot less effort to get working.
                tedX=(int)(NPCManager.manage.npcsOnMap[tedNPCId].currentPosition.x/2);
                tedY=(int)(NPCManager.manage.npcsOnMap[tedNPCId].currentPosition.z/2);
                playerX=(int)(NetworkMapSharer.share.localChar.transform.position.x/2);
                playerY=(int)(NetworkMapSharer.share.localChar.transform.position.z/2);
                dX=tedX-playerX;
                dY=tedY-playerY;
                tedDistance=(int)Math.Sqrt((double)((dX*dX)+(dY*dY)));  
                tedDirection=(int)((Math.Atan(Math.Abs((double)dY/(double)dX)))*(180/Math.PI));  //east =0°. Get angle to east/west axis then adjust  based on the quadrent.
                if (dX<0 && dY>=0) {tedDirection=180f-tedDirection;}
                if (dX<0 && dY<0) {tedDirection=tedDirection+180f;}
                if (dX>0 && dY<0) {tedDirection=360f-tedDirection;}
               
                // TO DO: make this an enum and a loop instead of this mess, and also work in NNE style directions.
                if (tedDirection<22.5f) {tedDirName="east";}
                else if (tedDirection<(22.5f+(1f*45f))) {tedDirName="northeast";}
                else if (tedDirection<(22.5f+(2f*45f))) {tedDirName="north";}
                else if (tedDirection<(22.5f+(3f*45f))) {tedDirName="northwest";}
                else if (tedDirection<(22.5f+(4f*45f))) {tedDirName="west";}
                else if (tedDirection<(22.5f+(5f*45f))) {tedDirName="southwest";}
                else if (tedDirection<(22.5f+(6f*45f))) {tedDirName="south";}
                else if (tedDirection<(22.5f+(7f*45f))) {tedDirName="southeast";}
                else  {tedDirName="east";}

                //Now tell the player where he is
                if (tedDistance<=5)
                {
                    NotificationManager.manage.createChatNotification("Ted Selly is very close.");                        
                }
                else
                {
                    NotificationManager.manage.createChatNotification(String.Format("Ted Selly is {0} metres to the {1}.",tedDistance,tedDirName));         
                }

            }

            
        }

    }



}

