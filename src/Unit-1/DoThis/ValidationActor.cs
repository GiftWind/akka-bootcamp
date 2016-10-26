using Akka.Actor;
using System;

namespace WinTail
{
    /// <summary>
    /// Actor that validates user input and signals result to others
    /// </summary>
    class ValidationActor : UntypedActor
    {
        private readonly IActorRef _consoleWriterActor;

        public ValidationActor(IActorRef consoleWriterActor)
        {
            _consoleWriterActor = consoleWriterActor;
        }

        protected override void OnReceive(object message)
        {
            var msg = message as string;

            if (String.IsNullOrEmpty(msg))
            {
                // signals that user need to supply an input
                _consoleWriterActor.Tell(new Messages.NullInputError("No input received"));
            }

            else
            {
                var valid = IsValid(msg);
                if (valid)
                {
                    // sends success to ConsoleWriterActor
                    _consoleWriterActor.Tell(new Messages.InputSuccess("Thanks for valid input!"));
                }
                else
                {
                    // signals that input is incorrect
                    _consoleWriterActor.Tell(new Messages.ValidationError("Invalid: input had odd number of characters"));
                }
            }

            // Tell sender to continue its work
            Sender.Tell(new Messages.ContinueProcessing());
            
        }

        /// <summary>
        /// Determines if the <see cref="message"/> received is valid.
        /// Checks if number of characters in message is even.
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        private static bool IsValid(string message)
        {
            var valid = message.Length%2 == 0;
            return valid;
        }
    }
}
