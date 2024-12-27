public static class PlayerData
{
    private static int playerJumps = 0;
    private static float deltaSelectionTime = 0f;
    private static float sessionSelectionTime = 0f;
    private static float deltaPlayerDistance = 0f;
    private static float playerDistance = 0f;

    public static int PlayerJumps { get { return playerJumps; } }
    public static float DeltaSelectionTime { get { return deltaSelectionTime; } }
    public static float SessionSelectionTime {  get { return sessionSelectionTime; } }
    public static float DeltaPlayerDistance { get { return deltaPlayerDistance; } }
    public static float PlayerDistance { get { return playerDistance; } }

    public static void IncrementPlayerJumps() { playerJumps++; }
    public static void AddSelectionTime(float deltaTime) { deltaSelectionTime = deltaTime; sessionSelectionTime += deltaTime; }
    public static void UpdatePlayerDistance(float distance)
    {
        deltaPlayerDistance = distance - playerDistance;
        playerDistance = distance;
    }
    public static void ResetData()
    {
        playerJumps = 0;
        deltaSelectionTime = 0f;
        sessionSelectionTime = 0f;
        playerDistance = 0f;
    }
}