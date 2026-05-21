using UnityEngine;

public class AppManager : PersistentSingleton<AppManager>
{
    public bool IsInitialized { get; private set; }

    protected override void Awake()
    {
        base.Awake();
        if (Instance != this) return;

        Application.runInBackground = true;
        IsInitialized = true;
    }
}
