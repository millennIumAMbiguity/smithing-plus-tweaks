using System;
using System.Collections.Generic;
using System.Reflection;
using Vintagestory.API.Client;
using Vintagestory.API.Common;
using Vintagestory.API.Server;

namespace SmithingPlusTweaks.Config;

public class ConfigLoader : ModSystem
{
    private const string SMITHING_PLUS_CONFIG_NAME = "SmithingPlus.json";
    private const string CLIENT_CONFIG_NAME = "SmithingPlusClient.json";
    
    private static ClientConfig? _clientConfig;
    
    private PropertyInfo[] _syncVars = [];

    public override double ExecuteOrder()
    {
        // Load after SmithingPlus
        return 0.04;
    }

    public override void StartServerSide(ICoreServerAPI api)
    {
        Mod.Logger.VerboseDebug("Sending config to clients...");
        SendConfigToClient(api);
    }
    
    public override void StartClientSide(ICoreClientAPI api)
    {
        Mod.Logger.VerboseDebug("Reading config from server...");
        ReadConfigFromServer(api);
        LoadClientConfig(api);
        PatchServerConfig(api);
        ClearConfig();
    }
    
    private void LoadClientConfig(ICoreAPI api)
    {
        try
        {
            _clientConfig = api.LoadModConfig<ClientConfig>(CLIENT_CONFIG_NAME);
            if (_clientConfig == null)
            {
                _clientConfig = new ClientConfig();
                Mod.Logger.VerboseDebug("SmithingPlusClient config file not found, creating a new one...");
            }

            api.StoreModConfig(_clientConfig, CLIENT_CONFIG_NAME);
        }
        catch (Exception e)
        {
            Mod.Logger.Error("Failed to load SmithingPlusClient config: {0}", e);
            _clientConfig = new ClientConfig();
        }
    }

    /// <summary>
    /// Apply client config and save the config file.
    /// </summary>
    /// <exception cref="NullReferenceException">Thrown if ether config is null</exception>
    private void PatchServerConfig(ICoreAPI api)
    {
        Mod.Logger.Notification("Patching SmithingPlus config with client config file...");
        
        if (SmithingPlus.Config.ConfigLoader.Config == null || _clientConfig == null)
            throw new NullReferenceException("Ether client or server config was null, which was not expected.");

        SmithingPlus.Config.ConfigLoader.Config.AnvilShowRecipeVoxels       = _clientConfig.AnvilShowRecipeVoxels;
        SmithingPlus.Config.ConfigLoader.Config.HandbookExtraInfo           = _clientConfig.HandbookExtraInfo;
        SmithingPlus.Config.ConfigLoader.Config.RememberHammerToolMode      = _clientConfig.RememberHammerToolMode;
        SmithingPlus.Config.ConfigLoader.Config.ShowWorkableTemperature     = _clientConfig.ShowWorkableTemperature;
        SmithingPlus.Config.ConfigLoader.Config.AnvilRecipeSelectionColumns = _clientConfig.AnvilRecipeSelectionColumns;
        
        api.StoreModConfig(SmithingPlus.Config.ConfigLoader.Config, SMITHING_PLUS_CONFIG_NAME);
    }

    public void SendConfigToClient(ICoreAPI api)
    {
        if (SmithingPlus.Config.ConfigLoader.Config == null)
            throw new NullReferenceException("Server config was null, which was not expected.");

        SetVars(api);
    }
    
    public void ReadConfigFromServer(ICoreAPI api)
    {
        if (SmithingPlus.Config.ConfigLoader.Config == null)
            throw new NullReferenceException("Server config was null, which was not expected.");

        GetVars(api);
    }

    PropertyInfo[] GetSyncVars()
    {
        if (_syncVars.Length != 0) return _syncVars;
        
        var fields = typeof(SmithingPlusConfig).GetProperties();
        var final = new List<PropertyInfo>();
        var t = typeof(SmithingPlus.Config.ServerConfig);
        
        // Filter list to only
        for (int i = fields.Length - 1; i >= 0; i--)
        {
            if (fields[i].GetCustomAttribute(typeof(SyncConfigAttribute)) != null)
            {
                var property = t.GetProperty(fields[i].Name);
                if (property == null)
                {
                    Mod.Logger.Error(fields[i].Name + " property not found");
                    continue;
                }
                final.Add(property);
                Mod.Logger.VerboseDebug("Syncing " + fields[i].Name + " config");
            }
        }

        return _syncVars = final.ToArray();
    }

    void SetVars(ICoreAPI api)
    {
        var vars = GetSyncVars();

        foreach (var field in vars)
        {
            if (field.PropertyType == typeof(bool))
            {
                api.World.Config.SetBool("SmithingPlus_" + field.Name, (bool)field.GetValue(SmithingPlus.Config.ConfigLoader.Config)!);
            }
            else if (field.PropertyType == typeof(float))
            {
                api.World.Config.SetFloat("SmithingPlus_" + field.Name, (float)field.GetValue(SmithingPlus.Config.ConfigLoader.Config)!);
            }
            else if (field.PropertyType == typeof(int))
            {
                api.World.Config.SetInt("SmithingPlus_" + field.Name, (int)field.GetValue(SmithingPlus.Config.ConfigLoader.Config)!);
            }
        }
    }
    
    void GetVars(ICoreAPI api)
    {
        var vars = GetSyncVars();
        Mod.Logger.Debug("Get " + vars.Length + " configs");

        foreach (var field in vars)
        {
            if (field.PropertyType == typeof(bool))
            {
                field.SetValue(SmithingPlus.Config.ConfigLoader.Config, api.World.Config.GetBool("SmithingPlus_" + field.Name, (bool)field.GetValue(SmithingPlus.Config.ConfigLoader.Config)!));
            }
            else if (field.PropertyType == typeof(float))
            {
                field.SetValue(SmithingPlus.Config.ConfigLoader.Config, api.World.Config.GetFloat("SmithingPlus_" + field.Name, (float)field.GetValue(SmithingPlus.Config.ConfigLoader.Config)!));
            }
            else if (field.PropertyType == typeof(int))
            {
                field.SetValue(SmithingPlus.Config.ConfigLoader.Config, api.World.Config.GetInt("SmithingPlus_" + field.Name, (int)field.GetValue(SmithingPlus.Config.ConfigLoader.Config)!));
            }
        }
    }

    private void ClearConfig()
    {
        _clientConfig = null;
        _syncVars = [];
    }

    public override void Dispose()
    {
        ClearConfig();
        base.Dispose();
    }
}