namespace SummonTed
{
    using System;
    using BepInEx;
    using BepInEx.Configuration;
    using UnityEngine;

    [BepInPlugin(PluginInfo.PLUGIN_GUID, PluginInfo.PLUGIN_NAME, PluginInfo.PLUGIN_VERSION)]
    public class Plugin : BaseUnityPlugin
    {
        private readonly ConfigEntry<KeyCode> _hotKey;
        private static readonly int TedSellyNpcNumber = 5;

        public Plugin()
        {
            _hotKey = Config.Bind("General", "HotKey", KeyCode.F11, "Press to summon Ted");
        }

        private void Awake()
        {
            Logger.LogInfo($"Plugin {PluginInfo.PLUGIN_GUID} is loaded!");
        }

        private void Update()
        {
            if (Input.GetKeyDown(_hotKey.Value))
            {
                AnimalManager.manage.trapperCanSpawn = false; //so he doesn't spawn a second time
            
                Logger.LogInfo($"Attempting to summon Ted");
                
                FindTed();

                GetPlayerXYCoodinates(out var playerX, out var playerY);

                SpawnTedSelly(playerX, playerY);
            }
        }

        private static void FindTed()
        {
            foreach (var npc in NPCManager.manage.npcsOnMap)
            {
                if (npc.npcId == TedSellyNpcNumber)
                {
                    npc.moveOffNavMesh(NetworkMapSharer.share.localChar.transform.position);
                }
            }
        }

        private static void SpawnTedSelly(int playerX, int playerY)
        {
            NPCManager.manage.npcsOnMap.Add(new NPCMapAgent(TedSellyNpcNumber, playerX, playerY));
            NetworkMapSharer.share.RpcPlayTrapperSound(new Vector3((float)(playerX * 2), 0f, (float)(playerY * 2)));

            NotificationManager.manage.createChatNotification(String.Format("Ted Selly has been summoned!"));
        }


        private static void GetPlayerXYCoodinates(out int playerX, out int playerY)
        {
            var position = NetworkMapSharer.share.localChar.transform.position;
            playerX = (int)(position.x / 2);
            playerY = (int)(position.z / 2);
        }
    }
}
