﻿
namespace i7llyvmR2
{    internal struct GamepadStatistics
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

        public override string ToString()
        {
            return $"A: {A} B: {B} X: {X} Y: {Y}\nLB: {LB} RB: {RB} LS: {LS} RS: {RS}\n" +
                $"Start: {Start} Back: {Back}";
        }
    }
}