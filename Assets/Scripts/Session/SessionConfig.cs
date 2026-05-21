[System.Serializable]
public class SessionConfig
{
    public string patientId = "";
    public string therapistName = "";
    public bool affectedLeftSide = true;
    public bool robotEnabled = false;
    public int sessionDurationMin = 10;
    public int repetitions = 10;
}
