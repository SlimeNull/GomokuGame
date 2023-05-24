namespace LibGomokuGame
{

    public class GomokuSession
    {
        public string ID { get; set; } = string.Empty;
        public GomokuSessionState State { get; set; } = GomokuSessionState.Wait;

        public string HostPlayer { get; set; } = string.Empty;
        public string GuestPlayer { get; set; } = string.Empty;

        public int HostPlayerLastStep { get; set; } = -1;
        public int GuestPlayerLastStep { get; set; } = -1;

        public bool HostPlayerNeetMove { get; set; } = true;
    }
}