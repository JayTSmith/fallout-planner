using System.Collections;
using System.Collections.Generic;
using System.IO;
using System;
using UnityEngine;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

public class ItemDataManager
{
    private readonly string DATAFILE = Application.persistentDataPath + "/items.json";
    private static ItemDataManager instance;

    public Dictionary<string, Weapon> Weapons { get => weapons; set => weapons = value; }
    public Dictionary<string, Armor> Armors { get => armors; set => armors = value; }
    public Dictionary<string, Ammo> Ammos { get => ammos; set => ammos = value; }
    public Dictionary<string, CharacterBase> Characters { get => characters; set => characters = value; }

    [JsonProperty]
    private Dictionary<string, Ammo> ammos;
    [JsonProperty]
    private Dictionary<string, Weapon> weapons;
    [JsonProperty]
    private Dictionary<string, Armor> armors;
    [JsonProperty]
    private Dictionary<string, CharacterBase> characters;

    private ItemDataManager() {
        Weapons = new Dictionary<string, Weapon>();
        Ammos = new Dictionary<string, Ammo>();
        Armors = new Dictionary<string, Armor>();
        Characters = new Dictionary<string, CharacterBase>();

        initialize();
    }

    private void initialize() {
        foreach (Armor.ARMOR_ID aid in Enum.GetValues(typeof(Armor.ARMOR_ID))) {
            Armor armor = ArmorFactory.Build(aid);
            if (armor == null) continue;
            Armors.Add(armor.ID, armor);
        }

        foreach (Weapon.WEAPON_ID wid in Enum.GetValues(typeof(Weapon.WEAPON_ID))) {
            Weapon wep;
            
            try
            {
                wep = WeaponFactory.Build(wid);
            }
            catch (KeyNotFoundException)
            {
                continue;
            }
            Weapons.Add(wep.ID, wep);
        }

        string[] aids = { "9mm", "556" };
        foreach (string aid in aids) {
            Ammo ammo = AmmoFactory.Get(aid);
            Ammos.Add(ammo.id, ammo);
        }
    }

    public void loadFile() { loadFile(DATAFILE); }
    public void loadFile(string path) {
        if (File.Exists(path))
        {
            using (FileStream fs = File.OpenRead(path))
            {
                using (StreamReader sr = new StreamReader(fs))
                {
                    JsonSerializerSettings settings = new JsonSerializerSettings
                    {
                        Converters = new List<JsonConverter> { new DiceConverter() }
                    };

                    JsonSerializer js = JsonSerializer.Create(settings);
                    using (JsonReader jsonReader = new JsonTextReader(sr))
                    {
                        //Debug.Log(jsonReader.Value);

                        //Debug.Log(js.Deserialize(jsonReader, typeof(ItemDataManager)));
                    }
                }
            }
        }
        else
        {
            Debug.LogError($"No data found at file {path}");
        }
        //initialize();
    }

    public void saveFile() { saveFile(DATAFILE); }

    public void saveFile(string path) {
        Debug.LogError($"Saving at {path}");
        using (FileStream fs = File.OpenWrite(path)) {
            using (StreamWriter sw = new StreamWriter(fs))
            {
                using (JsonWriter jsonWriter = new JsonTextWriter(sw))
                {
                    Debug.LogError(JsonConvert.SerializeObject(this));

                    JsonSerializer serializer = JsonSerializer.Create();
                    serializer.Serialize(jsonWriter, this);
                }
            }
        }
    }

    public static ItemDataManager GetItemDataManager() {
        if (instance == null)
        {
            instance = new ItemDataManager();
        }
        return instance;
    }
}
