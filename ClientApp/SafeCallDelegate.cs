using System;

namespace ClientApp
{
    internal class SafeCallDelegate
    {
        private Action<string> printInLogger;

        public SafeCallDelegate(Action<string> printInLogger)
        {
            this.printInLogger = printInLogger;
        }
    }
}