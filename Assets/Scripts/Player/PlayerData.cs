public static class PlayerData
{
    private static int playerJumps = 0;
    private static float deltaSelectionTime = 0f;
    private static float sessionSelectionTime = 0f;
    private static float playerDistance = 0f;

    public static int PlayerJumps { get { return playerJumps; } private set { } }
    public static float DeltaSelectionTime { get { return deltaSelectionTime; } }
    public static float SessionSelectionTime {  get { return sessionSelectionTime; } }
    public static float PlayerDistance { get { return playerDistance; } }

    public static void IncrementPlayerJumps() { playerJumps++; }
    public static void AddSelectionTime(float deltaTime) { deltaSelectionTime = deltaTime; sessionSelectionTime += deltaTime; }
    public static void SetPlayerDistance(float distance) { playerDistance = distance; }
    public static void ResetData()
    {
        playerJumps = 0;
        deltaSelectionTime = 0f;
        sessionSelectionTime = 0f;
        playerDistance = 0f;
    }
}