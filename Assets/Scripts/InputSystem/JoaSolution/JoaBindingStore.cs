using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Joa.Contracts;
using UnityEngine;
using UnityEngine.InputSystem;

[CreateAssetMenu(fileName = "BindingServiceSO", menuName = "Scriptable Objects/BindingServiceSO")]
public class JoaBindingStore : ScriptableObject, IBindingStore
{
    public Task SaveBindingsAsync(string key, [NotNull] InputActionAsset actions, CancellationToken ct = default)
    {
        string json = actions.SaveBindingOverridesAsJson() ?? string.Empty;
        Directory.CreateDirectory(JoaFilePaths.Bindings.Default); // Only creates a new dir if it doesn't exist already -> "idempotent"
        string path = Path.Combine(JoaFilePaths.Bindings.Default, key);
        path += ".json";
        
        try
        {
            using FileStream stream = new(path, FileMode.Create, FileAccess.Write, FileShare.Write);
            using StreamWriter writer = new(stream, new UTF8Encoding());
            writer.Write(json);
            Debug.Log($"Bindings Saved for key {key} at path {path}");
        }
        catch (Exception e)
        {
            Debug.LogError($"Bindings Save Failed! Error: {e}");
        }
        
        return Task.CompletedTask;
    }
    
    public Task LoadBindingsAsync(string key, [NotNull] InputActionAsset actions, CancellationToken ct = default)
    {
        string path = Path.Combine(JoaFilePaths.Bindings.Default, key);
        path += ".json";

        if (!File.Exists(path))
        {
            Debug.LogWarning($"No file at path {path}. Is this the initial launch?");
            return Task.CompletedTask;
        }
        
        try
        {
            using FileStream stream = new(path, FileMode.Open, FileAccess.Read, FileShare.Read);
            using StreamReader reader = new(stream, new UTF8Encoding());
            string json = reader.ReadToEnd();
            actions.LoadBindingOverridesFromJson(json);
            Debug.Log($"Bindings Loaded for key {key} at path {path}");
        }
        catch (Exception e)
        {
            Debug.LogError($"Bindings Load Failed! Error: {e}");
        }
        finally
        {
            Debug.Log("LoadBindings() complete, one way or another."); // Here for pedagogic reasons
        }

        return Task.CompletedTask;
    }

    public Task ClearBindingsAsync(string key, CancellationToken ct = default)
    {
        string path = Path.Combine(JoaFilePaths.Bindings.Default, key);
        path += ".json";
        
        try
        {
            if (File.Exists(path)) File.Delete(path);
            
            Debug.Log($"Bindings Cleared for key {key} at path {path}");
        }
        catch (Exception e)
        {
            Debug.LogError($"Bindings Clear Failed! Error: {e}");
        }
        
        return Task.CompletedTask;
    }
}
