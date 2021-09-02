using System;
using System.Collections.Generic;
using System.CommandLine.IO;
using Serious.Abbot.CommandLine.IO;

namespace UnitTests.Fakes
{
    public class FakeConsole : TestConsole, IExtendedConsole
    {
        readonly Queue<string> _lines = new();
        readonly Queue<ConsoleKeyInfo> _chars = new();

        public void PushLine(string line) => _lines.Enqueue(line);

        public void PushKey(char key) => _chars.Enqueue(new ConsoleKeyInfo(key, 0, false, false, false));

        public void Clear() => Console.Clear();
        public string? ReadLine() => _lines.Dequeue();

        public int Read() => _chars.Dequeue().KeyChar;

        public ConsoleKeyInfo ReadKey() => _chars.Dequeue();

        public ConsoleKeyInfo ReadKey(bool intercept) => _chars.Dequeue();
    }
}