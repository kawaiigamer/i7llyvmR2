
namespace i7llyvmR2
{
     internal struct GamepadStatistics
    {
        public ulong A;
        public ulong B;
        public ulong X;
        public ulong Y;
        public ulong LB;
        public ulong RB;
        public ulong LS;
        public ulong RS;
        public ulong Start;
        public ulong Back;
        public ulong DU;
        public ulong DD;
        public ulong DL;
        public ulong DR;

        public ulong RT;
        public ulong LT;
               
        public string ToStringButtons()
        {
            return $"A: {A} B: {B} X: {X} Y: {Y}\nLB: {LB} RB: {RB} LS: {LS} RS: {RS}\n" +
                $"Start: {Start} Back: {Back}\n" +
                $"DPAD Up: {DU} Down: {DD} Left: {DL} Right: {DR}";
        }

        public string ToStringTriggers()
        {
            return $"LT: {LT} RT: {RT}";
        }

        public override string ToString()
        {
            return $"{ToStringButtons}\n{ToStringTriggers()}";
        }

    }
}
